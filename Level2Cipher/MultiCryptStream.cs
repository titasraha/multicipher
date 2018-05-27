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

namespace MultiCipher
{
    public class MultiCryptStream : Stream
    {
        
        protected Stream m_sBaseStream;
        protected bool m_bWriting;
        protected Algorithm m_algo;


        protected MultiCryptStream(Stream sbaseStream, bool bWriting)
        {
            m_sBaseStream = sbaseStream;
            m_bWriting = bWriting;
        }

        public override bool CanRead
        {
            get { return !m_bWriting; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return m_bWriting; }
        }

        public override void Flush()
        {
            m_sBaseStream.Flush();
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
                throw new NotSupportedException();
            }
        }


        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }



        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }


    }

    public enum Algorithm { AES_3DES = 0 };
}
