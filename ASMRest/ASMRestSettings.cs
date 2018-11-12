using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficViewerSDK;

namespace ASMRest
{
    public class ASMRestSettings
    {
        private string _hostAndPort;

        public string HostAndPort
        {
            get { return _hostAndPort; }
            set { _hostAndPort = value; }
        }
        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        private string _fullName = "Unspecified";

        public string FullName
        {
            get { return _fullName; }
            set { _fullName = value; }
        }

        private string _password;

        public string Password
        {
            get { return Encryptor.DecryptToString(_password); }
            set { _password = Encryptor.EncryptToString(value); }
        }


        private string _ascSessionId = String.Empty;

        public string AscSessionId
        {
            get { return _ascSessionId; }
            set { _ascSessionId = value; }
        }

        private string _ascSsoToken = String.Empty;

        public string AscSsoToken
        {
            get { return _ascSsoToken; }
            set { _ascSsoToken = value; }
        }

        private Version _serverVersion = new Version();
        /// <summary>
        /// Server version
        /// </summary>
        public Version ServerVersion
        {
            get { return _serverVersion; }
            set { _serverVersion = value; }
        }

        private Dictionary<string, string> _issueAttributeMap = new Dictionary<string, string>();

        public Dictionary<string, string> IssueAttributeMap
        {
            get { return _issueAttributeMap; }
        }

    }
}
