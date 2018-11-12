using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrafficViewerSDK;
using System.IO;
using TrafficViewerUnitTest.Properties;
using System.Net;
using ManualExplorerUnitTest;
using Testing;
using TrafficViewerSDK.Http;
using System.Threading;
using System.Runtime.InteropServices;

namespace TrafficViewerUnitTest
{
    
    public class MockTestController : ITestController
    {
        public const string PATH_TRAVERSAL = "../../../../../../../../etc/passwd";
        public const string PATH_TRAVERSAL_RESPONSE = "HTTP/1.1 200 OK\r\nroot::";
        Dictionary<string, string> _issuesFound = new Dictionary<string, string>();
        ITrafficDataAccessor _mockSite = new TrafficViewerFile();
        IHttpClient _httpClient;

        public Dictionary<string, string> IssuesFound
        {
            get { return _issuesFound; }
            set { _issuesFound = value; }
        }

        List<string> _mutatedRequests = new List<string>();

        public List<string> MutatedRequests
        {
            get { return _mutatedRequests; }
            set { _mutatedRequests = value; }
        }

        public void Log(string format, params object[] args)
        {
            //do nothing
        }

        public MockTestController(ITrafficDataAccessor mockSite)
        {
            _mockSite = mockSite;
            _httpClient = new MockHttpClient(_mockSite);
        }

        public MockTestController():this(new TrafficViewerFile())
        {
         
        }

        public string SendTest(string mutatedRequest, string host, int port, bool useSSL)
        {
            //do nothing
            _mutatedRequests.Add(mutatedRequest);

            if (mutatedRequest.Contains(PATH_TRAVERSAL))
            {
                return PATH_TRAVERSAL_RESPONSE;
            }

            HttpRequestInfo testReqInfo = new HttpRequestInfo(mutatedRequest);
            testReqInfo.Host = host;
            testReqInfo.Port = port;
            testReqInfo.IsSecure = useSSL;
            HttpResponseInfo respInfo = _httpClient.SendRequest(testReqInfo);
            if (respInfo != null)
            {
                return respInfo.ToString();
            }
            return null;

        }

        public string UpdateSessionIds(string mutatedRequest, bool useSSL)
        {
            //do nothing
            return mutatedRequest;
        }

        public void HandleIssueFound(string origRawReq, string origRawResp, Uri requestUri, string englishIssueTypeName, string parameterName, List<string> testRequestList, List<string> testResponseList, string validation, string comment)
        {

            _issuesFound.Add(parameterName, englishIssueTypeName);

        }
    }


    [TestClass]
    public class CustomTestUnit
    {
       
        
        [TestMethod]
        public void CustomTester_MatchFileValidation()
        {
            TrafficViewerFile mockSite = new TrafficViewerFile();
            MockTestController mockTestController = new MockTestController(mockSite);


            string testRequest = "GET /search.aspx?txtSearch=a&a1=a HTTP/1.1\r\nHost: 127.0.0.1\r\n\r\n";
            string paramName = "txtSearch";
           
            CustomTestsFile file = GetCustomTestFile();
            Tester tester = new Tester(mockTestController, file);
            CustomTestDef def = file.GetCustomTests()["Path Traversal"];
            
            TempFile tempFile = new TempFile();
            tempFile.Write("boogers\r\nroot\r\n");

            def.Validation = "$match_file=" + tempFile.Path;

            HttpRequestInfo original = new HttpRequestInfo(testRequest, true);
            Uri uri = new Uri(original.FullUrl);

            string entityId = tester.GetEntityId(uri, paramName);
            string entityString = tester.GetEntityString(testRequest, uri, paramName, original.QueryVariables[paramName]);
            TestJob testJob = new TestJob(paramName, original.QueryVariables[paramName], RequestLocation.Query, def);
            string mutatedRequest = tester.GenerateMutatedRequestList(testRequest, testJob, entityString, entityId)[0];
            Assert.IsTrue(tester.ValidateSingleTest(testRequest, "HTTP/1.1 200 OK\r\nbla", new Uri("http://demo.testfire.net/search.aspx"), 
                paramName, entityId, def, mutatedRequest, "HTTP/1.1 200 OK\r\nroot::"));
            
        }

