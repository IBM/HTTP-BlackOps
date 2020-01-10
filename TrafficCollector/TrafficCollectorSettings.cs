/**
Copyright 2019 Trend Micro, Incorporated, All Rights Reserved.
SPDX-License-Identifier: Apache-2.0
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing;

namespace TrafficCollector
{
    class TrafficCollectorSettings
    {
        private static TrafficCollectorSettings _instance = null;
        private static Object _lock = new Object();
        private string _dumpDir = "c:\\Temp";
        private string _ip = null;
        private string _testFile = null;
        private string _allowedHostsPattern = "";
          
        private TrafficCollectorSettings()
        {
        }

        public static TrafficCollectorSettings Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new TrafficCollectorSettings();
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Where to dump the files
        /// </summary>
        public string DumpDir { get => _dumpDir; set => _dumpDir = value; }
        /// <summary>
        /// The ip of the server
        /// </summary>
        public string Ip { get => _ip; set => _ip = value; }
        /// <summary>
        /// The test file to be used for security tests
        /// </summary>
        public string TestFile { get => _testFile; set => _testFile = value; }
        /// <summary>
        /// The target host should match this pattern
        /// </summary>
        public string AllowedHostsPattern { get => _allowedHostsPattern; set => _allowedHostsPattern = value; }
    }
}
