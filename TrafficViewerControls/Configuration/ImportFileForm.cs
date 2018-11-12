using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TrafficViewerSDK;
using System.IO;
using TrafficViewerSDK.Options;
using TrafficViewerSDK.Importers;
using TrafficViewerInstance;

namespace TrafficViewerControls
{
	public partial class ImportFileForm : Form
	{
		/// <summary>
		/// Used to separate the files in the import text box
		/// </summary>
		private const string FILE_SEPARATOR = "|";

		/// <summary>
		/// Maps profiles file names to their full path
		/// </summary>
		private Dictionary<string, string> _availableProfiles = new Dictionary<string, string>();

		/// <summary>
		/// Current parsing options
		/// </summary>
		private ParsingOptions _currentProfile = null;

		/// <summary>
		/// Returns the import options
		/// </summary>
		private ImportDialogResult _importResult = new ImportDialogResult();

		/// <summary>
		/// Singleton
		/// </summary>
		private static ImportFileForm __instance = null;

		/// <summary>
		/// Lock for the instance creation
		/// </summary>
		private static object _instanceLock = new object();


		private List<ITrafficParser> _parsers = new List<ITrafficParser>();

		private ImportFileForm()
		{
			InitializeComponent();
			_dialogSelectParser.InitialDirectory = TrafficViewerOptions.Instance.InstallDir;
			string profilesDir = TrafficViewerOptions.TrafficViewerAppDataDir + "\\Profiles";
			if (Directory.Exists(profilesDir))
			{
				_dialogSelectProfile.InitialDirectory = profilesDir;
			}

			//load the dialog controls

			string defaultImport = TrafficViewerOptions.Instance.StartupImport;

			if (defaultImport != string.Empty)
			{
				_boxTargetPath.Text = defaultImport;
			}

			//load parsers
			_parsers.AddRange(TrafficViewer.Instance.TrafficParsers);

			foreach (ITrafficParser parser in _parsers)
			{
				_boxParserDll.Items.Add(parser.Name);
			}
			_boxParserDll.SelectedIndex = 0;

			//load profiles
			List<string> profilePaths = TrafficViewerOptions.Instance.GetProfilePaths();
			string savedSelectedProfile = TrafficViewerOptions.Instance.SelectedProfile;
			int selectedIndex = 0;
			foreach (string path in profilePaths)
			{
				try
				{
					string fName = Path.GetFileName(path);
					_availableProfiles.Add(fName, path);
					_boxParserProfile.Items.Add(fName);
					if (fName == savedSelectedProfile)
					{
						selectedIndex = _boxParserProfile.Items.Count - 1;
					}
				}
				catch { }
			}

			_boxParserProfile.SelectedIndex = selectedIndex;

			SelectBoxItemByText(TrafficViewerOptions.Instance.SelectedProfile, _boxParserDll);

			_boxTargetPath.Text = TrafficViewerOptions.Instance.StartupImport;

		}

		/// <summary>
		/// Shows the import form and returns the values of the fields to the caller
		/// </summary>
		/// <param name="defaultImportSettings">Values to pre-populate the dialog</param>
		/// <returns>The values selected in the form</returns>
		public static ImportDialogResult Execute(ImportInfo defaultImportSettings)
		{
			lock (_instanceLock)
			{
				if (__instance == null)
				{
					__instance = new ImportFileForm();
				}
				__instance._importResult = new ImportDialogResult();
				__instance._importResult.DialogResult = DialogResult.Cancel;
				if (defaultImportSettings != null && defaultImportSettings.Parser != null)
				{
					SelectBoxItemByText(defaultImportSettings.Parser.Name, __instance._boxParserDll);
					__instance._importResult.ImportInfo = defaultImportSettings;
					if (defaultImportSettings.Sender != null)
					{
						__instance._checkSender.Visible = true;

						if ((defaultImportSettings.Parser.ImportSupport & ImportMode.Objects) != 0)
						{
							__instance._checkSender.Checked = true;
						}
						else
						{
							__instance._checkSender.Checked = false;
							__instance._boxTargetPath.Text = String.Join("|", __instance._importResult.ImportInfo.TargetFiles.ToArray());
						}
					}
				}

				__instance.ShowDialog();
			}
			return __instance._importResult;
		}



		/// <summary>
		/// Shows the import form and returns the values of the fields to the caller
		/// </summary>
		/// <returns>The values selected in the form</returns>
		public static ImportDialogResult Execute()
		{
			return Execute(null);
		}


		/// <summary>
		/// Selects an item by its text
		/// </summary>
		/// <param name="text"></param>
		/// <param name="box"></param>
		private static void SelectBoxItemByText(string text, ComboBox box)
		{
			for (int i = 0; i < box.Items.Count; i++)
			{
				if (String.Compare(text, (string)box.Items[i]) == 0)
				{
					box.SelectedIndex = i;
					return;
				}
			}
		}

