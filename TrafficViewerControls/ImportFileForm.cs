/******************************************************************
* IBM Confidential
* OCO Source Materials
* IBM Rational Traffic Viewer
* (c) Copyright IBM Corp. 2010 All Rights Reserved.
* 
* The source code for this program is not published or otherwise
* divested of its trade secrets, irrespective of what has been
* deposited with the U.S. Copyright Office.
******************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TrafficViewerSDK;
using System.IO;

namespace TrafficViewerControls
{
	public partial class ImportFileForm : Form
	{
		/// <summary>
		/// Maps parser file names to their full path
		/// </summary>
		private Dictionary<string, string> _availableParsers = new Dictionary<string, string>();

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

			string defaultImport = TrafficViewerOptions.Instance.DefaultImport;

			if (defaultImport != string.Empty)
			{
				_boxTargetPath.Text = defaultImport;
			}

			//load parsers
			List<string> parserPaths = TrafficViewerOptions.Instance.GetParserPaths();
			foreach (string path in parserPaths)
			{
				try
				{
					string fName = Path.GetFileName(path);
					_availableParsers.Add(fName, path);
					_boxParserDll.Items.Add(fName);
				}
				catch { }
			}

			if (_boxParserDll.Items.Count > 0)
			{
				_boxParserDll.SelectedIndex = 0;
			}

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


		}

		/// <summary>
		/// Shows the import form and returns the values of the fields to the caller
		/// </summary>
		/// <returns>The values selected in the form</returns>
		public static ImportDialogResult Execute()
		{
			lock (_instanceLock)
			{
				if (__instance == null)
				{
					__instance = new ImportFileForm();
				}
				__instance._importResult = new ImportDialogResult();
				__instance._importResult.DialogResult = DialogResult.Cancel;
				__instance.ShowDialog();
			}
			return __instance._importResult;
		}

		private void FileImportFormLoad(object sender, EventArgs e)
		{
		}

		private void ImportClick(object sender, EventArgs e)
		{
			_importResult.DialogResult = DialogResult.OK;
			_importResult.TargetFile = _boxTargetPath.Text;
			_importResult.ParserPath = _availableParsers[_boxParserDll.Text];

			if (_currentProfile == null)
			{
				_currentProfile = new ParsingOptions();
				_currentProfile.Load(_availableProfiles[(string)_boxParserProfile.SelectedItem]);

			}

			_currentProfile.UseExclusions = _checkExclusions.Checked;

			_importResult.Profile = _currentProfile;
			_importResult.Append = _checkAppendToFile.Checked;

			if (_checkAutoLoad.Checked)
			{
				TrafficViewerOptions.Instance.DefaultImport = _boxTargetPath.Text;
			}

			TrafficViewerOptions.Instance.SelectedProfile = (string)_boxParserProfile.SelectedItem;

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
				_boxTargetPath.Text = _dialogSelectFile.FileName;
			}
		}

		private void AddParserDllClick(object sender, EventArgs e)
		{
			DialogResult dr = _dialogSelectParser.ShowDialog();
			if (dr == DialogResult.OK)
			{
				string newParserPath = _dialogSelectParser.FileName;
				string parserName = Path.GetFileName(newParserPath);
				if (!_availableParsers.ContainsKey(parserName))
				{
					_availableParsers.Add(parserName, newParserPath);
					_boxParserDll.Items.Add(parserName);
					_boxParserDll.SelectedIndex = _boxParserDll.Items.Count - 1;
					TrafficViewerOptions.Instance.SetParserPaths(_availableParsers.Values);
				}
			}
		}

		private void RemoveParserDllClick(object sender, EventArgs e)
		{
			if (_boxParserDll.SelectedIndex > -1)
			{
				string itemToRemove = (string)_boxParserDll.SelectedItem;
				_boxParserDll.Items.RemoveAt(_boxParserDll.SelectedIndex);
				_availableParsers.Remove(itemToRemove);
				TrafficViewerOptions.Instance.SetParserPaths(_availableParsers.Values);
				if (_boxParserDll.Items.Count > 0)
				{
					_boxParserDll.SelectedIndex = 0;
				}
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

	}
}