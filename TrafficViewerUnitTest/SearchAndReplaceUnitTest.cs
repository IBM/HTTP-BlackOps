using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficViewerSDK;
using TrafficViewerSDK.Search;
using TrafficViewerInstance;

namespace TrafficViewerUnitTest
{
	[TestClass]
	public class SearchAndReplaceUnitTest
	{
        [TestInitialize]
        public void Initialize()
        {
            //prevent caching
            SearchResultCache.Instance.CacheEnabled = false;
        }

		[TestMethod]
		public void TestLineSearchInRequestLineNoRegex()
		{
			TrafficViewer.Instance.NewTvf();
			ITrafficDataAccessor tvf = TrafficViewer.Instance.TrafficViewerFile;
			string firstRequest = "GET /a1 HTTP/1.1\r\nHeader1: a1";
			string secondRequest = "GET /a2 HTTP/1.1\r\nHeader1: a2";
			tvf.AddRequestResponse(firstRequest, "HTTP 200 OK");
			tvf.AddRequestResponse(secondRequest, "HTTP 200 OK");

			Assert.AreEqual(2, tvf.RequestCount);

			LineSearcher searcher = new LineSearcher();
			LineMatches result = new LineMatches();

			SearchCriteriaSet criteriaSet = new SearchCriteriaSet();
			criteriaSet.Add(new SearchCriteria(SearchContext.RequestLine, false, "a1"));

			searcher.Search(tvf, criteriaSet, result);

			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(0,result[0].RequestId);
			Assert.AreEqual(SearchContext.Request, result[0].Context);
			Assert.AreEqual(1, result[0].MatchCoordinatesList.Count);
			Assert.AreEqual("a1",firstRequest.Substring(result[0].MatchCoordinatesList[0].MatchPosition,result[0].MatchCoordinatesList[0].MatchLength));
			
		}


		[TestMethod]
		public void TestLineSearchInRequestLineRegex()
		{
			TrafficViewer.Instance.NewTvf();
			ITrafficDataAccessor tvf = TrafficViewer.Instance.TrafficViewerFile;
			string firstRequest = "GET /a1 HTTP/1.1\r\nHeader1: a1";
			string secondRequest = "GET /a2 HTTP/1.1\r\nHeader1: a2";
			tvf.AddRequestResponse(firstRequest, "HTTP 200 OK");
			tvf.AddRequestResponse(secondRequest, "HTTP 200 OK");

			Assert.AreEqual(2, tvf.RequestCount);

			LineSearcher searcher = new LineSearcher();
			LineMatches result = new LineMatches();

			SearchCriteriaSet criteriaSet = new SearchCriteriaSet();
			criteriaSet.Add(new SearchCriteria(SearchContext.RequestLine, true, @"a\d"));

			searcher.Search(tvf, criteriaSet, result);

			Assert.AreEqual(2, result.Count);
			Assert.AreEqual(0, result[0].RequestId);
			Assert.AreEqual(1, result[1].RequestId);
			Assert.AreEqual(SearchContext.Request, result[0].Context);
			Assert.AreEqual(1, result[0].MatchCoordinatesList.Count);
			Assert.AreEqual(1, result[1].MatchCoordinatesList.Count);
			Assert.AreEqual("a1", firstRequest.Substring(result[0].MatchCoordinatesList[0].MatchPosition, result[0].MatchCoordinatesList[0].MatchLength));
			Assert.AreEqual("a2", secondRequest.Substring(result[1].MatchCoordinatesList[0].MatchPosition, result[1].MatchCoordinatesList[0].MatchLength));
		}


