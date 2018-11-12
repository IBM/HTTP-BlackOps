using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace TrafficViewerSDK
{
    /// <summary>
    /// Provides the default implementation for the interface ILogWriter
    /// </summary>
    public class DefaultLogWriter : ILogWriter
    {
        private object _lock = new object();
        private string _logFileName;
        
        private TraceLevel _traceLevelSetting = TraceLevel.Off;
        /// <summary>
        /// Level of log
        /// </summary>
        public TraceLevel TraceLevelSetting
        {
            get { return _traceLevelSetting; }
            set { _traceLevelSetting = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public DefaultLogWriter()
        {
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logFileName"></param>
        /// <param name="traceLevelSetting"></param>
        public DefaultLogWriter(string logFileName, TraceLevel traceLevelSetting)
        {
            _logFileName = logFileName;
            _traceLevelSetting = traceLevelSetting;
        }

        /// <summary>
        /// Log the specified formatted message in a thread safe way to the file system
        /// </summary>
        /// <param name="traceLevel"></param>
        /// <param name="formattedMessage"></param>
        /// <param name="args"></param>
        public void Log(TraceLevel traceLevel, string formattedMessage, params object[] args)
        {
            lock(_lock)
            {
                try
                {
                    if (traceLevel == TraceLevel.Off)
                    {
                        return;
                    }

                    if (_traceLevelSetting >= traceLevel)
                    {
                        if (!String.IsNullOrEmpty(_logFileName))
                        {
                            if ((args != null) && (args.Length > 0))
                            {
                                formattedMessage = String.Format(formattedMessage, args);
                            }
                            formattedMessage = String.Format("{0} {1} {2}\r\n", DateTime.Now, traceLevel, formattedMessage);
                            File.AppendAllText(_logFileName, formattedMessage);
                        }
                    }
                }
                catch { }
            }
        }
    }
}
