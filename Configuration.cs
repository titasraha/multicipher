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
using KeePass.Plugins;
using KeePass.UI;
using KeePassLib.Keys;
using System.Windows.Forms;
using System.Diagnostics;
using System;
using KeePassLib.Cryptography.KeyDerivation;
using KeePassLib.Security;
using System.Security;
using System.Security.Cryptography;
using KeePassLib.Utility;
using KeePassLib.Cryptography;
using MultiCipher.KeeChallenge;

namespace MultiCipher
{
    /// <summary>
    /// In memory configuration structure
    /// </summary>
    public class Configuration
    {
        private CompositeKey m_DualPassword;
        public IPluginHost Host { get; private set; }

        internal SymAlgoCode Algorithm1 { get; set; }
        internal SymAlgoCode Algorithm2 { get; set; }
        internal KeyOption KeyOption { get; set; }
        internal ulong Key2Transformations { get; set; }
        internal byte YubikeySlot { get; set; }
        internal byte[] YubikeyChallenge { get; set; }
        internal byte YubikeyChallengeLength { get; set; }

        internal byte[] MasterSeed { get; set; }
        internal byte[] TransformSeed { get; set; }
        internal KeyDerivation KeyDerivation { get; set; }

        internal ConfigYubikey Yubikey { get; private set; }


        public Configuration(IPluginHost Host)
        {
            Debug.Assert(Host != null);
            this.Host = Host;

            // Defaults
            Algorithm1 = SymAlgoCode.AES256;
            Algorithm2 = SymAlgoCode.ThreeDES;
            KeyOption = KeyOption.SinglePassword;
            Key2Transformations = 10000;
            m_DualPassword = null;
            KeyDerivation = KeyDerivation.AESKDF;
            YubikeySlot = 2;

            Yubikey = new ConfigYubikey(this);

            var RndGenerator = CryptoRandom.Instance;
            MasterSeed = RndGenerator.GetRandomBytes(32);
            TransformSeed = RndGenerator.GetRandomBytes(32);
            YubikeyChallenge = RndGenerator.GetRandomBytes(64);
        }

        private string GetAlgoLabel()
        {
            return Algorithm1.GetDescription() + " + " + Algorithm2.GetDescription();
        }

        /// <summary>
        /// Opens a create new password dialog
        /// </summary>
        /// <returns></returns>
        public CompositeKey GetNewDualPassword()
        {
            CompositeKey newPwd = null;
            PasswordFrm f = new PasswordFrm(true);
            f.AlgoLabel = GetAlgoLabel();
            DialogResult dr = f.ShowDialog();

            if (dr == DialogResult.OK)
            {
                newPwd = f.Password;
                m_DualPassword = newPwd;
            }
                
            UIUtil.DestroyForm(f);

            return newPwd;
        }

        
        /// <summary>
        /// Generates 32 bit 2nd Key based on key option selected
        /// </summary>
        /// <param name="MasterSeed">Seed used to generate the key from m_2Key</param>
        /// <param name="TransformSeed">Seed for key transformation</param>
        /// <param name="NumRounds">Iteration count of transformation</param>
        /// <returns></returns>
        public byte[] Get2ndKey32(byte[] Hash, byte[] MasterSeed, byte[] TransformSeed)
        {
            byte[] GeneratedKey = new byte[32];

            byte[] HashBuffer = new byte[32 + 32 + 32]; // MasterSeed + DualKey + FirstStreamHash
            byte[] Key256Bits = null;
            byte[] pKey32 = null;

            try
            {
                Array.Copy(MasterSeed, 0, HashBuffer, 0, 32);

                KdfParameters KdfParams = new AesKdf().GetDefaultParameters();
                KdfParams.SetUInt64(AesKdf.ParamRounds, Key2Transformations);
                KdfParams.SetByteArray(AesKdf.ParamSeed, TransformSeed);

                var Key2nd = Get2ndKey();
                if (Key2nd == null)
                    throw new SecurityException("Invalid 2nd Key");

                ProtectedBinary pbinKey = Key2nd.GenerateKey32(KdfParams);
                if (pbinKey == null)
                    throw new SecurityException("Invalid Key");

                pKey32 = pbinKey.ReadData();
                if ((pKey32 == null) || (pKey32.Length != 32))
                    throw new SecurityException("Invalid Key Data");

                Array.Copy(pKey32, 0, HashBuffer, 32, 32);
                Array.Copy(Hash, 0, HashBuffer, 64, 32);

                SHA256Managed sha = new SHA256Managed();
                Key256Bits = sha.ComputeHash(HashBuffer);

                Array.Copy(Key256Bits, GeneratedKey, 32);

            }
            finally
            {
                MemUtil.ZeroByteArray(HashBuffer);
                if (Key256Bits != null) MemUtil.ZeroByteArray(Key256Bits);
                if (pKey32 != null) MemUtil.ZeroByteArray(pKey32);
            }

            return GeneratedKey;

        }