		[TestMethod]
		public void TestLineSearchInRequestNoRegex()
		{
			TrafficViewer.Instance.NewTvf();
			ITrafficDataAccessor tvf = TrafficViewer.Instance.TrafficViewerFile;
			string firstRequest = "GET /a1 HTTP/1.1\r\nHeader1: a1";
			string secondRequest = "GET /a2 HTTP/1.1\r\nHeader1: a2";
			tvf.AddRequestResponse(firstRequest, "HTTP 200 OK");
			tvf.AddRequestResponse(secondRequest, "HTTP 200 OK");

			Assert.AreEqual(2, tvf.RequestCount);

			LineSearcher searcher = new LineSearcher();
			LineMatches result = new LineMatches();

			SearchCriteriaSet criteriaSet = new SearchCriteriaSet();
			criteriaSet.Add(new SearchCriteria(SearchContext.Request, false, "a1"));

			searcher.Search(tvf, criteriaSet, result);

			Assert.AreEqual(2, result.Count);
			Assert.AreEqual(0, result[0].RequestId);
			Assert.AreEqual(0, result[1].RequestId);

			Assert.AreEqual(1, result[0].MatchCoordinatesList.Count);
			Assert.AreEqual(1, result[1].MatchCoordinatesList.Count);

			Assert.AreEqual("a1", firstRequest.Substring(result[0].MatchCoordinatesList[0].MatchPosition, result[0].MatchCoordinatesList[0].MatchLength));
			Assert.AreEqual("a1", firstRequest.Substring(result[1].MatchCoordinatesList[0].MatchPosition, result[1].MatchCoordinatesList[0].MatchLength));

		}


		[TestMethod]
		public void TestLineSearchInRequestRegex()
		{
			TrafficViewer.Instance.NewTvf();
			ITrafficDataAccessor tvf = TrafficViewer.Instance.TrafficViewerFile;
			string firstRequest = "GET /a1 HTTP/1.1\r\nHeader1: a1";
			string secondRequest = "GET /a2 HTTP/1.1\r\nHeader1: a2";
			tvf.AddRequestResponse(firstRequest, "HTTP 200 OK");
			tvf.AddRequestResponse(secondRequest, "HTTP 200 OK");

			Assert.AreEqual(2, tvf.RequestCount);

			LineSearcher searcher = new LineSearcher();
			LineMatches result = new LineMatches();

			SearchCriteriaSet criteriaSet = new SearchCriteriaSet();
			criteriaSet.Add(new SearchCriteria(SearchContext.Request, true, @"a\d"));

			searcher.Search(tvf, criteriaSet, result);

			Assert.AreEqual(4, result.Count);

			Assert.AreEqual(0, result[0].RequestId);
			Assert.AreEqual(0, result[1].RequestId);
			Assert.AreEqual(1, result[2].RequestId);
			Assert.AreEqual(1, result[3].RequestId);


			Assert.AreEqual("a1", firstRequest.Substring(result[0].MatchCoordinatesList[0].MatchPosition, result[0].MatchCoordinatesList[0].MatchLength));
			Assert.AreEqual("a1", firstRequest.Substring(result[1].MatchCoordinatesList[0].MatchPosition, result[1].MatchCoordinatesList[0].MatchLength));
			Assert.AreEqual("a2", secondRequest.Substring(result[2].MatchCoordinatesList[0].MatchPosition, result[2].MatchCoordinatesList[0].MatchLength));
			Assert.AreEqual("a2", secondRequest.Substring(result[3].MatchCoordinatesList[0].MatchPosition, result[3].MatchCoordinatesList[0].MatchLength));

		}


