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
    public partial class TrapsSetup : Form
    {
       

        public TrapsSetup()
        {
            InitializeComponent();
        }

        private void CancelClick(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void TrapsSetup_Load(object sender, EventArgs e)
        {
            List<string> trapVals = new List<string>();
            foreach (HttpTrapDef def in HttpTrap.Instance.TrapDefs)
            {
                trapVals.Add(def.ToString());
            }
            _trapsGrid.SetValues(trapVals);
        }

        private void ApplyClick(object sender, EventArgs e)
        {
            TrafficViewerOptions.Instance.SetMultiValueOption("Traps", _trapsGrid.GetValues());
            HttpTrap.Instance.TrapDefs = TrafficViewerOptions.Instance.GetTraps();
            this.Hide();
        }


     
    }
}
