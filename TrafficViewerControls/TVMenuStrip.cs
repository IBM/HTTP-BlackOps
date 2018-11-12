using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using TrafficViewerSDK.AnalysisModules;
using TrafficViewerSDK.Exploiters;

namespace TrafficViewerControls
{
	public partial class TVMenuStrip : UserControl
	{
		private SortedDictionary<string, IExploiter> _exploiters;

		public void LoadExploiters(IList<IExploiter> exploiters)
		{
			if (exploiters != null)
			{ 
				_exploiters = new SortedDictionary<string,IExploiter>();
                foreach(IExploiter exploiter in exploiters)
                {
                    if (!_exploiters.ContainsKey(exploiter.Caption))
                    {
                        _exploiters.Add(exploiter.Caption, exploiter);
                    }
                }
				_exploitMenu.DropDownItems.Clear();

                foreach (KeyValuePair<string,IExploiter> kvp in _exploiters)
				{
                    ToolStripMenuItem newEntry = new ToolStripMenuItem(kvp.Key);
                    newEntry.Click += ExploiterClick;
                    _exploitMenu.DropDownItems.Add(newEntry);
                }
                
			}
		}

		private void ExploiterClick(object sender, EventArgs e)
		{
			if (this.AnalysisModuleClicked != null)
			{
				string currModCaption = (sender as ToolStripMenuItem).Text;
				IExploiter currMod;
				if (_exploiters.TryGetValue(currModCaption, out currMod))
				{
					this.ExploiterItemClicked.Invoke(new ExploiterClickArgs(currMod));
				}
			}
		}



		private Dictionary<string, IAnalysisModule> _analysisModulesList;
		/// <summary>
		/// Loads the list of available analysis modules
		/// </summary>
		public void LoadAnalysisModules(IEnumerable<IAnalysisModule> modules)
		{
			if (modules != null)
			{
				_analysisModulesList = new Dictionary<string, IAnalysisModule>();
				_analysisModulesMenu.DropDownItems.Clear();

				foreach (IAnalysisModule module in modules)
				{
					_analysisModulesList.Add(module.Caption, module);
					ToolStripMenuItem newEntry = new ToolStripMenuItem(module.Caption);
					newEntry.Click += new EventHandler(AnalysisModuleClick);
					_analysisModulesMenu.DropDownItems.Add(newEntry);
				}
			}


		}

		void AnalysisModuleClick(object sender, EventArgs e)
		{
			if (this.AnalysisModuleClicked != null)
			{
				string currModCaption = (sender as ToolStripMenuItem).Text;
				IAnalysisModule currMod;
				if (_analysisModulesList.TryGetValue(currModCaption, out currMod))
				{
					this.AnalysisModuleClicked.Invoke(new AnalysisModuleClickArgs(currMod));
				}
			}
		}


		public TVMenuStrip()
		{
			InitializeComponent();
		}

		private TVMenuStripStates _state;
		/// <summary>
		/// Gets/sets the state of the menu bar
		/// </summary>
		public TVMenuStripStates State
		{
			get
			{
				return _state;
			}
			set
			{
				_state = value;
				switch (value)
				{
					case TVMenuStripStates.Loaded:
						_visualizeTraffic.Enabled = _export.Enabled = _new.Enabled = _openUnpacked.Enabled = _open.Enabled = true;
						_importLog.Enabled = true;
						_save.Enabled = true;
						_saveAs.Enabled = true;
						_search.Enabled = true;
						_tail.Enabled = true;
						_tail.Visible = true;
						_stopTail.Visible = false;
						
						break;
					case TVMenuStripStates.Loading:
						_visualizeTraffic.Enabled = _export.Enabled = _new.Enabled = _openUnpacked.Enabled = _open.Enabled = false;
						_importLog.Enabled = false;
						_save.Enabled = false;
						_saveAs.Enabled = false;
						
						_search.Enabled = true;
						_tail.Enabled = false;
						_tail.Visible = false;
						_stopTail.Visible = true;
						_stopTail.Enabled = true;
						
						break;
					case TVMenuStripStates.Listening:
						_visualizeTraffic.Enabled = _export.Enabled = _new.Enabled = _openUnpacked.Enabled = _open.Enabled = false;
						_importLog.Enabled = false;
						_save.Enabled = false;
						_saveAs.Enabled = false;
						
						_search.Enabled = true;
						_tail.Enabled = false;
						
						break;
				}
			}
		}

		


		#region Events

		/// <summary>
		/// Occurs when a module was activated in the menu
		/// </summary>
		public event AnalysisModuleClickEvent AnalysisModuleClicked;