		[TestMethod]
		public void TestLineSearchInRequestBodyNoRegex()
		{
			TrafficViewer.Instance.NewTvf();
			ITrafficDataAccessor tvf = TrafficViewer.Instance.TrafficViewerFile;
			string firstRequest = "POST /a1 HTTP/1.1\r\nHeader1: a1\r\n\r\na=1";
			string secondRequest = "POST /a2 HTTP/1.1\r\nHeader1: a2\r\n\r\na=2";
			tvf.AddRequestResponse(firstRequest, "HTTP 200 OK");
			tvf.AddRequestResponse(secondRequest, "HTTP 200 OK");

			Assert.AreEqual(2, tvf.RequestCount);

			LineSearcher searcher = new LineSearcher();
			LineMatches result = new LineMatches();

			SearchCriteriaSet criteriaSet = new SearchCriteriaSet();
			criteriaSet.Add(new SearchCriteria(SearchContext.RequestBody, false, "a=1"));

			searcher.Search(tvf, criteriaSet, result);

			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(0, result[0].RequestId);
			Assert.AreEqual(SearchContext.Request, result[0].Context);
			Assert.AreEqual(1, result[0].MatchCoordinatesList.Count);
			Assert.AreEqual("a=1", firstRequest.Substring(result[0].MatchCoordinatesList[0].MatchPosition, result[0].MatchCoordinatesList[0].MatchLength));
		}

		[TestMethod]
		public void TestLineSearchInRequestBodyRegex()
		{
			TrafficViewer.Instance.NewTvf();
			ITrafficDataAccessor tvf = TrafficViewer.Instance.TrafficViewerFile;
			string firstRequest = "POST /a1 HTTP/1.1\r\nHeader1: a1\r\n\r\na=1";
			string secondRequest = "POST /a2 HTTP/1.1\r\nHeader1: a2\r\n\r\na=2";
			tvf.AddRequestResponse(firstRequest, "HTTP 200 OK");
			tvf.AddRequestResponse(secondRequest, "HTTP 200 OK");

			Assert.AreEqual(2, tvf.RequestCount);

			LineSearcher searcher = new LineSearcher();
			LineMatches result = new LineMatches();

			SearchCriteriaSet criteriaSet = new SearchCriteriaSet();
			criteriaSet.Add(new SearchCriteria(SearchContext.RequestBody, true, @"a=\d"));

			searcher.Search(tvf, criteriaSet, result);

			Assert.AreEqual(2, result.Count);
			Assert.AreEqual(0, result[0].RequestId);
			Assert.AreEqual(1, result[1].RequestId);
			Assert.AreEqual(SearchContext.Request, result[0].Context);
			Assert.AreEqual(SearchContext.Request, result[1].Context);

			Assert.AreEqual("a=1", firstRequest.Substring(result[0].MatchCoordinatesList[0].MatchPosition, result[0].MatchCoordinatesList[0].MatchLength));
			Assert.AreEqual("a=2", secondRequest.Substring(result[1].MatchCoordinatesList[0].MatchPosition, result[1].MatchCoordinatesList[0].MatchLength));
		}




		[TestMethod]
		public void TestLineSearchInResponseNoRegex()
		{
			TrafficViewer.Instance.NewTvf();
			ITrafficDataAccessor tvf = TrafficViewer.Instance.TrafficViewerFile;
			string firstRequest = "POST /a1 HTTP/1.1\r\nHeader1: a1\r\n\r\na=1";
			string secondRequest = "POST /a2 HTTP/1.1\r\nHeader1: a2\r\n\r\na=2";
			string firstResponse = "HTTP 200 OK\r\n<r>1</r>";
			string secondResponse = "HTTP 200 OK\r\n<r>2</r>";
			tvf.AddRequestResponse(firstRequest, firstResponse);
			tvf.AddRequestResponse(secondRequest, secondResponse);

			Assert.AreEqual(2, tvf.RequestCount);

			LineSearcher searcher = new LineSearcher();
			LineMatches result = new LineMatches();

			SearchCriteriaSet criteriaSet = new SearchCriteriaSet();
			criteriaSet.Add(new SearchCriteria(SearchContext.Response, false, "<r>2</r>"));

			searcher.Search(tvf, criteriaSet, result);

			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(1, result[0].RequestId);
			Assert.AreEqual(SearchContext.Response, result[0].Context);
			Assert.AreEqual(1, result[0].MatchCoordinatesList.Count);
			Assert.AreEqual("<r>2</r>", secondResponse.Substring(result[0].MatchCoordinatesList[0].MatchPosition, result[0].MatchCoordinatesList[0].MatchLength));
		}

