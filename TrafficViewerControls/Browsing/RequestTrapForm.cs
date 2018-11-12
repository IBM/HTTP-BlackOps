using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TrafficViewerControls.Properties;

namespace TrafficViewerControls.Browsing
{
    public partial class RequestTrapForm : Form
    {
        /// <summary>
        /// Creates a request trap form
        /// </summary>
        /// <param name="requestLine"></param>
        public RequestTrapForm(string requestLine, bool isResponse)
        {
            InitializeComponent();
			_label.Text = isResponse ? Resources.ResponseWasTrapped : Resources.RequestWasTrapped;
			Text = isResponse ? Resources.ResponseTrappedTitle : Resources.RequestTrappedTitle;
            _textRequestLine.Text = requestLine;
        }

        private void ReleaseClick(object sender, EventArgs e)
        {
            //close the form
            this.Close();
        }

		private void RequestTrapForm_Load(object sender, EventArgs e)
		{
			this.Activate();
		}

		private void _label_Click(object sender, EventArgs e)
		{

		}
    }
}
