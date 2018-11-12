using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficViewerSDK
{
	/// <summary>
	/// Provides a memory efficient way to construct a byte array from byte chunks or to simply store 
	/// an existing byte[] buffer and add to it.
	/// </summary>
	public class ByteArrayBuilder
	{

		private LinkedListNode<byte[]> _curr = null;
		private int _currIndex = 0;


		/// <summary>
		/// Will store all the chunks
		/// </summary>
		protected LinkedList<byte[]> _chunks = new LinkedList<byte[]>();

		/// <summary>
		/// The total size of the array
		/// </summary>
		protected int _totalBytes = 0;

		/// <summary>
		/// How many chunks in the array
		/// </summary>
		protected int _chunksCount = 0;

		/// <summary>
		/// The maximum size of the array. If greater than zero all arrays will get truncated
		/// </summary>
		private int _maxSize = -1;

		/// <summary>
		/// Whether the current array was truncated
		/// </summary>
		protected bool _isTruncated = false;


		/// <summary>
		/// Whether the current array was truncated
		/// </summary>
		public virtual bool IsTruncated
		{
			get { return _isTruncated; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="maxSize">The maximum size that the builder will have. Everything past that will be truncated</param>
		public ByteArrayBuilder(int maxSize)
		{
			_maxSize = maxSize;
		}

		/// <summary>
		/// Constructor for an unlimited size byte array builder
		/// </summary>
		public ByteArrayBuilder() : this(-1) { }

		/// <summary>
		/// Clears all the chunks
		/// </summary>
		public void ClearChunks()
		{
			_chunks.Clear();
			_totalBytes = 0;
			ResetChunkPosition();
		}

		/// <summary>
		/// Resets the current chunk position to the beggining
		/// </summary>
		public void ResetChunkPosition()
		{
			_currIndex = 0;
			_curr = null;
		}

		/// <summary>
		/// Reads a chunk from the current response
		/// </summary>
		/// <returns>The chunk. If there are no HTTP chunks or all the chunks were read null is returned</returns>
		public byte[] ReadChunk()
		{
			byte[] result = null;

			//check if there are still nodes we haven't read or if the list is not empty
			if (_currIndex < _chunks.Count)
			{
				if (_curr == null)
				{
					_curr = _chunks.First;
				}

				result = _curr.Value;

				_curr = _curr.Next;

				_currIndex++;
			}
			else
			{
				ResetChunkPosition();
			}

			return result;
		}




		/// <summary>
		/// Gets the full array. 
		/// </summary>
		public virtual byte[] ToArray()
		{
			byte[] fullArray;

			if (_chunksCount == 0)
			{
				fullArray = null; //there are no chunks means there's no response
			}
			else if (_chunksCount == 1)
			{
				fullArray = _chunks.First.Value; //there's only one chunk no need to iterate through the list
			}
			else
			{
				fullArray = MergeChunks();
			}
			return fullArray;

		}

		/// <summary>
		/// Returns the total size of the response
		/// </summary>
		public int Length
		{
			get
			{
				return _totalBytes;
			}
		}	

		/// <summary>
		/// Merges all the chunks
		/// </summary>
		/// <returns></returns>
		protected byte[] MergeChunks()
		{
			byte[] response;
			response = new byte[_totalBytes]; // create a new array with the size of all chunks
			LinkedListNode<byte[]> curr = _chunks.First;

			if (curr != null)
			{
				int index = 0;
				do
				{
					//if the total bytes is smaller than the sum of all chunks truncate response
					int len = Math.Min(curr.Value.Length, _totalBytes - index);

					Array.Copy(curr.Value, 0, response, index, len);
					index += len;
					curr = curr.Next;
				}
				while (index < _totalBytes);
			}

			return response;
		}

		/// <summary>
		/// Adds a reference to a chunk of bytes to the builder. 
		/// </summary>
		/// <param name="chunk">Byte chunk</param>
		public virtual void AddChunkReference(byte[] chunk)
		{
			if (chunk != null)
			{
				AddChunkReference(chunk, chunk.Length);
			}
		}

		/// <summary>
		/// Adds a reference to a chunk of bytes to the response. 
		/// </summary>
		/// <param name="chunk">Byte chunk</param>
		/// <param name="length">
		/// The length of the relevant bytes in the chunk. If this is the last chunk
		/// the chunk length might be less than the size of the chunk. In this case the chunk will be
		/// resized before being added to the response.
		/// </param>
		public virtual void AddChunkReference(byte[] chunk, int length)
		{
			int availableSpace;
			int savedLength = length;

			if (_isTruncated)
			{
				return; //don't add anything if we already reached the max size
			}
			
			if (_maxSize > -1) // if the maxSize was set
			{
				availableSpace = _maxSize - _totalBytes;
				if (availableSpace < length)
				{
					_isTruncated = true;
					savedLength = availableSpace;
				}
			}
			

            byte[] chunkToAdd = new byte[savedLength];
            Array.Copy(chunk, chunkToAdd, savedLength);          

			_totalBytes += savedLength;
			_chunksCount++;
            _chunks.AddLast(chunkToAdd); //if the list is empty the new node becomes first and last
		}
	}
}