		/// Occurs when a exploite in the menu
		/// </summary>
		public event ExploiterClickEvent ExploiterItemClicked;

		/// <summary>
		/// Occurs when the user clicks on the Open menu
		/// </summary>
		public event EventHandler OpenClick
		{
			add
			{
				_open.Click += value;
			}
			remove
			{
				_open.Click -= value;
			}
		}

		/// <summary>
		/// Occurs when the user clicks on the New menu
		/// </summary>
		public event EventHandler NewClick
		{
			add
			{
				_new.Click += value;
			}
			remove
			{
				_new.Click -= value;
			}
		}


		/// <summary>
		/// Occurs when the user clicks on the Open menu
		/// </summary>
		public event EventHandler ExportClick
		{
			add
			{
				_export.Click += value;
			}
			remove
			{
				_export.Click -= value;
			}
		}


		/// <summary>
		/// Occurs when the user clicks on the Open Unpacked menu
		/// </summary>
		public event EventHandler OpenUnpackedClick
		{
			add
			{
				_openUnpacked.Click += value;
			}
			remove
			{
				_openUnpacked.Click -= value;
			}
		}

		/// <summary>
		/// Occurs when the user click on the Connect to Traffic Log menu item
		/// </summary>
		public event EventHandler ImportConnectToLog
		{
			add
			{
				_importLog.Click += value;
			}
			remove
			{
				_importLog.Click -= value;
			}
		}

		/// <summary>
		/// Occurs when the user clicks on the Save menu item
		/// </summary>
		public event EventHandler SaveClick
		{
			add
			{
				_save.Click += value;
			}
			remove
			{
				_save.Click -= value;
			}
		}

		/// <summary>
		/// Occurs whent the user clicks on the Save As menu item
		/// </summary>
		public event EventHandler SaveAsClick
		{
			add
			{
				_saveAs.Click += value;
			}
			remove
			{
				_saveAs.Click -= value;
			}
		}

		/// <summary>
		/// Occurs when the user clicks on the Stop Load menu item
		/// </summary>
		public event EventHandler StopLoad
		{
			add
			{
				_stopLoad.Click += value;
			}
			remove
			{
				_stopLoad.Click -= value;
			}
		}

		/// <summary>
		/// Occurs whent the user clicks on the Clear menu item
		/// </summary>
		public event EventHandler Clear
		{
			add
			{
				_clear.Click += value;
			}
			remove
			{
				_clear.Click -= value;
			}
		}

		/// <summary>
		/// Occurs when the user clicks the Tail button
		/// </summary>
		public event EventHandler Tail
		{
			add
			{
				_tail.Click += value;
			}
			remove
			{
				_tail.Click -= value;
			}
		}

		/// <summary>
		/// Occurs when the user clicks the Stop Tail button
		/// </summary>
		public event EventHandler StopTail
		{
			add
			{
				_stopTail.Click += value;
			}
			remove
			{
				_stopTail.Click -= value;
			}
		}


		/// <summary>
		/// Hook for the Visualize Traffic menu
		/// </summary>
		public event EventHandler VisualizeTraffic
		{
			add
			{
				_visualizeTraffic.Click += value;
			}
			remove
			{
				_visualizeTraffic.Click -= value;
			}
		}

		/// <summary>
		/// Triggers the options
		/// </summary>
		public event EventHandler Options
		{
			add
			{
				_options.Click += value;
			}
			remove
			{
				_options.Click -= value;
			}
		}

		/// <summary>
		/// Triggers the search
		/// </summary>
		public event EventHandler Search
		{
			add
			{
				_search.Click += value;
			}
			remove
			{
				_search.Click -= value;
			}
		}

		
		

		/// <summary>
		/// Occurs when the manual explore embedded browser button is clicked
		/// </summary>
		public event EventHandler EmbeddedBrowserClicked
		{
			add
			{
				_useEmbeddedBrowser.Click += value;
			}
			remove
			{
				_useEmbeddedBrowser.Click -= value;
			}
		}


        

        /// <summary>
        /// Occurs when the manual explore external browser button is clicked
        /// </summary>
        public event EventHandler CurrentProfileClicked
        {
            add
            {
                _editCurrentProfile.Click += value;
            }
            remove
            {
                _editCurrentProfile.Click -= value;
            }
        }

		
		#endregion

		#region Reactions


		private void TailClick(object sender, EventArgs e)
		{
			_tail.Enabled = false;
		}

		private void StopTailClick(object sender, EventArgs e)
		{
			_stopTail.Enabled = false;
		}


		#endregion

        




		
	}

	public enum TVMenuStripStates
	{
		Empty,
		Loading,
		Loaded,
		Listening
	}
}
