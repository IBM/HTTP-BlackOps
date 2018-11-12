using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficViewerSDK.Search;
using TrafficViewerSDK;

namespace TrafficViewerUnitTest
{
	/// <summary>
	/// Summary description for LogSync
	/// </summary>
	[TestClass]
	public class LogSync
	{
		public LogSync()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		public void TestLogSyncCache()
		{
			//make some test timestamps here
			DateTime excessTS = new DateTime(2010, 12, 8, 6, 0, 0, 0); //2010-12-08 6:00:00.000
			long excessPos = 1111;
			DateTime firstTS = new DateTime(2010, 12, 8, 6, 47, 0, 0); //2010-12-08 6:47:00.000
			long firstPos = 100;
			DateTime intermediateTS1 = new DateTime(2010, 12, 8, 6, 48, 0, 0); //2010-12-08 6:48:00.000
			DateTime intermediateTS2 = new DateTime(2010, 12, 8, 6, 49, 0, 0); //2010-12-08 6:49:00.000
			DateTime secondTS = new DateTime(2010, 12, 8, 6, 50, 0, 10); //2010-12-08 6:50:00.010
			DateTime intermediateTS3 = new DateTime(2010, 12, 8, 6, 50, 0, 200); //2010-12-08 6:49:00.200
			long secondPos = 200;
			DateTime thirdTS = new DateTime(2010, 12, 8, 7, 51, 0, 500); //2010-12-08 7:50:00.500
			long thirdPos = 300;
			DateTime lastTS = new DateTime(2010, 12, 8, 7, 55, 0, 0); //2010-12-08 7:50:00.500


			//add them to the cache
			TimeStampsPositionsCache cache = new TimeStampsPositionsCache();
			cache.MaxSize = 3;

			//check the case when the cache is empty
			Assert.IsTrue(cache.FindBestEntry(firstTS) == 0);
			
			cache.Add(excessTS, excessPos);
			cache.Add(thirdTS, thirdPos);
			cache.Add(secondTS, secondPos);
			cache.Add(firstTS, firstPos);		

			//the excess TS should be deleted
			Assert.IsTrue(cache.FindBestEntry(excessTS) == 0);
			
			//the first pos should be returned for any the first intermediate ts
			Assert.AreEqual(firstPos, cache.FindBestEntry(intermediateTS1));
			Assert.AreEqual(secondPos, cache.FindBestEntry(intermediateTS3));

			//the first pos should be returned for the first ts
			Assert.AreEqual(cache.FindBestEntry(firstTS), firstPos);
			
			//same rule applies to the other two respectively
			Assert.AreEqual(cache.FindBestEntry(secondTS), secondPos);
			Assert.AreEqual(cache.FindBestEntry(thirdTS), thirdPos);

			//last TS returns third pos
			Assert.AreEqual(cache.FindBestEntry(lastTS), thirdPos);

		}

		[TestMethod]
		public void TestLogSyncTimeStampsFinder()
		{
			TempFile temp = new TempFile(".txt");
			temp.Write(Properties.Resources.ItemLog);

			TimeStampsFinder finder = new TimeStampsFinder(temp.Path);

			DateTime ts = new DateTime(2010, 09, 10, 13, 53, 55);

			List<ByteArrayBuilder> list = finder.Find(ts, 1);

			Assert.AreEqual(2, list.Count);

			string firstEntry = Constants.DefaultEncoding.GetString(list[0].ToArray());

			Assert.IsTrue(firstEntry.Contains("WEO"));

		}
	}
}
