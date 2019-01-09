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
using System.Collections.Generic;
using System.ComponentModel;

namespace MultiCipher
{
    internal static class Extensions
    {        
        public static string GetDescription<T>(this T enumerationValue) where T : struct
        {
            var type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("Must be of Enum type");
            }
            var memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return enumerationValue.ToString();
        }

        public static ulong ToLittleEndianUInt64(byte[] Bytes)
        {
            return BitConverter.ToUInt64(GetLittleEndian(Bytes),0);
        }

        public static int ToLittleEndianInt32(byte[] Bytes)
        {
            return BitConverter.ToInt32(GetLittleEndian(Bytes), 0);
        }

        public static byte[] GetLittleEndianBytes(ulong ULong)
        {
            return GetLittleEndian(BitConverter.GetBytes(ULong));
        }

        public static byte[] GetLittleEndianBytes(int Int)
        {
            return GetLittleEndian(BitConverter.GetBytes(Int));
        }

        private static byte[] GetLittleEndian(byte[] Source)
        {
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(Source);
            return Source;
        }
    }


}

// Extension method hack for .Net 2.0
namespace System.Runtime.CompilerServices
{
    public class ExtensionAttribute : Attribute { }
}
