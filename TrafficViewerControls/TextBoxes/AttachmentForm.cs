using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using TrafficViewerControls.Properties;
using CommonControls;

namespace TrafficViewerControls.TextBoxes
{
    public partial class AttachmentForm : Form
    {
        public AttachmentForm()
        {
            InitializeComponent();
        }

        private string _value;
        /// <summary>
        /// gets the value of the attachment
        /// </summary>
        public string Value
        {
            get { return _value; }
        }


        private void button1_Click(object sender, EventArgs e)
        {

            if (File.Exists(_fileSelector.Text))
            {
                if (_checkEncode.Checked)
                {
                    _value = Convert.ToBase64String(File.ReadAllBytes(_fileSelector.Text));
                }
                else
                {
                    _value = File.ReadAllText(_fileSelector.Text);
                }
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            else
            {
                ErrorBox.ShowDialog(Resources.InvalidFilePath);
            }
        }
    }
}
