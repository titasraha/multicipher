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
using System;
using System.IO;
using System.Diagnostics;
using KeePassLib;
using KeePassLib.Cryptography.Cipher;
using KeePassLib.Keys;
using KeePassLib.Utility;

namespace MultiCipher
{
    public sealed class MultiCipherEngine : ICipherEngine2
    {
        private PwUuid m_uuidCipher;        
        private Configuration m_Config;

        private static readonly byte[] Level2CipherUuidBytes = new byte[]{
            0x99, 0x83, 0x1D, 0x63, 0x2D, 0x12, 0x4C, 0xE2,
            0x8F, 0x79, 0x35, 0x2F, 0x77, 0x9F, 0xFD, 0xA2
		};

        public MultiCipherEngine()
        {
            m_uuidCipher = new PwUuid(Level2CipherUuidBytes);
        }
         

        public PwUuid CipherUuid
        {
            get 
            {
                Debug.Assert(m_uuidCipher != null);
                return m_uuidCipher;
            }
        }

        public string DisplayName
        {
            get { return "Multi Cipher"; }
        }

        public void SetConfig(Configuration Config)
        {
            Debug.Assert(Config != null);

            m_Config = Config;
        }

        public int KeyLength { get { return 32; } }  // Formalize the use of 32 byte Key

        public int IVLength { get { return 16; } }   // Formalize the use of 16 byte IV

        

        public Stream EncryptStream(Stream sPlainText, byte[] pbKey32, byte[] pbIV16)
        {
            Debug.Assert(m_Config != null);
                                              
            return new DualCipherStream(m_Config, pbKey32, pbIV16, sPlainText, true);
        }

        public Stream DecryptStream(Stream sEncrypted, byte[] pbKey32, byte[] pbIV16)
        {
            Debug.Assert(m_Config != null);

            int version = sEncrypted.ReadByte();
            if (version != 1 && version != 2)            
                throw new Exception("Plugin version not supported, please check for newer MultiCipher Plugin");

            CompositeKey DualKey = null;

            if (version == 1)
            {
                MessageService.ShowWarning("MultiCipher Plugin:", "You are opening Version 1.0 of MultiCipher Keepass Database, a one way upgrade will be performed.","Once saved, you will not be able to open the database in an older version of the plugin.");

                m_Config.Algorithm1 = SymAlgoCode.AES256;
                m_Config.Algorithm2 = SymAlgoCode.ThreeDES;
                m_Config.KeyOption = KeyOption.DualPassword;

                int algo = sEncrypted.ReadByte();
                if (0 != algo)
                    throw new Exception("Invalid algorithm");

                DualKey = m_Config.GetDualPassword();
                if (DualKey == null)
                    throw new Exception("Unable to decrypt data");

                return new AES3DESStream(sEncrypted, false, pbKey32, pbIV16, DualKey);
            }
            else
            {
                return new DualCipherStream(m_Config, pbKey32, pbIV16, sEncrypted, false);
            }

            
        }



    }
}
