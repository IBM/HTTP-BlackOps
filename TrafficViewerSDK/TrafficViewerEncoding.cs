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
	public class TrafficViewerEncoding : Encoding
	{
		private const string SPECIAL_CHAR_FORMAT = @"\x{0:x4}";
		private const int SPECIAL_CHAR_LEN = 6;
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
			byte[] buffer = new byte[chars.Length * 2];
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
			if (_cachedBytes != null)
			{
				localBytes = _cachedBytes;
			}
			else
			{
				List<char> localChars = new List<char>();
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
							long ord;
							if (long.TryParse(number.Substring(2), NumberStyles.HexNumber, null, out ord))
							{
								localChars.Add(System.Convert.ToChar(ord));
								index = subIndex;
								continue;
							}
						}
					}

					localChars.Add(c);
					index++;

				}

				localBytes = Encoding.UTF8.GetBytes(localChars.ToArray());
			}
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
			char[] utf8Chars = Encoding.UTF8.GetChars(bytes);
			int finalCount = 0;
			foreach (char c in utf8Chars)
			{
				if (IsWritable(c))
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

		private static bool IsWritable(char c)
		{
            
			return char.IsLetterOrDigit(c) 
                //|| char.IsSeparator(c) 
                || char.IsPunctuation(c) 
                || char.IsSymbol(c) 
                || char.IsWhiteSpace(c);
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
			char[] localChars = new char[chars.Length];

			int writtenChars = Encoding.UTF8.GetChars(bytes, byteIndex, byteCount, localChars, charIndex);

			StringBuilder sb = new StringBuilder(localChars.Length);

			for (int index = 0; index < writtenChars; index++)
			{
				char c = localChars[index];
				if (IsWritable(c))
				{
					sb.Append(c);
				}
				else
				{
					int numericValue = System.Convert.ToInt32(c);

					sb.Append(String.Format(SPECIAL_CHAR_FORMAT, numericValue));
				}

			}
			localChars = sb.ToString().ToCharArray();
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
			return Encoding.UTF8.GetMaxByteCount(charCount);
		}

		/// <summary>
		/// Gets the max chars
		/// </summary>
		/// <param name="byteCount"></param>
		/// <returns></returns>
		public override int GetMaxCharCount(int byteCount)
		{
			return Encoding.UTF8.GetMaxCharCount(byteCount) / 4;
		}


	}
}
