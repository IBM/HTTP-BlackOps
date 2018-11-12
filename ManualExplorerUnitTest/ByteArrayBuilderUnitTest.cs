using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficViewerSDK;

namespace ManualExplorerUnitTest
{
	[TestClass]
	public class ByteArrayBuilderUnitTest
	{
		private byte[] GetBytesFromString(string s)
		{
			return Encoding.UTF8.GetBytes(s);
		}


		private void RunTest(string firstChunk, int firstChunkSize, string secondChunk, int secondChunkSize, string expectedResult, int maxSize)
		{
			ByteArrayBuilder builder = new ByteArrayBuilder(maxSize);
			builder.AddChunkReference(GetBytesFromString(firstChunk), firstChunkSize);
			builder.AddChunkReference(GetBytesFromString(secondChunk), secondChunkSize);

			string result = Encoding.UTF8.GetString(builder.ToArray());

			Assert.AreEqual(expectedResult, result);

			if (firstChunkSize + secondChunkSize > maxSize)
			{
				Assert.IsTrue(builder.IsTruncated);
			}

		}

		[TestMethod]
		public void TestMergingEqualChunks()
		{
			RunTest("123", 3, "123", 3, "123123", 10);
		}

		[TestMethod]
		public void TestMergingSmallerAndBiggerChunks()
		{
			RunTest("12", 2, "1234", 4, "121234", 10);
		}


		[TestMethod]
		public void TestMergingBiggerAndSmallerChunks()
		{
			RunTest("1234", 4, "12", 2, "123412", 10);
		}


		[TestMethod]
		public void TestMergingChunksWithSmallerSpecifiedSize()
		{
			RunTest("123xxx", 3, "12xxxx", 2, "12312", 5);
		}

		[TestMethod]
		public void TestMergingChunksWithBiggerSpecifiedSize()
		{
			bool exceptionThrown = false;

			try
			{
				RunTest("123", 5, "12", 4, "12312", 10);
			}
			catch 
			{
				exceptionThrown = true;
			}

			Assert.IsTrue(exceptionThrown);
		}


		[TestMethod]
		public void TestMergingANullChunk()
		{
			bool exceptionThrown = false;

			try
			{
				RunTest(null, 5, "12", 4, "12312", 10);
			}
			catch
			{
				exceptionThrown = true;
			}

			Assert.IsTrue(exceptionThrown);
		}


		[TestMethod]
		public void TestMergingChunksOneWithExactAndOneWithSmallerSpecifiedSize()
		{
			RunTest("123", 3, "12xxx", 2, "12312", 10);
		}



		[TestMethod]
		public void TestMergingChunksExceedingTheMax()
		{
			RunTest("12345", 5, "12345xxx", 7, "1234512345", 10);
		}


		[TestMethod]
		public void TestMergingEmptyChunk()
		{
			RunTest("", 0, "12345", 5, "12345", 5);
		}

		[TestMethod]
		public void TestReadChunk()
		{
			ByteArrayBuilder builder = new ByteArrayBuilder();
			string firstChunk = "123";
			string secondChunk = "ab";
			string thirdChunk = "--+";

			byte[] firstChunkBytes = GetBytesFromString(firstChunk);
			byte[] secondChunkBytes = GetBytesFromString(secondChunk);
			byte[] thirdChunkBytes = GetBytesFromString(thirdChunk);

			builder.AddChunkReference(firstChunkBytes, 3);
			builder.AddChunkReference(secondChunkBytes, 2);
			builder.AddChunkReference(thirdChunkBytes, 3);

			byte[] returnedBytes = builder.ReadChunk();
			string returnedBytesString = Encoding.UTF8.GetString(returnedBytes);
			Assert.AreEqual(firstChunk, returnedBytesString);
			returnedBytes = builder.ReadChunk();
			returnedBytesString = Encoding.UTF8.GetString(returnedBytes);
			Assert.AreEqual(secondChunk, returnedBytesString);
			//reset the current chunk
			builder.ResetChunkPosition();
			returnedBytes = builder.ReadChunk();
			returnedBytesString = Encoding.UTF8.GetString(returnedBytes);
			Assert.AreEqual(firstChunk, returnedBytesString);
			//read to end
			returnedBytes = builder.ReadChunk();
			returnedBytes = builder.ReadChunk();
			returnedBytes = builder.ReadChunk();
			Assert.IsNull(returnedBytes);
			//test that the builder resets to the beggining now
			returnedBytes = builder.ReadChunk();
			returnedBytesString = Encoding.UTF8.GetString(returnedBytes);
			Assert.AreEqual(firstChunk, returnedBytesString);
		}


		[TestMethod]
		public void TestClearChunk()
		{
			ByteArrayBuilder builder = new ByteArrayBuilder();
			string firstChunk = "123";
			string secondChunk = "ab";

			byte[] firstChunkBytes = GetBytesFromString(firstChunk);
			byte[] secondChunkBytes = GetBytesFromString(secondChunk);


			builder.AddChunkReference(firstChunkBytes, 3);
			builder.AddChunkReference(secondChunkBytes, 2);


			byte[] returnedBytes = builder.ToArray();
			string returnedBytesString = Encoding.UTF8.GetString(returnedBytes);

			Assert.AreEqual("123ab", returnedBytesString);

			//now clear
			builder.ClearChunks();

			Assert.AreEqual("", Encoding.UTF8.GetString(builder.ToArray()));

			//check that the index gets reset after clear
			builder.AddChunkReference(firstChunkBytes, 3);

			returnedBytes = builder.ReadChunk();
			returnedBytesString = Encoding.UTF8.GetString(returnedBytes);

			Assert.AreEqual(firstChunk, returnedBytesString);
		}

	}
}
