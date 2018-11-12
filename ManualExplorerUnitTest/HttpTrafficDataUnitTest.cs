using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficViewerSDK;
using System.IO;

namespace ManualExplorerUnitTest
{
	[TestClass]
	public class HttpTrafficDataUnitTest
	{

		[TestMethod]
		public void SaveAndOpen()
		{
			string expectedRequest = "GET / HTTP/1.1";
			string expectedResponse = "HTTP/1.1 200 OK";

			TrafficViewerFile file = new TrafficViewerFile();
			int reqId = file.AddRequestResponse(expectedRequest, expectedResponse);
			file.GetRequestInfo(reqId).IsHttps = true;

			Assert.AreEqual(1, file.RequestCount);

			TempFile temp = new TempFile(".tvf");
			file.Save(temp.Path);
			//verify that the file can be saved
			Assert.IsTrue(File.Exists(temp.Path),"Cannot save the file");

			file.Close(false);
			
			//make a new file and verify we can open
			TrafficViewerFile file2 = new TrafficViewerFile();
			file2.Open(temp.Path);
			//verify actual file was open
			Assert.AreEqual(1, file2.RequestCount, "Incorrect request count after opening saved file");
			//verify request data is correct
			int requestId = -1;
			TVRequestInfo info = file2.GetNext(ref requestId);
			Assert.IsNotNull(info, "Cannot obtain request info");

			//veryfy transport info
			Assert.IsTrue(info.IsHttps);

			//verify request data
			string loadedRequest = Encoding.UTF8.GetString(file2.LoadRequestData(info.Id));

			Assert.AreEqual(expectedRequest, loadedRequest);

			string loadedResponse = Encoding.UTF8.GetString(file2.LoadResponseData(info.Id));

			Assert.AreEqual(expectedResponse, loadedResponse);


			file2.Close(false);
		}

		[TestMethod]
		public void EditARequest()
		{
			string originalRequest = "GET / HTTP/1.1";
			string originalResponse = "HTTP/1.1 200 OK";

			TrafficViewerFile file = new TrafficViewerFile();
			int reqId = file.AddRequestResponse(originalRequest, originalResponse);
			Assert.AreEqual(1,	 file.RequestCount);
			TVRequestInfo reqInfo = file.GetRequestInfo(reqId);

			string newRequest = "POST /login HTTP/1.1";
			string newResponse = "HTTP/1.1 302 Redirect";

			file.SaveRequest(reqId, Encoding.UTF8.GetBytes(newRequest));
			file.SaveResponse(reqId, Encoding.UTF8.GetBytes(newResponse));

			//check the response info was updated
			Assert.AreEqual(newRequest, reqInfo.RequestLine);
			Assert.AreEqual("302", reqInfo.ResponseStatus);
			Assert.AreEqual(newRequest.Length, reqInfo.RequestLength);
			Assert.AreEqual(newResponse.Length, reqInfo.ResponseLength);

			string loadedRequest = Encoding.UTF8.GetString(file.LoadRequestData(reqId));
			Assert.AreEqual(newRequest, loadedRequest);
			string loadedResponse = Encoding.UTF8.GetString(file.LoadResponseData(reqId));
			Assert.AreEqual(newResponse, loadedResponse);
			file.Close(false);
		}

		[TestMethod]
		public void RemoveARequest()
		{
			string originalRequest = "GET / HTTP/1.1";
			string originalResponse = "HTTP/1.1 200 OK";

			TrafficViewerFile file = new TrafficViewerFile();
			int reqId = file.AddRequestResponse(originalRequest, originalResponse);
			//add one more
			file.AddRequestResponse(originalRequest, originalResponse);

			Assert.AreEqual(2, file.RequestCount);

			file.RemoveRequest(reqId);

			Assert.AreEqual(1, file.RequestCount);

			Assert.IsNull(file.GetRequestInfo(reqId));

			file.Close(false);
		
		}


