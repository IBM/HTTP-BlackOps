/**
Copyright 2019 Trend Micro, Incorporated, All Rights Reserved.
SPDX-License-Identifier: Apache-2.0
*/
using System;
using System.Collections.Generic;
using System.Linq;
using TrafficServer;
using TrafficViewerSDK.Http;

namespace TrafficCollector
{
    class CollectorProxyList
    {

        private static CollectorProxyList _instance = null;
        private static Object _lock = new Object();
        private Dictionary<int, IHttpProxy> _proxyList;

        private CollectorProxyList()
        {
            _proxyList = new Dictionary<int, IHttpProxy>();
        }

        /// <summary>
        /// The global list instance
        /// </summary>
        public static CollectorProxyList Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new CollectorProxyList();
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// List of proxy objects that are collecting traffic
        /// </summary>
        public Dictionary<int, IHttpProxy> ProxyList { get => _proxyList; }
    }
}
