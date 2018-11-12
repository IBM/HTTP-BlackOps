using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing;
using TrafficViewerSDK.Http;

namespace CustomTestsUI
{
    /// <summary>
    /// Responsible for test execution
    /// </summary>
    public interface ITestRunner
    {
        void Pause();

        void Cancel();

        int LeftToTest { get;  }

        void Run();

        bool IsRunning { get;  }

        void SetTestFile(Testing.CustomTestsFile testFile);

        BaseAttackProxy GetTestProxy(INetworkSettings networkSettings, bool isSequential);

        string GetPatternOfRequestsToTest();
    }
}
