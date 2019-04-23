/**
Copyright 2019 Trend Micro, Incorporated, All Rights Reserved.
SPDX-License-Identifier: Apache-2.0
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrafficViewerSDK.Http;

namespace Testing
{
    public class MultiThreadedTestExecution
    {
        private Tester _tester;
        private int _numThreads;
        private bool _runnable = false;
        /// <summary>
        /// Gets whether the test execution is still running
        /// </summary>
        public bool IsRunning
        {
            get { return _runnable; }
           
        }
        private object _lock = new object();
        private Queue<TestJob> _testsQueue;
        /// <summary>
        /// Gets/sets the test queue
        /// </summary>
        public Queue<TestJob> TestsQueue
        {
            get { return _testsQueue; }
            set { _testsQueue = value; }
        }
        private string _rawRequest;
        private string _rawResponse;
        private Uri _reqUri;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tester"></param>
        /// <param name="rawRequest"></param>
        /// <param name="rawResponse"></param>
        /// <param name="reqUri"></param>
        /// <param name="numThreads"></param>
        public MultiThreadedTestExecution(Tester tester, string rawRequest, string rawResponse, Uri reqUri, int numThreads)
        {
            _tester = tester;
            _rawRequest = rawRequest;
            _rawResponse = rawResponse;
            _reqUri = reqUri;
            _numThreads = numThreads;
            _testsQueue = new Queue<TestJob>();
        }

        
        /// <summary>
        /// Starts test execution
        /// </summary>
        public void StartTestsAsync()
        {
            _runnable = true;
            for (int count = 0; count < _numThreads; count++)
            {
                Thread t = new Thread(TestThread);
                t.Start();
            }
        }

        /// <summary>
        /// Gets executed async
        /// </summary>
        private void TestThread()
        {
            while(_runnable)
            {
                TestJob testJob = null;
                lock (_lock)
                {
                    if (_testsQueue.Count > 0)
                    {
                        testJob = _testsQueue.Dequeue();
                    }
                    else
                    {
                        _runnable = false;
                    }

                }

                if (testJob != null)
                {
                    _tester.ExecuteTests(_rawRequest, _rawResponse, _reqUri, testJob.ParameterName, testJob.ParameterValue, testJob.RequestLocation, testJob.TestDef);
                }
            }
        }

        /// <summary>
        /// Cancel test run
        /// </summary>
        public void CancelTests()
        {
            _runnable = false;
        }
    }
}
