/*

    MultiCrypt Plugin for Keepass Password Safe
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security;
using System.Security.Cryptography;


namespace WindowsFormsApplication1
{

    // Test Class only
    // Intended to replicate the None padding with the standard tranform function
    class CryptStream:Stream
    {

        private const int BLOCK_SIZE_BYTES = 16;
        private byte[] m_RemainderBytesBuffer;
        private int m_RemainderBytePos;
        private ICryptoTransform m_transformer;
        private MemoryStream m_sBufferedStream;
        private Stream m_sBaseStream;
        private bool bWriting;
        private bool bFlushed = false;

        public CryptStream(Stream baseStream, ICryptoTransform transformer, CryptoStreamMode cMode)
        {           
            m_sBaseStream = baseStream;
            m_RemainderBytesBuffer = new byte[BLOCK_SIZE_BYTES];
            m_RemainderBytePos = 0;
            m_transformer = transformer;
            bWriting = (cMode == CryptoStreamMode.Write);
        }

        public override bool CanRead
        {
            get { return !bWriting; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return bWriting; }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Length
        {
            get { return m_sBaseStream.Length; }
        }

        public override long Position
        {
            get
            {
                return m_sBaseStream.Position;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public void FlushFinalBlock()
        {
            byte[] OutputBuffer = new byte[BLOCK_SIZE_BYTES];

            int rem = BLOCK_SIZE_BYTES - m_RemainderBytePos;

            for (int i = m_RemainderBytePos; i < BLOCK_SIZE_BYTES; i++)
                m_RemainderBytesBuffer[i] = (byte)rem;
            m_transformer.TransformBlock(m_RemainderBytesBuffer, 0, BLOCK_SIZE_BYTES, OutputBuffer, 0);
            m_sBaseStream.Write(OutputBuffer, 0, BLOCK_SIZE_BYTES);            
            bFlushed = true;
        }

        public override void Close()
        {
            if (!bFlushed)
                FlushFinalBlock();
            m_sBaseStream.Close();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (count <= 0)
                return;

            int RemainderCount = count % BLOCK_SIZE_BYTES;
            int BlockCount = count / BLOCK_SIZE_BYTES;
            int CopyCount;

            // fill the remainder buffer
            if (BlockCount > 0 || (RemainderCount + m_RemainderBytePos > BLOCK_SIZE_BYTES))
                CopyCount = BLOCK_SIZE_BYTES - m_RemainderBytePos;
            else
                CopyCount = RemainderCount;

            if (CopyCount > 0)
            {
                Array.Copy(buffer, offset, m_RemainderBytesBuffer, m_RemainderBytePos, CopyCount);

                offset += CopyCount;
                count -= CopyCount;
                m_RemainderBytePos += CopyCount;
            }

            byte[] OutputBuffer = new byte[BLOCK_SIZE_BYTES];

            while (count > 0)
            {

                m_transformer.TransformBlock(m_RemainderBytesBuffer, 0, BLOCK_SIZE_BYTES, OutputBuffer, 0);
                m_sBaseStream.Write(OutputBuffer, 0, BLOCK_SIZE_BYTES);

                if (count > BLOCK_SIZE_BYTES)
                    Array.Copy(buffer, offset, m_RemainderBytesBuffer, 0, BLOCK_SIZE_BYTES);
                else
                {
                    Array.Copy(buffer, offset, m_RemainderBytesBuffer, 0, count);
                    m_RemainderBytePos = count;  // guarenteed to set this value before while loop exit
                }

                count -= BLOCK_SIZE_BYTES;
                offset += BLOCK_SIZE_BYTES;
            }
        }
    }
}
