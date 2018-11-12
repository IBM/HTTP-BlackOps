using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficViewerSDK;

namespace Testing
{
    public class TestJob
    {
        string _parameterName;
        /// <summary>
        /// Entity name
        /// </summary>
        public string ParameterName
        {
            get { return _parameterName; }
            set { _parameterName = value; }
        }

        string _parameterValue;
        /// <summary>
        /// Parameter value
        /// </summary>
        public string ParameterValue
        {
            get { return _parameterValue; }
            set { _parameterValue = value; }
        }

        private CustomTestDef _testDef;
        /// <summary>
        /// Test definition
        /// </summary>
        public CustomTestDef TestDef
        {
            get { return _testDef; }
            set { _testDef = value; }
        }

        RequestLocation _requestLocation;
        /// <summary>
        /// Where is the parameter entity located
        /// </summary>
        public RequestLocation RequestLocation
        {
            get { return _requestLocation; }
            set { _requestLocation = value; }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="location"></param>
        /// <param name="test"></param>
        public TestJob(string parameterName, string parameterValue, RequestLocation location, CustomTestDef test)
        {
            // TODO: Complete member initialization
            _parameterName = parameterName;
            _parameterValue = parameterValue;
            _requestLocation = location;
            _testDef = test;
        }


    }
}
