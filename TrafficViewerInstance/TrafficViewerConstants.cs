using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrafficViewerInstance
{
    /// <summary>
    /// Stores constants used by Black Ops
    /// </summary>B
    public static class TrafficViewerConstants
    {
        //public const string BUILD = "292";
        public static string BUILD {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
        public const string DLL_VERSION = "2.1.*";

#if DEBUG
		public const string VERSION = "(Debug)";
#else
			public const string VERSION = "";
#endif

		public const string APP_NAME = "HTTP Black Ops";

		public const string OPTIONS_FILE_NAME = "BlackOpsOptions.xml";
		public const int PLAYBACK_CONSTRAINT = 100;
	}
}
