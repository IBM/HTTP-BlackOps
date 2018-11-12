using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficViewerSDK;

namespace ASMRest
{
	/// <summary>
	/// Gets the current applications
	/// </summary>
	public class ApplicationListCall : BaseRestCall<Dictionary<string,string>>
	{
        private bool _loadAll = false;
        /// <summary>
        /// Whether to load all apps
        /// </summary>
        public bool LoadAll
        {
            get { return _loadAll; }
            set { _loadAll = value; }
        }

        public ApplicationListCall(ASMRestSettings settings) : base(settings) { }

		public override string Path
		{
			get 
            { 
                string path = "applications";
                if (!_loadAll)
                {
                    path += "?query=Testing%20Status%3dIn%20Progress";
                }
                return path;
            }
		}
	}


	/// <summary>
	/// Gets the issue attribute definitions
	/// </summary>
	public class IssueAttributeDefinitionListCall : BaseRestCall<IssueAttributeDefinitionCollection>
	{
		public override string Path
		{
			get { return "issueattributedefinitions"; }
		}

        public IssueAttributeDefinitionListCall(ASMRestSettings settings) : base(settings) { }
	}



	/// <summary>
	/// Gets app
	/// </summary>
	public class AppCall : BaseRestCall<ApplicationWithAttributes>
	{

		public override string Path
		{
			get { return "applications"; }
		}

        public AppCall(ASMRestSettings settings) : base(settings) { }

	}

	/// <summary>
	/// Gets the issue 
	/// </summary>
	public class IssueCall : BaseRestCall<IssueWithAttributes>
	{
		private ApplicationWithAttributes _app;

		public override string Path
		{
			get { return "issues"; }
		}



		public IssueCall(ApplicationWithAttributes app, ASMRestSettings settings):base(settings)
		{
			_app = app;
		}

		public override IssueWithAttributes Get(string id)
		{
			return base.Get(String.Format("{0}/application/{1}",id,_app.id));
		}



	}

	public enum Severity
	{ 
		High = 1,
		Medium = High << 1,
		Low = Medium << 1,
		Information = Low << 1,
		Undetermined = Information << 1,
		All = High|Medium|Low|Information
	}

	public enum GroupType
	{ 
		IssueType,
		Severity
	}

	public enum IssueItemType
	{
		HighestThreat = 1,
		Countermeasure = HighestThreat << 1,
		Group = Countermeasure << 1,
		Child = Group << 1,
		Unspecified = Child << 1,
		Vulnerability = Unspecified << 1,
		All = HighestThreat | Countermeasure | Group | Child | Unspecified | Vulnerability
	}

	/// <summary>
	/// Gets the issue types
	/// </summary>
	public class OpenIssueTypeListCall : BaseRestCall<Dictionary<string, string>>
	{
		private string _appName;
		
		private string _issueQuery = "Application Name={0},Status=Open,Status=Reopened,Status=InProgress,";
		private string _groupType = "issueType";
		private string _itemType = "Item Type=Unspecified,";
		private string _severityQuery;

		public override string Path
		{
			get { return "issuegroups"; }
		}


		public OpenIssueTypeListCall(string appName, ASMRestSettings settings, Severity severity = Severity.All, GroupType groupType = GroupType.IssueType, IssueItemType itemType=IssueItemType.Unspecified)
		:base(settings)
        {
			_appName = Utils.HtmlDecode(appName);
			if((severity & Severity.High) == Severity.High)
            { 
                _severityQuery = "Severity=Critical,Severity=High";
            }
            if((severity & Severity.Medium) == Severity.Medium)
            { 
                _severityQuery += ",Severity=Medium";
            }
            if((severity & Severity.Low) == Severity.Low)
            { 
                _severityQuery += ",Severity=Low";
            }
			
            if((severity & Severity.Information) == Severity.Information)
            { 
                _severityQuery += ",Severity=Information";
            }

            if((severity & Severity.Undetermined) == Severity.Undetermined)
            { 
                _severityQuery += ",Severity=Undetermined";
            }
            _severityQuery = _severityQuery.Trim(',');

			_groupType = groupType == GroupType.IssueType ? "issueType" : "severity";

			_itemType = "";
            if ((itemType & IssueItemType.Child) == IssueItemType.Child)
			{
				_itemType += "Item Type=Child,";
			}
            if ((itemType & IssueItemType.Countermeasure) == IssueItemType.Countermeasure)
			{
				_itemType += "Item Type=Countermeasure,Countermeasure Level=Absent,Countermeasure Level=Sporadic,Countermeasure Level=Defective,";
			}
            if ((itemType & IssueItemType.HighestThreat) == IssueItemType.HighestThreat)
			{
				_itemType += "Item Type=Highest Threat,";
			}
			if ((itemType & IssueItemType.Group) == IssueItemType.Group)
			{
				_itemType += "Item Type=Group,";
			}
			if ((itemType & IssueItemType.Unspecified) == IssueItemType.Unspecified)
			{
				_itemType += "Item Type=Unspecified,";
			}
			if ((itemType & IssueItemType.Vulnerability) == IssueItemType.Vulnerability)
			{
				_itemType += "Item Type=Vulnerability,";
			}

			if (this.ASMRestSettings.ServerVersion.CompareTo(new Version("9.0.2.0")) >= 0)
			{
				_issueQuery += "Status=New,";
			}


		}

