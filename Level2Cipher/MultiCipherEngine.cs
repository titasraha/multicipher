/*

    MultiCipher Plugin for Keepass Password Safe
    Copyright (C) 2016 Titas Raha <support@titasraha.com>

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
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security;

using KeePassLib;
using KeePassLib.Cryptography;
using KeePassLib.Cryptography.Cipher;
using KeePassLib.Keys;
using KeePassLib.Security;


namespace MultiCipher
{
    public sealed class MultiCipherEngine : ICipherEngine
    {
        private PwUuid m_uuidCipher;
        private CompositeKey pwd;

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

        public Stream EncryptStream(Stream sPlainText, byte[] pbKey, byte[] pbIV)
        {

            if (pwd == null)
            {
                PasswordFrm f = new PasswordFrm(true);
                if (f.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    throw new Exception("Unable to encrypt the data");
                pwd = f.Password;
            }



            sPlainText.WriteByte((byte)1);  // File Version 1

            return new AES3DESStream(sPlainText, true, pbKey, pbIV, pwd);

        }

        public Stream DecryptStream(Stream sEncrypted, byte[] pbKey, byte[] pbIV)
        {
            int version = sEncrypted.ReadByte();
            if (version != 1)              // File Version 1
                throw new Exception("Invalid Version");

            int algo = sEncrypted.ReadByte();
            if (Algorithm.AES_3DES != (Algorithm)algo)
                throw new Exception("Invalid algorithm");

            //if (pwd == null)
            //{

            PasswordFrm f = new PasswordFrm(false);
            if (f.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                throw new Exception("Unable to decrypt the data");

            pwd = f.Password;
            //}

            return new AES3DESStream(sEncrypted, false, pbKey, pbIV, pwd);
        }



    }
}
