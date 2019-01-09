﻿/*

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
using KeePassLib.Cryptography;
using KeePassLib.Cryptography.KeyDerivation;
using KeePassLib.Keys;
using KeePassLib.Security;
using KeePassLib.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Security.Cryptography;

namespace MultiCipher
{
    internal class DualCipherStream: MultiCipherStream
    {

        // Read
        private MemoryStream m_ReadPlainTextStream;

        // Write
        private List<byte[]> m_WriteDataBytesList;
        private int m_WriteBytesLength;
        
        // Common
        private Configuration m_Config;
        private byte[] m_1Key32;
        private byte[] m_1IV16;
        private CompositeKey m_2Key;
        private bool Is_Disposed;


        /// <summary>
        /// Construct for Reading/Writing
        /// </summary>
        /// <param name="Config">Configuration information</param>
        /// <param name="pbKey32">32 Byte Key provided by Keepass</param>
        /// <param name="pbIV16">16 Byte Key provided by Keepass</param>
        /// <param name="sbaseStream">Underlining stream to read/write to</param>
        /// <param name="bWriting">Indicate Read or Write</param>
        public DualCipherStream(
            Configuration Config,
            byte[] pbKey32, 
            byte[] pbIV16,
            Stream sbaseStream, 
            bool bWriting) : base(sbaseStream, bWriting)
        {
            Is_Disposed = false;
            m_Config = Config;
            m_1Key32 = pbKey32;
            m_1IV16 = pbIV16;
            m_ReadPlainTextStream = null;
            m_WriteDataBytesList = null;


            if (bWriting)
                InitWrite();
            else
                InitRead();           
        }

        private void InitWrite()
        {

            if (m_Config.KeyOption == KeyOption.DualPassword)
            {
                m_2Key = m_Config.GetNewDualPassword(false);
                if (m_2Key == null)
                    throw new Exception("Unable to encrypt data");
            }
            else
                m_2Key = DeriveDualKey(m_Config.Host.Database.MasterKey);

            m_WriteBytesLength = 0;
            m_WriteDataBytesList = new List<byte[]>();
        }

        private void InitRead()
        {
            CipherInfo Cipher1 = null;
            CipherInfo Cipher2 = null;

            byte[] Key2 = null;
            byte[] IV2 = null;

            try
            {
                // File format version has been read
                var Subversion = m_sBaseStream.ReadByte(); // Subversion 
                if (Subversion != 0)
                    throw new InvalidDataException("Invalid subversion");

                m_Config.Algorithm1 = (SymAlgoCode)m_sBaseStream.ReadByte();
                Cipher1 = new CipherInfo(m_Config.Algorithm1);
                Cipher1.SetKey(m_1Key32, m_1IV16);

                m_Config.Algorithm2 = (SymAlgoCode)m_sBaseStream.ReadByte();
                Cipher2 = new CipherInfo(m_Config.Algorithm2);

                m_Config.KeyOption = (KeyOption)m_sBaseStream.ReadByte();

                if (m_Config.KeyOption == KeyOption.DualPassword)
                    m_2Key = m_Config.GetDualPassword();
                else
                    m_2Key = DeriveDualKey(m_Config.Host.Database.MasterKey);

                if (m_2Key == null)
                    throw new SecurityException("Invalid Key");

                m_sBaseStream.ReadByte(); // Derivation Method ignore for now


                var MasterSeed = new byte[32];
                m_sBaseStream.Read(MasterSeed, 0, 32);

                var TransformSeed = new byte[32];
                m_sBaseStream.Read(TransformSeed, 0, 32);

                IV2 = new byte[Cipher2.IVSizeInBytes];
                m_sBaseStream.Read(IV2, 0, (int)Cipher2.IVSizeInBytes);

                byte[] NumRoundByteArray = new byte[8];
                m_sBaseStream.Read(NumRoundByteArray, 0, 8);
                ulong NumRounds = Extensions.ToLittleEndianUInt64(NumRoundByteArray);

                m_Config.Key2Transformations = NumRounds;

                byte[] PlainTextLengthBytes = new byte[4];
                m_sBaseStream.Read(PlainTextLengthBytes, 0, 4);
                
                var PlainTextLength = Extensions.ToLittleEndianInt32(PlainTextLengthBytes);

                int ContentBufferLength = Get64BlockAlignSize(PlainTextLength);
                byte[] PlainTextBuffer = new byte[ContentBufferLength];

                
                int read = m_sBaseStream.Read(PlainTextBuffer, 0, ContentBufferLength);
                if (read != ContentBufferLength)
                    throw new InvalidDataException("Invalid Data length");

                using (var Transformer = Cipher1.GetCipherTransformer())
                    Transformer.Decrypt(PlainTextBuffer, 0, ContentBufferLength);

                var PlainTextHash32 = new SHA256Managed();
                PlainTextHash32.TransformFinalBlock(PlainTextBuffer, 0, PlainTextLength);

                byte[] ContentBuffer2 = new byte[ContentBufferLength];               
                                
                read = m_sBaseStream.Read(ContentBuffer2, 0, ContentBufferLength);
                if (read != ContentBufferLength)
                    throw new InvalidDataException("Invalid Data length 2");

                Key2 = GetKey32(PlainTextHash32.Hash, MasterSeed, TransformSeed, NumRounds);
                Cipher2.SetKey(Key2, IV2);


                try
                {
                    using (var Transformer2 = Cipher2.GetCipherTransformer())
                        Transformer2.Decrypt(ContentBuffer2, 0, ContentBufferLength);

                    for (int i = 0; i < PlainTextLength; i++)
                        PlainTextBuffer[i] ^= ContentBuffer2[i];
                }
                finally
                {
                    Array.Clear(ContentBuffer2, 0, ContentBuffer2.Length);
                }


                m_ReadPlainTextStream = new MemoryStream(PlainTextBuffer, 0, PlainTextLength, false, true);
            }
            finally
            {
                if (Key2 != null) MemUtil.ZeroByteArray(Key2);
                if (IV2 != null) MemUtil.ZeroArray(IV2);

                if (Cipher1 != null) Cipher1.Dispose();
                if (Cipher2 != null) Cipher2.Dispose();
            }
        }


        private CompositeKey DeriveDualKey(CompositeKey MasterKey)
        {
            CompositeKey NewKey = new CompositeKey();
            foreach (var Key in MasterKey.UserKeys)
                NewKey.AddUserKey(Key);

            NewKey.AddUserKey(new KcpPassword("TR"));
            return NewKey;
        }

        // use 64 byte block align
        private int Get64BlockAlignSize(int Length)
        {            
            int Remainder = Length % 64;
            if (Remainder == 0)
                return Length;

            return Length + 64 - Remainder;
        }

        /// <summary>
        /// Generates the key based on m_2Key, m_PlainTextHash32 and the seed parameters
        /// </summary>
        /// <param name="MasterSeed">Seed used to generate the key from m_2Key</param>
        /// <param name="TransformSeed">Seed for key transformation</param>
        /// <param name="NumRounds">Iteration count of transformation</param>
        /// <returns></returns>
        private byte[] GetKey32(byte[] Hash, byte[] MasterSeed, byte[] TransformSeed, ulong NumRounds)
        {
            byte[] GeneratedKey = new byte[32];

            byte[] HashBuffer = new byte[32 + 32 + 32]; // MasterSeed + DualKey + FirstStreamHash
            byte[] Key256Bits = null;
            byte[] pKey32 = null;           

            try
            {
                Array.Copy(MasterSeed, 0, HashBuffer, 0, 32);

                KdfParameters KdfParams = new AesKdf().GetDefaultParameters();
                KdfParams.SetUInt64(AesKdf.ParamRounds, NumRounds);
                KdfParams.SetByteArray(AesKdf.ParamSeed, TransformSeed);


                ProtectedBinary pbinKey = m_2Key.GenerateKey32(KdfParams);
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

        public override int Read(byte[] buffer, int offset, int count)
        {
            Debug.Assert(!m_bWriting);
            if (m_bWriting) throw new InvalidOperationException();

            if (Is_Disposed) throw new ObjectDisposedException("DualCipherStream");

            int nRead = m_ReadPlainTextStream.Read(buffer, offset, count);

            return nRead;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Debug.Assert(m_bWriting);
            if (!m_bWriting) throw new InvalidOperationException();

            if (Is_Disposed) throw new ObjectDisposedException("DualCipherStream");

            if (count < 0)
                throw new IOException("Length to big");

            if (count == 0)
                return;

            byte[] bytes = new byte[count];
            Array.Copy(buffer, offset, bytes, 0, count);

            // MemoryStream does not clear memory on resize, so use a list<bytes[]>
            m_WriteDataBytesList.Add(bytes);
            m_WriteBytesLength += count;

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !Is_Disposed)
            {
                if (m_bWriting)
                {                  
                    byte[] Key2 = null;
                    byte[] IV2 = null;
                    CipherInfo Cipher1 = null;
                    CipherInfo Cipher2 = null;

                    try
                    {
                        if (m_sBaseStream == null)
                            throw new IOException("Stream not open");

                        // First encryption
                        Cipher1 = new CipherInfo(m_Config.Algorithm1);
                        Cipher1.SetKey(m_1Key32, m_1IV16);

                        int BufferSize = Get64BlockAlignSize(m_WriteBytesLength);

                        // Make sure we have a buffer that is 64 byte aligned
                        byte[] PlainTextBuffer = new byte[BufferSize];
                        int Pos = 0;

                        // Dump the collected data to this buffer
                        foreach (byte[] Bytes in m_WriteDataBytesList)
                        {
                            Array.Copy(Bytes, 0, PlainTextBuffer, Pos, Bytes.Length);
                            Array.Clear(Bytes, 0, Bytes.Length);
                            Pos += Bytes.Length;
                        }
                        m_WriteDataBytesList = null;

                        var RndGenerator = CryptoRandom.Instance;
                        byte[] RandomDataBytes = RndGenerator.GetRandomBytes((uint)BufferSize);

                        // XOR the plaintext with random data
                        for (int i = 0; i < BufferSize; i++)
                            PlainTextBuffer[i] ^= RandomDataBytes[i];

                        // Calculate hash of data part of XORred buffer
                        var XORedTextHash32 = new SHA256Managed();
                        XORedTextHash32.TransformFinalBlock(PlainTextBuffer, 0, m_WriteBytesLength);

                        // First encrypted buffer
                        var Encrypted1Buffer = PlainTextBuffer;
                        using (var Transformer = Cipher1.GetCipherTransformer())
                            Transformer.Encrypt(Encrypted1Buffer, 0, BufferSize);
                        

                        // 2nd Cipher initialization information          
                        Cipher2 = new CipherInfo(m_Config.Algorithm2);
                        byte[] MasterSeed = RndGenerator.GetRandomBytes(32);
                        byte[] TransformSeed = RndGenerator.GetRandomBytes(32);
                        IV2 = RndGenerator.GetRandomBytes(Cipher2.IVSizeInBytes);
                        ulong NumRounds = m_Config.Key2Transformations;

                        Key2 = GetKey32(XORedTextHash32.Hash, MasterSeed, TransformSeed, NumRounds);
                        Cipher2.SetKey(Key2, IV2);

                        ///////  Start writing to base stream //////////
                        
                        m_sBaseStream.WriteByte(2);  // File Version 2
                        m_sBaseStream.WriteByte(0);  // Sub Version value currently 0
                        m_sBaseStream.WriteByte((byte)m_Config.Algorithm1);
                        m_sBaseStream.WriteByte((byte)m_Config.Algorithm2);
                        m_sBaseStream.WriteByte((byte)m_Config.KeyOption);
                        m_sBaseStream.WriteByte(1);   // Key derivation method

                        m_sBaseStream.Write(MasterSeed, 0, 32);
                        m_sBaseStream.Write(TransformSeed, 0, 32);
                        m_sBaseStream.Write(IV2, 0, IV2.Length);
                        m_sBaseStream.Write(Extensions.GetLittleEndianBytes(NumRounds), 0, 8);
                        m_sBaseStream.Write(Extensions.GetLittleEndianBytes(m_WriteBytesLength), 0, 4); // Write the length

                        m_sBaseStream.Write(Encrypted1Buffer, 0, BufferSize);

                        using (var Transformer = Cipher2.GetCipherTransformer())
                            Transformer.Encrypt(RandomDataBytes, 0, BufferSize);

                        m_sBaseStream.Write(RandomDataBytes, 0, BufferSize);



                    }
                    finally
                    {
                        if (Key2 != null) MemUtil.ZeroByteArray(Key2);
                        if (IV2 != null) MemUtil.ZeroArray(IV2);

                        if (Cipher1 != null) Cipher1.Dispose();
                        if (Cipher2 != null) Cipher2.Dispose();

                        if (m_WriteDataBytesList != null)
                            foreach (byte[] Bytes in m_WriteDataBytesList)
                                Array.Clear(Bytes, 0, Bytes.Length);
                    }


                }
                else
                {
                    if (m_ReadPlainTextStream != null)
                    {
                        // Have to clear the underlying buffer
                        byte[] buf = m_ReadPlainTextStream.GetBuffer();
                        if (buf != null)
                            MemUtil.ZeroByteArray(buf);

                        m_ReadPlainTextStream.Dispose();
                    }
                }
                Is_Disposed = true;

            }
            base.Dispose(disposing);
        }
    }
}