        public byte[] Get2ndKey32(byte[] Hash)
        {
            return Get2ndKey32(Hash, MasterSeed, TransformSeed);
        }



        private CompositeKey Get2ndKey()
        {
            // If the 2nd key is set then return it provided: 
            // 1) The database is open as we do not want to use the cached key of previous open attempt
            // 2) We are not in single password mode as it is possible that the master password was changed and we have to ensure we have the lastest key 
            if (m_DualPassword != null && Host.Database.IsOpen && KeyOption != KeyOption.SinglePassword)  
                return m_DualPassword;
            else if (KeyOption == KeyOption.DualPassword)
                return GetDualPassword();
            else if (KeyOption == KeyOption.SinglePassword)
                return DeriveDualKey();
            else if (KeyOption == KeyOption.Yubikey_HMAC_SHA1)
                return GetFromYubikeyHMACSHA1();
            throw new SecurityException("Invalid Key Option");

        }

        public CompositeKey GetDualPassword()
        {
            PasswordFrm f = new PasswordFrm(false);
            f.AlgoLabel = GetAlgoLabel();
            DialogResult dr = f.ShowDialog();

            if (dr == DialogResult.OK && f.Password != null)
                m_DualPassword = f.Password;
            else
                m_DualPassword = null;

            UIUtil.DestroyForm(f);

            return m_DualPassword;
        }

        public CompositeKey DeriveDualKey()
        {
            CompositeKey NewKey = new CompositeKey();
            foreach (var Key in Host.Database.MasterKey.UserKeys)
                NewKey.AddUserKey(Key);

            NewKey.AddUserKey(new KcpPassword("TR"));

            m_DualPassword = NewKey;

            return NewKey;
        }

        public CompositeKey GetFromRecovery(ProtectedBinary recovery)
        {
            CompositeKey NewKey = null;

            NewKey = new CompositeKey();
            NewKey.AddUserKey(new KeyData(recovery));
            NewKey.AddUserKey(new KcpPassword("TR"));

            m_DualPassword = NewKey;

            return NewKey;
        }

        public CompositeKey GetFromYubikeyHMACSHA1()
        {
            CompositeKey NewKey = null;
            ProtectedBinary resp = Yubikey.GetYubikeyResponse();

            if (resp != null)
                NewKey = GetFromRecovery(resp);

            m_DualPassword = NewKey;

            return NewKey;

        }
    }

    public class KeyData : IUserKey
    {
        private ProtectedBinary m_pbKeyData;

        ProtectedBinary IUserKey.KeyData
        {
            get { return m_pbKeyData; }
        }

        public KeyData (ProtectedBinary Key)
        {
            m_pbKeyData = Key;
        }
                        
    }

    public enum KeyOption
    {
        DualPassword = 0, SinglePassword = 1, Yubikey_HMAC_SHA1 = 2
    }

    public enum KeyDerivation
    {
        AESKDF = 1
    }
}
