using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.IO.Compression;
using System.Xml;
using System.Web;
using TrafficViewerSDK.Options;
using Ionic.Zip;

namespace TrafficViewerSDK
{
    /// <summary>
    /// Contains utility functions
    /// </summary>
    public static class Utils
    {
        private const int MIN_PORT = 1025;
        private const int MAX_PORT = 65535;

        /// <summary>
        /// Gets the current thread id
        /// </summary>
        /// <returns>Thread id</returns>
        [DllImport("Kernel32", EntryPoint = "GetCurrentThreadId", ExactSpelling = true)]
        public static extern Int32 GetCurrentWin32ThreadId();


        #region Constants and Properties

        //used for the random string
        private const string CHARS = "01234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private static int lastCharIndex = CHARS.Length - 1;
        private static Random randGen = new Random();

        #endregion

        #region Compression & File Operations

        /// <summary>
        /// Compresses all the files in the specified top directory. Directory structure is not kept.
        /// Main purpose is to pack the TrafficViewer file data
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <param name="targetArchivePath"></param>
        public static void CompressFiles(string sourceDirectory, string targetArchivePath)
        {
            try
            {
                using (ZipFile zip = new ZipFile())
                {
                    string[] files = Directory.GetFiles(sourceDirectory);
                    zip.AddFiles(files, false, String.Empty);
                    zip.Save(targetArchivePath);
                }
            }
            catch (Exception ex)
            {
                SdkSettings.Instance.Logger.Log(TraceLevel.Error, "Cannot compress files: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Parse a HTTP port from the specified string
        /// </summary>
        /// <param name="portString"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool ParsePort(string portString, out int port)
        {
            port = -1;
            if (String.IsNullOrEmpty(portString) || !int.TryParse(portString, out port) || port < MIN_PORT || port > MAX_PORT)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Determines if a ZIP/RAR entry is valid by checking that the file will extract within the current directory
        /// ZIP files can contain relative paths (including ..\..\..\..\ etc) which would allow writing of arbitrary files anywhere on the file system.
        /// </summary>   
        /// <param name="filename">Filename of the zip entry</param>
        /// <param name="extractToPath">Destination path for intended decompression</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool IsSafeZipEntry(string filename, string extractToPath)
        {
            // Determine the absolute path of the given filename (zip entry)
            string absolutePath = Path.GetFullPath(extractToPath + @"\" + filename);
            string absoluteExtractToPath = Path.GetFullPath(extractToPath);
            // Ensure that it is inside the folder we're unzipping to
            if (absolutePath.StartsWith(absoluteExtractToPath, StringComparison.InvariantCultureIgnoreCase))
                return true;

            return false;
        }

        /// <summary>
        /// Extracts files from the specified archive into the target directory. Directories are ignored.
        /// </summary>
        /// <param name="sourceArchivePath"></param>
        /// <param name="targetFolderPath"></param>
        public static void DecompressFiles(string sourceArchivePath, string targetFolderPath)
        {
            try
            {
                if (!Directory.Exists(targetFolderPath))
                {
                    Directory.CreateDirectory(targetFolderPath);
                }

                using (ZipFile zip = ZipFile.Read(sourceArchivePath))
                {
                    foreach (ZipEntry entry in zip)
                    {
                        if (IsSafeZipEntry(entry.FileName, targetFolderPath))
                        {
                            entry.Extract(targetFolderPath, ExtractExistingFileAction.OverwriteSilently);
                        }
                        else
                        {
                            SdkSettings.Instance.Logger.Log(TraceLevel.Warning, "Invalid file path: '{0}'", entry.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SdkSettings.Instance.Logger.Log(TraceLevel.Error, "Cannot decompress files: {0}", ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Decompresses the specified bytes
        /// </summary>
        /// <param name="compressedBytes"></param>
        /// <returns></returns>
        public static byte[] DecompressData(byte[] compressedBytes)
        {
            ByteArrayBuilder builder = new ByteArrayBuilder();

            try
            {
                MemoryStream ms = new MemoryStream(compressedBytes);

                ZipInputStream zipStream = new ZipInputStream(ms);

                const int CHUNK_SIZE = 10240;

                ZipEntry entry = zipStream.GetNextEntry();

                if (entry != null)
                {
                    int bytesRead = 0;

                    do
                    {
                        byte[] chunk = new byte[CHUNK_SIZE];
                        bytesRead = zipStream.Read(chunk, 0, CHUNK_SIZE);
                        builder.AddChunkReference(chunk, bytesRead);
                    }
                    while (bytesRead > 0);
                }
            }
            catch (Exception ex)
            {
                SdkSettings.Instance.Logger.Log(TraceLevel.Warning, "Utils : DecompressData - Zip Exception: '{0}'", ex.Message);
            }
            return builder.ToArray();
        }


        /// <summary>
        /// Compress the data argument to a base64 string
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string CompressToBase64String(byte[] bytes)
        {
            using (ZipFile zip = new ZipFile())
            {
                zip.AddEntry("data", bytes);

                using (MemoryStream byteStream = new MemoryStream())
                {
                    zip.Save(byteStream);
                    return Convert.ToBase64String(byteStream.ToArray());
                }
            }
        }

        /// <summary>
        /// Compress the data argument to a base64 string
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string CompressToBase64String(string data)
        {
            var bytes = Constants.DefaultEncoding.GetBytes(data);
            return CompressToBase64String(bytes);
        }



        /// <summary>
        /// Decompresses the specified base 64 string
        /// </summary>
        /// <param name="base64EncodedString">The compressed bytes in base64 string format</param>
        /// <returns>The decompressed bytes</returns>
        public static byte[] DecompressBytesFromBase64String(string base64EncodedString)
        {
            if (String.IsNullOrWhiteSpace(base64EncodedString))
            {
                return new byte[0] { };
            }
            byte[] data = Convert.FromBase64String(base64EncodedString);
            byte[] decompressedData = Utils.DecompressData(data);
            if (decompressedData == null)
            {
                return new byte[0] { };
            }
            return decompressedData;
        }

        /// <summary>
        /// Decompresses the specified base 64 string
        /// </summary>
        /// <param name="base64EncodedString">The compressed bytes in base64 string format</param>
        /// <returns></returns>
        public static string DecompressFromBase64String(string base64EncodedString)
        {
            return Constants.DefaultEncoding.GetString(DecompressBytesFromBase64String(base64EncodedString));
        }

        /// <summary>
        /// Copies bytes from the source stream and appends them into the destination stream
        /// </summary>
        /// <param name="sourceStream"></param>
        /// <param name="destinationStream"></param>
        /// <param name="startPosition">The start position in the source stream from where to start the copying</param>
        /// <param name="length">The number of bytes to copy</param>
        /// <param name="bufferSize">How many bytes will be loaded into memory at a point in time</param>
        public static void CopyBytesToStream(Stream sourceStream, Stream destinationStream, long startPosition, long length, int bufferSize)
        {
            byte[] buffer = new byte[bufferSize];
            sourceStream.Position = startPosition;
            //make sure the length of the block does not actually exceed the length of the source file
            if (length > sourceStream.Length - sourceStream.Position)
            {
                length = sourceStream.Length - sourceStream.Position;
            }

            while (length > 0)
            {
                //make sure the buffer size is not bigger than the remaining length
                if (bufferSize > sourceStream.Length - sourceStream.Position)
                {
                    bufferSize = (int)(sourceStream.Length - sourceStream.Position);
                }
                sourceStream.Read(buffer, 0, bufferSize);
                destinationStream.Write(buffer, 0, bufferSize);
                length -= bufferSize;
            }
        }

        /// <summary>
        /// Readline for byte streams
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="estimatedLineLength">
        /// The estimated length of a line. This is how much to allocate at first for the return value. If this value is exceeded then
        /// the array is resized. Set this value high for greater performance or lower for better memory
        /// </param>
        /// <param name="lineEnding"></param>
        /// <returns></returns>
        public static byte[] ReadLine(Stream stream, int estimatedLineLength, LineEnding lineEnding)
        {
            if (stream.Position == stream.Length)
            {
                return null;//EOF
            }

            byte[] val = new byte[estimatedLineLength]; //will store all the read bytes
            int bufferSize = estimatedLineLength + 2; //estimated line length plus two extra bytes for new lines chars
            byte[] buffer = new byte[bufferSize];
            long streamPosition = stream.Position;
            byte c;
            int totalBytesAccumulated = 0;
            int i, bytesRead, newSize;
            bool newLine = false;
            bool cr = false;
            bool any = lineEnding == LineEnding.Any;

            do
            {
                bytesRead = stream.Read(buffer, 0, bufferSize);//read a chunk of text
                i = 0;
                while (i < bytesRead && !newLine)
                {
                    c = buffer[i];
                    if (c == 13)
                    {
                        cr = true;
                    }
                    else if (c == 10 && (cr || any))
                    {
                        newLine = true;
                    }
                    else
                    {
                        cr = false;
                        newLine = false;
                    }
                    i++;
                }
                streamPosition += i;

                //HANDLE THE NEW LINE BYTES
                //what if the last char read was a CR and we don't know if LF follows?
                if (i == bufferSize && cr)
                {
                    //decrease the file position so we can re-process it on the next iteration as a full \r\n
                    streamPosition--;
                    //decrease i so we don't include the CR in the data that was read and we do one more round
                    //if the next chunk is just a \r\n 0 bytes will be added
                    i--;
                }

                if (newLine)
                {
                    //do not include the CR/LF in the final chunk
                    if (cr) i -= 2;
                    else i--;
                }

                //ADD THE BYTES READ WITHOUT NEW LINE TO VAL
                newSize = totalBytesAccumulated + i;

                //resize val if the estimated line size is smaller than the actual line size
                if (newSize > val.Length)
                {
                    Array.Resize<byte>(ref val, newSize);
                }

                //copy the read chunk to the return value
                Array.Copy(buffer, 0, val, totalBytesAccumulated, i);

                //update the number of bytes accumulated in the byteChunks
                totalBytesAccumulated = newSize;
                //do this as long as the for loop was not broken by a CR or there are still bytes in the file
            }
            while (i == bufferSize); //this means that the chunk was read to the end so theres still bytes and no newline was found

            //update stream position
            stream.Position = streamPosition;

            if (totalBytesAccumulated == 0)
            {
                return new byte[0];//Empty string
            }

            //resize val if the estimated line size was bigger than what was actually read
            if (totalBytesAccumulated < val.Length)
            {
                Array.Resize<byte>(ref val, totalBytesAccumulated);
            }
            return val;
        }

        /// <summary>
        /// Read a line in DOS format
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="estimatedLineLength">
        /// The estimated length of a line. This is how much to allocate at first for the return value. If this value is exceeded then
        /// the array is resized. Set this value high for greater performance or lower for better memory
        /// </param>
        /// <returns></returns>
        public static byte[] ReadLine(Stream stream, int estimatedLineLength)
        {
            return Utils.ReadLine(stream, estimatedLineLength, LineEnding.DosOnly);
        }

        /// <summary>
        /// Writes a line to the specified strem
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="line"></param>
        public static void WriteLine(Stream stream, string line)
        {
            line += Environment.NewLine;
            byte[] bytes = Constants.DefaultEncoding.GetBytes(line);
            stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Opens the a file in such a manner that it can be read and written by multiple processes
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static FileStream OpenFile(string filePath)
        {
            try
            {
                return File.Open(filePath, FileMode.Open,
                    FileAccess.Read, FileShare.Read);
            }
            catch (Exception e)
            {
                SdkSettings.Instance.Logger.Log(TraceLevel.Error, "Cannot open raw traffic log: {0}", e.Message);
                return null;
            }
        }
        #endregion

        #region Text & Regex


        /// <summary>
        /// Splits path from query
        /// </summary>
        /// <param name="originalPathAndQuery"></param>
        /// <param name="path"></param>
        /// <param name="query"></param>
        public static void ExtractPathAndQuery(string originalPathAndQuery, out string path, out string query)
        {
            if (originalPathAndQuery == null)
            {
                throw new Exception("Invalid originalPathAndQuery");
            }

            int indexOfQuery = originalPathAndQuery.IndexOf("?");
            if (indexOfQuery > -1)
            {
                path = originalPathAndQuery.Substring(0, indexOfQuery);
                query = originalPathAndQuery.Substring(indexOfQuery + 1);
            }
            else
            {
                path = originalPathAndQuery;
                query = String.Empty;
            }
        }

        /// <summary>
        /// Escapes quotes
        /// </summary>
        /// <param name="source"></param>
        /// <returns>Escaped string</returns>
        public static string AddSlashes(string source)
        {
            source = source.Replace("\\", "\\\\");
            source = source.Replace("\"", "\\\"");
            source = source.Replace("'", "\\\'");
            source = source.Replace("\r", "\\r");
            source = source.Replace("\n", "\\n");

            return source;
        }

        /// <summary>
        /// Performs a selective url encode only for \r\n
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string UrlEncodeTags(string source)
        {
            source = source.Replace("<", "%3c");
            source = source.Replace(">", "%3e");
            return source;
        }

        /// <summary>
        /// COnverts an array of bytes to string
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ByteToString(byte[] bytes)
        {
            if (bytes == null) return null;
            return Constants.DefaultEncoding.GetString(bytes);
        }

        /// <summary>
        /// Returns the value of the first group in the specified regex
        /// </summary>
        /// <param name="input"></param>
        /// <param name="regex"></param>
        /// <returns>Extracted value or String.Empty if the value is not found</returns>
        public static string RegexFirstGroupValue(string input, string regex)
        {
            try
            {
                Match m = Regex.Match(input, regex, RegexOptions.IgnoreCase);
                if (m.Groups.Count > 1)
                {
                    return m.Groups[1].Value;
                }
            }
            catch { }
            return String.Empty;
        }

        /// <summary>
        /// Replaces all the groups with the specified string. The match is case
        /// insensitive and is executed on each line
        /// </summary>
        /// <param name="input"></param>
        /// <param name="regex"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string ReplaceGroups(string input, string regex, string replacement)
        {
            return ReplaceGroups(input, regex, replacement, RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }


        /// <summary>
        /// Replaces all the groups with the specified string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="regex"></param>
        /// <param name="replacement"></param>
        /// <param name="flags">Regex flags</param>
        /// <returns></returns>
        public static string ReplaceGroups(string input, string regex, string replacement, RegexOptions flags)
        {
            MatchCollection matches = Regex.Matches(input, regex, flags);

            for (int i = matches.Count - 1; i > -1; i--)
            {
                Match m = matches[i];
                for (int j = m.Groups.Count - 1; j >= 1; j--)
                {
                    if (m.Groups[j].Success)
                    {
                        string firstPart = input.Substring(0, m.Groups[j].Index);
                        string secondPart = input.Substring(m.Groups[j].Index + m.Groups[j].Value.Length);
                        input = firstPart + replacement + secondPart;
                    }
                }
            }

            return input;
        }

        /// <summary>
        /// Performs a error safe case insensitive regex search
        /// </summary>
        /// <param name="input"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        public static bool IsMatch(string input, string regex)
        {
            if (input == null || input == String.Empty || regex == null || regex == String.Empty)
            {
                return false;
            }

            try
            {
                if (Regex.IsMatch(input, regex, RegexOptions.IgnoreCase))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        /// <summary>
        /// Performs a error safe case insensitive regex search
        /// </summary>
        /// <param name="input"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        public static bool IsMatch(string input, Regex regex)
        {
            if (input == null || input == String.Empty || regex == null || String.IsNullOrEmpty(regex.ToString()))
            {
                return false;
            }

            try
            {
                if (regex.IsMatch(input))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the input matches any of the regular expressions
        /// </summary>
        /// <param name="input"></param>
        /// <param name="listOfRegexes"></param>
        /// <returns></returns>
        public static bool IsMatchInList(string input, IEnumerable<string> listOfRegexes)
        {
            foreach (string regex in listOfRegexes)
            {
                if (IsMatch(input, regex))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Transforms time format into regular expression
        /// Example formats:
        /// ddd - Fri
        /// dd - 13
        /// MMM - Jun
        /// yyyy - 2010
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetTimeRegex(string format)
        {
            format = format.Replace("dddd", @"\w+");
            format = format.Replace("MMMM", @"\w+");
            format = format.Replace("ddd", @"\w\w\w");
            format = format.Replace("MMM", @"\w\w\w");
            format = Regex.Replace(format, "[dmMyhHsf]", @"\d");
            format = "(" + format + ")";
            return format;
        }

        /// <summary>
        /// Performs a case insensitive search operation of the needle in stack
        /// </summary>
        /// <param name="needle">The string to search for</param>
        /// <param name="stack">The string to search in</param>
        /// <returns></returns>
        public static bool CIContains(string needle, string stack)
        {
            needle = needle.ToLower();
            stack = stack.ToLower();
            return stack.Contains(needle);
        }

        /// <summary>
        /// Extracts the name of the file in the URL
        /// </summary>
        /// <param name="uri">Full url to be processed</param>
        /// <returns>
        /// - File name
        /// - String.Empty if there is no file name
        ///</returns>
        public static string GetFileNameFromUri(string uri)
        {
            //remove the portion following the query sign
            int index = uri.IndexOfAny(new char[2] { '?', ' ' });
            if (index > -1)
            {
                uri = uri.Substring(0, index);
            }

            //remove the portion up to the last forward slash
            index = uri.LastIndexOf('/');
            if (index > -1)
            {
                uri = uri.Substring(index + 1);
            }

            return uri;
        }

        /// <summary>
        /// Generates a random string
        /// </summary>
        /// <param name="min">Minimum length</param>
        /// <param name="max">Maximum length</param>
        /// <returns></returns>
        public static string RandomString(int min, int max)
        {
            int i, size = randGen.Next(min, max);
            StringBuilder randString = new StringBuilder(size);

            for (i = 0; i < size; i++)
            {
                int ord = randGen.Next(lastCharIndex);
                randString.Append(CHARS[ord]);
            }

            return randString.ToString();
        }

        #endregion

        #region Encoding


        /// <summary>
        /// Returns a ASCII Encoded version of the given text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ASCIIEncode(string text)
        {

            byte[] bytes = Constants.DefaultEncoding.GetBytes(text);
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        /// <summary>
        /// Returns a ASCII decoded version of the given text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ASCIIDecode(string text)
        {
            byte[] bytes = new byte[text.Length / 2];
            try
            {
                for (int i = 0; i < bytes.Length; i++)
                {

                    bytes[i] = Convert.ToByte(text.Substring(i * 2, 2), 16);

                }
            }
            catch
            {
                return text;
            }

            return Constants.DefaultEncoding.GetString(bytes);
        }


        /// <summary>
        /// Returns a Url Encoded version of the given text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string UrlEncode(string text)
        {
            text = HttpUtility.UrlEncode(text);
            //replace + with %20
            text = text.Replace("+", "%20");

            return text;
        }


        /// <summary>
        /// Returns a JSON Encoded version of the given text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string JSONEncode(string text)
        {
            text = HttpUtility.JavaScriptStringEncode(text);
            return text;
        }


       
        /// <summary>
        /// Applies url encoding to everything including ASCII characters
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string UrlEncodeAll(string text)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                byte code = (byte)text[i];
                result.Append(String.Format("%{0:X}", code));
            }
            return result.ToString();
        }

        /// <summary>
        /// Returns a Url Decoded version of the given text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string UrlDecode(string text)
        {
            return HttpUtility.UrlDecode(text);
        }

        /// <summary>
        /// Returns a Html Encoded version of the given text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string HtmlEncode(string text)
        {
            return HttpUtility.HtmlEncode(text);
        }

        /// <summary>
        /// Returns a Html Decoded version of the given text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string HtmlDecode(string text)
        {
            return HttpUtility.HtmlDecode(text);
        }

        /// <summary>
        /// Returns a Base64 Encoded version of the given text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Base64Encode(string text)
        {
            return Convert.ToBase64String(Constants.DefaultEncoding.GetBytes(text));
        }

        /// <summary>
        /// Returns a Base64 Decoded version of the given text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Base64Decode(string text)
        {

            string[] textParts = text.Split(',', '.', '_', '-', ';', '\n', '\r', '\\', ' ');

            StringBuilder sb = new StringBuilder();
            foreach (var part in textParts)
            {
                sb.Append(SimpleBase64Decode(part));
            }


            return sb.ToString();

        }

        private static string SimpleBase64Decode(string text)
        {
            try
            {
                text = text.Trim();
                text = Constants.DefaultEncoding.GetString(Convert.FromBase64String(text));
            }
            catch
            {
                try
                {
                    text = Constants.DefaultEncoding.GetString(Convert.FromBase64String(text + "="));
                }
                catch
                {
                    try
                    {
                        text = Constants.DefaultEncoding.GetString(Convert.FromBase64String(text + "=="));
                    }
                    catch { }
                }

            }
            return text;
        }




        #endregion

        #region Registry

        /// <summary>
        /// Verfies if script debugging is enabled. This is a option that can cause problems vieweing requests in the browser view
        /// </summary>
        /// <returns></returns>
        public static bool IsScriptDebuggingEnabled()
        {
            string scriptDebuggingDisabledOption = GetRegistryKey(@"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main",
                                                            "Disable Script Debugger") as string;
            if (scriptDebuggingDisabledOption == null || String.Compare(scriptDebuggingDisabledOption, "yes", true) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Disables script debugging. This is a option that can cause problems vieweing requests in the browser view
        /// </summary>
        public static void DisableScriptDebugging()
        {
            ModRegistryKey("HKEY_CURRENT_USER\\Software\\Microsoft\\Internet Explorer\\Main", "Disable Script Debugger", "yes");
        }

        /// <summary>
        /// Disables certificate name mismatch warnings
        /// </summary>
        public static void DisableCertNameMismatchWarning()
        {
            ModRegistryKey(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings",
                "WarnonBadCertRecving", 0);
        }

        /// <summary>
        /// Enables certificate name mismatch warnings
        /// </summary>
        public static void EnableCertNameMismatchWarning()
        {
            ModRegistryKey(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings",
                "WarnonBadCertRecving", 1);
        }


        /// <summary>
        /// Modifies the registry key at the specified path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ModRegistryKey(string path, string name, object value)
        {
            try
            {
                Registry.SetValue(path, name, value);
            }
            catch { return false; }

            return true;
        }

        /// <summary>
        /// Gets the value of a registry key at the specified path, 
        /// </summary>
        /// <param name="path">Full path: HKEY_CURRENT_USER\Software\</param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static object GetRegistryKey(string path, string name)
        {
            object value = null;
            try
            {
                // Attempt to open the key
                value = Registry.GetValue(path, name, null);
            }
            catch { }

            return value;
        }

        /// <summary>
        /// Gets the value of a registry key at the specified path, 
        /// </summary>
        /// <param name="path">Full path: HKEY_CURRENT_USER\Software\</param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetRegistryKeyValueString(string path, string name)
        {
            string value = GetRegistryKey(path, name) as string;
            value = value ?? String.Empty;
            return value;
        }


        /// <summary>
        /// Traverses the subtree under the specified registry key and 
        /// reads all the values in the subtree into a sorted dictionary. 
        /// The dictionary maps fullRegistryKeyPath\valueName to value.   
        /// </summary>
        /// <param name="rootRegistryKey">the root of the subtree to read</param>
        /// <returns>a SortedDictionary mapping fullKeyRegistryPath\valueName to value for the subtree of rootRegistryKey</returns>
        public static SortedDictionary<string, object> ReadRegistrySubTree(RegistryKey rootRegistryKey)
        {
            //initialize the dictionary that will be returned
            SortedDictionary<string, object> values = new SortedDictionary<string, object>();

            if (rootRegistryKey != null)
            {
                //this stack is used for DF traversal of the subtree
                Stack<RegistryKey> stack = new Stack<RegistryKey>();

                //visit every value under the rootRegistry in a DF manner 
                stack.Push(rootRegistryKey);

                RegistryKey currentKey;
                while (stack.Count > 0)
                {
                    currentKey = stack.Pop();

                    //retrive and add children keys for the currentKey to the stack
                    string[] subKeyNames = currentKey.GetSubKeyNames();
                    foreach (string subKeyName in subKeyNames)
                    {
                        stack.Push(currentKey.OpenSubKey(subKeyName));
                    }

                    //retrive and add valueNames with their values for the currentKey to dictionary
                    string[] valueNames = currentKey.GetValueNames();
                    foreach (string valueName in valueNames)
                    {
                        values.Add(currentKey.Name + "\\" + valueName, currentKey.GetValue(valueName));
                    }
                }
            }
            return values;
        }

        /// <summary>
        /// Restarts the traffic viewer process
        /// </summary>
        public static void RestartTrafficViewer()
        {
            Process tv = new Process();
            tv.StartInfo.FileName = "TrafficViewer.exe";
            if (!tv.Start())
            {
                throw new Exception("Could not start a new Traffic Viewer");
            }
            else
            {
                Process.GetCurrentProcess().CloseMainWindow();

            }

        }

        #endregion

    }
}
