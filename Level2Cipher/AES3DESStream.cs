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

using System.Security;
using System.Security.Cryptography;

using KeePassLib;
using KeePassLib.Cryptography;
using KeePassLib.Cryptography.Cipher;
using KeePassLib.Keys;
using KeePassLib.Security;

namespace MultiCipher
{
    public class AES3DESStream: MultiCryptStream
    {               
        private SHA256Managed m_hash;

        private byte[] m_MasterSeed;
        private byte[] m_TransformSeed;
        private byte[] m_3DESIV;
        private ulong m_NumRounds;
        private CompositeKey m_2Key;


        protected MemoryStream m_RandomStream;
        protected MemoryStream m_AESStream;
        private CryptoStream m_CryptoStreamAES;

        int m_ContentLength;

        private byte[] GetKey()
        {
            byte[] ThreeDESKey = new byte[24];
            MemoryStream ms = new MemoryStream();

            ms.Write(m_MasterSeed, 0, 32);
            ProtectedBinary pbinKey = m_2Key.GenerateKey32(m_TransformSeed, m_NumRounds);
            if (pbinKey == null)
                throw new SecurityException("Invalid Key");

            byte[] pKey32 = pbinKey.ReadData();
            if ((pKey32 == null) || (pKey32.Length != 32))
                throw new SecurityException("Invalid Key Data");

            ms.Write(pKey32, 0, 32);

            byte[] sRandom = m_hash.Hash;
            ms.Write(sRandom, 0, sRandom.Length);

            SHA256Managed sha = new SHA256Managed();
            byte[] Key256 = sha.ComputeHash(ms.ToArray());

            Array.Copy(Key256, ThreeDESKey, ThreeDESKey.Length);

            ms.Close();

            Array.Clear(pKey32, 0, 32);
            Array.Clear(Key256, 0, 32);

            return ThreeDESKey;

        }

        public AES3DESStream(Stream sbaseStream, bool bWriting, byte[] AESKey, byte[] AESIV, CompositeKey Level2Key):
            base(sbaseStream, bWriting)
        {

            m_RandomStream = new MemoryStream();
            m_ContentLength = 0;
            m_hash = new SHA256Managed();
            m_2Key = Level2Key;

            ICryptoTransform AESTransformer;
                       
            RijndaelManaged r = new RijndaelManaged();
            r.BlockSize = 128;
            r.IV = AESIV;
            r.KeySize = 256;
            r.Key = AESKey;
            r.Mode = CipherMode.CBC;
            r.Padding = PaddingMode.None; // We are taking care of the padding to make sure it is within 32 byte boundary


            if (bWriting)
            {
                CryptoRandom cr = CryptoRandom.Instance;
                m_MasterSeed = cr.GetRandomBytes(32);
                m_TransformSeed = cr.GetRandomBytes(32);
                m_3DESIV = cr.GetRandomBytes(8);
                m_NumRounds = 10000;

                m_sBaseStream.WriteByte((byte)Algorithm.AES_3DES);
                m_sBaseStream.Write(m_MasterSeed, 0, 32);
                m_sBaseStream.Write(m_TransformSeed, 0, 32);
                m_sBaseStream.Write(m_3DESIV, 0, 8);
                m_sBaseStream.Write(BitConverter.GetBytes(m_NumRounds), 0, 8);


                m_AESStream = new MemoryStream();

                AESTransformer = r.CreateEncryptor();
                m_CryptoStreamAES = new CryptoStream(m_AESStream, AESTransformer, CryptoStreamMode.Write);
            }
            else
            {
                // File format version and algorithm has been read
                m_MasterSeed = new byte[32];
                sbaseStream.Read(m_MasterSeed, 0, 32);

                m_TransformSeed = new byte[32];
                sbaseStream.Read(m_TransformSeed, 0, 32);

                m_3DESIV = new byte[8];
                sbaseStream.Read(m_3DESIV, 0, 8);

                byte[] buffer = new byte[8];
                sbaseStream.Read(buffer, 0, 8);
                m_NumRounds = BitConverter.ToUInt64(buffer, 0);

                byte[] len = new byte[4];
                m_sBaseStream.Read(len, 0, 4);

                m_ContentLength = BitConverter.ToInt32(len, 0);
                int remainder = m_ContentLength % 32;

                int buflen = m_ContentLength + 32;

                if (remainder > 0)
                    buflen -= remainder;
                else
                    buflen -= 32;
                                
                byte[] AESBuffer = new byte[buflen];

                AESTransformer = r.CreateDecryptor();
                m_CryptoStreamAES = new CryptoStream(m_sBaseStream, AESTransformer, CryptoStreamMode.Read);

                int read = m_CryptoStreamAES.Read(AESBuffer, 0, buflen);

                if (read != buflen)
                    throw new InvalidDataException("Invalid Data length");

                byte[] DES3Buffer = new byte[buflen];

                m_hash.TransformFinalBlock(AESBuffer, 0, buflen);

                CryptoStream Stream3DES = new CryptoStream(m_sBaseStream, Get3DES().CreateDecryptor(), CryptoStreamMode.Read);
                read = Stream3DES.Read(DES3Buffer, 0, buflen);

                if (read != buflen)
                    throw new InvalidDataException("Invalid Data length 2");

                for (int i = 0; i < m_ContentLength; i++)
                    AESBuffer[i] ^= DES3Buffer[i];
               
                m_AESStream = new MemoryStream(AESBuffer, 0, m_ContentLength);

            }
        }

