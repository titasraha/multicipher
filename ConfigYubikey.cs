/*

    MultiCipher Plugin for Keepass Password Safe
    Copyright (C) 2019 Titas Raha <support@titasraha.com>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/
using KeePassLib.Security;
using MultiCipher.KeeChallenge;
using MultiCipher.Yubikey;
using System;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace MultiCipher
{
    public class ConfigYubikey
    {
        public static byte CHALLENGE_LEN_64 = 0x40;
        public static byte CHALLENGE_LEN_VARIABLE = 0x3C;

        private Configuration m_Config;

        public ConfigYubikey(Configuration config)
        {
            m_Config = config;
        }

        private byte[] GetYubikeyChallenge64(byte ChallengeLength)
        {
            byte[] NewChallenge = new byte[ChallengeLength];
            Array.Copy(m_Config.YubikeyChallenge, NewChallenge, ChallengeLength);
            return NewChallenge;
        }

        public ProtectedBinary GetYubikeyResponse()
        {
            return GetYubikeyResponse(m_Config.YubikeySlot, m_Config.YubikeyChallengeLength, null, true);
        }

        public ProtectedBinary GetYubikeyResponse(byte Slot, byte ChallengeLength, ProtectedBinary SecretKeyToVerify, bool AllowRecovery)
        {
            byte[] resp = new byte[YubiWrapper.yubiRespLen];

            var Challenge = GetYubikeyChallenge64(ChallengeLength);

            YubiSlot slot = YubiSlot.SLOT2;

            if (Slot == 1)
                slot = YubiSlot.SLOT1;

            var f = new KeyEntry(slot, Challenge, AllowRecovery);

            var result = f.ShowDialog();
            
            if (result == DialogResult.OK)
            {
                f.Response.CopyTo(resp, 0);
                Array.Clear(f.Response, 0, f.Response.Length);

                
                bool verified = true;

                if (SecretKeyToVerify != null)
                {                    
                    byte[] SecretKey = SecretKeyToVerify.ReadData();
                    HMACSHA1 sha1 = new HMACSHA1(SecretKey);
                    var hash = sha1.ComputeHash(Challenge);
                    Array.Clear(SecretKey, 0, SecretKey.Length);
                    if (hash == null || resp == null || hash.Length == 0)
                        verified = false;
                    else
                        for (int i = 0; i < hash.Length; i++)
                            if (hash[i] != resp[i])
                            {
                                verified = false;
                                break;
                            }
                    Array.Clear(hash, 0, hash.Length);
                    
                }

                ProtectedBinary respProtected = new ProtectedBinary(true, resp);

                Array.Clear(resp, 0, resp.Length);

                if (!verified)
                    return null;

                
                return respProtected;
            }
            else if (f.RecoveryMode)
            {
                var recovery = new RecoveryKeyFrm();
                if (recovery.ShowDialog() != DialogResult.OK) return null;

                return recovery.Key;

               
            }
            return null;
        }

    }
}
