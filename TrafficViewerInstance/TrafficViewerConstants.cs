/**
Copyright 2019 Trend Micro, Incorporated, All Rights Reserved.
SPDX-License-Identifier: Apache-2.0
 */
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
        public const string BUILD = "306";
        
        public const string DLL_VERSION = "2.2.0."+BUILD;

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
