using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TVDiff.Implementations;
using TVDiff;
using TVDiff.DiffObjects;
using TVDiff.Algorithms;
using TrafficViewerSDK;
using TrafficViewerControls.Diff;
using System.Threading;

namespace TrafficViewerUnitTest
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class Diff
	{
		public Diff()
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
		public void LinesDiff()
		{
			string text1 = "LineLine1.1\r\n" +
						   "eniLLine1.3\r\n" +
						   "ABC1.4";
			string text2 = "LineLine2.1\r\n" +
						   "XYZ2.4\r\n" +
						   "eniLLine2.3";

			LinesDiffer differ = new LinesDiffer();

			DiffResult result = differ.DoDiff(text1, text2);
			IDiffObjectsCollection firstDiffs = result.DifferencesForFirst;
			IDiffObjectsCollection secondDiffs = result.DifferencesForSecond;

			firstDiffs.SortByPosition();
			secondDiffs.SortByPosition();

			Assert.IsTrue(firstDiffs[0].ValueEquals(new LetterDiffObject(0, 0, "1")));
			Assert.IsTrue(secondDiffs[0].ValueEquals(new LetterDiffObject(0, 0, "2")));
			Assert.IsTrue(firstDiffs[1].ValueEquals(new LetterDiffObject(0, 0, "1")));
			Assert.IsTrue(secondDiffs[1].ValueEquals(new LineDiffObject(0, 0, "XYZ2.4")));
			Assert.IsTrue(firstDiffs[2].ValueEquals(new LineDiffObject(0, 0, "ABC1.4")));
			Assert.IsTrue(secondDiffs[2].ValueEquals(new LetterDiffObject(0, 0, "2")));
		}

		[TestMethod]
		public void WordsDiff()
		{
			string text1 = "first second;third1";
			string text2 = "second;first fourth-third";

			WordsDiffer differ = new WordsDiffer();

			DiffResult result = differ.DoDiff(text1, text2);
			IDiffObjectsCollection firstDiffs = result.DifferencesForFirst;
			IDiffObjectsCollection secondDiffs = result.DifferencesForSecond;

			Assert.IsTrue(firstDiffs[0].ValueEquals(new WordDiffObject(0, 0, "first ")));
			Assert.IsTrue(firstDiffs[1].ValueEquals(new LetterDiffObject(0, 0, "1")));
			Assert.IsTrue(secondDiffs[0].ValueEquals(new WordDiffObject(0, 0, "first ")));
			Assert.IsTrue(secondDiffs[1].ValueEquals(new WordDiffObject(0, 0, "fourth-")));

			//test a sorted diff
			differ.Properties.Sorted = true;
			result = differ.DoDiff(text1, text2);

			firstDiffs = result.DifferencesForFirst;
			secondDiffs = result.DifferencesForSecond;

			Assert.IsTrue(firstDiffs[0].ValueEquals(new LetterDiffObject(0, 0, "1")));
			Assert.IsTrue(secondDiffs[0].ValueEquals(new WordDiffObject(0, 0, "fourth-")));

		}

		[TestMethod]
		public void LettersDiff()
		{
			string text1 = "abcdefg;1";
			string text2 = "xabcdfg1!";

			LettersDiffer differ = new LettersDiffer();

			DiffResult result = differ.DoDiff(text1, text2);
			IDiffObjectsCollection firstDiffs = result.DifferencesForFirst;
			IDiffObjectsCollection secondDiffs = result.DifferencesForSecond;

			Assert.IsTrue(firstDiffs[0].ValueEquals(new LetterDiffObject(0, 0, "e")));
			Assert.IsTrue(firstDiffs[1].ValueEquals(new LetterDiffObject(0, 0, ";")));
			Assert.IsTrue(secondDiffs[0].ValueEquals(new LetterDiffObject(0, 0, "x")));
			Assert.IsTrue(secondDiffs[1].ValueEquals(new LetterDiffObject(0, 0, "!")));
		}

		[TestMethod]
		public void Similarity()
		{
			string text1 = "asldbcv3o9322lif;owq4o-u-4hf3fn4ofwh094f4qq43fqldadsa2GSUACUVAoo32g3f";
			string text2 = "xsbadkvivuvsd;ojve;rieorqro8@O*ho848oq34floowahoOAGAGAODOWWOoq38aqo3af";

			LettersDiffer differ = new LettersDiffer();

			differ.AddTask(text1, 0);
			differ.AddTask(text2, 0);
			double diffSimilarity = differ.DoDiff();
			double aseSimilarity = ASESimilarityAlgorithm.CalculateSimilarity(text1, text2);

			Assert.AreEqual(aseSimilarity, diffSimilarity, 0.05, "Incorrect similarity factor");

		}

		[TestMethod]
		public void RequestDifferTest()
		{
			TrafficViewerFile tvf = UnitTestUtils.GenerateTestTvf();

			RequestsDiffer reqDiffer = new RequestsDiffer();

			//compare two similar requests

			RequestsDifferResult res = reqDiffer.Diff(24, 29, tvf);

			Assert.AreEqual(16, res.DiffsForFirst.Count);
			Assert.AreEqual(14, res.DiffsForSecond.Count);

			Assert.AreEqual(0.9613687022159203, res.BodyAproximateSimilarity, 0.009);

			//first request is logged out
			Assert.IsTrue(res.DiffsForFirst[11].ToString().Contains
				("You do not appear to have authenticated yourself with the application."));

			//second request is logged in
			Assert.IsTrue(res.DiffsForSecond[6].ToString().Contains
				("out of the Online Banking application."));
		}

		//[TestMethod]
		public void LargeDiffTask()
		{
			//generate two large chars strings
			string first = Utils.RandomString(10010, 12000);
			string second = Utils.RandomString(9000, 12000);

			DateTime start = DateTime.Now;

			LettersDiffer lettersDiffer = new LettersDiffer();

			DiffResult res = lettersDiffer.DoDiff(first, second);

			DateTime end = DateTime.Now;

			TimeSpan delta = end.Subtract(start);

			Assert.IsTrue(delta.Minutes == 0 && delta.Seconds < 20,
				String.Format("Diff time was {0}", delta));

			//generate two large word strings
			first = GenerateWordsSequence(900);
			second = GenerateWordsSequence(1020);

			WordsDiffer wordsDiffer = new WordsDiffer();

			start = DateTime.Now;

			res = wordsDiffer.DoDiff(first, second);

			end = DateTime.Now;

			delta = end.Subtract(start);

			Assert.IsTrue(delta.Minutes == 0 && delta.Seconds < 20,
				String.Format("Diff time was {0}", delta));

		}

		private string GenerateWordsSequence(int size)
		{
			StringBuilder wordsSequence = new StringBuilder();
			int i;
			for (i = 0; i < size; i++)
			{
				wordsSequence.Append(Utils.RandomString(30, 100));
				wordsSequence.Append(" ");
			}
			return wordsSequence.ToString();
		}

	}
}
