using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficViewerSDK
{
	/// <summary>
	/// Stores request and response data. To save memory add bytes as they are being read using the AddToRequest and
	/// AddToResponse functions
	/// </summary>
    public class RequestResponseBytes:ICloneable
    {

		private ByteArrayBuilder _requestBuilder = new ByteArrayBuilder();
		private ByteArrayBuilder _responseBuilder = new ByteArrayBuilder();
		
		/// <summary>
		/// Adds a chunk of bytes to the request
		/// </summary>
		/// <param name="data"></param>
		public void AddToRequest(byte[] data)
		{
			_requestBuilder.AddChunkReference(data, data.Length);
		}

		/// <summary>
		/// Adds the specified string to the request
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public void AddToRequest(string data)
		{
			AddToRequest(Constants.DefaultEncoding.GetBytes(data));
		}

		

		/// <summary>
		/// Returns a full request as a byte array. Since we need to merge the chunks anyways the chunks list will contain 
		/// only one element storing the entire array. 
		/// </summary>
		public byte[] RawRequest
		{
			get
			{
				return _requestBuilder.ToArray();
			}
			set
			{
				_requestBuilder = new ByteArrayBuilder();
				_requestBuilder.AddChunkReference(value, value.Length);
			}
		}

		/// <summary>
		/// The total size of the request
		/// </summary>
		public int RequestSize
		{
			get { return _requestBuilder.Length; }
		}

		/// <summary>
		/// Adds a chunk of bytes to the response
		/// </summary>
		/// <param name="data"></param>
		public void AddToResponse(byte[] data)
		{
			_responseBuilder.AddChunkReference(data, data.Length);
		}
		/// <summary>
		/// Add the data to the response
		/// </summary>
		/// <param name="data"></param>
		public void AddToResponse(string data)
		{
			AddToResponse(Constants.DefaultEncoding.GetBytes(data));
		}

		
		/// <summary>
		/// Total size of the response
		/// </summary>
		public int ResponseSize
		{
			get { return _responseBuilder.Length; }
		}

		/// <summary>
		/// Gets/sets the raw response in bytes
		/// </summary>
		public byte[] RawResponse
		{
			get
			{
				return _responseBuilder.ToArray();
			}
			set
			{
				_responseBuilder = new ByteArrayBuilder();
				_responseBuilder.AddChunkReference(value, value.Length);
			}
		}

		/// <summary>
		/// Resets the request chunk position to the beggining
		/// </summary>
		/// <returns></returns>
		public void ResetRequestChunkPosition()
		{
			_requestBuilder.ResetChunkPosition();
		}


		/// <summary>
		/// Reads the next chunk of request
		/// </summary>
		/// <returns></returns>
		public byte[] ReadRequestChunk()
		{
			return _requestBuilder.ReadChunk();
		}


		/// <summary>
		/// Resets the response chunk position to the beggining
		/// </summary>
		/// <returns></returns>
		public void ResetResponseChunkPosition()
		{
			_responseBuilder.ResetChunkPosition();
		}


		/// <summary>
		/// Reads the next chunk of response
		/// </summary>
		/// <returns></returns>
		public byte[] ReadResponseChunk()
		{
			return _responseBuilder.ReadChunk();
		}

		#region ICloneable Members
		/// <summary>
		/// Creates a deep copy of the request data object
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			RequestResponseBytes clone = new RequestResponseBytes();
			clone.RawRequest = this.RawRequest.Clone() as byte[];
			clone.RawResponse = this.RawResponse.Clone() as byte[];
			
			return clone;
		}

		#endregion
	}
}