        private TripleDESCryptoServiceProvider Get3DES()
        {
            TripleDESCryptoServiceProvider ThreeDES = new TripleDESCryptoServiceProvider();
            ThreeDES.BlockSize = 64;
            ThreeDES.IV = m_3DESIV;
            ThreeDES.KeySize = 192;
            ThreeDES.Key = GetKey();
            ThreeDES.Mode = CipherMode.CBC;
            ThreeDES.Padding = PaddingMode.None;
            return ThreeDES;
        }

        public override void Close()
        {
            if (m_sBaseStream == null)
                return;

            if (m_bWriting)
            {
            
                int blockremainder = 32 - (m_ContentLength % 32);
                if (blockremainder < 32)
                {
                    // keep in block boundary
                    byte[] randombytes = CryptoRandom.Instance.GetRandomBytes((uint)blockremainder);
                    m_CryptoStreamAES.Write(randombytes, 0, blockremainder);
                    m_CryptoStreamAES.FlushFinalBlock();
                    m_hash.TransformBlock(randombytes, 0, blockremainder, randombytes, 0);

                    randombytes = CryptoRandom.Instance.GetRandomBytes((uint)blockremainder);
                    m_RandomStream.Write(randombytes, 0, blockremainder);                   
                }

                m_hash.TransformFinalBlock(new byte[0], 0, 0);               
                                             

                ///////  Start writing to base stream //////////
                m_sBaseStream.Write(BitConverter.GetBytes(m_ContentLength), 0, 4); // write the length

                m_AESStream.WriteTo(m_sBaseStream);

                CryptoStream CryptoStream3DES;

                CryptoStream3DES = new CryptoStream(m_sBaseStream, Get3DES().CreateEncryptor(), CryptoStreamMode.Write);

                m_RandomStream.WriteTo(CryptoStream3DES);
                CryptoStream3DES.FlushFinalBlock();
                CryptoStream3DES.Close();                               

                m_CryptoStreamAES.Close();                
            }
            m_sBaseStream.Close();
            m_sBaseStream = null;
            
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (m_bWriting) throw new InvalidOperationException();

            int nRead = m_AESStream.Read(buffer, offset, count);

            return nRead;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!m_bWriting) throw new InvalidOperationException();

            if (count > 0)
            {

                byte[] RandomDataBytes = CryptoRandom.Instance.GetRandomBytes((uint)count);
                byte[] SourceBuffer = new byte[count];

                Array.Copy(buffer, offset, SourceBuffer, 0, count);
                for (int i = 0; i < count; i++)
                    SourceBuffer[i] ^= RandomDataBytes[i];

                m_CryptoStreamAES.Write(SourceBuffer, 0, count);
                m_hash.TransformBlock(SourceBuffer, 0, count, SourceBuffer, 0);
                m_RandomStream.Write(RandomDataBytes, 0, count);
                m_ContentLength += count;
            }
            
        }

    }
}
