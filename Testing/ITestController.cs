using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing
{
    public interface ITestController
    {
        /// <summary>
        /// Logs an event from the test
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Log(string format, params object[] args);

        /// <summary>
        /// Sends an HTTP Request to the site
        /// </summary>
        /// <param name="mutatedRequest"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="useSSL"></param>
        /// <returns></returns>
        string SendTest(string mutatedRequest, string host, int port, bool useSSL);


        /// <summary>
        /// Updates session identifiers for this request
        /// </summary>
        /// <param name="mutatedRequest"></param>
        /// <param name="useSSL"></param>
        /// <returns></returns>
        string UpdateSessionIds(string mutatedRequest, bool useSSL);


        /// <summary>
        /// What to do when a issue is found
        /// </summary>
        /// <param name="origRawReq"></param>
        /// <param name="origRawResp"></param>
        /// <param name="requestUri"></param>
        /// <param name="englishIssueTypeName"></param>
        /// <param name="parameterName"></param>
        /// <param name="testRequest"></param>
        /// <param name="testResponse"></param>
        /// <param name="validation"></param>
        /// <param name="comment"></param>
        void HandleIssueFound(string origRawReq, string origRawResp, Uri requestUri, string englishIssueTypeName, string parameterName, List<string> testRequestList, List<string> testResponseList, string validation, string comment);

    }
}