		[TestMethod]
		public void RemoveARangeOfRequests()
		{
			string originalRequest = "GET / HTTP/1.1";
			string originalResponse = "HTTP/1.1 200 OK";

			List<int> addedRequests = new List<int>();

			TrafficViewerFile file = new TrafficViewerFile();
			addedRequests.Add(file.AddRequestResponse(originalRequest, originalResponse));
			addedRequests.Add(file.AddRequestResponse(originalRequest, originalResponse));

			//add one more
			int savedRequestId = file.AddRequestResponse(originalRequest, originalResponse);

			addedRequests.Add(file.AddRequestResponse(originalRequest, originalResponse));
			addedRequests.Add(file.AddRequestResponse(originalRequest, originalResponse));

			Assert.AreEqual(5, file.RequestCount);

			file.RemoveRequestBatch(addedRequests);
			

			Assert.AreEqual(1, file.RequestCount);

			Assert.IsNotNull(file.GetRequestInfo(savedRequestId));

			file.Close(false);

		}


		[TestMethod]
		public void VerifyTempDir()
		{
			string originalRequest = "GET / HTTP/1.1";
			string originalResponse = "HTTP/1.1 200 OK";
			string predefinedTempDir = Environment.GetEnvironmentVariable("temp");

			TrafficViewerFile file = new TrafficViewerFile(predefinedTempDir);
			file.AddRequestResponse(originalRequest, originalResponse);

			Assert.IsTrue(Directory.Exists(file.TempFileFolder));

			file.Close(true);

			Assert.IsTrue(Directory.Exists(file.TempFileFolder));

			file = new TrafficViewerFile(predefinedTempDir);
			file.AddRequestResponse(originalRequest, originalResponse);

			Assert.IsTrue(Directory.Exists(file.TempFileFolder));


			file.Close(false);
			//after the file is closed the file gets closed with the option to not keep temp files
			Assert.IsFalse(Directory.Exists(file.TempFileFolder));
		}


		[TestMethod]
		public void OpenMultipleFilesAtOnce()
		{
			string predefinedTempDir = Environment.GetEnvironmentVariable("temp");

			TrafficViewerFile file1 = new TrafficViewerFile(predefinedTempDir);
			Assert.IsTrue(Directory.Exists(file1.TempFileFolder));
			TrafficViewerFile file2 = new TrafficViewerFile(predefinedTempDir);
			Assert.IsTrue(Directory.Exists(file2.TempFileFolder));
			Assert.IsTrue(Directory.Exists(file1.TempFileFolder));
			TrafficViewerFile file3 = new TrafficViewerFile(predefinedTempDir);
			Assert.IsTrue(Directory.Exists(file3.TempFileFolder));
			//make sure that the 3 files have different temp file folders
			Assert.AreNotEqual(file1.TempFileFolder, file2.TempFileFolder);
			Assert.AreNotEqual(file2.TempFileFolder, file3.TempFileFolder);
			file1.Close(false);
			Assert.IsFalse(Directory.Exists(file1.TempFileFolder));
			Assert.IsTrue(Directory.Exists(file2.TempFileFolder));
			Assert.IsTrue(Directory.Exists(file3.TempFileFolder));
			file2.Close(false);
			file3.Close(false);
			Assert.IsFalse(Directory.Exists(file2.TempFileFolder));
			Assert.IsFalse(Directory.Exists(file3.TempFileFolder));
			
		}

		[TestMethod]
		public void IsSafeZipEntryUnitTest()
		{ 
			//check that the function used to validate the entry paths against path traversal is working correctly
			Assert.IsTrue(Utils.IsSafeZipEntry("file.txt",@"c:\windows\temp"));
			Assert.IsFalse(Utils.IsSafeZipEntry(@"..\file.txt", @"c:\windows\temp"));
			Assert.IsTrue(Utils.IsSafeZipEntry("file.txt", @"c:\progra~1\common~1"));

		}

	}
}