		private void ImportClick(object sender, EventArgs e)
		{
			_importResult.DialogResult = DialogResult.OK;
			_importResult.ImportInfo.TargetFiles.Clear();
			_importResult.ImportInfo.TargetFiles.AddRange(
				_boxTargetPath.Text.Split(new string[1] { FILE_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries));
			_importResult.ImportInfo.Parser = _parsers[_boxParserDll.SelectedIndex];

			if (_currentProfile == null)
			{
				_currentProfile = new ParsingOptions();
				_currentProfile.Load(_availableProfiles[(string)_boxParserProfile.SelectedItem]);

			}

			_currentProfile.UseExclusions = _checkExclusions.Checked;

			_importResult.ImportInfo.Profile = _currentProfile;
			_importResult.ImportInfo.Append = _checkAppendToFile.Checked;

			TrafficViewerOptions.Instance.SelectedProfile = (string)_boxParserProfile.SelectedItem;
			TrafficViewerOptions.Instance.SelectedParser = (string)_boxParserDll.SelectedItem;

			//save the options
			TrafficViewerOptions.Instance.Save();

			this.Close();
		}

		private void CancelClick(object sender, EventArgs e)
		{
			_importResult.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void BrowseToTargetClick(object sender, EventArgs e)
		{
			DialogResult dr = _dialogSelectFile.ShowDialog();
			if (dr == DialogResult.OK)
			{
				_boxTargetPath.Text = String.Empty;
				foreach (string fileName in _dialogSelectFile.FileNames)
				{
					_boxTargetPath.Text += fileName + FILE_SEPARATOR;
				}
				_boxTargetPath.Text = _boxTargetPath.Text.TrimEnd(FILE_SEPARATOR.ToCharArray());
			}
		}

		private void AddParserProfileClick(object sender, EventArgs e)
		{
			DialogResult dr = _dialogSelectProfile.ShowDialog();
			if (dr == DialogResult.OK)
			{
				string newProfilePath = _dialogSelectProfile.FileName;
				string profileName = Path.GetFileName(newProfilePath);
				if (!_availableProfiles.ContainsKey(profileName))
				{
					_availableProfiles.Add(profileName, newProfilePath);
					_boxParserProfile.Items.Add(profileName);
					_boxParserProfile.SelectedIndex = _boxParserProfile.Items.Count - 1;
					TrafficViewerOptions.Instance.SetProfilePaths(_availableProfiles.Values);
				}
			}
		}

		private void RemoveProfileDllClick(object sender, EventArgs e)
		{
			if (_boxParserProfile.SelectedIndex > -1)
			{
				string itemToRemove = (string)_boxParserProfile.SelectedItem;
				_boxParserProfile.Items.RemoveAt(_boxParserProfile.SelectedIndex);
				_availableProfiles.Remove(itemToRemove);
				TrafficViewerOptions.Instance.SetProfilePaths(_availableProfiles.Values);
				if (_boxParserProfile.Items.Count > 0)
				{
					_boxParserProfile.SelectedIndex = 0;
				}
			}
		}

		private void EditParserProfileClick(object sender, EventArgs e)
		{
			if (_currentProfile == null)
			{
				_currentProfile = new ParsingOptions();
				_currentProfile.Load(_availableProfiles[(string)_boxParserProfile.SelectedItem]);
			}
			ProfileEditor.Edit(_currentProfile);
		}

		private void ParserProfileSelectedIndexChanged(object sender, EventArgs e)
		{
			_currentProfile = new ParsingOptions();
			_currentProfile.Load(_availableProfiles[(string)_boxParserProfile.SelectedItem]);
		}

		private void SelectedIndexChanged(object sender, EventArgs e)
		{
			ITrafficParser selectedParser = _parsers[_boxParserDll.SelectedIndex];
			//construct filter
			StringBuilder sb = new StringBuilder();

			_checkSender.Checked = (selectedParser.ImportSupport & ImportMode.Objects) != 0 & _importResult.ImportInfo.Sender != null;

			foreach (string importType in selectedParser.ImportTypes.Keys)
			{
				sb.Append(importType);
				sb.Append('|');
				sb.Append(selectedParser.ImportTypes[importType]);
				sb.Append('|');
			}

			_dialogSelectFile.Filter = sb.ToString().TrimEnd('|');
		}

		private void CheckSenderChecked(object sender, EventArgs e)
		{
			_boxTargetPath.Enabled = !_checkSender.Checked;
			_buttonBrowseToTarget.Enabled = !_checkSender.Checked;

			if (!_checkSender.Checked)
			{
				if (_importResult.ImportInfo.TargetFiles.Count > 0)
				{
					_boxTargetPath.Text = String.Join("|", _importResult.ImportInfo.TargetFiles.ToArray());
				}
				else
				{
					_boxTargetPath.Text = TrafficViewerOptions.Instance.StartupImport;
				}
			}
			else
			{
				_boxTargetPath.Text = String.Empty;
			}
		}



	}
}