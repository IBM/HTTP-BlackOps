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
	public partial class ProfileEditor : Form
	{

		private ParsingOptions _currentProfile;

		private ProfileEditor()
		{
			InitializeComponent();
			string profilesDir = TrafficViewerOptions.TrafficViewerAppDataDir + "\\Profiles";
			if (Directory.Exists(profilesDir))
			{
				_dialogSave.InitialDirectory = profilesDir;
			}
		}

		/// <summary>
		/// The singleton instance
		/// </summary>
		private static ProfileEditor __instance = null;

		/// <summary>
		/// The lock for the creation of this instance
		/// </summary>
		private static object __instanceLock = new object();

		/// <summary>
		/// Allows viewing and editing of a parsing profile
		/// </summary>
		/// <param name="profile"></param>
		public static void Edit(ParsingOptions profile)
		{
			lock (__instanceLock)
			{
				if (__instance == null)
				{
					__instance = new ProfileEditor();
				}

				__instance._currentProfile = profile;
				__instance._boxBeginThread.Text = profile.BeginThreadRegex;
				__instance._boxDescription.Text = profile.DescriptionRegex;
				__instance._boxEndThread.Text = profile.EndThreadRegex;
				__instance._boxNonHttp.Text = profile.NonHttpTrafficRegex;
				__instance._boxProxyConnection.Text = profile.ProxyConnectionToSiteRegex;
				__instance._boxResponseReceived.Text = profile.ResponseReceivedMessageRegex;
				__instance._boxThreadId.Text = profile.ThreadIdRegex;
				__instance._boxTimeFormat.Text = profile.TimeFormat;

				__instance._exclusions.SetValues(profile.GetExclusions());

				profile.SetHighlightingDefinitions(profile.GetHighlightingDefinitions());
				__instance._highlightingDefs.SetValues((List<string>)profile.GetOption("HighlightingDefinitions"));

				profile.SetCustomFields(profile.GetCustomFields());
				__instance._customFields.SetValues((List<string>)profile.GetOption("CustomFields"));

				__instance.ShowDialog();
			}
		}

		private void Apply(ParsingOptions profileObject)
		{
			profileObject.BeginThreadRegex = _boxBeginThread.Text;
			profileObject.DescriptionRegex = _boxDescription.Text;
			profileObject.EndThreadRegex = _boxEndThread.Text;
			profileObject.NonHttpTrafficRegex = _boxNonHttp.Text;
			profileObject.ProxyConnectionToSiteRegex = _boxProxyConnection.Text;
			profileObject.ResponseReceivedMessageRegex = _boxResponseReceived.Text;
			profileObject.ThreadIdRegex = _boxThreadId.Text;
			profileObject.TimeFormat = _boxTimeFormat.Text;
			profileObject.SetMultiValueOption("CustomFields", _customFields.GetValues());
			profileObject.SetMultiValueOption("HighlightingDefinitions", _highlightingDefs.GetValues());
			profileObject.SetExclusions(_exclusions.GetValues());

		}

		private void SaveClick(object sender, EventArgs e)
		{
			Apply(_currentProfile);
			_currentProfile.Save();
			this.Close();
		}

		private void SaveAsClick(object sender, EventArgs e)
		{
			DialogResult dr = _dialogSave.ShowDialog();
			if (dr == DialogResult.OK)
			{
				Apply(_currentProfile);
				_currentProfile.SaveAs(_dialogSave.FileName);
				this.Close();
			}

		}

		private void CancelClick(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}