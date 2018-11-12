using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TrafficViewerSDK
{
    /// <summary>
    /// Provides a common interface for log
    /// </summary>
    public interface ILogWriter
    {
    
        /// <summary>
        /// Gets/sets the log trace level
        /// </summary>
        TraceLevel TraceLevelSetting
        {
            get;
            set;
        }

        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="traceLevel"></param>
        /// <param name="formattedMessage"></param>
        /// <param name="args"></param>
        void Log(TraceLevel traceLevel, string formattedMessage, params object[] args);
        
    }
}

	
