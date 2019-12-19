using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficViewerSDK;
using TrafficViewerSDK.Options;
using TrafficViewerSDK.Http;
using System.Threading;
using TrafficViewerSDK.Exporters;
using System.IO;
using System.Xml;
using TrafficViewerSDK.Importers;
using System.Security.AccessControl;

namespace TrafficViewerUnitTest
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class TrafficViewerFileTest
	{
		public TrafficViewerFileTest()
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
        public void TestEncryptedRequest()
        {
            TrafficViewerFile file = new TrafficViewerFile();
            string request1 = "GET /unencrypted HTTP/1.1";
            string request2 = "GET /encrypted\r\n\r\nsecret=123456789 HTTP/1.1";
            string response1 = "HTTP 200 OK\r\n\r\nUnencrypted Response";
            string response2 = "HTTP 200 OK\r\n\r\nEncrypted Response (secret 1234567789)";
            file.AddRequestResponse(request1,response1);
            file.AddRequestResponse(request2, response2);

            var reqInfo = file.GetRequestInfo(1);
            Assert.IsFalse(reqInfo.IsEncrypted,"Default should be unencrypted");
            reqInfo.IsEncrypted = true;
            //resave the request
            file.SaveRequestResponse(1, request2, response2);
            TempFile tempFile = new TempFile();
            file.EnableDefrag = true; //defrag the raw file
            file.Save(tempFile.Path);

            file = new TrafficViewerFile();
            
            file.Open(tempFile.Path);


            Assert.IsFalse(file.GetRequestInfo(0).IsEncrypted, "First request should not be encrypted");
            Assert.IsTrue(file.GetRequestInfo(1).IsEncrypted, "Second request should be encrypted");


            string testRequest = Constants.DefaultEncoding.GetString(file.LoadRequestData(1));
            
            Assert.AreEqual(request2, testRequest);

            string testResponse = Constants.DefaultEncoding.GetString(file.LoadResponseData(1));
            Assert.AreEqual(response2, testResponse);
            file.Close(false);
            File.Delete(tempFile.Path);

        }

		[TestMethod]
		public void ImportASE()
		{
			ITrafficParser parser = new DefaultTrafficParser();
			//test ASE import
			TestImportParser(parser,MakeDummyASETrafficLog(),ParsingOptions.GetDefaultProfile());
		}

		[TestMethod]
		public void ImportAppscan()
		{
			ParsingOptions options = ParsingOptions.GetLegacyAppScanProfile();

			ITrafficParser parser = new DefaultTrafficParser();
			
			//test appscan import
			TrafficViewerFile tvFile = new TrafficViewerFile();
			TempFile log = new TempFile();
			log.Write(Properties.Resources.AppScanMETraffic);
			
			tvFile.StartImport(parser,log.Path, options);

			Assert.AreEqual(8, tvFile.RequestCount);

			string response = Encoding.UTF8.GetString(tvFile.LoadResponseData(3));

			Assert.IsTrue(response.Contains("ONLINE BANKING LOGIN"));
		}


		//[TestMethod]
		public void ExportExdUtil()
		{
			string sourcePath = @"c:\_transfer\jaguarmanualexplorefiltered.htd";
			TrafficViewerFile source = new TrafficViewerFile();
			source.Open(sourcePath);

			int id = -1;
			int index = 0;
			int count = source.RequestCount;
			int partNo = 1;
			int numberOfParts = 6;

			int partSize = count / numberOfParts;

			TVRequestInfo info;
			TrafficViewerFile currentPart = new TrafficViewerFile();
			while ((info = source.GetNext(ref id)) != null)
			{
				if (index < partSize * partNo)
				{
					byte [] request = source.LoadRequestData(info.Id);
					byte [] response = source.LoadResponseData(info.Id);
					currentPart.AddRequestResponse(request, response);
				}
				else
				{
					ExportPart(partNo, currentPart);
					currentPart.Close(false);
					currentPart = new TrafficViewerFile();
					partNo++;
				}
				index++;
			}

			if (currentPart.RequestCount > 0)
			{
				ExportPart(partNo, currentPart);
			}

		}

		private static void ExportPart(int part, TrafficViewerFile currentPart)
		{
			
			string exportFileFormat = @"c:\_export\meexport_{0}.exd";
			//export the current part
			ASEExdExporter exporter = new ASEExdExporter();
			string fName = String.Format(exportFileFormat, part);
			FileStream file = new FileStream(fName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
			exporter.Export(currentPart, file);
			file.Close();
		}

		[TestMethod]
		public void ExportAppscanToEXD()
		{
			ParsingOptions options = ParsingOptions.GetLegacyAppScanProfile();

			ITrafficParser parser = new DefaultTrafficParser();

			//test appscan import
			TrafficViewerFile tvFile = new TrafficViewerFile();
			TempFile log = new TempFile();
			log.Write(Properties.Resources.AppScanMETraffic);

			tvFile.StartImport(parser, log.Path, options);

			Assert.AreEqual(8, tvFile.RequestCount);

			ITrafficExporter exdExporter = new ManualExploreExporter();

			TempFile temp = new TempFile();
			
			Stream stream = temp.OpenStream();

			exdExporter.Export(tvFile, stream, "newHost.com", 8080);

			Assert.IsTrue(stream.Length > 0);

			stream.Flush();

			stream.Position = 0;

			XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
			doc.Load(stream);

			int noOfRequests = doc.SelectNodes("//request").Count;

			Assert.AreEqual(8, noOfRequests);

			//check that the post request is properly formed
			XmlNode postRequest = doc.SelectSingleNode("//request[@method='POST']");

			Assert.AreEqual(3, postRequest.SelectNodes("parameter").Count);
			Assert.AreEqual(2, postRequest.SelectNodes("cookie").Count);
			Assert.AreEqual(11, postRequest.SelectNodes("header").Count);

			stream.Close();

			}



		private static void TestImportParser(ITrafficParser parser, TempFile log, ParsingOptions profile)
		{
			TrafficViewerFile tvFile = new TrafficViewerFile();
			tvFile.StartImport(parser, log.Path, profile);
			ValidateASEFile(tvFile);
		}


		private static void ValidateASEFile(TrafficViewerFile tvFile)
		{
			//after the import we should have 2 requests
			Assert.AreEqual(2, tvFile.RequestCount);
			int i = -1;
			TVRequestInfo first = tvFile.GetNext(ref i);
			TVRequestInfo second = tvFile.GetNext(ref i);

			Assert.AreEqual("GET /index1 HTTP/1.1", first.RequestLine);
			Assert.AreEqual("[1000]", first.ThreadId);
			Assert.AreEqual("Stage::Purpose1", first.Description);

			Assert.AreEqual("POST /index2 HTTP/1.1", second.RequestLine);
			Assert.AreEqual("[2000]", second.ThreadId);
			Assert.AreEqual("Stage::Purpose2", second.Description);

			TimeSpan diff = second.RequestTime.Subtract(first.RequestTime);

			Assert.AreEqual(10, diff.Milliseconds);
			Assert.AreEqual("  0.03s", first.Duration);
			//check the requests
			HttpRequestInfo req1 = new HttpRequestInfo(tvFile.LoadRequestData(first.Id));
			HttpRequestInfo req2 = new HttpRequestInfo(tvFile.LoadRequestData(second.Id));

			Assert.AreEqual("demo.testfire.net", req1.Host);
			Assert.AreEqual("www.altoromutual.com", req2.Host);

			//check the responses
			Assert.AreEqual("200", first.ResponseStatus);
			Assert.AreEqual("302", second.ResponseStatus);

			HttpResponseInfo resp1 = new HttpResponseInfo();
			HttpResponseInfo resp2 = new HttpResponseInfo();

			resp1.ProcessResponse(tvFile.LoadResponseData(first.Id));
			resp2.ProcessResponse(tvFile.LoadResponseData(second.Id));

			string firstBody = resp1.ResponseBody.ToString();
			string secondBody = resp2.ResponseBody.ToString();

			Assert.IsTrue(firstBody.Contains("interrupt"));
			Assert.IsFalse(firstBody.Contains("--function"));

			Assert.IsTrue(secondBody.Contains("inter\r\nrupt"));
		}

		private static TempFile MakeDummyASETrafficLog()
		{
			TempFile trafficFile = new TempFile(".log");
			trafficFile.WriteLine("===> Test traffic");
			trafficFile.WriteLine("--- Begin Thread [1000] <2010-01-01 01:00:00.000> ---- Stage::Purpose1");
			trafficFile.WriteLine("====> Sending request ");
			trafficFile.WriteLine("GET /index1 HTTP/1.1");
			trafficFile.WriteLine("Accept: */*");
			trafficFile.WriteLine("--- End Thread [1000] ---- Stage::Purpose1");
			trafficFile.WriteLine("--- Begin Thread [2000] <2010-01-01 01:00:00.010> ---- Stage::Purpose2");
			trafficFile.WriteLine("POST /index2 HTTP/1.1");
			trafficFile.WriteLine("Accept: */*");
			trafficFile.WriteLine("Host: www.altoromutual.com");
			trafficFile.WriteLine("");
			trafficFile.WriteLine("--- End Thread [2000] ---- Stage::Purpose2");
			trafficFile.WriteLine("--- Begin Thread [1000] <2010-01-01 01:00:00.020> ---- Stage::Purpose1");
			trafficFile.WriteLine("\r\nHost: demo.testfire.net");
			trafficFile.WriteLine("");
			trafficFile.WriteLine("--- End Thread [1000] ---- Stage::Purpose1");
			trafficFile.WriteLine("--- Begin Thread [1000] <2010-01-01 01:00:00.030> ---- Stage::Purpose1");
			//start responses
			trafficFile.WriteLine("====> Receiving the response for the request GET /index1 HTTP/1.1 ");
			trafficFile.WriteLine("HTTP/1.1 200 OK");
			trafficFile.WriteLine("Header: Value");
			trafficFile.WriteLine("");
			//interrupt the response with the second thread
			trafficFile.WriteLine("Starting response. Here we inter");
			trafficFile.WriteLine("--- End Thread [1000] ---- Stage::Purpose1");
			trafficFile.WriteLine("--- Begin Thread [2000] <2010-01-01 01:00:00.040> ---- Stage::Purpose2");
			trafficFile.WriteLine("====> Receiving the response for the request GET /index2 HTTP/1.1 ");
			trafficFile.WriteLine("HTTP/1.1 302 Redirect");
			trafficFile.WriteLine("Header: Value");
			trafficFile.WriteLine("");
			trafficFile.WriteLine("Starting response. Here we inter\nrupted the text.");
			trafficFile.WriteLine("--- End Thread [2000] ---- Stage::Purpose2");
			trafficFile.WriteLine("--- Begin Thread [1000] <2010-01-01 01:00:00.050> ---- Stage::Purpose1");
			trafficFile.WriteLine("rupted the response.");
			trafficFile.WriteLine("//---------------------------\r\n");
			trafficFile.WriteLine("--- End Thread [1000] ---- Stage::Purpose1");
			trafficFile.WriteLine("--- Begin Thread [1000] <2010-01-01 01:00:00.060> ---- Stage::Purpose1");
			trafficFile.WriteLine("function testJS(){}");
			trafficFile.WriteLine("--- End Thread [1000] ---- Stage::Purpose1");
			return trafficFile;
		}

		private static TrafficViewerFile MakeDummyTrafficFile()
		{
			TrafficViewerFile tvf = new TrafficViewerFile();
			TempFile log = MakeDummyASETrafficLog();
			ITrafficParser parser = new DefaultTrafficParser();
			tvf.StartImport(parser, log.Path, ParsingOptions.GetDefaultProfile());
			return tvf;
		}

		[TestMethod]
		public void SaveAndOpen()
		{
			TrafficViewerFile tvf = MakeDummyTrafficFile();
			TempFile temp = new TempFile(".tvf");
			tvf.Save(temp.Path);
			tvf.Close(false);
			//verfiy that we can open
			tvf.Open(temp.Path);
			//run all validations
			ValidateASEFile(tvf);
			tvf.Close(false);
		}

		[TestMethod]
		public void Clear()
		{
			TrafficViewerFile tvf = MakeDummyTrafficFile();
			TempFile temp = new TempFile(".tvf");
			tvf.Save(temp.Path);
			Assert.AreNotSame(0, tvf.RequestCount);

			tvf.Clear(false);

			Assert.AreEqual(0, tvf.RequestCount);
			int i = -1;
			Assert.IsNull(tvf.GetNext(ref i));

			tvf.Close(false);
		}

		[TestMethod]
		public void EditTVF()
		{
			TrafficViewerFile tvf = UnitTestUtils.GenerateTestTvf();
			//check delete
			int initialCount = tvf.RequestCount;
			//get the first request id
			int i = -1;
			TVRequestInfo first = tvf.GetNext(ref i);
			TVRequestInfo second = tvf.GetNext(ref i);

			HttpRequestInfo secondRequest = new HttpRequestInfo(tvf.LoadRequestData(second.Id));
			
			HttpResponseInfo secondResponse = new HttpResponseInfo();
			byte [] respBytes = tvf.LoadResponseData(second.Id);
			secondResponse.ProcessResponse(respBytes);
			int referenceResponseStatus = secondResponse.Status;

			int referenceHash = secondRequest.GetHashCode();
		    
			Assert.IsTrue(tvf.RemoveRequest(first.Id));
			Assert.AreEqual(initialCount - 1, tvf.RequestCount);
			Assert.IsNull(tvf.GetPrevious(ref i));

			RequestDataCache.Instance.Clear();
			//check that 

			//check add

			TVRequestInfo reqInfo = new TVRequestInfo();
			reqInfo.RequestLine = "GET /newrequest HTTP/1.1";
			string request = "GET /newrequest HTTP/1.1\r\nHeader1:1\r\n\r\n";
			string response = "HTTP 200 OK\r\nHeader1:1\r\n\r\n<html><body>Added request</body></html>";

			RequestResponseBytes reqData = new RequestResponseBytes();
			reqData.AddToRequest(Constants.DefaultEncoding.GetBytes(request));
			reqData.AddToResponse(Constants.DefaultEncoding.GetBytes(response));

			tvf.AddRequestInfo(reqInfo);
			tvf.SaveRequest(reqInfo.Id, reqData);
			tvf.SaveResponse(reqInfo.Id, reqData);

			//Check that the request was added
			response = Constants.DefaultEncoding.GetString(tvf.LoadResponseData(reqInfo.Id));

			Assert.AreEqual(38, response.IndexOf("Added request"));
			Assert.AreEqual(65, response.Length);
			//modify the recently added request slightly

		}
	}
}