		[TestMethod]
		public void TestLineSearchInResponseRegex()
		{
			TrafficViewer.Instance.NewTvf();
			ITrafficDataAccessor tvf = TrafficViewer.Instance.TrafficViewerFile;
			string firstRequest = "POST /a1 HTTP/1.1\r\nHeader1: a1\r\n\r\na=1";
			string secondRequest = "POST /a2 HTTP/1.1\r\nHeader1: a2\r\n\r\na=2";
			string firstResponse = "HTTP 200 OK\r\n<r>1</r>";
			string secondResponse = "HTTP 200 OK\r\n<r>2</r>";
			tvf.AddRequestResponse(firstRequest, firstResponse);
			tvf.AddRequestResponse(secondRequest, secondResponse);

			Assert.AreEqual(2, tvf.RequestCount);

			LineSearcher searcher = new LineSearcher();
			LineMatches result = new LineMatches();

			SearchCriteriaSet criteriaSet = new SearchCriteriaSet();
			criteriaSet.Add(new SearchCriteria(SearchContext.Response, true, @"<r>\d+</r>|a1"));

			searcher.Search(tvf, criteriaSet, result);

			Assert.AreEqual(2, result.Count);
			Assert.AreEqual(0, result[0].RequestId);
			Assert.AreEqual(SearchContext.Response, result[0].Context);
			Assert.AreEqual(1, result[0].MatchCoordinatesList.Count);
			Assert.AreEqual("<r>2</r>", secondResponse.Substring(result[1].MatchCoordinatesList[0].MatchPosition, result[1].MatchCoordinatesList[0].MatchLength));
		}

		[TestMethod]
		public void TestLineSearchFullRegex()
		{
			TrafficViewer.Instance.NewTvf();
			ITrafficDataAccessor tvf = TrafficViewer.Instance.TrafficViewerFile;
			string firstRequest = "POST /a1 HTTP/1.1\r\nHeader1: a1\r\n\r\na=1";
			string secondRequest = "POST /a2 HTTP/1.1\r\nHeader1: a2\r\n\r\na=2";
			string firstResponse = "HTTP 200 OK\r\n<r>1</r>";
			string secondResponse = "HTTP 200 OK\r\n<r>2</r>";
			tvf.AddRequestResponse(firstRequest, firstResponse);

            string testValue = Constants.DefaultEncoding.GetString(tvf.LoadRequestData(0));
            Assert.AreEqual(firstRequest, testValue, "Incorrect first request");
            testValue = Constants.DefaultEncoding.GetString(tvf.LoadResponseData(0));
            Assert.AreEqual(firstResponse, testValue, "Incorrect first response");

			tvf.AddRequestResponse(secondRequest, secondResponse);

			Assert.AreEqual(2, tvf.RequestCount,"Correct number of requests not added");
            

			LineSearcher searcher = new LineSearcher();
			LineMatches result = new LineMatches();

			SearchCriteriaSet criteriaSet = new SearchCriteriaSet();
			criteriaSet.Add(new SearchCriteria(SearchContext.Full, true, "a=1|<r>2</r>"));

			searcher.Search(tvf, criteriaSet, result);

			Assert.AreEqual(2, result.Count);
			Assert.AreEqual(0, result[0].RequestId);
			Assert.AreEqual(1, result[1].RequestId);
			Assert.AreEqual(SearchContext.Request, result[0].Context);
			Assert.AreEqual(SearchContext.Response, result[1].Context);

			Assert.AreEqual("a=1", firstRequest.Substring(result[0].MatchCoordinatesList[0].MatchPosition, result[0].MatchCoordinatesList[0].MatchLength));
			Assert.AreEqual("<r>2</r>", secondResponse.Substring(result[1].MatchCoordinatesList[0].MatchPosition, result[1].MatchCoordinatesList[0].MatchLength));
		}

