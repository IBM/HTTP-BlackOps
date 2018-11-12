using System;
using System.Windows.Forms;
using TVDiff;
using TVDiff.Implementations;
using TrafficViewerControls;
using TrafficViewerSDK.Options;
using System.Collections.Generic;
using TrafficViewerSDK;
using TrafficViewerSDK.Importers;
using System.Text.RegularExpressions;
using System.Diagnostics;
using TrafficViewerInstance;

namespace TrafficViewerProgram
{
	static class Program
	{

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
            
			string[] args = Environment.GetCommandLineArgs();
			string currentTVFilePath = null;
			ImportInfo importInfo = null;

			if (args.Length > 1) //a command line argument was specified
			{
				string path = args[1];

				if (path == "INSTALL")
				{ 
					//this is the first installation allow the setup to finish and 
					//lanch a separate instance
					Process.Start(Application.ExecutablePath); 
					return;
				}


				if (path.EndsWith(".tvf", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".htd", StringComparison.OrdinalIgnoreCase))
				{
					currentTVFilePath = path;
				}
				else
				{
					importInfo = new ImportInfo();
					importInfo.TargetFiles = new List<string>();
					importInfo.TargetFiles.Add(path);
					importInfo.Profile = new ParsingOptions();
					if (TrafficViewerOptions.Instance.StartupImportProfile != String.Empty)
					{
						importInfo.Profile.Load(TrafficViewerOptions.TrafficViewerAppDataDir
							+ "\\Profiles\\" + TrafficViewerOptions.Instance.StartupImportProfile);
					}
					else
					{
						importInfo.Profile = ParsingOptions.GetDefaultProfile();
					}
				}
			}

			//if a file was defined start the auto import on startup and there were no start arguments
			if (importInfo == null && String.IsNullOrEmpty(currentTVFilePath) && TrafficViewerOptions.Instance.StartupImport != String.Empty)
			{
				importInfo = new ImportInfo();
				importInfo.TargetFiles = new List<string>();
				if (!String.IsNullOrEmpty(TrafficViewerOptions.Instance.StartupImport))
				{
					importInfo.TargetFiles.Add(TrafficViewerOptions.Instance.StartupImport);
				}
				importInfo.Profile = new ParsingOptions();
				if (TrafficViewerOptions.Instance.StartupImportProfile != String.Empty)
				{
					importInfo.Profile.Load(TrafficViewerOptions.TrafficViewerAppDataDir
						+ "\\Profiles\\" + TrafficViewerOptions.Instance.StartupImportProfile);
				}
				else
				{
					importInfo.Profile = ParsingOptions.GetDefaultProfile();
				}


				importInfo.Parser = TrafficViewer.Instance.GetParser(TrafficViewerOptions.Instance.StartupParser);

			}


			if (importInfo!=null && importInfo.TargetFiles != null && importInfo.TargetFiles.Count > 0 &&
				(importInfo.Parser == null || !ParserMatchesFile(importInfo.Parser, TrafficViewerOptions.Instance.StartupImport)))
			{
				foreach (ITrafficParser parser in TrafficViewer.Instance.TrafficParsers)
				{
					if (ParserMatchesFile(parser, importInfo.TargetFiles[0]))
					{
						importInfo.Parser = parser;
						break;
					}
				}

				if (importInfo.Parser == null)
				{
					importInfo = null;
				}
			}

			Application.Run(new TrafficViewerForm(importInfo, currentTVFilePath));
		}

		private static bool ParserMatchesFile(ITrafficParser parser, string filePath)
		{
			foreach (string typeMatch in parser.ImportTypes.Values)
			{
				Regex typeMatchRegex = new Regex(typeMatch.Replace("*", ".*") + "$");
				if (typeMatchRegex.IsMatch(filePath))
				{
					return true;
				}
			}
			return false;
		}

	}
}