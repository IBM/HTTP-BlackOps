using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Used to store Http response body
	/// </summary>
	public class HttpResponseBody : ByteArrayBuilder
	{


		private bool _isChunked = false;
		/// <summary>
		/// Gets/sets if this is a chunked response body
		/// </summary>
		public bool IsChunked
		{
			get { return _isChunked; }
			set { _isChunked = value; }
		}


		/// <summary>
		/// Overriden ToString() method gets raw body string attempting UTF8 decoding
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			Encoding encoding;
			return ToString("text/html; charset=utf-8", out encoding);
		}

		/// <summary>
		/// Overriden ToString gets the raw body using the specified content-type header
		/// </summary>
		/// <param name="contentTypeHeader"></param>
		/// <returns></returns>
		public string ToString(string contentTypeHeader)
		{
			Encoding encoding;
			return ToString(contentTypeHeader, out encoding);
		}

		/// <summary>
		/// Convets to string using the encoding specified in the content type header
		/// </summary>
		/// <param name="contentTypeHeader"></param>
		/// <param name="encoding">Outputs the encoding for further use</param>
		/// <returns></returns>
		public string ToString(string contentTypeHeader, out Encoding encoding)
		{
			string html = String.Empty;

			encoding = HttpUtil.GetEncoding(contentTypeHeader);

			Decoder decoder = encoding.GetDecoder();


			if (_chunks.Count > 0)
			{
                LinkedListNode<byte[]> currChunk = _chunks.First;
                int charSize = 0;
                do
                {
                    //get the number of unicode chars in the current secquence
                    charSize += decoder.GetCharCount(currChunk.Value, 0, currChunk.Value.Length, true);
                    currChunk = currChunk.Next;
                }
                while (currChunk != null);
                
                currChunk = _chunks.First;
                
                char[] chars = new char[charSize];
				int totalLen = 0;
				do
				{
					//get the number of unicode chars in the current secquence
					int count = decoder.GetChars(currChunk.Value, 0, currChunk.Value.Length, chars, totalLen);
					totalLen += count; //add the current char size to the total length
					currChunk = currChunk.Next;
				}
				while (currChunk != null);
				//if the totalLen is smaller after the transformation shrink the chars
				html = new String(chars, 0, totalLen);
			}
			return html;
		}

		
	}
}