		[TestMethod]
		public void TestRequestSearchRequestLineRegex()
		{
			TrafficViewer.Instance.NewTvf();
			ITrafficDataAccessor tvf = TrafficViewer.Instance.TrafficViewerFile;
			string firstRequest = "POST /a1 HTTP/1.1\r\nHeader1: a1\r\n\r\na=1";
			string secondRequest = "POST /a2 HTTP/1.1\r\nHeader1: a2\r\n\r\na=2";
			string firstResponse = "HTTP 200 OK\r\n<r>1</r>";
			string secondResponse = "HTTP 200 OK\r\n<r>2</r>";
			tvf.AddRequestResponse(firstRequest, firstResponse);
			tvf.AddRequestResponse(secondRequest, secondResponse);

			Assert.AreEqual(2, tvf.RequestCount);

			RequestSearcher searcher = new RequestSearcher();
			RequestMatches result = new RequestMatches();

			SearchCriteriaSet criteriaSet = new SearchCriteriaSet();
			criteriaSet.Add(new SearchCriteria(SearchContext.RequestLine, true, "a1|a=1|<r>2</r>"));

			searcher.Search(tvf, criteriaSet, result);

			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(0, result[0]);
			
		}


		[TestMethod]
		public void TestRequestSearchRequestBodyRegex()
		{
			TrafficViewer.Instance.NewTvf();
			ITrafficDataAccessor tvf = TrafficViewer.Instance.TrafficViewerFile;
			string firstRequest = "POST /a1 HTTP/1.1\r\nHeader1: a1\r\n\r\na=1";
			string secondRequest = "POST /a2 HTTP/1.1\r\nHeader1: a2\r\n\r\na=2";
			string firstResponse = "HTTP 200 OK\r\n<r>1</r>";
			string secondResponse = "HTTP 200 OK\r\n<r>2</r>";
			tvf.AddRequestResponse(firstRequest, firstResponse);
			tvf.AddRequestResponse(secondRequest, secondResponse);

			Assert.AreEqual(2, tvf.RequestCount);

			RequestSearcher searcher = new RequestSearcher();
			RequestMatches result = new RequestMatches();

			SearchCriteriaSet criteriaSet = new SearchCriteriaSet();
			criteriaSet.Add(new SearchCriteria(SearchContext.RequestBody, true, "a1|a=2|<r>2</r>"));

			searcher.Search(tvf, criteriaSet, result);

			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(1, result[0]);

		}

		[TestMethod]
		public void TestRequestSearchRequestBodyRegexNoBody()
		{
			
			ITrafficDataAccessor tvf = new TrafficViewerFile();
			string firstRequest = "GET /a1 HTTP/1.1\r\nHeader1: a1";
			string secondRequest = "GET /a2 HTTP/1.1\r\nHeader1: a2";
			string firstResponse = "HTTP 200 OK\r\n<r>1</r>";
			string secondResponse = "HTTP 200 OK\r\n<r>2</r>";
			tvf.AddRequestResponse(firstRequest, firstResponse);
			tvf.AddRequestResponse(secondRequest, secondResponse);

			Assert.AreEqual(2, tvf.RequestCount);

			RequestSearcher searcher = new RequestSearcher();
			RequestMatches result = new RequestMatches();
            
			SearchCriteriaSet criteriaSet = new SearchCriteriaSet();
			criteriaSet.Add(new SearchCriteria(SearchContext.RequestBody, true, "a=2|a1|<r>2</r>"));

			searcher.Search(tvf, criteriaSet, result);

			Assert.AreEqual(0, result.Count);
			

		}


