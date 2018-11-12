using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TrafficViewerSDK
{
    /// <summary>
    /// Special encoding that converts to unicode by default
    /// </summary>
    public class ModifiedExtendedASCIIEncoding : Encoding
    {
        
        private const string SPECIAL_CHAR_FORMAT = @"\x{0:x2}";
        private const int SPECIAL_CHAR_LEN = 4;
        private byte[] _cachedBytes = null;

        /// <summary>
        /// Gets the byte count
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int GetByteCount(char[] chars, int index, int count)
        {
            byte[] buffer = new byte[chars.Length];
            _cachedBytes = null;
            return this.GetBytes(chars, index, count, buffer, 0);
        }

        /// <summary>
        /// Gets the bytes
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="charIndex"></param>
        /// <param name="charCount"></param>
        /// <param name="bytes"></param>
        /// <param name="byteIndex"></param>
        /// <returns></returns>
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {

            byte[] localBytes;

            List<byte> byteList = new List<byte>();
            int index = 0, subIndex = 0;
            char c;

            while (index < chars.Length)
            {
                c = chars[index];
                if (c == '\\')
                {
                    subIndex = index;
                    StringBuilder sb = new StringBuilder();
                    while (sb.Length < SPECIAL_CHAR_LEN && subIndex < chars.Length)
                    {
                        sb.Append(chars[subIndex]);
                        subIndex++;
                    }
                    string number = sb.ToString();
                    if (number.Length == SPECIAL_CHAR_LEN && number[1] == 'x') //\x
                    {
                        byte ord;
                        if (byte.TryParse(number.Substring(2), NumberStyles.HexNumber, null, out ord))
                        {
                            byteList.Add(ord);
                            index = subIndex;
                            continue;
                        }
                    }
                }

                byteList.Add((byte)c);
                index++;

            }

            localBytes = byteList.ToArray();

            Array.Copy(localBytes, 0, bytes, byteIndex, localBytes.Length);
            return localBytes.Length;
        }

        /// <summary>
        /// Get the number of chars
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            
            int finalCount = 0;
            foreach (byte b in bytes)
            {
                if (IsWritable(b))
                {
                    finalCount++;
                }
                else
                {
                    finalCount += SPECIAL_CHAR_LEN;
                }

            }
            return finalCount;
        }

        private static bool IsWritable(byte c)
        {

            return c == 9 || c == 10 || c == 13 || (c > 31 && c < 127);
        }

        /// <summary>
        /// Gets the chars
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="byteIndex"></param>
        /// <param name="byteCount"></param>
        /// <param name="chars"></param>
        /// <param name="charIndex"></param>
        /// <returns></returns>
        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
           
            
            StringBuilder sb = new StringBuilder(byteCount);

            for (int index = 0; index < byteCount; index++)
            {
                byte b = bytes[index];
                if (IsWritable(b))
                {
                    sb.Append((char)b);
                }
                else
                {
                    sb.Append(String.Format(SPECIAL_CHAR_FORMAT, b));
                }

            }
            char [] localChars = sb.ToString().ToCharArray();
            Array.Copy(localChars, 0, chars, charIndex, localChars.Length);
            return localChars.Length;
        }

        /// <summary>
        /// Get the max bytes supported for the specified chars
        /// </summary>
        /// <param name="charCount"></param>
        /// <returns></returns>
        public override int GetMaxByteCount(int charCount)
        {
            return charCount;
        }

        /// <summary>
        /// Gets the max chars
        /// </summary>
        /// <param name="byteCount"></param>
        /// <returns></returns>
        public override int GetMaxCharCount(int byteCount)
        {
            return byteCount * SPECIAL_CHAR_LEN;
        }


    }
}
