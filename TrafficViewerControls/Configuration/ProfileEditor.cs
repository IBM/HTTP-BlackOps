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
using TrafficViewerInstance;
using TrafficViewerSDK.Http;

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
				profile.SetRequestReplacements(profile.GetRequestReplacements());
                __instance._requestReplacements.SetValues((List<string>)profile.GetOption("RequestReplacements"));
                profile.SetResponseReplacements(profile.GetResponseReplacements());
                __instance._responseReplacements.SetValues((List<string>)profile.GetOption("ResponseReplacements"));
                profile.SetTrackingPatterns(profile.GetTrackingPatterns());
                __instance._trackingPatterns.SetValues((List<string>)profile.GetOption("TrackingPatterns"));

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
			profileObject.SetMultiValueOption("RequestReplacements",_requestReplacements.GetValues());
            profileObject.SetMultiValueOption("ResponseReplacements", _responseReplacements.GetValues());
            profileObject.SetMultiValueOption("TrackingPatterns", _trackingPatterns.GetValues());
            PatternTracker.Instance.PatternsToTrack = profileObject.GetTrackingPatterns();
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