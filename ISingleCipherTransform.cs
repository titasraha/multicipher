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
using KeePassLib.Cryptography.Cipher;
using System;
using System.Security.Cryptography;

namespace MultiCipher
{
    interface ISingleCipherTransform: IDisposable
    {
        void Encrypt(byte[] DataBlock, int Offset, int Count);
        void Decrypt(byte[] DataBlock, int Offset, int Count);
    }

    internal class CryptoTransformer: ISingleCipherTransform
    {
        private SymmetricAlgorithm m_SymmetricAlgorithm;
        private ICryptoTransform m_Crypto;

        public CryptoTransformer(SymmetricAlgorithm SymmetricAlgo)
        {
            
            m_SymmetricAlgorithm = SymmetricAlgo;
            m_Crypto = null;            
        }

        public void Decrypt(byte[] DataBlock, int Offset, int Count)
        {
            if (m_Crypto == null)
                m_Crypto = m_SymmetricAlgorithm.CreateDecryptor();

            m_Crypto.TransformBlock(DataBlock, 0, Count, DataBlock, 0);
        }

        public void Dispose()
        {
            m_SymmetricAlgorithm.Clear();  // Where is the Dispose in framework 2.0?
            if (m_Crypto != null)
                m_Crypto.Dispose();
        }

        public void Encrypt(byte[] DataBlock, int Offset, int Count)
        {
            if (m_Crypto == null)
                m_Crypto = m_SymmetricAlgorithm.CreateEncryptor();

            if (!m_Crypto.CanTransformMultipleBlocks)
                throw new CryptographicException("Unable to transform blocks");
            
            m_Crypto.TransformBlock(DataBlock, 0, Count, DataBlock, 0);            
        }
    }

    internal class CtrBlockCipherTransformer : ISingleCipherTransform
    {
        private CtrBlockCipher m_Cipher;

        public CtrBlockCipherTransformer(CtrBlockCipher Cipher)
        {
            m_Cipher = Cipher;
        }

        public void Decrypt(byte[] DataBlock, int Offset, int Count)
        {
            m_Cipher.Decrypt(DataBlock, Offset, Count);
        }

        public void Dispose()
        {
            m_Cipher.Dispose();
        }

        public void Encrypt(byte[] DataBlock, int Offset, int Count)
        {
            m_Cipher.Encrypt(DataBlock, Offset, Count);
        }
    }


}