		[TestMethod]
		public void TestRequestSearchMultipleTexts()
		{
			TrafficViewer.Instance.NewTvf();
			ITrafficDataAccessor tvf = TrafficViewer.Instance.TrafficViewerFile;
			string firstRequest = "POST /a1 HTTP/1.1\r\nHeader1: a1\r\n\r\na=1";
			string secondRequest = "POST /a2 HTTP/1.1\r\nHeader1: a2\r\n\r\na=1&b=2";
			string firstResponse = "HTTP 200 OK\r\n<r>1</r>";
			string secondResponse = "HTTP 200 OK\r\n<r>2</r>";
			tvf.AddRequestResponse(firstRequest, firstResponse);
			tvf.AddRequestResponse(secondRequest, secondResponse);

			Assert.AreEqual(2, tvf.RequestCount);

			RequestSearcher searcher = new RequestSearcher();
			RequestMatches result = new RequestMatches();

			SearchCriteriaSet criteriaSet = new SearchCriteriaSet();
			criteriaSet.Add(new SearchCriteria(SearchContext.RequestBody, false, "a=1"));
			criteriaSet.Add(new SearchCriteria(SearchContext.RequestBody, false, "b=2"));
			searcher.Search(tvf, criteriaSet, result);

			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(1, result[0]);

		}

		[TestMethod]
		public void TestLineSearchMultipleMatchesPerLineNoRegex()
		{
			TrafficViewer.Instance.NewTvf();
			ITrafficDataAccessor tvf = TrafficViewer.Instance.TrafficViewerFile;
			
			string firstRequest = "POST /a1/a1 HTTP/1.1\r\nHeader1: a1\r\n\r\na=1";
			string firstResponse = "HTTP 200 OK\r\n<r>1</r>";
			
			tvf.AddRequestResponse(firstRequest, firstResponse);
			

			Assert.AreEqual(1, tvf.RequestCount);

			LineSearcher searcher = new LineSearcher();
			LineMatches result = new LineMatches();

			SearchCriteriaSet criteriaSet = new SearchCriteriaSet();
			criteriaSet.Add(new SearchCriteria(SearchContext.RequestLine, false, "a1"));
			
			searcher.Search(tvf, criteriaSet, result);

			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(2, result[0].MatchCoordinatesList.Count);

		}

		[TestMethod]
		public void TestLineSearchMultipleMatchesPerLineRegex()
		{
			TrafficViewer.Instance.NewTvf();
			ITrafficDataAccessor tvf = TrafficViewer.Instance.TrafficViewerFile;

			string firstRequest = "POST /a1/a1 HTTP/1.1\r\nHeader1: a1\r\n\r\na=1";
			string firstResponse = "HTTP 200 OK\r\n<r>1</r>";

			tvf.AddRequestResponse(firstRequest, firstResponse);


			Assert.AreEqual(1, tvf.RequestCount);

			LineSearcher searcher = new LineSearcher();
			LineMatches result = new LineMatches();

			SearchCriteriaSet criteriaSet = new SearchCriteriaSet();
			criteriaSet.Add(new SearchCriteria(SearchContext.RequestLine, true, @"a\d"));

			searcher.Search(tvf, criteriaSet, result);

			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(2, result[0].MatchCoordinatesList.Count);

		}


		[TestMethod]
		public void TestFullReplaceWithLongerString()
		{
			ReplaceTest("replacement");
		}

		[TestMethod]
		public void TestFullReplaceWithEmptyString()
		{
			ReplaceTest(String.Empty);
		}

