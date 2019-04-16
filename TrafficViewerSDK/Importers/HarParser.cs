/**
Copyright 2019 Trend Micro, Incorporated, All Rights Reserved.
SPDX-License-Identifier: Apache-2.0
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficViewerSDK.Http;
using TrafficViewerSDK.Options;
using TrafficViewerSDK.Properties;
using HarSharp;

namespace TrafficViewerSDK.Importers
{
    public class HarParser : ITrafficParser
    {
        private ParsingOptions _options = new ParsingOptions();
        private TrafficParserStatus _status = TrafficParserStatus.Stopped;
        private Dictionary<string, string> _importTypes = new Dictionary<string, string>();

        /// <summary>
        /// Constructor
        /// </summary>
        public HarParser()
        {
            _importTypes.Add("Har Files", "*.har");
            _importTypes.Add("All Files", "*.*");
        }

        /// <summary>
        /// Returns parser status
        /// </summary>
        public TrafficParserStatus ParserStatus
        {
            get { return _status; }
        }
        /// <summary>
        /// Gets the current options
        /// </summary>
        public ParsingOptions Options
        {
            get { return _options; }
        }

        /// <summary>
        /// Checks if a url is excluded
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="exclusions"></param>
        /// <returns></returns>
        private bool IsExcluded(Uri uri, IEnumerable<string> exclusions)
        {
            string uriString = uri.ToString();
            foreach (var exclusion in exclusions)
            {
                if (Utils.IsMatch(uriString, exclusion))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Executes the parsing operation
        /// </summary>
        /// <param name="pathOfFileToImport"></param>
        /// <param name="currentFile"></param>
        /// <param name="options"></param>
        public void Parse(string pathOfFileToImport, ITrafficDataAccessor currentFile, ParsingOptions options)
        {
            _options = options;
            var exclusions = options.GetExclusions();
            _status = TrafficParserStatus.Running;
            Har har = HarConvert.DeserializeFromFile(pathOfFileToImport);
            foreach (Entry entry in har.Log.Entries)
            {
                try
                {
                    if (!IsExcluded(entry.Request.Url, exclusions))
                    {
                        AddRequest(currentFile, entry);
                    }
                }
                catch (Exception ex)
                {
                    SdkSettings.Instance.Logger.Log(TraceLevel.Error, "URI Parser - Failed to add request: {0}", ex.Message);
                }
            }

            _status = TrafficParserStatus.Stopped;
        }

        private void AddRequest(ITrafficDataAccessor currentFile, Entry entry)
        {

            Uri uri = entry.Request.Url;
            //check exclusions
            Request harRequest = entry.Request;
            string request = String.Format("{0} {1} {2}\r\n\r\n", harRequest.Method,uri.PathAndQuery, harRequest.HttpVersion);
            HttpRequestInfo requestInfo = new HttpRequestInfo(request);
            //add the headers
            foreach (var header in harRequest.Headers)
            {
                if (!header.Name.ToLower().Equals("accept-encoding") &&
                    !header.Name.ToLower().Equals("if-modified-since") &&
                    !header.Name.ToLower().Equals("if-none-match"))
                {
                    requestInfo.Headers.Add(header.Name, header.Value);
                }
            }
            if (harRequest.PostData != null)
            {
                requestInfo.ContentData = Constants.DefaultEncoding.GetBytes(harRequest.PostData.Text);
            }
            TVRequestInfo tvReqInfo = new TVRequestInfo();

            tvReqInfo.Description = Resources.HarParserDescription;
            tvReqInfo.RequestLine = HttpRequestInfo.GetRequestLine(request);
            tvReqInfo.ThreadId = "N/A";
            tvReqInfo.RequestTime = DateTime.Now;
            tvReqInfo.IsHttps = String.Compare(uri.Scheme, "https", true) == 0;
            tvReqInfo.Host = uri.Host;

            Response harResponse = entry.Response;
            string responseHead = String.Format("{0} {1}\r\n\r\n", harResponse.HttpVersion, harResponse.Status);
            HttpResponseInfo respInfo = new HttpResponseInfo(responseHead);
            foreach (var header in harResponse.Headers)
            {
                respInfo.Headers.Add(header.Name, header.Value);
            }
            if(harResponse.Content != null && !String.IsNullOrWhiteSpace(harResponse.Content.Text))
            {
                respInfo.ResponseBody.AddChunkReference(Constants.DefaultEncoding.GetBytes(harResponse.Content.Text));
            }
            
            currentFile.AddRequestInfo(tvReqInfo);
            currentFile.SaveRequestResponse(tvReqInfo.Id, requestInfo.ToArray(false), respInfo.ToArray());
        }

        /// <summary>
        /// Executes a parsing operation from an object - not implemented
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="currentFile"></param>
        /// <param name="options"></param>
        public void Parse(object sender, ITrafficDataAccessor currentFile, ParsingOptions options)
        {
            throw new NotImplementedException("Not applicable for this import type");
        }


        /// <summary>
        /// Stop parsing
        /// </summary>
        public void Stop()
        {
            ;
        }

        /// <summary>
        /// Resume parsing
        /// </summary>
        public void Resume()
        {
            ;
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="tailChunk"></param>
        public void Tail(int tailChunk)
        {
            ;
        }

        /// <summary>
        /// Not applicable
        /// </summary>
        public void ClearSource()
        {
            throw new NotImplementedException("Not applicable for this import type");
        }


        /// <summary>
        /// Type of files supported
        /// </summary>
        public Dictionary<string, string> ImportTypes
        {
            get
            {
                return _importTypes;
            }
        }


        /// <summary>
        /// The name of the parser
        /// </summary>
        public string Name
        {
            get { return Resources.HarParserName; }
        }


        /// <summary>
        /// The type of imports that it supports
        /// </summary>
        public ImportMode ImportSupport
        {
            get { return ImportMode.Tail; }
        }
    }
}
