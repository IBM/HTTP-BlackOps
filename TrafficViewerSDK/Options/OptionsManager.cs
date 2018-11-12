using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace TrafficViewerSDK.Options
{
	/// <summary>
	/// Base class for options
	/// </summary>
    public abstract class OptionsManager
    {
        /// <summary>
        /// Stores the options
        /// </summary>
        protected XmlDocument _optionsDoc;

        /// <summary>
        /// If the file was loaded from the harddrive, stores the location
        /// </summary>
        protected string _optionsDocPath;

		private Dictionary<string, object> _options;

        #region Protected Methods


        /// <summary>
        /// Retrieves an option or option set from the settings file
        /// </summary>
        /// <param name="optionName">The name of the option </param>
        /// <returns>Single string or List&lt;string&gt; if the option contains multiple nodes</returns>
        public virtual object GetOption(string optionName)
        {
			//try getting the option from the cache
			if (_options.ContainsKey(optionName))
			{
				return _options[optionName];
			}
			else
			{
				try
				{
					XmlNode optionNode;

					optionNode = _optionsDoc.SelectSingleNode("//" + optionName);
					if (optionNode != null && optionNode.HasChildNodes)
					{
						if (optionNode.FirstChild.NodeType == XmlNodeType.Text)
						{
							return optionNode.InnerText;
						}

						List<string> nodeValues = new List<string>();
						foreach (XmlNode x in optionNode.ChildNodes)
						{
							nodeValues.Add(x.InnerText);
						}

						//cache the value
						_options.Add(optionName, nodeValues);

						return nodeValues;

					}
				}
				catch { }
			}
            return null;
        }

        /// <summary>
        /// Writes an option to the TrafficViewerOptions.xml
        /// </summary>
        /// <param name="optionName"></param>
        /// <param name="value"></param>
        public virtual void SetSingleValueOption(string optionName, object value)
        {

            XmlNode node;
            XmlNode childNode;

			if (value == null) //protect against clients setting the value null
			{
				value = String.Empty;
			}

			//update the cache
			if (_options.ContainsKey(optionName))
			{
				_options[optionName] = value.ToString();
			}
			else
			{
				_options.Add(optionName,value.ToString());
			}

            node = _optionsDoc.SelectSingleNode("//" + optionName);
            if (node == null)
            {
                node = _optionsDoc.SelectSingleNode("//Options");
                childNode = _optionsDoc.CreateElement(optionName);
                childNode.InnerText = Convert.ToString(value);
                node.AppendChild(childNode);
            }
            else
            {
                node.InnerText = Convert.ToString(value);
            }
        }

        /// <summary>
        /// Sets options for settings with multiple values, e.g. Exclusions
        /// </summary>
        /// <param name="optionName"></param>
        /// <param name="values"></param>
        public virtual void SetMultiValueOption(string optionName, IEnumerable<string> values)
        {

            XmlNode node;
            XmlNode childNode;

			//update the cache
			if (_options.ContainsKey(optionName))
			{
				_options[optionName] = values;
			}
			else
			{
				_options.Add(optionName, values);
			}

            node = _optionsDoc.SelectSingleNode("//" + optionName);

            if (node == null)
            {
                node = _optionsDoc.SelectSingleNode("//Options");
                childNode = _optionsDoc.CreateElement(optionName);
                node.AppendChild(childNode);
                node = childNode;
            }
            else
            {
                node.RemoveAll();
            }
            //child nodes will have the parent name less a char, for example for Exclusions -> Exclusion
            string childName = optionName.Substring(0, optionName.Length - 1);
            foreach (string s in values)
            {
                childNode = _optionsDoc.CreateElement(childName);
                childNode.InnerText = s;
                node.AppendChild(childNode);
            }

        }

        #endregion
    
        #region Public Methods
		/// <summary>
		/// Constructor
		/// </summary>
        public OptionsManager()
        {
            // Create a Options root element in a blank XML document
            InitDoc();
			//init options
			_options = new Dictionary<string, object>();
        }

        /// <summary>
		/// Loads the source XML file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual bool Load(string path)
        {
            _options.Clear();
            if (!System.IO.File.Exists(path))
            {
				InitDoc();
                return false;
            }
            try
            {
                _optionsDoc.Load(path);
                _optionsDocPath = path;
            }
            catch
            {
                InitDoc();
                return false;
            }
            return true;
        }


        /// <summary>
        /// Reloads the settings from the file (if the file has changed)
        /// </summary>
        public void Reload()
        {
            Load(_optionsDocPath);
        }

        
		/// <summary>
		/// Save the current options
		/// </summary>
        public virtual void Save()
        {
            if (_optionsDocPath != null)
            {
                _optionsDoc.Save(_optionsDocPath);
            }
            else
            {
                SdkSettings.Instance.Logger.Log(TraceLevel.Error, "Cannot save a options file that hasn't been loaded. Use SaveAs");
            }
        }

		/// <summary>
		/// Save the options to a differnt file than the current doc
		/// </summary>
		/// <param name="path"></param>
        public void SaveAs(string path)
        {
            _optionsDocPath = path;
            _optionsDoc.Save(path);
            
        }

        /// <summary>
        /// Creates a Options root element in a blank XML document
        /// </summary>
        protected void InitDoc()
        {
            _optionsDoc = new XmlDocument();
            _optionsDoc.XmlResolver = null;
            XmlNode childNode = _optionsDoc.CreateElement("Options");
            _optionsDoc.AppendChild(childNode);
        }

        #endregion

    }
}