        [TestMethod]
        public void CustomTester_MatchBodyValidation()
        {
            TrafficViewerFile mockSite = new TrafficViewerFile();
            MockTestController mockTestController = new MockTestController(mockSite);


            string testRequest = "GET /search.aspx?txtSearch=a&a1=a HTTP/1.1\r\nHost: 127.0.0.1\r\n\r\n";
            string paramName = "txtSearch";

            CustomTestsFile file = GetCustomTestFile();
            Tester tester = new Tester(mockTestController, file);
            CustomTestDef def = file.GetCustomTests()["Path Traversal"];



            def.Validation = "$body=" + "root::";

            HttpRequestInfo original = new HttpRequestInfo(testRequest, true);
            Uri uri = new Uri(original.FullUrl);

            string entityId = tester.GetEntityId(uri, paramName);
            string entityString = tester.GetEntityString(testRequest, uri, paramName, original.QueryVariables[paramName]);
            TestJob testJob = new TestJob(paramName, original.QueryVariables[paramName], RequestLocation.Query, def);
            string mutatedRequest = tester.GenerateMutatedRequestList(testRequest, testJob, entityString, entityId)[0];
            Assert.IsTrue(tester.ValidateSingleTest(testRequest, "HTTP/1.1 200 OK\r\nbla", new Uri("http://demo.testfire.net/search.aspx"),
                paramName, entityId, def, mutatedRequest, "HTTP/1.1 200 OK\r\n\r\nroot::"));
            Assert.IsFalse(tester.ValidateSingleTest(testRequest, "HTTP/1.1 200 OK\r\nbla", new Uri("http://demo.testfire.net/search.aspx"),
                paramName, entityId, def, mutatedRequest, "HTTP/1.1 200 OK\r\nroot::\r\n\r\nbody"));
        }

        [TestMethod]
        public void CustomTester_MatchHeaderValidation()
        {
            TrafficViewerFile mockSite = new TrafficViewerFile();
            MockTestController mockTestController = new MockTestController(mockSite);


            string testRequest = "GET /search.aspx?txtSearch=a&a1=a HTTP/1.1\r\nHost: 127.0.0.1\r\n\r\n";
            string paramName = "txtSearch";

            CustomTestsFile file = GetCustomTestFile();
            Tester tester = new Tester(mockTestController, file);
            CustomTestDef def = file.GetCustomTests()["Path Traversal"];



            def.Validation = "$header=" + "root:\\s?:";

            HttpRequestInfo original = new HttpRequestInfo(testRequest, true);
            Uri uri = new Uri(original.FullUrl);

            string entityId = tester.GetEntityId(uri, paramName);
            string entityString = tester.GetEntityString(testRequest, uri, paramName, original.QueryVariables[paramName]);
            TestJob testJob = new TestJob(paramName, original.QueryVariables[paramName], RequestLocation.Query, def);
            string mutatedRequest = tester.GenerateMutatedRequestList(testRequest, testJob, entityString, entityId)[0];
            Assert.IsFalse(tester.ValidateSingleTest(testRequest, "HTTP/1.1 200 OK\r\nbla", new Uri("http://demo.testfire.net/search.aspx"),
                paramName, entityId, def, mutatedRequest, "HTTP/1.1 200 OK\r\n\r\nroot::"));
            Assert.IsTrue(tester.ValidateSingleTest(testRequest, "HTTP/1.1 200 OK\r\nbla", new Uri("http://demo.testfire.net/search.aspx"),
                paramName, entityId, def, mutatedRequest, "HTTP/1.1 200 OK\r\nroot::\r\n\r\nbody"));
        }


        [TestMethod]
        public void CustomTester_TestScriptingEngine()
        {
            using (ScriptEngine engine = new ScriptEngine(ScriptEngine.JavaScriptLanguage))
            {
                ParsedScript parsed = engine.Parse(
"function Callback(x){return '1,2,'+x;}");
                
                var val = parsed.CallMethod("Callback", 3) as string;

                Assert.IsNotNull(val);
                Assert.AreEqual("1,2,3", val);
            }

        }

        [TestMethod]
        public void CustomTester_TestScriptingRuleManyAs()
        {
            TrafficViewerFile mockSite = new TrafficViewerFile();
            CustomTestDef def = new CustomTestDef("ManyAs", "Buffer Overflow", 
                "$js_code=function Callback(){var ret = ''; for(var i=0;i<100;i++){ret+='A';} return ret;}", "");
            TestJob job = new TestJob("x", "y", RequestLocation.Query, def);
            CustomTestsFile file = GetCustomTestFile();
            Tester tester = new Tester(new MockTestController(mockSite), file);

            var list = tester.GeneratePayloadListFromMutation("GET /x=y HTTP/1.1\r\n", job, false, "bla");

            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);
            string expected = "";
            for (int i = 0; i < 100; i++) expected += "A";
            Assert.AreEqual(expected, list[0]);

        }

