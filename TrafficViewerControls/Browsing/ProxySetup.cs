using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TrafficViewerSDK;
using TrafficServer;
using TrafficViewerControls.Properties;
using TrafficViewerSDK.Http;
using TrafficViewerInstance;
using System.Net;
using System.Net.Security;
using CommonControls;

namespace TrafficViewerControls.Browsing
{
    public partial class ProxySetup : Form
    {
        IHttpProxy _proxy;

        public ProxySetup(IHttpProxy proxy)
        {
            _proxy = proxy;

            InitializeComponent();

        }



        private void _buttonStart_Click(object sender, EventArgs e)
        {
            _proxy.Host = _hostBox.Text;
            int port = 0;
            if (!int.TryParse(_portBox.Text, out port))
            {
                ErrorBox.ShowDialog("Invalid port");
                return;
            }
            else
            {
                _proxy.Port = port;
            }



            int securePort = 0;
            if (!int.TryParse(_securePort.Text, out securePort))
            {
                ErrorBox.ShowDialog("Invalid port");
                return;
            }
            else
            {
                _proxy.SecurePort = securePort;
            }

            IEnumerable<string> extraOptions = _extraOptionsGrid.GetValues();
            foreach (string line in extraOptions)
            {
                string[] keyValPair = line.Split(Constants.VALUES_SEPARATOR.ToCharArray());
                if (keyValPair.Length == 2)
                {
                    if (_proxy.ExtraOptions.ContainsKey(keyValPair[0]))
                    {
                        _proxy.ExtraOptions[keyValPair[0]] = keyValPair[1];
                    }
                    else
                    {
                        _proxy.ExtraOptions.Add(keyValPair[0], keyValPair[1]);
                    }
                }
            }

            if (_proxy.IsListening)
            {
                _proxy.Stop();
                _proxy.Start();
            }

            this.Hide();
        }

        private void ProxySetup_Load(object sender, EventArgs e)
        {
            string strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            _hostBox.Items.Add("127.0.0.1");
            if (addr != null && addr.Length > 0)
            {
                foreach (IPAddress ip in addr)
                {
                    string ipString = ip.ToString();
                    if (!_hostBox.Items.Contains(ipString))
                    {
                        _hostBox.Items.Add(ipString);
                    }
                }
            }
            _hostBox.Text = _proxy.Host;
            _portBox.Text = _proxy.Port.ToString();
            _securePort.Text = _proxy.SecurePort.ToString();

            List<string> extraOptions = new List<string>();
            foreach (string key in _proxy.ExtraOptions.Keys)
            { 
                extraOptions.Add(String.Format("{0}{1}{2}",key,Constants.VALUES_SEPARATOR,_proxy.ExtraOptions[key]));
            }
            _extraOptionsGrid.SetValues(extraOptions);
        }

     
    }
}
