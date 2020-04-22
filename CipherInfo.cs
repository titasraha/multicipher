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
using System.Diagnostics;
using System.Security;
using System.Security.Cryptography;
using System.ComponentModel;
using MultiCipher.Medved;
using KeePassLib.Cryptography;

namespace MultiCipher
{
    internal class CipherInfo: IDisposable
    {
        internal static SymAlgoInfo[] List { get; private set; }

        static CipherInfo()
        {
            List = new SymAlgoInfo[] {
                new SymAlgoInfo() { SymAlgoCode = SymAlgoCode.AES256, Description="AES/Rijndael (256-bit Key)", IVSize = 16, KeySize = 32 },
                new SymAlgoInfo() { SymAlgoCode = SymAlgoCode.ThreeDES, Description="3DES (192-bit Key)", IVSize = 8, KeySize = 24 },
                new SymAlgoInfo() { SymAlgoCode = SymAlgoCode.ChaCha20, Description="ChaCha20 (256-bit Key) KeePass Implementation", IVSize = 12, KeySize = 32 },
                new SymAlgoInfo() { SymAlgoCode = SymAlgoCode.Salsa20, Description="Salsa20 (256-bit Key) KeePass Implementation", IVSize = 8, KeySize = 32 },
                new SymAlgoInfo() { SymAlgoCode = SymAlgoCode.Twofish, Description="Twofish (256-bit Key) Josip Medved", IVSize = 16, KeySize = 32 }
            };

        }

        private SymAlgoCode m_SymAlgo;
        private byte[] m_Key;
        private byte[] m_IV;
        private SymAlgoInfo m_AlgoInfo;

        public CipherInfo(SymAlgoCode Algo)
        {
            m_SymAlgo = Algo;
            m_Key = null;
            m_IV = null;

            foreach (SymAlgoInfo AlgoInfo in List)
                if (AlgoInfo.SymAlgoCode == Algo)
                {
                    m_AlgoInfo = AlgoInfo;
                    break;
                }

            Debug.Assert(m_AlgoInfo != null);
            if (m_AlgoInfo == null) throw new SecurityException("Invalid Algorithm");
        }

        public uint IVSizeInBytes { get { return m_AlgoInfo.IVSize; } }
        public uint KeySizeInBytes { get { return m_AlgoInfo.KeySize; } }


        public void SetKey(byte[] Key32, byte[] IV)
        {
            Debug.Assert(Key32 != null && Key32.Length == 32);
            Debug.Assert(IV != null);

            m_IV = new byte[IVSizeInBytes];
            m_Key = new byte[KeySizeInBytes];

            Array.Copy(IV, m_IV, m_IV.Length);
            Array.Copy(Key32, m_Key, m_Key.Length);

        }

        public ISingleCipherTransform GetCipherTransformer()
        {                          
            if (m_SymAlgo == SymAlgoCode.AES256 || m_SymAlgo == SymAlgoCode.ThreeDES || m_SymAlgo == SymAlgoCode.Twofish)
            {
                SymmetricAlgorithm SymAlgo;

                if (m_SymAlgo == SymAlgoCode.ThreeDES)
                {
                    SymAlgo = new TripleDESCryptoServiceProvider
                    {
                        BlockSize = 64,
                        IV = m_IV,
                        KeySize = 192,
                        Key = m_Key,
                        Mode = CipherMode.CBC,
                        Padding = PaddingMode.None
                    };


                }
                else if (m_SymAlgo == SymAlgoCode.AES256)
                    SymAlgo = new RijndaelManaged
                    {
                        BlockSize = 128,
                        IV = m_IV,
                        KeySize = 256,
                        Key = m_Key,
                        Mode = CipherMode.CBC,
                        Padding = PaddingMode.None
                    };
                else
                    SymAlgo = new TwofishManaged
                    {
                        BlockSize = 128,
                        IV = m_IV,
                        KeySize = 256,
                        Key = m_Key,
                        Mode = CipherMode.CBC,
                        Padding = PaddingMode.None
                    };

                return new CryptoTransformer(SymAlgo);
            }
            else if (m_SymAlgo == SymAlgoCode.ChaCha20 || m_SymAlgo == SymAlgoCode.Salsa20)
            {
                CtrBlockCipher c;

                if (m_SymAlgo == SymAlgoCode.ChaCha20)
                    c = new ChaCha20Cipher(m_Key, m_IV);
                else
                    c = new Salsa20Cipher(m_Key, m_IV);

                return new CtrBlockCipherTransformer(c);
            }
            

            throw new SecurityException("Invalid Algorithm");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }

        public virtual void Dispose(bool Disposing)
        {
            if (Disposing)
            {
                if (m_Key != null) Array.Clear(m_Key, 0, m_Key.Length);
                if (m_IV != null)  Array.Clear(m_IV, 0, m_IV.Length);
            }
        }
    }

    internal class SymAlgoInfo
    {
        public SymAlgoCode SymAlgoCode;
        public string Description;
        public byte IVSize;
        public byte KeySize;
    }

    internal enum SymAlgoCode
    {
        [Description("AES-256")]
        AES256 = 1,
        [Description("3DES-192")]
        ThreeDES = 2,
        [Description("ChaCha20 ")]
        ChaCha20 = 3,
        [Description("Salsa20")]
        Salsa20 = 4,
        [Description("Twofish")]
        Twofish = 5
    }
}
