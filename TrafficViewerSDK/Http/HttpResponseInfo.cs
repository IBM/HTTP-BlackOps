using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Globalization;
using TrafficViewerSDK.Options;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// 
	/// </summary>
	public class HttpResponseInfo
	{
		private const byte LF = 10;
		private const byte CR = 13;
		private int ESTIMATED_LINE_SIZE = 1024;
		private const int STATUS_START = 9;

		private const string CONNECTION_HEADER = "Connection";

		/// <summary>
		/// Constructor
		/// </summary>
		public HttpResponseInfo() { }

		/// <summary>
		/// Constructs a response info from the specified string
		/// </summary>
		/// <param name="rawResponse"></param>
		public HttpResponseInfo(string rawResponse)
		{
			ProcessResponse(rawResponse);
		}
		
		/// <summary>
		/// Constructs a response info from the specified bytes
		/// </summary>
		/// <param name="responseBytes"></param>
		public HttpResponseInfo(byte[] responseBytes)
		{
			ProcessResponse(responseBytes);
		}

		/// <summary>
		/// Optimized function to obtain the status code
		/// </summary>
		/// <param name="rawResponse"></param>
		/// <returns></returns>
		public static string GetResponseStatus(byte[] rawResponse)
		{
			int j = STATUS_START, count = rawResponse.Length;
			StringBuilder statusBuilder = new StringBuilder();
			//append the digits
			char c;
			while (j < count)
			{
				c = (char)rawResponse[j];
				if (Char.IsDigit(c))
				{
					statusBuilder.Append(c);
					j++;
				}
				else
				{
					break;
				}
			}

			return statusBuilder.ToString();
		}


		/// <summary>
		/// Optimized function to obtain the status code
		/// </summary>
		/// <param name="rawResponse"></param>
		/// <returns></returns>
		public static string GetResponseStatus(string rawResponse)
		{
			int j = STATUS_START, count = rawResponse.Length;
			StringBuilder statusBuilder = new StringBuilder();
			//append the digits
			while (j < count && Char.IsDigit(rawResponse[j])) { statusBuilder.Append(rawResponse[j]); j++; }

			return statusBuilder.ToString();
		}


		/// <summary>
		/// Parses the raw response
		/// </summary>
		/// <param name="fullResponse"></param>
		public void ProcessResponse(byte[] fullResponse)
		{
			//reset all properties
			_statusLine = String.Empty;
			_headers = new HTTPHeaders();
			_responseBody = null;
			byte[] lineBytes;
			string line;
			_responseBody = new HttpResponseBody();

			MemoryStream ms = new MemoryStream(fullResponse);
			while (true)
			{
				lineBytes = Utils.ReadLine(ms, ESTIMATED_LINE_SIZE, LineEnding.Any);

				if (lineBytes != null)
				{
					line = Constants.DefaultEncoding.GetString(lineBytes);
				}
				else
				{
					return; //end of response
				}

				if (line == String.Empty)
				{
					if (_status != 100)
					{
						break; //end of headers
					}
					else
					{
						//the response status was 100 Continue expect a new set of headers and a response line
						//clear the current information
						_headers = new HTTPHeaders();
						_statusLine = String.Empty;
						continue;
					}
				}

				if (_statusLine == String.Empty)
				{
					_statusLine = line;
					int j = STATUS_START, count = _statusLine.Length;
					while (j<count && !Char.IsDigit(_statusLine[j])) j++; //for some data coming from logs or files the file has some additional characters
					StringBuilder statusBuilder = new StringBuilder();
					//append the digits

					while (j < count && Char.IsDigit(_statusLine[j])) { statusBuilder.Append(_statusLine[j]); j++; }

					int.TryParse(statusBuilder.ToString(), out _status);

				}
				else
				{
					//this is a header split it 
					string[] header = line.Split(new char[1] { ':' }, 2);
					if (header.Length > 1)
					{
						_headers.Add(header[0], header[1]);
					}
				}
			}

			long contentLength = ms.Length - ms.Position;


			//find out if the response is chunked
			List<HTTPHeader> transferEncoding = _headers.GetHeaders("Transfer-Encoding");

			if (transferEncoding.Count > 0 && String.Compare(transferEncoding[0].Values[0], "chunked", true) == 0)
			{
				_responseBody.IsChunked = true;
				//save the ms position in case something happens
				long msPos = ms.Position;
				try
				{
					ProcessChunkedResponse(ms);
				}
				catch
				{
					//reset the ms position to what it was before the process chunk failed
					ms.Position = msPos;
					ProcessChunkedResponseHeuristically(ms);
				}
				
				//finish by setting the content length to the total size of the chunks
				contentLength = _responseBody.Length;
			}
			else
			{
				if (contentLength > 0)
				{
					//add one big chunk with the whole response
					_responseBody.IsChunked = false;
					byte[] fullArray = new byte[contentLength];
					ms.Read(fullArray, 0, (int)contentLength);
					_responseBody.AddChunkReference(fullArray, fullArray.Length);
				}
				//do not update the content length header with the current content length
				//like this _headers["Content-Length"] = _contentLength.ToString();
                //we don't know if the response body size will change
			}



		}

		/// <summary>
		/// Reads a chunked response
		/// </summary>
		/// <param name="ms"></param>
		private void ProcessChunkedResponse(MemoryStream ms)
		{
			byte[] lineBytes;
			string line;
			do
			{
				lineBytes = Utils.ReadLine(ms, ESTIMATED_LINE_SIZE, LineEnding.Any);
				if (lineBytes != null)
				{
					line = Constants.DefaultEncoding.GetString(lineBytes);

					int scIndex = line.IndexOf(';');
					if (scIndex > -1)
					{
						line = line.Substring(0, scIndex);
					}

					int chunkSize;
					if (line != String.Empty && Uri.IsHexDigit(line[0]) && Int32.TryParse(line, NumberStyles.HexNumber, null, out chunkSize))
					{
						//read the specified chunk of bytes
						byte[] chunk = new byte[chunkSize];
						ms.Read(chunk, 0, (int)chunkSize);
						//add the chunk to the body
						_responseBody.AddChunkReference(chunk, chunkSize);
						//advance one line after that
						lineBytes = Utils.ReadLine(ms, ESTIMATED_LINE_SIZE, LineEnding.Any);
						//the line read should be empty
						if (lineBytes !=null && lineBytes.Length > 0)
						{
							throw new Exception("Invalid chunk");
						}
					}
					else
					{
						throw new Exception("Invalid chunk size");
					}
				}
			}
			while (lineBytes != null);
		}

		/// <summary>
		/// Processes a chunked response by determining a chunk size line based on the line format and skipping those lines
		/// This applies to responses that do not contain correct chunk sizes
		/// </summary>
		/// <param name="ms"></param>
		private void ProcessChunkedResponseHeuristically(MemoryStream ms)
		{
			_responseBody.ClearChunks();

			byte[] lineBytes = new byte[0];
			string line;

			long posBefRdLn;
            long startChunkPos = ms.Position;
           
			long chunkLen = 0;

			while (true)
			{
				//save the position so we can revert to reading the full chunk
				posBefRdLn = ms.Position;
				lineBytes = Utils.ReadLine(ms, ESTIMATED_LINE_SIZE, LineEnding.Any);

				if (lineBytes != null)
				{
					line = Constants.DefaultEncoding.GetString(lineBytes);
				}
				else
				{
                    if (_responseBody.Length < ms.Position - startChunkPos) //end of stream but there are remaining bytes that were not added
                    {
                        ms.Position = startChunkPos;
                        //read the remaining response
                        byte[] chunk = new byte[chunkLen];
                        ms.Read(chunk, 0, (int)chunkLen);
                        //add the chunk to the response
                        _responseBody.AddChunkReference(chunk, (int)chunkLen);
                    }
					break; //end of response
				}

				int scIndex = line.IndexOf(';');
				if (scIndex > -1)
				{
					line = line.Substring(0, scIndex);
				}

				//this is done this way because some traffic files are malformed and
				//do not correctly display the chunk sizes
				int chunkSize;
				if (line != String.Empty && Uri.IsHexDigit(line[0]) && Int32.TryParse(line, NumberStyles.HexNumber, null, out chunkSize))
				{
					if (chunkLen > 0)
					{
						long currPos = ms.Position;
						ms.Position = startChunkPos;
						byte[] chunk = new byte[chunkLen];
						ms.Read(chunk, 0, (int)chunkLen);
						//revert back to the last position
						ms.Position = currPos;
						//add the chunk to the response
						_responseBody.AddChunkReference(chunk, (int)chunkLen);
					}
					//save the current position as the beggining of a new chunk
					startChunkPos = ms.Position;
				}
				else
				{
					//calculate the current chunk size
					//...which is the length of the current line
					//plus the position in stream before reading the current line
					//minus the position in stream when the last chunk header was read
					chunkLen = posBefRdLn + lineBytes.Length - startChunkPos;
				}
			}
		}

		
		/// <summary>
		/// Gets the content length of the response. Returns -1 if not set
		/// </summary>
		public long ContentLength
		{
			get
			{
				return _responseBody.Length;
			}
			
		}

		private string _statusLine = String.Empty;
		/// <summary>
		/// Return the status line
		/// </summary>
		public string StatusLine
		{
			get { return _statusLine; }
		}

		private int _status = 0;
		/// <summary>
		/// Gets the status
		/// </summary>
		public int Status
		{
			get { return _status; }
		}



		private HTTPHeaders _headers = new HTTPHeaders();
		/// <summary>
		/// The headers collection associated with the response
		/// </summary>
		public HTTPHeaders Headers
		{
			get { return _headers; }
		}

		/// <summary>
		/// Gets the response heade string
		/// </summary>
		public string ResponseHeadString
		{
			get
			{
				//assemble the status line and the headers

				string responseHead = _statusLine + Environment.NewLine;

                //update the content length if this is not chunked
                if (!_responseBody.IsChunked)
                {
                    _headers["Content-Length"] = ContentLength.ToString();
                }

				responseHead += _headers.ToString();

				responseHead += Environment.NewLine;

				return responseHead;
			}
		}

		/// <summary>
		/// Returns the status line and the response headers
		/// </summary>
		public byte[] ResponseHead
		{
			get
			{
				return Constants.DefaultEncoding.GetBytes(ResponseHeadString);
			}
		}

		private HttpResponseBody _responseBody;
		/// <summary>
		/// Retrieves the raw response body
		/// </summary>
		public HttpResponseBody ResponseBody
		{
			get { return _responseBody; }
		}


		/// <summary>
		/// Process a string response
		/// </summary>
		/// <param name="rawResponse"></param>
		public void ProcessResponse(string rawResponse)
		{
			byte[] bytes = Constants.DefaultEncoding.GetBytes(rawResponse);
			ProcessResponse(bytes);
		}

		/// <summary>
		/// Overriden ToString method 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(ResponseHeadString);
			sb.Append(ResponseBody.ToString(_headers["Content-Type"]));
			return sb.ToString();
		}

		/// <summary>
		/// Merges all the response chunks into one byte array
		/// </summary>
		/// <returns></returns>
		public byte[] ToArray()
		{
			ByteArrayBuilder builder = new ByteArrayBuilder();
			builder.AddChunkReference(ResponseHead, ResponseHead.Length);
			if (ResponseBody != null)
			{
				if (ResponseBody.IsChunked)
				{
					byte[] chunk;
					byte[] chunkBuf;
					while ((chunk = ResponseBody.ReadChunk()) != null)
					{
						//write the  chunk size line
						chunkBuf = Constants.DefaultEncoding.GetBytes(String.Format("{0:x}\r\n", chunk.Length));
						builder.AddChunkReference(chunkBuf, chunkBuf.Length);
						//add the chunk
						builder.AddChunkReference(chunk, chunk.Length);
						//add a new line after the chunk
						chunkBuf = Constants.DefaultEncoding.GetBytes("\r\n");
						builder.AddChunkReference(chunkBuf, chunkBuf.Length);
					}
					//write a last chunk with the value 0
					// write the last chunk size
					chunkBuf = Constants.DefaultEncoding.GetBytes("0\r\n\r\n");
					builder.AddChunkReference(chunkBuf, chunkBuf.Length);
				}
				else
				{
					byte[] chunk = ResponseBody.ToArray();
					if (chunk != null)
					{
						builder.AddChunkReference(chunk, chunk.Length);
					}
				}
			}
			return builder.ToArray();
		}
	}

}
