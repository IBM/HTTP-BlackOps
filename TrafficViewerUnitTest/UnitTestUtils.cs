using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK;

namespace TrafficViewerUnitTest
{
	internal class UnitTestUtils
	{
		public static TrafficViewerFile GenerateTestTvf()
		{
			TrafficViewerFile tvf = new TrafficViewerFile();
			TempFile temp = new TempFile(".tvf");
			temp.Write(Properties.Resources.altoro);
			tvf.Open(temp.Path);
			return tvf;
		}
	}
}
