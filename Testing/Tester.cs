/**
Copyright 2019 Trend Micro, Incorporated, All Rights Reserved.
SPDX-License-Identifier: Apache-2.0
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;
using TVDiff.Algorithms;


namespace Testing
{
    /// <summary>
    /// In charge with executing tests for one or more requests
    /// </summary>
    public class Tester
    {
        private ITestController _testController;
        private CustomTestsFile _testFile;
        private Dictionary<string, IEnumerable<string>> _matchFiles;
        private const string JAVASCRIPT_FUNCTION_TAG = "$js_code=";
        private const string CALLBACK_NAME = "Callback";
        private const string TIMEOUT_FUNC = "$timeout";
        private const string TAUTOLOGY_VERIFICATION_FUNC = "$verify_tautology";
        private string ENCODED_FUZZ = Utils.UrlEncode(Constants.FUZZ_STRING);


        public Tester(ITestController testController, CustomTestsFile testFile)
        {
            _testController = testController;
            _testFile = testFile;
            _matchFiles = new Dictionary<string, IEnumerable<string>>();

        }

        /// <summary>
        /// Executes a set of tests on the specified url
        /// </summary>
        /// <param name="rawRequest"></param>
        /// <param name="rawResponse"></param>
        /// <param name="requestUri"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="testsToExecute"></param>
        public void ExecuteTests(string rawRequest, string rawResponse, Uri requestUri, string parameterName, string parameterValue, RequestLocation location, params CustomTestDef[] testsToExecute)
        {
            //remove any special characters coming from AppScan
            parameterName = CleanupParameterName(parameterName);

            if (!String.IsNullOrWhiteSpace(_testFile.PatternEntityExclusion) && Regex.IsMatch(parameterName, _testFile.PatternEntityExclusion))
            {
                _testController.Log("Skipping tests for entity '{0}' due to exclusion", parameterName);
                return;
            }

            if (!String.IsNullOrWhiteSpace(_testFile.PatternRequestExclusion) && Regex.IsMatch(rawRequest, _testFile.PatternRequestExclusion))
            {
                _testController.Log("Skipping tests for request '{0}' due to exclusion", requestUri);
                return;
            }

            //extract the string in the request that defines our entity
            string entityString = GetEntityString(rawRequest, requestUri, parameterName, parameterValue);


            _testController.Log("Running Custom Tests on '{0}' ({1})", requestUri, parameterName);
            _testController.Log("- Entity string: '{0}'", entityString);

            //get the entity id string which is specified in the definition by $entity_id
            string entityId = GetEntityId(requestUri, parameterName);
            _testController.Log("- Entity id: '{0}'", entityId);



            foreach (CustomTestDef testDef in testsToExecute)
            {

                //update session identifies
                bool useSSL = requestUri.Scheme.Equals("https");

                

                List<string> mutatedRequestList = GenerateMutatedRequestList(rawRequest, new TestJob(parameterName, parameterValue, location, testDef), entityString, entityId);
                List<string> testResponseList = new List<string>();
                foreach (string mutatedRequest in mutatedRequestList)
                {
                    //send the test to server
                    var updMutatedRequest = _testController.UpdateSessionIds(mutatedRequest, useSSL);
                    string testResponse = _testController.SendTest(updMutatedRequest, requestUri.Host, requestUri.Port, useSSL);
                    testResponseList.Add(testResponse);
                }


                //validate the response and insert a new issue
                ValidateTest(rawRequest, rawResponse, requestUri, parameterName, entityId, testDef, mutatedRequestList, testResponseList);
            }
        }

        /// <summary>
        /// Mutates a request in order to execute a test
        /// </summary>
        /// <param name="rawRequest"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="reqLocation"></param>
        /// <param name="entityString"></param>
        /// <param name="entityId"></param>
        /// <param name="testDef"></param>
        /// <returns></returns>
        public List<string> GenerateMutatedRequestList(string rawRequest, TestJob testJob, string entityString, string entityId)
        {
            //generate payload
            bool hasFuzz = rawRequest.Contains(Constants.FUZZ_STRING) || rawRequest.Contains(ENCODED_FUZZ);


            List<string> mutatedRequestsList = new List<string>();

            List<string> payloadList = GeneratePayloadListFromMutation(rawRequest, testJob, hasFuzz, entityId);

            for (int i = 0; i < payloadList.Count; i++)
            {
                string payload = payloadList[i];

                _testController.Log("Running test '{0}' with payload: '{1}'", testJob.TestDef.Name, payload);
                string mutatedRequest;

                if (hasFuzz)
                {
                    mutatedRequest = rawRequest.Replace(Constants.FUZZ_STRING, payload);
                    mutatedRequest = mutatedRequest.Replace(ENCODED_FUZZ, payload);

                }
                else if (entityString != null && OccurenceCount(entityString, testJob.ParameterValue) == 1)
                {
                    string mutatedString = entityString.Replace(testJob.ParameterValue, payload);

                    mutatedRequest = rawRequest.Replace(entityString, mutatedString);
                }
                else
                {
                    //try to parse the request
                    HttpRequestInfo mutatedReqInfo = new HttpRequestInfo(rawRequest, true);

                    //attempt to find the parameter

                    switch (testJob.RequestLocation)
                    {
                        case RequestLocation.Body: mutatedReqInfo.BodyVariables.Set(testJob.ParameterName, payload); break;
                        case RequestLocation.Cookies: mutatedReqInfo.Cookies.Set(testJob.ParameterName, payload); break;
                        case RequestLocation.Headers: mutatedReqInfo.Headers[testJob.ParameterName] = payload; break;
                        case RequestLocation.Query: mutatedReqInfo.QueryVariables.Set(testJob.ParameterName, payload); break;
                        case RequestLocation.Path: mutatedReqInfo.PathVariables.Set(testJob.ParameterName, payload); break;
                    }

                    mutatedRequest = mutatedReqInfo.ToString();

                }
                //update dynamic values
                mutatedRequest = mutatedRequest.Replace("__dynamic_value__ticks__", (DateTime.Now.Ticks + i).ToString());

                mutatedRequestsList.Add(mutatedRequest);
            }
            return mutatedRequestsList;
        }


        /// <summary>
        /// Get a payload list for the mutation
        /// </summary>
        /// <param name="rawRequest"></param>
        /// <param name="mutationRule"></param>
        /// <param name="hasFuzz">Whether the request contains <fuzz></param>
        /// <param name="entityId">The entitiy id</param>
        /// <returns></returns>
        public List<string> GeneratePayloadListFromMutation(string rawRequest, TestJob testJob, bool hasFuzz, string entityId)
        {

            List<string> payloadList = new List<string>();

            string mutationRule = testJob.TestDef.Mutation;
            string payloadString = "";
            if (mutationRule.StartsWith(JAVASCRIPT_FUNCTION_TAG))
            {
                //extract JS function from rule
                string jsCode = mutationRule.Substring(JAVASCRIPT_FUNCTION_TAG.Length);
                //function should match the signature Callback(<string>,<string>,

                using (ScriptEngine engine = new ScriptEngine(ScriptEngine.JavaScriptLanguage))
                {

                    ParsedScript parsed = engine.Parse(jsCode);
                    payloadString = parsed.CallMethod(CALLBACK_NAME,
                        rawRequest, testJob.ParameterName, testJob.ParameterValue, testJob.RequestLocation.ToString()) as string;
                }
            }
            else
            {
                payloadString = mutationRule;
            }

            if (hasFuzz)
            {
                payloadString = payloadString.Replace("$original", String.Empty);
            }
            else
            {
                payloadString = payloadString.Replace("$original", testJob.ParameterValue);
            }
            payloadString = payloadString.Replace("$entity_id", entityId);

            if (!String.IsNullOrWhiteSpace(payloadString))
            {
                payloadString = payloadString.Replace(@"\,", "__comma__");
                var list = payloadString.Split(',');
                foreach (string payload in list)
                {
                    string finalPayload = payload.Replace("__comma__", ",");
                    payloadList.Add(finalPayload);
                    if (_testFile.GenerateAllEncodings) //also add encoding variants for the payload
                    {
                        payloadList.Add(Utils.UrlEncode(finalPayload));
                        payloadList.Add(Utils.UrlEncode(Utils.UrlEncode(finalPayload)));
                        payloadList.Add(Utils.UrlEncodeAll(finalPayload));
                        payloadList.Add(Utils.JSONEncode(finalPayload));
                        payloadList.Add(Utils.HtmlEncode(finalPayload));
                        payloadList.Add(Utils.Base64Encode(finalPayload));
                    }
                }
            }

            return payloadList;
        }



        /// <summary>
        /// Counts the occurence of a string
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="needle"></param>
        /// <returns></returns>
        private int OccurenceCount(string stack, string needle)
        {
            int count = 0;
            int i = 0;
            while ((i = stack.IndexOf(needle, i)) != -1)
            {
                i += needle.Length;
                count++;
            }
            return count;
        }

        /// <summary>
        /// Gets an entity string to replace for the specified request
        /// </summary>
        /// <param name="rawRequest"></param>
        /// <param name="requestUri"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns>Entity string</returns>
        public string GetEntityString(string rawRequest, Uri requestUri, string parameterName, string parameterValue)
        {
            string entityString = null;

            if (String.IsNullOrWhiteSpace(parameterValue))
            {
                _testController.Log("WARNING Custom Tests - no value for entity '{0}' ({1})", requestUri, parameterName);
                return null;
            }

            if (String.IsNullOrWhiteSpace(parameterName))
            {
                _testController.Log("SKIPPING Custom Tests - no parameter name for value '{0}' ({1})", requestUri, parameterValue);
                return null;
            }


            string PARAM_REGEX = @"{0}\b.*?{1}";
            //extract entity substring
            string entityRegex = String.Format(PARAM_REGEX,
                Regex.Escape(parameterName), Regex.Escape(parameterValue));

            Match m = Regex.Match(rawRequest, entityRegex);

            if (m.Captures.Count == 0)
            {
                parameterValue = Utils.UrlEncode(parameterValue);
                entityRegex = String.Format(PARAM_REGEX,
                    Regex.Escape(Utils.UrlEncode(parameterName)), Regex.Escape(parameterValue));

                m = Regex.Match(rawRequest, entityRegex);

                //if still no capture return;
                if (m.Captures.Count == 0)
                {
                    _testController.Log("SKIPPING Custom Tests - could not extract entity string for entity '{0}' ({1})",
                        requestUri, parameterName);
                    return null;
                }
            }

            entityString = m.Captures[0].Value;

            return entityString;
        }

        public string CleanupParameterName(string parameterName)
        {
            parameterName = parameterName.Replace("->", "");
            parameterName = parameterName.Trim(new char[4] { '_', '"', '[', ']' });
            return parameterName;
        }

        /// <summary>
        /// Gets the entity id for a request and parameter (to insert in a test)
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public string GetEntityId(Uri requestUri, string parameterName)
        {
            string entityId = Regex.Replace(requestUri.PathAndQuery, "[^\\w]", "_") + Regex.Replace(parameterName, "[^\\w]", "_");
            return entityId;
        }

        /// <summary>
        /// Validates a response and inserts a vulnerability
        /// </summary>
        /// <param name="rawRequest"></param>
        /// <param name="rawResponse"></param>
        /// <param name="requestUri"></param>
        /// <param name="parameterName"></param>
        /// <param name="entityId"></param>
        /// <param name="testDef"></param>
        /// <param name="payload"></param>
        /// <param name="mutatedRequestList"></param>
        /// <param name="testResponseList"></param>
        public bool ValidateTest(string rawRequest, string rawResponse, Uri requestUri,
            string parameterName, string entityId, CustomTestDef testDef, List<string> mutatedRequestList, List<string> testResponseList)
        {
            string validation = testDef.Validation.Trim();
            if (String.IsNullOrWhiteSpace(validation)) return false; //this is a callback test

            bool found = false;

            bool isMultiValidationRule = IsMultiValidationRule(validation);

            if (!isMultiValidationRule)
            {
                for (int idx = 0; idx < mutatedRequestList.Count; idx++)
                {
                    string mutatedRequest = mutatedRequestList[idx];
                    string testResponse = testResponseList[idx];
                    found |= ValidateSingleTest(rawRequest, rawResponse, requestUri, parameterName, entityId, testDef, mutatedRequest, testResponse);
                }


            }
            else
            {
                if (validation.StartsWith(JAVASCRIPT_FUNCTION_TAG))
                {
                    found = RunJavaScriptValidation(validation, testResponseList);
                }
                else if (validation.StartsWith(TAUTOLOGY_VERIFICATION_FUNC))
                {
                    if (testResponseList != null && testResponseList.Count % 3 == 0)
                    {
                        int encodingsCount = testResponseList.Count / 3;
                        for (int i = 0; i < encodingsCount && !found; i++)
                        {
                            int first = i;
                            int second = i + encodingsCount;
                            int third = i + 2 * encodingsCount;
                            double firstAndThirdSimilarity = ASESimilarityAlgorithm.CalculateSimilarity(testResponseList[first], testResponseList[third]);
                            double firstAndSecondSimilarity = ASESimilarityAlgorithm.CalculateSimilarity(testResponseList[first], testResponseList[second]);
                            found = firstAndThirdSimilarity - firstAndSecondSimilarity > 0.2; //first and third are more similar than first and second
                            if (found)
                            {
                                var newMutatedRequestList = new List<string>();
                                var newTestResponseList = new List<string>();

                                newMutatedRequestList.Add(mutatedRequestList[first]);
                                newMutatedRequestList.Add(mutatedRequestList[second]);
                                newMutatedRequestList.Add(mutatedRequestList[third]);
                                newTestResponseList.Add(testResponseList[first]);
                                newTestResponseList.Add(testResponseList[second]);
                                newTestResponseList.Add(testResponseList[third]);

                                mutatedRequestList = newMutatedRequestList;
                                testResponseList = newTestResponseList;
                            }
                        }
                    }
                    else
                    {
                        _testController.Log("INCORRECT NUMBER OF PAYLOADS FOR TAUTOLOGY VERIFICATION (MUST BE DIVISIBLE WITH 3)");
                    }
                }


                if (found)
                {
                    for (int i = 0; i < mutatedRequestList.Count; i++)

                        _testController.Log("MULTI-VALIDATION rule match on entity '{0}', test '{1}'", parameterName, testDef.Name);
                    _testController.HandleIssueFound(rawRequest, rawResponse, requestUri, testDef.Type, parameterName, mutatedRequestList, testResponseList, validation, "");
                }

            }

            return found;
        }

        public bool IsMultiValidationRule(string validation)
        {
            bool isMultiValidationRule = validation.StartsWith(JAVASCRIPT_FUNCTION_TAG) || validation.StartsWith(TAUTOLOGY_VERIFICATION_FUNC);
            return isMultiValidationRule;
        }

        /// <summary>
        /// Run Javascript validation
        /// </summary>
        /// <param name="validation"></param>
        /// <param name="testResponseList"></param>
        /// <returns></returns>
        private bool RunJavaScriptValidation(string validation, List<string> testResponseList)
        {
            //extract JS function from rule
            string jsCode = validation.Substring(JAVASCRIPT_FUNCTION_TAG.Length);
            //function should match the signature boolean Callback(response1,...,responseN)
            bool found = false;
            using (ScriptEngine engine = new ScriptEngine(ScriptEngine.JavaScriptLanguage))
            {
                ParsedScript parsed = engine.Parse(jsCode);
                found = (bool)parsed.CallMethod(CALLBACK_NAME, testResponseList.ToArray());
            }
            return found;
        }

        /// <summary>
        /// Validates a single test payload
        /// </summary>
        /// <param name="rawRequest"></param>
        /// <param name="rawResponse"></param>
        /// <param name="requestUri"></param>
        /// <param name="parameterName"></param>
        /// <param name="entityId"></param>
        /// <param name="testDef"></param>
        /// <param name="mutatedRequest"></param>
        /// <param name="testResponse"></param>
        /// <returns></returns>
        public bool ValidateSingleTest(string rawRequest, string rawResponse, Uri requestUri, string parameterName, string entityId, CustomTestDef testDef, string mutatedRequest, string testResponse)
        {
            bool found = false;

            if (!String.IsNullOrWhiteSpace(testResponse) && !String.IsNullOrWhiteSpace(testDef.Exclusion) && Utils.IsMatch(testResponse,testDef.Exclusion))
            {
                return false; //this type of response was excluded from testing
            }


            if (String.IsNullOrWhiteSpace(testResponse) && testDef.Validation.Contains(TIMEOUT_FUNC))
            {
                found = true;
            }


            if (!found && !String.IsNullOrWhiteSpace(testResponse) && !String.IsNullOrWhiteSpace(testDef.Validation))
            {

                string matchFilePath = null;
                IEnumerable<string> matchList = null;

                if (!String.IsNullOrWhiteSpace(testDef.Validation) && testDef.Validation.StartsWith("$match_file"))
                {
                    string[] parts = testDef.Validation.Split(new char[1] { '=' }, 2);
                    if (parts.Length == 2)
                    {
                        matchFilePath = parts[1];
                        if (_matchFiles.ContainsKey(matchFilePath))
                        {
                            matchList = _matchFiles[matchFilePath];
                        }
                        else
                        {
                            //cache the matches
                            matchList = File.ReadLines(matchFilePath);
                            _matchFiles[matchFilePath] = matchList;

                        }
                    }
                }

                string validation = String.Empty;
                if (matchList != null)
                {
                    foreach (string match in matchList)
                    {

                        if (!String.IsNullOrWhiteSpace(match) && testResponse.Contains(match) && !rawResponse.Contains(match))
                        {
                            found = true;
                            validation = Regex.Escape(match);
                            break;
                        }
                    }
                }
                else
                {
                    validation = testDef.Validation.Replace("$entity_id", entityId);
                    string testResponsePartToValidate = testResponse;
                    string origResponsePartToValidate = rawResponse;

                    if (validation.StartsWith("$body=") || validation.StartsWith("$header="))
                    {
                        //parse responses
                        HttpResponseInfo testRespInfo = new HttpResponseInfo(testResponse);
                        HttpResponseInfo origRespInfo = new HttpResponseInfo(rawResponse);
                        //process the body directive
                        if (validation.StartsWith("$body="))
                        {
                            testResponsePartToValidate = testRespInfo.ResponseBody.ToString();
                            origResponsePartToValidate = origRespInfo.ResponseBody.ToString();
                            validation = validation.Substring("$body=".Length);
                        }
                        else if (validation.StartsWith("$header="))
                        {
                            testResponsePartToValidate = testRespInfo.ResponseHeadString;
                            origResponsePartToValidate = origRespInfo.ResponseBody.ToString();
                            validation = validation.Substring("$header=".Length);
                        }
                    }
                    found = Utils.IsMatch(testResponsePartToValidate, validation);
                    
                    if (found)
                    {
                        //make sure raw response does not contain the validation
                        found &= !Utils.IsMatch(origResponsePartToValidate, validation);
                    }

                }
            }

            if (found)
            {
                _testController.Log("ISSUE FOUND for test '{0}'", testDef.Name);
                List<string> mutatedRequestList = new List<string>();
                mutatedRequestList.Add(mutatedRequest);
                List<string> testResponseList = new List<string>();
                testResponseList.Add(testResponse);
                _testController.HandleIssueFound(rawRequest, rawResponse, requestUri, testDef.Type, parameterName, mutatedRequestList, testResponseList, testDef.Validation, "");
            }

            return found;
        }









    }
}
