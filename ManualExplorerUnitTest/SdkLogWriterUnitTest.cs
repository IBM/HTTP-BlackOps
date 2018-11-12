using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficViewerSDK;
using System.Diagnostics;
using System.IO;

namespace ManualExplorerUnitTest
{
    [TestClass]
    public class SdkLogWriterUnitTest
    {
        [TestMethod]
        public void DoNothing()
        {
            SdkSettings.Instance.Logger = new DefaultLogWriter();
            SdkSettings.Instance.Logger.Log(TraceLevel.Error, "this is a test");
        }

        [TestMethod]
        public void FileSavePlainMessage()
        {
            try
            {
                TempFile temp = new TempFile(".txt");
                SdkSettings.Instance.Logger = new DefaultLogWriter(temp.Path, TraceLevel.Verbose);
                SdkSettings.Instance.Logger.Log(TraceLevel.Error, "This is a test");
                String output = File.ReadAllText(temp.Path);
                Assert.IsTrue(output.Contains("This is a test"));
                
            }
            catch (Exception ex)
            {
                Assert.Fail("Reason: {0}", ex.Message);
            }
        }
        
        [TestMethod]
        public void FileSaveMissingFormat()
        {
            try
            {
                TempFile temp = new TempFile(".txt");
                SdkSettings.Instance.Logger = new DefaultLogWriter(temp.Path, TraceLevel.Verbose);
                SdkSettings.Instance.Logger.Log(TraceLevel.Error, "This is a test: {0}");
                String output = File.ReadAllText(temp.Path);
                Assert.IsTrue(output.Contains("This is a test:"));
            }
            catch (Exception ex)
            {
                Assert.Fail("Reason: {0}", ex.Message);
            }
        }

        [TestMethod]
        public void FileSaveWithFormat()
        {
            try
            {
                TempFile temp = new TempFile(".txt");
                SdkSettings.Instance.Logger = new DefaultLogWriter(temp.Path, TraceLevel.Verbose);
                SdkSettings.Instance.Logger.Log(TraceLevel.Error, "This is a test: {0}", "Add as Params");
                String output = File.ReadAllText(temp.Path);
                Assert.IsTrue(output.Contains("This is a test: Add as Params"));

            }
            catch (Exception ex)
            {
                Assert.Fail("Reason: {0}", ex.Message);
            }
        }

        [TestMethod]
        public void FileSaveWithTraceJNoMatch()
        {
            try
            {
                TempFile temp = new TempFile(".txt");
                SdkSettings.Instance.Logger = new DefaultLogWriter(temp.Path, TraceLevel.Warning);
                SdkSettings.Instance.Logger.Log(TraceLevel.Error, "Add This Line");
                SdkSettings.Instance.Logger.Log(TraceLevel.Verbose, "Dont Add This Line");
                String output = File.ReadAllText(temp.Path);
                Assert.IsTrue(output.Contains("Error Add This Line"));
                Assert.IsFalse(output.Contains("Dont Add This Line"));
            }
            catch (Exception ex)
            {
                Assert.Fail("Reason: {0}", ex.Message);
            }
        }

    }
}
