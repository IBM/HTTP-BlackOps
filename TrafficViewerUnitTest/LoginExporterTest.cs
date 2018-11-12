using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficViewerSDK;
using TrafficViewerUnitTest.Properties;
using TrafficViewerSDK.Exporters;
using System.IO;
using TrafficViewerSDK.Options;
using TrafficViewerSDK.Importers;
using System.Xml;
using TrafficViewerInstance;

namespace TrafficViewerUnitTest
{
	[TestClass]
	public class LoginExporterTest
	{
		//[TestMethod]
		public void TestLoginExportType()
		{
			TempFile temp = new TempFile();
			temp.Write(Resources.AltoroLogin);

			TrafficViewerFile origFile = new TrafficViewerFile();
			origFile.Open(temp.Path);

			Assert.AreEqual(4, origFile.RequestCount);

			//export

			IList<ITrafficExporter> exporters = TrafficViewer.Instance.TrafficExporters;

			ITrafficExporter loginExporter = null;

			foreach (ITrafficExporter exporter in exporters)
			{
				if (exporter.Caption == "ASE Login Files (.login)")
				{
					loginExporter = exporter;
				}
			}

			Assert.IsNotNull(loginExporter);

			TempFile exportedFile = new TempFile("exporttest.login");
			Stream stream = exportedFile.OpenStream();

			loginExporter.Export(origFile, stream, "demo.testfire.net", 80);

			stream.Close();

			//import the exported file

			TrafficViewerFile import = new TrafficViewerFile();

			ITrafficParser configurationParser = TrafficViewer.Instance.GetParser("Configuration Parser");

			Assert.IsNotNull(configurationParser);

			configurationParser.Parse(exportedFile.Path, import, ParsingOptions.GetLegacyAppScanProfile());


			Assert.AreEqual(origFile.RequestCount, import.RequestCount);

			int i = -1;
			TVRequestInfo origInfo;
			while ((origInfo = origFile.GetNext(ref i))!=null)
			{
				TVRequestInfo importInfo = import.GetRequestInfo(origInfo.Id);
				string origRequest = Constants.DefaultEncoding.GetString(origFile.LoadRequestData(origInfo.Id));
				string importedRequest = Constants.DefaultEncoding.GetString(import.LoadRequestData(origInfo.Id));

				Assert.AreEqual(origRequest, importedRequest);
			}

		}

        //[TestMethod]
        public void TestVariableDefinitions()
        {
            TempFile temp = new TempFile();
            temp.Write(Resources.AltoroLogin);

            TrafficViewerFile origFile = new TrafficViewerFile();
            origFile.Open(temp.Path);

            Assert.AreEqual(4, origFile.RequestCount);

            //export

            IList<ITrafficExporter> exporters = TrafficViewer.Instance.TrafficExporters;

            ITrafficExporter loginExporter = null;

            foreach (ITrafficExporter exporter in exporters)
            {
                if (exporter.Caption == "AppScan Login Files (.login)")
                {
                    loginExporter = exporter;
                }
            }

            Assert.IsNotNull(loginExporter);

            TempFile exportedFile = new TempFile("exporttest.xml");
            Stream stream = exportedFile.OpenStream();

            loginExporter.Export(origFile, stream, "demo.testfire.net", 80);

            stream.Close();

            //import the exported file
            XmlDocument loginDoc = new XmlDocument();
            loginDoc.XmlResolver = null;
            
            loginDoc.Load(exportedFile.Path);

            XmlNode varDef = loginDoc.SelectSingleNode("//VariableDefinition[@Name='amSessionId']");

            Assert.IsNotNull(varDef);
            Assert.AreEqual("Cookie", varDef.SelectSingleNode("VariableType").InnerText);
            Assert.AreEqual("True", varDef.SelectSingleNode("SessionIDEnabled").InnerText);
        }
    
    }
}
