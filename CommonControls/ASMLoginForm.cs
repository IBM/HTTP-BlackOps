using ASMRest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrafficViewerSDK;

namespace CommonControls
{
	public partial class ASMLoginForm : Form
	{
        public ASMLoginForm()
		{
			InitializeComponent();
			
		}

		private void LoginClick(object sender, EventArgs e)
		{
			

			ExecuteLogin();

		}

		private void ExecuteLogin()
		{

            ASMRestSettingsInstance.Instance.HostAndPort = _textHostAndPort.Text;
            ASMRestSettingsInstance.Instance.UserName = _textUserName.Text;
            ASMRestSettingsInstance.Instance.Password = _textPassword.Text;

            try
            {
                BaseRestHttpClient client = new BaseRestHttpClient(ASMRestSettingsInstance.Instance);
                client.Login();
                this.DialogResult = DialogResult.OK;
                this.Hide();

                if (_checkRemember.Checked)
                {
                    ASMLoginOptions options = new ASMLoginOptions();
                    options.Url = _textHostAndPort.Text;
                    options.UserName = _textUserName.Text;
                    options.Password = Encryptor.EncryptToString(_textPassword.Text);
                    options.FullName = ASMRestSettingsInstance.Instance.FullName;
                    options.Save();
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

			
		}

		private void TextKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				ExecuteLogin();
			}
		}

		private void LoginForm_Load(object sender, EventArgs e)
		{
            ASMLoginOptions options = new ASMLoginOptions();

			if (!String.IsNullOrWhiteSpace(options.Password))
			{
				_textHostAndPort.Text = options.Url;
				_textUserName.Text = options.UserName;
                ASMRestSettingsInstance.Instance.FullName = options.FullName;
				_textPassword.Text = Encryptor.DecryptToString(options.Password);
			}

			if (!String.IsNullOrWhiteSpace(_textUserName.Text))
			{
				ExecuteLogin();
			}
		}


	}
}
