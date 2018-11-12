using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrafficViewerSDK.Http;

namespace TrafficViewerUnitTest
{
    class MockCredentialsProvider : ICredentialsProvider
    {
        public bool Execute(out string domain, out string userName, out string password)
        {
            domain = "domain";
            userName = "jsmith";
            password = "Demo1234";
            return true;
        }
    }
}
