using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TrafficViewerSDK;
using System.Diagnostics;

namespace TrafficViewerSDK.Importers
{
    static class ParserUtils
    {
        /// <summary>
        /// Opens the Traffic Log file in such a manner that it can be written to while AppScan has it open
        /// </summary>
		/// <param name="filePath"></param>
        /// <returns></returns>
        public static FileStream OpenFile(string filePath) 
        {
            try
            {
                return File.Open(filePath, FileMode.Open,
                    FileAccess.Read, FileShare.ReadWrite); //needs to read for ASE
            }
            catch (Exception e)
            {
                SdkSettings.Instance.Logger.Log(TraceLevel.Error, "Cannot open raw traffic log: {0}", e.Message);
                return null;
            }
        }

    }

    enum LocationInThread
    { 
        NonHttpTraffic,
        InsideRequest,
        InsideResponse,
        Exclusion
    }



}
