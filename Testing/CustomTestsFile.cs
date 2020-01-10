using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficViewerSDK;
using TrafficViewerSDK.Options;

namespace Testing
{
    public class CustomTestsFile : OptionsManager
    {
        /// <summary>
        /// If tests should run automatically
        /// </summary>
        public bool AutoRunTests
        {
            get
            {
                object value = GetOption("AutoRunTests");
                if (value == null) return false;
                return Convert.ToBoolean(value);
            }
            set
            {
                SetSingleValueOption("AutoRunTests", value);
            }
        }

        /// <summary>
        /// If tests should be generated for all encodings
        /// </summary>
        public bool GenerateAllEncodings
        {
            get
            {
                object value = GetOption("GenerateAllEncodings");
                if (value == null) return false;
                return Convert.ToBoolean(value);
            }
            set
            {
                SetSingleValueOption("GenerateAllEncodings", value);
            }
        }

        

        /// <summary>
        /// Gets a list of attack targets
        /// </summary>
        /// <returns>A list of name/regex custom fields definitions</returns>
        public Dictionary<string, AttackTarget> GetAttackTargetList()
        {
            List<string> values = (List<string>)GetOption("AttackTargetList");
            Dictionary<string, AttackTarget> result = new Dictionary<string, AttackTarget>();
            string[] pair;
            if (values != null)
            {
                foreach (string v in values)
                {
                    pair = v.Split(Constants.VALUES_SEPARATOR.ToCharArray());
                    if (pair.Length == 3)
                    {
                        if (result.ContainsKey(pair[0]))
                        {
                            pair[0] += DateTime.Now.Ticks.ToString();
                        }
                        result.Add(pair[0], new AttackTarget(pair[0], pair[1], pair[2]));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets a list of attack targets in raw form
        /// </summary>
        public List<string> GetAttackTargetListRaw()
        {
            object value = GetOption("AttackTargetList");
            var list = value as List<string>;
            if (list == null)
            {
                list = new List<string>();
            }

            return list;
        }

        /// <summary>
        /// Stores attack targets
        /// </summary>
        /// <param name="attackTargets">Collection of name/regex pairs</param>
        public void SetAttackTargetList(Dictionary<string, AttackTarget> attackTargets)
        {
            List<string> values = new List<string>();
            foreach (AttackTarget def in attackTargets.Values)
            {
                values.Add(def.ToString());
            }

            SetMultiValueOption("AttackTargetList", values);
        }


        /// <summary>
        /// Stores attack targets
        /// </summary>
        /// <param name="value"></param>
        public void SetAttackTargetList(IEnumerable<string> value)
        {

            SetMultiValueOption("AttackTargetList", value);

        }


        /// <summary>
        /// Pattern from where to start testing for sequential proxy
        /// </summary>
        public string PatternOfFirstRequestToTest
        {
            get
            {
                object value = GetOption("PatternOfFirstRequestToTest");
                if (value == null) return String.Empty;
                return Convert.ToString(value);
            }
            set
            {
                SetSingleValueOption("PatternOfFirstRequestToTest", value);
            }
        }


        /// <summary>
        /// Number of threads to execute tests with
        /// </summary>
        public int NumberOfThreads
        {
            get
            {
                object value = GetOption("NumberOfThreads");
                if (value == null) return 10;
                return Convert.ToInt32(value);
            }
            set
            {
                SetSingleValueOption("NumberOfThreads", value);
            }
        }

        


        /// <summary>
        /// If should send tests only on parameters
        /// </summary>
        public bool TestOnlyParameters
        {
            get
            {
                object value = GetOption("TestOnlyParameters");
                if (value == null) return true;
                return Convert.ToBoolean(value);
            }
            set
            {
                SetSingleValueOption("TestOnlyParameters", value);
            }
        }


        /// <summary>
        /// Whether the testing is verbose (less performance)
        /// </summary>
        public bool Verbose
        {
            get
            {
                object value = GetOption("Verbose");
                if (value == null) return false;
                return Convert.ToBoolean(value);
            }
            set
            {
                SetSingleValueOption("Verbose",value);
            }
        }



        /// <summary>
        /// If should login before tests
        /// </summary>
        public bool LoginBeforeTests
        {
            get
            {
                object value = GetOption("LoginBeforeTests");
                if (value == null) return true;
                return Convert.ToBoolean(value);
            }
            set
            {
                SetSingleValueOption("LoginBeforeTests", value);
            }
        }

        /// <summary>
        /// Defines a list of custom fields that can be extracted from anywhere in the traffic
        /// </summary>
        /// <returns>A list of name/regex custom fields definitions</returns>
        public Dictionary<string, CustomTestDef> GetCustomTests()
        {
            List<string> values = (List<string>)GetOption("CustomTests");
            Dictionary<string, CustomTestDef> result = new Dictionary<string, CustomTestDef>();
            string[] cols;
            if (values != null)
            {
                foreach (string v in values)
                {
                    cols = v.Split(Constants.VALUES_SEPARATOR.ToCharArray());
                    if (cols.Length >= 4)
                    {
                        if (result.ContainsKey(cols[0]))
                        {
                            cols[0] += DateTime.Now.Ticks.ToString();
                        }
                        if(cols.Length == 5)
                            result.Add(cols[0], new CustomTestDef(cols[0], cols[1], cols[2], cols[3], cols[4]));
                        else //legacy definition file
                            result.Add(cols[0], new CustomTestDef(cols[0], cols[1], cols[2], cols[3], ""));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Stores custom defined field definitions
        /// </summary>
        /// <param name="CustomTests">Collection of name/regex pairs</param>
        public void SetCustomTests(Dictionary<string, CustomTestDef> CustomTests)
        {
            List<string> values = new List<string>();
            foreach (CustomTestDef def in CustomTests.Values)
            {
                values.Add(def.ToString());
            }

            SetMultiValueOption("CustomTests", values);
        }



        /// <summary>
        /// Gets a list of action based multistep files
        /// </summary>
        public List<string> GetMultiStepList()
        {
            object value = GetOption("MultiStepList");
            var multiStepList = value as List<string>;
            if (multiStepList == null)
            {
                multiStepList = new List<string>();
            }

            return multiStepList;
        }

        /// <summary>
        /// Sets the multi step files
        /// </summary>
        /// <param name="value"></param>
        public void SetMultiStepList(IEnumerable<string> value)
        {

            SetMultiValueOption("MultiStepList", value);

        }

        /// <summary>
        /// Skip parameters, cookies, headers matching this pattern
        /// </summary>
        public string PatternEntityExclusion
        {
            get
            {
                object value = GetOption("PatternEntityExclusion");
                if (value == null) return String.Empty;
                return Convert.ToString(value);
            }
            set
            {
                SetSingleValueOption("PatternEntityExclusion", value);
            }
        }


        /// <summary>
        /// Skip requests matching this pattern
        /// </summary>
        public string PatternRequestExclusion
        {
            get
            {
                object value = GetOption("PatternRequestExclusion");
                if (value == null) return String.Empty;
                return Convert.ToString(value);
            }
            set
            {
                SetSingleValueOption("PatternRequestExclusion", value);
            }
        }

        

    }
}