		private static void ReplaceTest(string replacement)
		{
			TrafficViewer.Instance.NewTvf();
			ITrafficDataAccessor tvf = TrafficViewer.Instance.TrafficViewerFile;
			string firstRequest = "GET /a1 HTTP/1.1\r\nHeader1: a1";
			string secondRequest = "GET /a2 HTTP/1.1\r\nHeader1: a2";
			string firstResponse = "HTTP 200 OK\r\n<r>1</r>";
			string secondResponse = "HTTP 200 OK\r\n<r>2</r><tag><r>3</r>";
			tvf.AddRequestResponse(firstRequest, firstResponse);
			tvf.AddRequestResponse(secondRequest, secondResponse);

			Assert.AreEqual(2, tvf.RequestCount);

			LineSearcher searcher = new LineSearcher();
			LineMatches result = new LineMatches();

			SearchCriteriaSet criteriaSet = new SearchCriteriaSet();
			criteriaSet.Add(new SearchCriteria(SearchContext.Full, true, @"a1|a=2|<r>\d</r>"));

			searcher.Search(tvf, criteriaSet, result);

			tvf.Replace(result, replacement);

			firstRequest = Constants.DefaultEncoding.GetString(tvf.LoadRequestData(0));
			secondRequest = Constants.DefaultEncoding.GetString(tvf.LoadRequestData(1));
			firstResponse = Constants.DefaultEncoding.GetString(tvf.LoadResponseData(0));
			secondResponse = Constants.DefaultEncoding.GetString(tvf.LoadResponseData(1));

			Assert.AreEqual("GET /"+replacement+" HTTP/1.1\r\nHeader1: "+replacement, firstRequest);
			Assert.AreEqual("HTTP 200 OK\r\n" + replacement + "<tag>" + replacement, secondResponse);
		}


		[TestMethod]
		public void TestRequestLineAfterReplace()
		{
			TrafficViewer.Instance.NewTvf();
			ITrafficDataAccessor tvf = TrafficViewer.Instance.TrafficViewerFile;
			string firstRequest = "GET http://site.com/a1 HTTP/1.1\r\nHeader1: a1";
			tvf.AddRequestResponse(firstRequest, String.Empty);

			TVRequestInfo reqInfo = tvf.GetRequestInfo(0);

			Assert.AreEqual("GET http://site.com/a1 HTTP/1.1", reqInfo.RequestLine);
			Assert.AreEqual(1, tvf.RequestCount);

			LineSearcher searcher = new LineSearcher();
			LineMatches result = new LineMatches();

			SearchCriteriaSet criteriaSet = new SearchCriteriaSet();
			criteriaSet.Add(new SearchCriteria(SearchContext.RequestLine, true, "a1|a=2|<r>2</r>"));

			searcher.Search(tvf, criteriaSet, result);

			tvf.Replace(result, "replacement");

			firstRequest = Constants.DefaultEncoding.GetString(tvf.LoadRequestData(0));

			Assert.AreEqual("GET http://site.com/replacement HTTP/1.1\r\nHeader1: a1", firstRequest);

			

			Assert.AreEqual("GET http://site.com/replacement HTTP/1.1", reqInfo.RequestLine);
		}


		[TestMethod]
		public void TestReplaceInvalidCoords()
		{
			TrafficViewer.Instance.NewTvf();
			ITrafficDataAccessor tvf = TrafficViewer.Instance.TrafficViewerFile;
			string firstRequest = "GET /a1 HTTP/1.1\r\nHeader1: a1";
			string firstResponse = "HTTP 200 OK\r\n<r>2</r><tag><r>3</r>";
			

			tvf.AddRequestResponse(firstRequest, firstResponse);

			LineSearcher searcher = new LineSearcher();
			LineMatches result = new LineMatches();

			SearchCriteriaSet criteriaSet = new SearchCriteriaSet();
			criteriaSet.Add(new SearchCriteria(SearchContext.Full, true, @"a1|a=2|<r>\d</r>"));
			searcher.Search(tvf, criteriaSet, result);

			(tvf as TrafficViewerFile).Clear(false);

			Assert.AreEqual(0, tvf.RequestCount);

			string secondRequest = "GET /a2 HTTP/1.1\r\nHeader1: a2";
			string secondResponse = "HTTP 200 OK\r\n<r>1</r>";
			
			tvf.AddRequestResponse(secondRequest, secondResponse);

			Assert.AreEqual(1, tvf.RequestCount);

			//this should not cause an exception
			tvf.Replace(result, "");

		}
	}
}
