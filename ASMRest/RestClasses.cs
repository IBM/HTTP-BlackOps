using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMRest
{

	public enum IssueAttributeType
	{ 
		Textbox =1,
		CommaSeparatedMutlivalue = 3,
		DropDown = 4,
		Formula = 5,
		TextArea = 7
	}

    public class Comment
    { 
        public DateTime date;
        public string comment;
        public string user;
        public string userId;
    }

	public class IssueWithAttributes 
	{
		public int appReleaseId;
		public int issueId;
	    public long dateCreated;
		public long lastUpdated;
		public IssueAttributeCollection attributeCollection;
		public int appIsOutOfDate;

		public string[] GetAttrValueList(string attrName)
		{
			if (attributeCollection != null && attributeCollection.attributeArray != null)
			{
				foreach (IssueAttribute attr in attributeCollection.attributeArray)
				{
					if (attrName.Equals(attr.name, StringComparison.OrdinalIgnoreCase))
					{
						return attr.value;
					}
				}
			}

			return new string[1] { "" };

		}

		public string GetAttrFirstVal(string attrName)
		{

			var valueList = GetAttrValueList(attrName);

            if (valueList.Length > 0)
            {
                return valueList[0];
            }
            else
            {
                return String.Empty;
            }

		}

		public string GetAttrVal(string attrName, string separator=",", string newLineString="<br>", bool perserveTabbing=false)
		{

            string val = string.Join(separator, GetAttrValueList(attrName));
            val =  val.Replace("&#13;","");
            val = val.Replace("\r","");
                        
            val = val.Replace("&#10;",newLineString);
            val = val.Replace("\n",newLineString);

            if (perserveTabbing) //html context
            { 
                //replace tabs with 4 spaces
                val = val.Replace("\t", "&nbsp; &nbsp; &nbsp;");
                val = val.Replace("  ", "&nbsp; ");
            }

            val = val.Replace("{_,_}", separator);

            return val;
		}

		public void SetAttrVal(string attrName, string value)
		{
			if (attributeCollection != null && attributeCollection.attributeArray != null)
			{
				foreach (IssueAttribute attr in attributeCollection.attributeArray)
				{
					if (attrName.Equals(attr.name, StringComparison.OrdinalIgnoreCase))
					{
						attr.value = new string[1] { value };
					}
				}
			}

		}
	}

	
	public class ApplicationWithAttributes 
	{
		public ApplicationAttributeCollection attributeCollection;
		public int id;
		public string name;
		public string description;
		public long dateCreated;
		public long lastUpdated;
		public int isOutOfDate;

		public string GetAttrVal(string attrName, string separator = ",")
		{

			return string.Join(separator, GetAttrValueList(attrName)).Replace("&#10;", "<br>");
		}

		public string[] GetAttrValueList(string attrName)
		{
			foreach (ApplicationAttribute attr in attributeCollection.attributeArray)
			{
				if (attrName.Equals(attr.name))
				{
					return attr.value;
				}
			}

			return new string[1] { "" };

		}
	}

	public class ApplicationAttributeCollection 
	{
		public ApplicationAttribute [] attributeArray;
	}

	public class ApplicationAttribute 
	{
		public string name;
		public string[] value;
	}
	
	public class IssueAttributeCollection 
	{
		public IssueAttribute [] attributeArray;
	}

	
	public class IssueAttribute 
	{
		public string name;
		public string [] value;
		public string lookup;
		public int issueAttributeDefinitionId;
		public int attributeType;
		public int attributeCategory;
		public bool contributesToUnique;
		public bool updateable;
	}



	public class IssueAttributeDefinitionCollection 
	{
		public IssueAttributeDefinitionEx[] attributeDefColl;
		public int [] deletedAttrDefIds;

		public IssueAttributeDefinitionEx GetAttributeByName(string name)
		{
			foreach (IssueAttributeDefinitionEx attr in attributeDefColl)
			{
				if (attr.name.Equals(name, StringComparison.OrdinalIgnoreCase))
				{
					return attr;
				}
			}

			return null;
		}
	}

	public class IssueAttributeDefinitionEx 
	{
		public IssueAttributeOptionCollection attrOptionColl;
		public bool isValid;
		public int id;
		public string lookup;
		public string name;
		public int type;
		public bool isVisible;
		public bool isVisibleModifiable;
		public bool isPredefined;
		public string formula;
		public int numOfDecimalPoints;
		public int issueTagGroupId;
		public bool showText;
		public bool mandatoryForScanners;
		public int overwrittenByASEScans;
		public int overwrittenByAdminOverrideOption;
		public long lastUpdated;
	}

	
	public class IssueAttributeOptionCollection 
	{
		public IssueAttributeOption [] attrOptionList;
		public int [] deletedAttrOptionIds;
	}


	public class IssueAttributeOption 
	{
		public int id;
		public int attributeDefinitionId;
		public string lookup;
		public string name;
		public bool isDefault;
		public string foregroundColor;
		public string backgroundColor;
		public string chartColor;
		public bool isBold;
		public bool isItalic;
		public bool isUnderline;
		public float numericValue;
		public int issueTagId;
		public bool isPredefined;
		public long lastUpdated;
	}

}