		public override Dictionary<string, string>[] Fetch()
		{
			Dictionary<string, string> queryParamList = new Dictionary<string, string>();
			string queryVal = String.Format(_issueQuery, _appName) + _itemType + _severityQuery;
			queryVal = queryVal.Trim(',');
			queryParamList.Add("query", Utils.UrlEncode(queryVal));
			queryParamList.Add("groupBy", _groupType);
			queryParamList.Add("sortBy", "-CVSS");
			return base.Fetch(queryParamList);
		}
	}

	public enum IssueFilterType
	{ 
		Type,
		Group,
		None
	}
	

	/// <summary>
	/// Gets all the issues for a given issue type
	/// </summary>
	public class IssueListCall : BaseRestCall<Dictionary<string, string>>
	{
		
		private string _issueTypeOrGroup = String.Empty;
		private string _issueQuery = "Application Name={0},Status=Open,Status=In Progress,Status=Reopened";
		private Dictionary<string, string> _issueAttributesMap;
		/// <summary>
		/// Optional specify a map of attributes
		/// </summary>
		public Dictionary<string, string> IssueAttributesMap
		{
			get { return _issueAttributesMap; }
			set { _issueAttributesMap = value; }
		}
		private ApplicationWithAttributes _app;


		
       


		public override string Path
		{
			get { return "issues"; }
		}

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="app"></param>
        /// <param name="settings"></param>
        /// <param name="filterType"></param>
        /// <param name="itemType"></param>
        /// <param name="issueTypeOrGroup"></param>
		public IssueListCall(ApplicationWithAttributes app, ASMRestSettings settings, IssueFilterType filterType = IssueFilterType.None, 
                string issueTypeOrGroup ="",
                string itemType="Item Type=Group,Item Type=Vulnerability,Item Type=Highest Threat,Item Type=Unspecified")
		:base(settings)
        {
			_app = app;
			_issueTypeOrGroup = Utils.HtmlDecode(issueTypeOrGroup);

			if (this.ASMRestSettings.ServerVersion.CompareTo(new Version("9.0.2.0")) >= 0)
			{
				_issueQuery += ",Status=New";
			}

            if (!String.IsNullOrWhiteSpace(_issueTypeOrGroup))
            {
                if (filterType == IssueFilterType.Group)
                {
                    _issueQuery += ",Item Type=Child,Issue Group={1}";
                }
                else
                {
                    _issueQuery += "," + itemType + ",Issue Type={1}";
                }
            }
			
		}


		public override Dictionary<string, string> Get(string id)
		{
			throw new NotImplementedException("Use IssueCall");
		}

		public override Dictionary<string, string>[] Fetch()
		{
			return this.Fetch("+id");
		}

		public Dictionary<string, string>[] Fetch(string sortBy)
		{
			Dictionary<string, string> queryParamList = new Dictionary<string, string>();
			
			string query = String.Format(_issueQuery, Utils.HtmlDecode(_app.name), _issueTypeOrGroup);
			
			queryParamList.Add("query", Utils.UrlEncode(query));
			queryParamList.Add("sortBy", sortBy);
			Dictionary<string, string>[] issueList = base.Fetch(queryParamList);

			if (_issueAttributesMap == null)
			{
				return issueList;
			}

			List<Dictionary<string, string>> translatedIssueList = new List<Dictionary<string, string>>();

			foreach (Dictionary<string, string> issue in issueList)
			{
				Dictionary<string, string> translatedIssue = new Dictionary<string, string>();
				foreach (string key in issue.Keys)
				{
					if (_issueAttributesMap.Keys.Contains(key))
					{
						string translatedKey = _issueAttributesMap[key];
						translatedIssue.Add(translatedKey, issue[key]);
					}
					else
					{
						translatedIssue.Add(key, issue[key]);
					}
				}
				translatedIssueList.Add(translatedIssue);
			}

			return translatedIssueList.ToArray();
		}
	}

}
