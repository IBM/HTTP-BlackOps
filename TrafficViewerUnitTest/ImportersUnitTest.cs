using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;
using TrafficViewerUnitTest.Properties;
using TrafficViewerSDK.Importers;
using TrafficViewerSDK.Options;
using TrafficViewerSDK.Exporters;
using System.IO;

namespace TrafficViewerUnitTest
{
	[TestClass]
	public class ImportersUnitTest
	{
		void ValidateTrafficSourcesRequestsAreSame(ITrafficDataAccessor src1, ITrafficDataAccessor src2, bool includeResponses = true)
		{
			Assert.AreEqual(src1.RequestCount, src2.RequestCount);

			int i = -1, j = -1;

			
			while (true)
			{
				TVRequestInfo first = src1.GetNext(ref i);
				TVRequestInfo second = src2.GetNext(ref j);

				if (first == null && second == null)
				{
					break;
				}

				Assert.AreEqual(first.RequestLine, second.RequestLine);
				

				//proceed to compare http requests

				byte[] firstRequest = src1.LoadRequestData(first.Id);
				byte[] secondRequest = src2.LoadRequestData(second.Id);

				HttpRequestInfo firstRequestInfo = new HttpRequestInfo(firstRequest);
				HttpRequestInfo seconRequestInfo = new HttpRequestInfo(secondRequest);

				Assert.AreEqual(firstRequestInfo.ToString(), seconRequestInfo.ToString());


				if (includeResponses)
				{
					//proceed to compare responses
					Assert.AreEqual(first.ResponseStatus, second.ResponseStatus);

					byte[] firstResponse = src1.LoadResponseData(first.Id);
					byte[] secondResponse = src2.LoadResponseData(second.Id);

					HttpResponseInfo firstResponseInfo = new HttpResponseInfo(firstResponse);
					HttpResponseInfo secondResponseInfo = new HttpResponseInfo(secondResponse);

					Assert.AreEqual(firstResponseInfo.ToString(), secondResponseInfo.ToString());
				}
			}
		}




		[TestMethod]
		public void TestManualExploreImportExport()
		{
	
			//validate against existing TVF
			TrafficViewerFile compareTVF = GetCompareTVF(Resources.demoExploreFromTrafficImport);
			//export the tvf to exd
			ITrafficExporter exporter = new ManualExploreExporter();
			TempFile temp = new TempFile(".exd");
			Stream tempStream = temp.OpenStream();
			exporter.Export(compareTVF, tempStream);
			tempStream.Close();

			TrafficViewerFile importTVF = new TrafficViewerFile();
			ITrafficParser parser = new ConfigurationParser();

			parser.Parse(temp.Path, importTVF, new ParsingOptions());

			ValidateTrafficSourcesRequestsAreSame(compareTVF, importTVF, false);
		}


	

		private static string GetPathToImport(string text)
		{
			TempFile importTemp = new TempFile();
			importTemp.Write(text);
			return importTemp.Path;
		}

		private static string GetPathToImport(byte [] bytes)
		{
			TempFile importTemp = new TempFile();
			importTemp.Write(bytes);
			return importTemp.Path;
		}

		private static TrafficViewerFile GetCompareTVF(byte[] bytes)
		{
			TempFile compareTemp = new TempFile();
			compareTemp.Write(bytes);
			TrafficViewerFile compareTVF = new TrafficViewerFile();
			compareTVF.Open(compareTemp.Path);
			return compareTVF;
		}
	}
}
