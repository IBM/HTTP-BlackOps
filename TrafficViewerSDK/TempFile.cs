using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TrafficViewerSDK
{
	/// <summary>
	/// Creates a file in the temp folder that will be automatically deleted in the destructor
	/// </summary>
	public class TempFile
	{

		private static long _uniqueNumber = DateTime.Now.Ticks;
		/// <summary>
		/// This is a unique number that gets appended to the file name
		/// </summary>
		private static long UniqueNumber
		{
			get
			{
				return _uniqueNumber++;
			}
		}

		private string _path = String.Empty;
		/// <summary>
		/// Gets the path of the file
		/// </summary>
		public string Path
		{
			get { return _path; }
			set { _path = value; }
		}

		private string _url = String.Empty;
		/// <summary>
		/// Gets the path of the file in a format that can be used by Internet Explorer
		/// </summary>
		public string Url
		{
			get { return _url; }
			set { _url = value; }
		}

		/// <summary>
		/// Opens a file stream to the temp path. It is your responsibility to close it after you're done with it
		/// </summary>
		/// <returns></returns>
		public Stream OpenStream()
		{
			return File.Open(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite); 
		}


		/// <summary>
		/// Contains the actual logic for creating the file
		/// </summary>
		/// <param name="fileName"></param>
		private void MakeFilePath(string fileName)
		{
			string uniqueVal = "tv_temp_" + UniqueNumber;
			string tempPath = Environment.GetEnvironmentVariable("temp");
			fileName = String.Format(@"{0}\{1}{2}", tempPath, uniqueVal, fileName);
			_path = fileName;

			//create the url representation, replace the column with a |
			string url = _path.Replace(':', '|');
			//replace the back slashes with forward slashes
			url = url.Replace('\\', '/');
			_url = "file:///" + url;
		}

		/// <summary>
		/// Appends the specified bytes to the temporary file after which it closes the file
		/// </summary>
		/// <param name="bytes">Byte array to be written to the file</param>
		public void Write(byte[] bytes)
		{
			Stream stream = OpenStream();
			//set the file position to the end of the file in order to append
			stream.Position = stream.Length;
			//write the stuff
			stream.Write(bytes, 0, bytes.Length);
			//close the file
			stream.Close();
		}

		/// <summary>
		/// Appends the specified text to the temp file after which it closes the file
		/// </summary>
		/// <param name="text"></param>
		public void Write(string text)
		{
			Write(Constants.DefaultEncoding.GetBytes(text));
		}

		/// <summary>
		/// Appends the specified text to the temp file after which it closes the file
		/// </summary>
		/// <param name="text"></param>
		public void WriteLine(string text)
		{
			Write(Constants.DefaultEncoding.GetBytes(text + Environment.NewLine));
		}
		/// <summary>
		/// Creates a file with a randomly generated name
		/// </summary>
		public TempFile()
		{
			MakeFilePath("");
		}

		/// <summary>
		/// Constructor for temp file
		/// </summary>
		/// <param name="fileName">The name of the file, example index.htm</param>
		public TempFile(string fileName)
		{
			MakeFilePath(fileName);
		}

		/// <summary>
		/// Deletes the temporary file
		/// </summary>
		~TempFile()
		{
			try
			{
				File.Delete(_path);
			}
			catch { }
		}

	}
}
