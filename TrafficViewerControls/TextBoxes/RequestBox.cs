using CommonControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using TrafficViewerInstance;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;
namespace TrafficViewerControls.TextBoxes
{
	public class RequestBox : TrafficTextBox
	{
		private const int MAX_BYTES = 102400;
		private ToolStripMenuItem _resendHttp;
		private ToolStripMenuItem _resendHttps;
		private ToolStripMenuItem _createTCPConnection;
		private ToolStripMenuItem _resendTCP;
		private ToolStripMenuItem _insertAttachment;
		private ToolStripMenuItem _urlEncodeBeforeSending;
		private object _lock = new object();
		private HttpClientConnection _currentConnection;
		private ByteArrayBuilder _responseBuilder;
		private byte[] _responseBuffer = new byte[102400];
		private GetTCPConnectionDetails _tcpConnForm = new GetTCPConnectionDetails();
		private bool _isReading;
		public event EventHandler RequestResent;
		public event HttpClientRequestCompleteEvent RequestCompleted;

		public override string Text
		{
			get
			{
				return base.Text.Replace("\n", "\r\n");//http specification is CRLF
			}
			set
			{
				base.Text = value;
			}
		}


		private void InitializeComponent()
		{
			ComponentResourceManager resources = new ComponentResourceManager(typeof(RequestBox));
			this._resendHttp = new ToolStripMenuItem();
			this._resendHttps = new ToolStripMenuItem();
			this._createTCPConnection = new ToolStripMenuItem();
			this._resendTCP = new ToolStripMenuItem();
			this._resendTCP.Enabled = false;
			this._urlEncodeBeforeSending = new ToolStripMenuItem();
			this._insertAttachment = new ToolStripMenuItem();
			base.SuspendLayout();
			resources.ApplyResources(this._textBox, "_textBox");
			this._resendHttp.Name = "_resendHttp";
			resources.ApplyResources(this._resendHttp, "_resendHttp");
			this._resendHttp.Click += new EventHandler(this.ResendHttpClick);
			this._resendHttps.Name = "_resendHttps";
			resources.ApplyResources(this._resendHttps, "_resendHttps");
			this._resendHttps.Click += new EventHandler(this.ResendHttpsClick);
			this._createTCPConnection.Name = "_createTCPConnection";
			resources.ApplyResources(this._createTCPConnection, "_createTCPConnection");
			this._createTCPConnection.Click += new EventHandler(this.CreateTCPConnectionClick);
			this._resendTCP.Name = "_resendTCP";
			resources.ApplyResources(this._resendTCP, "_resendTCP");
			this._resendTCP.Click += new EventHandler(this.ResendTCPClick);
			this._urlEncodeBeforeSending.Name = "_urlEncodeBeforeSending";
			resources.ApplyResources(this._urlEncodeBeforeSending, "_urlEncodeBeforeSending");
			this._urlEncodeBeforeSending.Click += new EventHandler(this.UrlEncodeBeforeSendingClick);
			this._insertAttachment.Name = "_insertAttachment";
			resources.ApplyResources(this._insertAttachment, "_insertAttachment");
			this._insertAttachment.Click += new EventHandler(this.InsertAttachmentClick);
			base.AutoScaleMode = AutoScaleMode.Inherit;
			resources.ApplyResources(this, "$this");
			base.Name = "RequestBox";
			base.Controls.SetChildIndex(this._textBox, 0);
			base.ResumeLayout(false);
		}
		private void ResendTCPClick(object sender, EventArgs e)
		{
			try
			{
				if (!this._currentConnection.Stream.CanWrite && !this.ConnectTCP())
				{
					this.RequestCompleted(new HttpClientRequestCompleteEventArgs("Cannot connect to server"));
				}
				else
				{
					if (this.RequestResent != null)
					{
						this.RequestResent(null, null);
					}
					if (this._currentConnection.Stream.CanWrite)
					{
						this._responseBuilder = new ByteArrayBuilder();
						byte[] bytes = Constants.DefaultEncoding.GetBytes(this.Text);
						lock (this._lock)
						{
							this._currentConnection.Stream.Write(bytes, 0, bytes.Length);
							if (this._currentConnection.Stream.CanRead && !this._isReading)
							{
								this._currentConnection.Stream.BeginRead(this._responseBuffer, 0, 102400, new AsyncCallback(this.ReadTCPResponse), this._currentConnection.Stream);
							}
							goto IL_10A;
						}
					}
					this.RequestCompleted(new HttpClientRequestCompleteEventArgs());
				IL_10A: ;
				}
			}
			catch (Exception ex)
			{
				this.RequestCompleted(new HttpClientRequestCompleteEventArgs(ex.Message));
				this._currentConnection.Close();
			}
		}
		private void ReadTCPResponse(IAsyncResult ar)
		{
			try
			{
				lock (this._lock)
				{
					this._isReading = true;
					Stream stream = (Stream)ar.AsyncState;
					int bytesRead = stream.EndRead(ar);
					this._isReading = false;
					if (bytesRead > 0)
					{
						this._responseBuilder.AddChunkReference(this._responseBuffer, bytesRead);
						if (stream.CanRead)
						{
							this._isReading = true;
							stream.BeginRead(this._responseBuffer, 0, 102400, new AsyncCallback(this.ReadTCPResponse), stream);
						}
					}
				}
			}
			catch
			{
			}
			finally
			{
				this.RequestCompleted(new HttpClientRequestCompleteEventArgs(this._responseBuilder.ToArray()));
			}
		}
		private void CreateTCPConnectionClick(object sender, EventArgs e)
		{
			this._tcpConnForm.ShowDialog();
			if (this._tcpConnForm.IsSaved)
			{
				bool connected = this.ConnectTCP();
				if (!connected)
				{
					ErrorBox.ShowDialog(string.Format("Cannot connect to {0}:{1}", this._tcpConnForm.Host, this._tcpConnForm.Port));
					this._currentConnection = null;
				}
				this._resendTCP.Enabled = connected;
			}
		}
		private bool ConnectTCP()
		{
			bool connected = false;
			if (this._currentConnection != null)
			{
				this._currentConnection.Close();
			}
			this._isReading = false;
			int port;
			if (int.TryParse(this._tcpConnForm.Port, out port))
			{
				this._currentConnection = new HttpClientConnection(this._tcpConnForm.Host, port, this._tcpConnForm.IsSecure);
				connected = this._currentConnection.Connect();
			}
			return connected;
		}
		private void UrlEncodeBeforeSendingClick(object sender, EventArgs e)
		{
			this._urlEncodeBeforeSending.Checked = !this._urlEncodeBeforeSending.Checked;
		}
		private void SendRequest(bool https)
		{
			if (this.RequestResent != null)
			{
				this.RequestResent(null, null);
			}
			BackgroundWorker worker = new BackgroundWorker();
			worker.DoWork += new DoWorkEventHandler(this.SendRequestAsync);
			worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.SendCompleted);
			worker.RunWorkerAsync(new object[]
			{
				this.Text,
				https
			});
		}
		private void SendCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (this.RequestCompleted != null)
			{
				if (e.Result == null)
				{
					this.RequestCompleted(new HttpClientRequestCompleteEventArgs());
					return;
				}
				this.RequestCompleted(new HttpClientRequestCompleteEventArgs(e.Result as HttpResponseInfo));
			}
		}
		private void SendRequestAsync(object sender, DoWorkEventArgs e)
		{
			object[] args = e.Argument as object[];
			string text = (string)args[0];

            text = text.Replace(Constants.SEQUENCE_VAR_PATTERN, DateTime.Now.Ticks.ToString());
			

			bool https = (bool)args[1];
			HttpRequestInfo reqInfo = new HttpRequestInfo(text, this._urlEncodeBeforeSending.Checked);
			if (this._urlEncodeBeforeSending.Checked)
			{
				if (reqInfo.BodyVariables != null && reqInfo.BodyVariables.MatchingDefinition.IsRegular && reqInfo.BodyVariables.Count > 0)
				{
					List<string> paramNames = new List<string>(reqInfo.BodyVariables.Keys);
					using (List<string>.Enumerator enumerator = paramNames.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							string name = enumerator.Current;
							string value = Utils.UrlDecode(reqInfo.BodyVariables[name]);
							reqInfo.BodyVariables[name] = Utils.UrlEncode(value);
						}
						goto IL_D9;
					}
				}
				reqInfo.ContentData = HttpUtility.UrlEncodeToBytes(reqInfo.ContentData);
			}
		IL_D9:
			reqInfo.IsSecure = https;
			if (reqInfo.ContentData != null)
			{
				reqInfo.Headers["Content-Length"] = reqInfo.ContentData.Length.ToString();
			}
            HttpResponseInfo respInfo;
            int reqId;
			try
			{
				respInfo = TrafficViewer.Instance.MakeHttpClient().SendRequest(reqInfo);
                reqId = TrafficViewer.Instance.TrafficViewerFile.AddRequestResponse(reqInfo.ToArray(), respInfo.ToArray(), https);
            }
			catch
			{
				respInfo = null;
                reqId = TrafficViewer.Instance.TrafficViewerFile.AddRequestResponse(reqInfo.ToArray(),new byte[0], https);
			}
            e.Result = respInfo;
            
            
		}
		private void ResendHttpClick(object sender, EventArgs e)
		{
			this.SendRequest(false);
		}
		private void ResendHttpsClick(object sender, EventArgs e)
		{
			this.SendRequest(true);
		}
		private void InsertAttachmentClick(object sender, EventArgs e)
		{
			AttachmentForm form = new AttachmentForm();
			if (form.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(form.Value))
			{
				int selectionIndex = this._textBox.SelectionStart;
				this.Text = this.Text.Insert(selectionIndex, form.Value);
			}
		}
		public RequestBox()
		{
			this.InitializeComponent();
			this._contextMenu.Items.Insert(0, new ToolStripSeparator());
			this._contextMenu.Items.Insert(0, this._resendTCP);
			this._contextMenu.Items.Insert(0, this._createTCPConnection);
			this._contextMenu.Items.Insert(0, this._resendHttps);
			this._contextMenu.Items.Insert(0, this._resendHttp);
			this._contextMenu.Items.Insert(0, this._urlEncodeBeforeSending);
			this._contextMenu.Items.Insert(0, this._insertAttachment);
		}
	}
}