        [TestMethod]
        public void CustomTester_TestScriptingRuleBasedOnComponent()
        {
            TrafficViewerFile mockSite = new TrafficViewerFile();
            CustomTestDef def = new CustomTestDef("BlindSQL", "BlindSQL",
                "$js_code=function Callback(rawRequest, entityName, entityValue, requestLocation){if(requestLocation.indexOf('Query') > -1) return encodeURIComponent(\"' or '1'='1\");}", "");
            TestJob job = new TestJob("x", "y", RequestLocation.Query, def);
            CustomTestsFile file = GetCustomTestFile();
            Tester tester = new Tester(new MockTestController(mockSite), file);

            var list = tester.GeneratePayloadListFromMutation("GET /x=y HTTP/1.1\r\n", job, false, "don't care");

            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);
            string expected = "'%20or%20'1'%3D'1";
            
            Assert.AreEqual(expected, list[0]);

        }


        [TestMethod]
        public void CustomTester_TestMultiPayloads()
        {
            TrafficViewerFile mockSite = new TrafficViewerFile();
            CustomTestDef def = new CustomTestDef("BlindSQLABC", "Blind SQL",
                @"a\,,b,c", "");
            TestJob job = new TestJob("x", "y", RequestLocation.Query, def);
            CustomTestsFile file = GetCustomTestFile();
            Tester tester = new Tester(new MockTestController(mockSite), file);

            var list = tester.GeneratePayloadListFromMutation("GET /x=y HTTP/1.1\r\n", job, false, "don't care");

            Assert.IsNotNull(list);
            Assert.AreEqual(3, list.Count);

            Assert.AreEqual("a,", list[0]);
            Assert.AreEqual("b", list[1]);
            Assert.AreEqual("c", list[2]);

        }

        [TestMethod]
        public void CustomTester_TestMultiEncoding()
        {
            TrafficViewerFile mockSite = new TrafficViewerFile();
            string payload = "<'\0a";
            CustomTestDef def = new CustomTestDef("LT", "LT",
                payload, "");
            TestJob job = new TestJob("x", "y", RequestLocation.Query, def);
            CustomTestsFile file = GetCustomTestFile();
            file.GenerateAllEncodings = true;
            Tester tester = new Tester(new MockTestController(mockSite), file);

            var list = tester.GeneratePayloadListFromMutation("GET /x=y HTTP/1.1\r\n", job, false, "don't care");

            Assert.IsNotNull(list);
            Assert.AreEqual(7, list.Count);

            Assert.AreEqual(payload, list[0]);
            Assert.AreEqual(Utils.UrlEncode(payload), list[1]);
            Assert.AreEqual(Utils.UrlEncode(Utils.UrlEncode(payload)), list[2]);
            Assert.AreEqual(Utils.UrlEncodeAll(payload), list[3]);
            Assert.AreEqual(Utils.JSONEncode(payload), list[4]);
            Assert.AreEqual(Utils.HtmlEncode(payload), list[5]);
            Assert.AreEqual(Utils.Base64Encode(payload), list[6]);

        }


        [TestMethod]
        public void CustomTester_TestMultiPayloadsWithTicks()
        {
            TrafficViewerFile mockSite = new TrafficViewerFile();
            CustomTestDef def = new CustomTestDef("BlindSQLABC", "Blind SQL",
                @"__dynamic_value__ticks__,__dynamic_value__ticks__,__dynamic_value__ticks__", "");
            TestJob job = new TestJob("x", "y", RequestLocation.Query, def);
            CustomTestsFile file = GetCustomTestFile();
            Tester tester = new Tester(new MockTestController(mockSite), file);
            var entity_string = tester.GetEntityString("GET /x=y HTTP/1.1\r\n", new Uri("http://localhost/x=y"), "x", "y");
            var entity_id = tester.GetEntityId(new Uri("http://localhost/x=y"), "x");
            var list = tester.GenerateMutatedRequestList("GET /x=y HTTP/1.1\r\n", job,entity_string,entity_id);

            Assert.IsNotNull(list);
            Assert.AreEqual(3, list.Count);

            Assert.AreNotEqual(list[0],list[1]);
            Assert.AreNotEqual(list[1],list[2]);
            

        }

        [TestMethod]
        public void CustomTester_SingleCharacterValue()
        {
            TrafficViewerFile mockSite = new TrafficViewerFile();
            MockTestController mockTestController = new MockTestController(mockSite);


            string testRequest = "GET /search.aspx?txtSearch=a&a1=a HTTP/1.1\r\nHost: 127.0.0.1\r\n\r\n";
            string paramName = "txtSearch";
            string paramName2 = "a1";
            CustomTestsFile file = GetCustomTestFile();
            Tester tester = new Tester(mockTestController, file);
            CustomTestDef def = file.GetCustomTests()["Path Traversal"];
            HttpRequestInfo original = new HttpRequestInfo(testRequest, true);
            Uri uri = new Uri(original.FullUrl);

            string entityId = tester.GetEntityId(uri, paramName);
            string entityString = tester.GetEntityString(testRequest, uri, paramName, original.QueryVariables[paramName]);
            TestJob testJob = new TestJob(paramName, original.QueryVariables[paramName], RequestLocation.Query, def);
            string mutatedRequest = tester.GenerateMutatedRequestList(testRequest, testJob, entityString, entityId)[0];
            HttpRequestInfo mutatedReqInfo = new HttpRequestInfo(mutatedRequest, true);

            Assert.IsTrue(mutatedReqInfo.QueryVariables.ContainsKey(paramName), "Could no longer find parameter");
            Assert.AreEqual(original.QueryVariables[paramName] + MockTestController.PATH_TRAVERSAL, mutatedReqInfo.QueryVariables[paramName], "Incorrect test value");
            Assert.AreEqual(original.QueryVariables[paramName2] , mutatedReqInfo.QueryVariables[paramName2], "Incorrect non-test value");

        }

        [TestMethod]
        public void CustomTester_ParameterWithNoValue()
        {
            TrafficViewerFile mockSite = new TrafficViewerFile();
            MockTestController mockTestController = new MockTestController(mockSite);


            string testRequest = "POST /search.aspx HTTP/1.1\r\nHost: 127.0.0.1\r\n\r\ntxtSearch=\r\n\r\n";
            string paramName = "txtSearch";
            
            CustomTestsFile file = GetCustomTestFile();
            Tester tester = new Tester(mockTestController, file);
            CustomTestDef def = file.GetCustomTests()["Path Traversal"];
            HttpRequestInfo original = new HttpRequestInfo(testRequest, true);
            Uri uri = new Uri(original.FullUrl);

            string entityId = tester.GetEntityId(uri, paramName);
            string entityString = tester.GetEntityString(testRequest, uri, paramName, original.BodyVariables[paramName]);
            TestJob testJob = new TestJob(paramName, original.BodyVariables[paramName], RequestLocation.Body, def);
            string mutatedRequest = tester.GenerateMutatedRequestList(testRequest, testJob, entityString, entityId)[0];
            HttpRequestInfo mutatedReqInfo = new HttpRequestInfo(mutatedRequest, true);

            Assert.IsTrue(mutatedReqInfo.BodyVariables.ContainsKey(paramName), "Could no longer find parameter");
            Assert.AreEqual(original.BodyVariables[paramName] + MockTestController.PATH_TRAVERSAL, mutatedReqInfo.BodyVariables[paramName], "Incorrect test value");
            
        }


        [TestMethod]
        public void CustomTester_Fuzz()
        {
            TrafficViewerFile mockSite = new TrafficViewerFile();
            MockTestController mockTestController = new MockTestController(mockSite);


            string testRequest = "GET /search.aspx?txtSearch=("+Constants.FUZZ_STRING+") HTTP/1.1\r\nHost: 127.0.0.1\r\n\r\n";
            string paramName = "txtSearch";
           
            CustomTestsFile file = GetCustomTestFile();
            Tester tester = new Tester(mockTestController, file);
            CustomTestDef def = file.GetCustomTests()["Path Traversal"];
            HttpRequestInfo original = new HttpRequestInfo(testRequest, true);
            Uri uri = new Uri(original.FullUrl);

            string entityId = tester.GetEntityId(uri, paramName);
            string entityString = tester.GetEntityString(testRequest, uri, paramName, original.QueryVariables[paramName]);
            TestJob testJob = new TestJob(paramName, original.QueryVariables[paramName], RequestLocation.Query, def);
            string mutatedRequest = tester.GenerateMutatedRequestList(testRequest, testJob, entityString, entityId)[0];
            HttpRequestInfo mutatedReqInfo = new HttpRequestInfo(mutatedRequest, true);

            Assert.IsTrue(mutatedReqInfo.QueryVariables.ContainsKey(paramName), "Could no longer find parameter");
            Assert.AreEqual("(" + MockTestController.PATH_TRAVERSAL + ")", mutatedReqInfo.QueryVariables[paramName], "Incorrect test value");
            

        }


        [TestMethod]
        public void CustomTester_EmptyQueryParamUnitTest()
        {

            TrafficViewerFile mockSite = new TrafficViewerFile();
            mockSite.AddRequestResponse(String.Format("GET /search.jsp?query={0} HTTP/1.1\r\nHost: 127.0.0.1\r\n\r\n",MockTestController.PATH_TRAVERSAL),
                    MockTestController.PATH_TRAVERSAL_RESPONSE);
            MockTestController mockTestController = new MockTestController(mockSite);


            string testRequest = "GET /search.jsp?query= HTTP/1.1\r\nHost: 127.0.0.1\r\n\r\n";
            string paramName = "query";
            CustomTestsFile file = GetCustomTestFile();
            Tester tester = new Tester(mockTestController, file);
            CustomTestDef def = file.GetCustomTests()["Path Traversal"];
            HttpRequestInfo original = new HttpRequestInfo(testRequest, true);
            Uri uri = new Uri(original.FullUrl);

            tester.ExecuteTests(testRequest, "", uri, paramName, null, RequestLocation.Query, def);
            Assert.IsTrue(mockTestController.IssuesFound.ContainsKey(paramName));
        }

        [TestMethod]
        public void CustomTester_ParametersWithSameValue()
        {
            
            MockTestController mockTestController = new MockTestController();


            string testRequest = "GET /search.jsp?a=1&b=1 HTTP/1.1\r\nHost: 127.0.0.1\r\n\r\n";
            string paramName1 = "a";
            string paramName2 = "b";
            CustomTestsFile file = GetCustomTestFile();
            Tester tester = new Tester(mockTestController, file);
            CustomTestDef def = file.GetCustomTests()["Path Traversal"];
            HttpRequestInfo original = new HttpRequestInfo(testRequest, true);
            Uri uri = new Uri(original.FullUrl);

            tester.ExecuteTests(testRequest, "", uri, paramName1, null, RequestLocation.Query, def);
            Assert.IsTrue(mockTestController.IssuesFound.ContainsKey(paramName1));
            string mutReq = mockTestController.MutatedRequests[0];
            HttpRequestInfo mutReqInfo = new HttpRequestInfo(mutReq);
            Assert.AreEqual(MockTestController.PATH_TRAVERSAL, mutReqInfo.QueryVariables[paramName1],"Invalid mutation for "+paramName1);
            Assert.AreEqual("1", mutReqInfo.QueryVariables[paramName2], "Invalid value for " + paramName2);
            tester.ExecuteTests(testRequest, "", uri, paramName2, null, RequestLocation.Query, def);
            Assert.IsTrue(mockTestController.IssuesFound.ContainsKey(paramName2));
            mutReq = mockTestController.MutatedRequests[1];
            mutReqInfo = new HttpRequestInfo(mutReq);
            Assert.AreEqual("1", mutReqInfo.QueryVariables[paramName1], "Invalid value for " + paramName1);
            Assert.AreEqual(MockTestController.PATH_TRAVERSAL, mutReqInfo.QueryVariables[paramName2], "Invalid mutation for " + paramName2);
        }


        [TestMethod]
        public void CustomTester_DynamicValue()
        {
            
            MockTestController mockTestController = new MockTestController();


            string testRequest = "GET /search.jsp?query= HTTP/1.1\r\nDyn:__dynamic_value__ticks__\r\nHost: 127.0.0.1\r\n\r\n";
            string paramName = "query";
            CustomTestsFile file = GetCustomTestFile();
            Tester tester = new Tester(mockTestController, file);
            CustomTestDef def = file.GetCustomTests()["Path Traversal"];
            HttpRequestInfo original = new HttpRequestInfo(testRequest, true);
            Uri uri = new Uri(original.FullUrl);

            tester.ExecuteTests(testRequest, "", uri, paramName, null, RequestLocation.Query, def);
            Assert.IsTrue(mockTestController.IssuesFound.ContainsKey(paramName));

            Assert.AreEqual(1, mockTestController.MutatedRequests.Count, "Incorrect number of mutated requests");
            HttpRequestInfo mutatedRequest = new HttpRequestInfo(mockTestController.MutatedRequests[0]);
            Assert.IsTrue(Utils.IsMatch(mutatedRequest.Headers["Dyn"],"\\d+"),"Incorrect dynamic header value");

        }


        [TestMethod]
        public void CustomTestProxy_TestJSValidation()
        {
            MockProxy mockSite = new MockProxy();
            string testReq = "GET /r1?p1=test HTTP/1.1\r\n";
         
            mockSite.MockSite.AddRequestResponse(testReq, "HTTP/1.1 200 OK\r\n\r\nFound user test");
            mockSite.Start();

            CustomTestsFile testFile = GetCustomTestFile();

            var tests = testFile.GetCustomTests();
            tests.Clear();
            tests.Add("PathTraversal",
                new CustomTestDef("PathTraversal", "Path Traversal",
                    "$original/" + MockTestController.PATH_TRAVERSAL,
                    "$js_code=function Callback(response){var found = false; if(response.indexOf('root')>-1) found=true; return found;}"));

            testFile.SetCustomTests(tests);
            testFile.Save();
                        
            TrafficViewerFile testDataStore = new TrafficViewerFile();
            MockTestController mockTestController = new MockTestController(mockSite.MockSite);

            var targetList = new Dictionary<string, AttackTarget>();
            targetList.Add("r1", new AttackTarget("r1", "Enabled", "r1"));
            testFile.SetAttackTargetList(targetList);
            DriveByAttackProxy testProxy = new DriveByAttackProxy(mockTestController, testFile, testDataStore);

            testProxy.Start();


            SendRequestThroughTestProxy(testReq, testProxy, mockSite);
            

            Thread.Sleep(100);

            testProxy.Stop();

            Assert.IsTrue(mockTestController.IssuesFound.ContainsKey("p1"));
            



        }


        [TestMethod]
        public void CustomTestProxy_TestPatternToTest()
        {
            MockProxy mockSite = new MockProxy();
            string first = "GET / HTTP/1.1\r\n";
            string second = "POST /r1 HTTP/1.1\r\n\r\np1=1234\r\n";
            string third = "POST /r2 HTTP/1.1\r\n\r\np2=1234\r\n\r\n";
            mockSite.MockSite.AddRequestResponse(first, "HTTP/1.1 200 OK\r\n\r\nbla");
            mockSite.MockSite.AddRequestResponse(second, "HTTP/1.1 200 OK\r\n\r\nroot:0:0");
            mockSite.MockSite.AddRequestResponse(third, "HTTP/1.1 200 OK\r\n\r\nroot:0:0");

            mockSite.Start();

            CustomTestsFile testFile = GetCustomTestFile();

            TrafficViewerFile testDataStore = new TrafficViewerFile();
            MockTestController mockTestController = new MockTestController();

            var targetList = new Dictionary<string, AttackTarget>();
            targetList.Add("r1", new AttackTarget("r1","Enabled","r1"));
            testFile.SetAttackTargetList(targetList);
            DriveByAttackProxy testProxy = new DriveByAttackProxy(mockTestController, testFile, testDataStore);

            testProxy.Start();


            SendRequestThroughTestProxy(first, testProxy, mockSite);
            SendRequestThroughTestProxy(second, testProxy, mockSite);
            SendRequestThroughTestProxy(third, testProxy, mockSite);

            Thread.Sleep(1000);

            testProxy.Stop();

            Assert.IsTrue(mockTestController.IssuesFound.ContainsKey("p1"));
            Assert.IsFalse(mockTestController.IssuesFound.ContainsKey("p2"));



        }


        [TestMethod]
        public void CustomTestProxy_TestPatternRequestExclusion()
        {
            MockProxy mockSite = new MockProxy();
            string first = "GET / HTTP/1.1\r\n";
            string second = "POST /r1 HTTP/1.1\r\n\r\np1=1234\r\n";
            string third = "POST /r2 HTTP/1.1\r\n\r\np2=1234\r\n\r\n";
            mockSite.MockSite.AddRequestResponse(first, "HTTP/1.1 200 OK\r\n\r\nbla");
            mockSite.MockSite.AddRequestResponse(second, "HTTP/1.1 200 OK\r\n\r\nroot:0:0");
            mockSite.MockSite.AddRequestResponse(third, "HTTP/1.1 200 OK\r\n\r\nroot:0:0");

            mockSite.Start();

            CustomTestsFile testFile = GetCustomTestFile();

            TrafficViewerFile testDataStore = new TrafficViewerFile();
            MockTestController mockTestController = new MockTestController();
            testFile.PatternRequestExclusion = "r2";
            DriveByAttackProxy testProxy = new DriveByAttackProxy(mockTestController, testFile, testDataStore);

            testProxy.Start();


            SendRequestThroughTestProxy(first, testProxy, mockSite);
            SendRequestThroughTestProxy(second, testProxy, mockSite);
            SendRequestThroughTestProxy(third, testProxy, mockSite);

            Thread.Sleep(1000);

            testProxy.Stop();

            Assert.IsTrue(mockTestController.IssuesFound.ContainsKey("p1"));
            Assert.IsFalse(mockTestController.IssuesFound.ContainsKey("p2"));

        }

        [TestMethod]
        public void CustomTestProxy_TestPatternEntityExclusion()
        {
            MockProxy mockSite = new MockProxy();
            string first = "GET / HTTP/1.1\r\n";
            string second = "POST /r1 HTTP/1.1\r\n\r\np1=1234\r\n";
            string third = "POST /r2 HTTP/1.1\r\n\r\np2=1234\r\n\r\n";
            mockSite.MockSite.AddRequestResponse(first, "HTTP/1.1 200 OK\r\n\r\nbla");
            mockSite.MockSite.AddRequestResponse(second, "HTTP/1.1 200 OK\r\n\r\nroot:0:0");
            mockSite.MockSite.AddRequestResponse(third, "HTTP/1.1 200 OK\r\n\r\nroot:0:0");

            mockSite.Start();

            CustomTestsFile testFile = GetCustomTestFile();

            TrafficViewerFile testDataStore = new TrafficViewerFile();
            MockTestController mockTestController = new MockTestController();
            testFile.PatternEntityExclusion = "p2";
            DriveByAttackProxy testProxy = new DriveByAttackProxy(mockTestController, testFile, testDataStore);

            testProxy.Start();


            SendRequestThroughTestProxy(first, testProxy, mockSite);
            SendRequestThroughTestProxy(second, testProxy, mockSite);
            SendRequestThroughTestProxy(third, testProxy, mockSite);

            Thread.Sleep(1000);

            testProxy.Stop();

            Assert.IsTrue(mockTestController.IssuesFound.ContainsKey("p1"));
            Assert.IsFalse(mockTestController.IssuesFound.ContainsKey("p2"));

        }




        [TestMethod]
        public void CustomTestProxy_TestFirstRequestRegex()
        {
            MockProxy mockSite = new MockProxy();
            string first = "GET / HTTP/1.1\r\n";
            string second = "POST /r1 HTTP/1.1\r\n\r\np1=1234\r\n";
            string third = "POST /r2 HTTP/1.1\r\n\r\np2=1234\r\n\r\n";
            mockSite.MockSite.AddRequestResponse(first, "HTTP/1.1 200 OK\r\n\r\nbla");
            mockSite.MockSite.AddRequestResponse(second, "HTTP/1.1 200 OK\r\n\r\nroot:0:0");
            mockSite.MockSite.AddRequestResponse(third, "HTTP/1.1 200 OK\r\n\r\nroot:0:0");

            mockSite.Start();

            CustomTestsFile testFile = GetCustomTestFile();

            TrafficViewerFile testDataStore = new TrafficViewerFile();
            MockTestController mockTestController = new MockTestController();
            SequentialAttackProxy testProxy = new SequentialAttackProxy(mockTestController, testFile, testDataStore);
            testFile.PatternOfFirstRequestToTest = "r2";
            testProxy.Start();


            SendRequestThroughTestProxy(first, testProxy, mockSite);
            SendRequestThroughTestProxy(second, testProxy, mockSite);
            SendRequestThroughTestProxy(third, testProxy, mockSite);

            testProxy.Stop();

            Assert.IsFalse(mockTestController.IssuesFound.ContainsKey("p1"));
            Assert.IsTrue(mockTestController.IssuesFound.ContainsKey("p2"));

        }


        [TestMethod]
        public void CustomTestProxy_500Error()
        {
            MockProxy mockSite = new MockProxy();
            string first = "GET / HTTP/1.1\r\n";
            string second = Resources.IdeasPmc;
            string third = "POST /r2 HTTP/1.1\r\n\r\np2=1234\r\n\r\n";
            mockSite.MockSite.AddRequestResponse(first, "HTTP/1.1 200 OK\r\n\r\nbla");
            mockSite.MockSite.AddRequestResponse(second, "HTTP/1.1 200 OK\r\n\r\nroot:0:0");
            mockSite.MockSite.AddRequestResponse(third, "HTTP/1.1 200 OK\r\n\r\nbla");

            mockSite.Start();

            CustomTestsFile testFile = GetCustomTestFile();

            TrafficViewerFile testDataStore = new TrafficViewerFile();
            MockTestController mockTestController = new MockTestController();
            SequentialAttackProxy testProxy = new SequentialAttackProxy(mockTestController, testFile, testDataStore);
            testFile.PatternOfFirstRequestToTest = ".*pmc";
            testProxy.Start();
            HttpResponseInfo respInfo;
            for (int i = 0; i < 2; i++)
            {
                respInfo = SendRequestThroughTestProxy(first, testProxy, mockSite);
                Assert.AreNotEqual(500, respInfo.Status);
                SendRequestThroughTestProxy(second, testProxy, mockSite);
                Assert.AreNotEqual(500, respInfo.Status);
                SendRequestThroughTestProxy(third, testProxy, mockSite);
                Assert.AreNotEqual(500, respInfo.Status);
            }

            testProxy.Stop();

            Assert.IsFalse(mockTestController.IssuesFound.ContainsKey("p2"));
            Assert.IsTrue(mockTestController.IssuesFound.ContainsKey("itoken"));

        }

        [TestMethod]
        public void CustomTestProxy_BodyParamUnitTest()
        {


            MockTestController mockTestController = new MockTestController();


            string testRequest = "POST /search.aspx HTTP/1.1\r\n\r\ntxtSearch=1234\r\n\r\n";

            HttpResponseInfo testResponse = CustomTestUnitExecution(testRequest, mockTestController);

            Assert.AreEqual(1, mockTestController.IssuesFound.Count, "No issues found");
            Assert.IsTrue(mockTestController.IssuesFound.ContainsKey("txtSearch"), "Incorrect parameter found");
            Assert.AreEqual("Path Traversal", mockTestController.IssuesFound["txtSearch"], "Incorrect issue found");




        }






   

        public HttpResponseInfo CustomTestUnitExecution(string request, MockTestController mockTestController, SequentialAttackProxy testProxy = null, MockProxy mockSite = null)
        {
            bool shouldStopMockSite = false;

            if (mockSite == null)
            {
                shouldStopMockSite = true;
                mockSite = new MockProxy(request, "HTTP/1.1 200 OK\r\n\r\nroot:0:0");

                mockSite.Start();
            }
            CustomTestsFile testFile = GetCustomTestFile();
            bool shouldStopTestProxy = false;
            if (testProxy == null)
            {
                shouldStopTestProxy = true;
                TrafficViewerFile testDataStore = new TrafficViewerFile();
                testProxy = new SequentialAttackProxy(mockTestController, testFile, testDataStore);
                testProxy.Start();
            }

            var response = SendRequestThroughTestProxy(request, testProxy, mockSite);

            if (shouldStopMockSite)
            {
                mockSite.Stop();
            }
            if (shouldStopTestProxy)
            {
                testProxy.Stop();
            }


            return response;


        }

        private static HttpResponseInfo SendRequestThroughTestProxy(string request, BaseAttackProxy testProxy, MockProxy mockSite)
        {
            HttpRequestInfo testReqInfo = new HttpRequestInfo(request, false);
            testReqInfo.Host = mockSite.Host;
            testReqInfo.Port = mockSite.Port;
            TrafficViewerHttpClient client = new TrafficViewerHttpClient();
            client.Timeout = 60 * 60 * 24;
            DefaultNetworkSettings netSettings = new DefaultNetworkSettings();
            netSettings.WebProxy = new WebProxy(testProxy.Host, testProxy.Port);
            client.SetNetworkSettings(netSettings);


            var response = client.SendRequest(testReqInfo);

            Assert.IsNotNull(response, "Error connecting to test proxy");
            Assert.AreNotEqual(504, response.Status, "Mock site not responding");


            return response;
        }

        private static CustomTestsFile GetCustomTestFile()
        {
            CustomTestsFile testFile = new CustomTestsFile();
            var customTests = testFile.GetCustomTests();
            customTests.Clear();
            customTests.Add("Path Traversal", new CustomTestDef("Path Traversal", "Path Traversal", "$original" + MockTestController.PATH_TRAVERSAL, "root\\:"));

            testFile.SetCustomTests(customTests);

            testFile.LoginBeforeTests = false;
            testFile.TestOnlyParameters = true;
            var targetList = testFile.GetAttackTargetList();
            targetList.Add("all", new AttackTarget("all", "Enabled", ".*"));
            testFile.SetAttackTargetList(targetList);

            return testFile;
        }


    }
}
