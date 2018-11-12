using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK.Options;


namespace TrafficViewerSDK.Importers
{
	/// <summary>
	/// Interface used for implementing traffic importers
	/// </summary>
    public interface ITrafficParser : ITrafficViewerExtension
    {
		/// <summary>
		/// Specifies what kind of import is supported by the parser
		/// </summary>
		ImportMode ImportSupport
		{
			get;
		}

		/// <summary>
		/// The name of the parser
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Specifies the import extension filters accompanied by their description e.g {Traffic File,*Traffic*} or {Manual Explore File,*.exd}
		/// </summary>
		Dictionary<string,string> ImportTypes
		{
			get;
		}	

		/// <summary>
		/// Used to control UI depending on the parser's activity
		/// </summary>
        TrafficParserStatus ParserStatus
        {
            get;
        }

		/// <summary>
		/// The options used by the parser. For example highlighting options
		/// </summary>
        ParsingOptions Options
        {
            get;
        }


		/// <summary>
		/// Imports the specified file into the specified traffic file or other data accessor implementation
		/// </summary>
		/// <param name="patOfFileToImport"></param>
		/// <param name="currentFile"></param>
		/// <param name="options"></param>
		void Parse(string patOfFileToImport, ITrafficDataAccessor currentFile, ParsingOptions options);

		/// <summary>
		/// Imports data using the specified sender. For example IAppScan
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="currentFile"></param>
		/// <param name="options"></param>
		void Parse(object sender, ITrafficDataAccessor currentFile, ParsingOptions options);

		/// <summary>
		/// Stops the parser
		/// </summary>
        void Stop();

		/// <summary>
		/// Resumes a stopped parser
		/// </summary>
        void Resume();

		/// <summary>
		/// Executes a tail operation reading the last chunk added at the end of the file from the last read
		/// </summary>
		/// <param name="tailChunk"></param>
		void Tail(int tailChunk);

		/// <summary>
		/// Clears the source traffic file
		/// </summary>
		void ClearSource();
    }
}
