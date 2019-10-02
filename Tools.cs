using KeePassLib.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiCipher
{
    public static class Tools
    {

        public static ProtectedString BytesToHexString(ProtectedBinary bin)
        {
            byte[] raw = bin.ReadData();
            byte[] rawUTF8 = new byte[raw.Length * 2];
            for (int i=0; i< raw.Length; i++)
            {
                byte ByteLow = (byte)(raw[i] & 0xF);
                if (ByteLow < 10)
                    ByteLow += 0x30;
                else
                    ByteLow += 0x37;
                rawUTF8[i * 2 + 1] = ByteLow;

                byte ByteHigh = (byte)(raw[i] >> 4);
                if (ByteHigh < 10)
                    ByteHigh += 0x30;
                else
                    ByteHigh += 0x37;
                rawUTF8[i * 2] = ByteHigh;
            }

            Array.Clear(raw, 0, raw.Length);
            ProtectedString s = new ProtectedString(true, rawUTF8);
            Array.Clear(rawUTF8, 0, rawUTF8.Length);
            return s;
        }

        public static ProtectedBinary HexStringToBytes(ProtectedString str, byte MaxBytes)
        {
            var secretBytesRaw = str.ReadUtf8();
            byte[] secretBytes = new byte[MaxBytes];


            try
            {

                int FillCounter = 0;
                int Idx = 0;
                int HexData = 0;

                for (int i = 0; i < secretBytesRaw.Length; i++)
                {
                    if (Idx >= secretBytes.Length)
                        throw new FormatException("Invalid Hex String");

                    FillCounter += 1;

                    byte b = secretBytesRaw[i];

                    if (b > 0x60 && b < 0x67)
                        HexData = (HexData << 4) | (b - 0x57);
                    else if (b > 0x40 && b < 0x47)
                        HexData = (HexData << 4) | (b - 0x37);
                    else if (b > 0x2f && b < 0x3a)
                        HexData = (HexData << 4) | (b - 0x30);
                    else if (b == 0x2d || b == 0x020)  // Ignore ' ' and '-'
                        FillCounter -= 1;
                    else
                        throw new FormatException("Invalid Hex String");

                    if (FillCounter == 2)
                    {
                        secretBytes[Idx++] = (byte)HexData;
                        FillCounter = 0;
                        HexData = 0;
                    }

                }

                if (Idx != secretBytes.Length)
                    throw new FormatException("Invalid Hex String");

                return new ProtectedBinary(true, secretBytes);
                //DialogResult = DialogResult.OK;
            }
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //    DialogResult = DialogResult.None;
            //    txtPassword.Focus();
            //    return;
            //}
            finally
            {
                Array.Clear(secretBytesRaw, 0, secretBytesRaw.Length);
                Array.Clear(secretBytes, 0, secretBytes.Length);
            }
        }

    }
}
