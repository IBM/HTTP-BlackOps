using CommonControls;
namespace TrafficViewerControls
{
	partial class OptionsEditor
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsEditor));
            this._buttonOk = new System.Windows.Forms.Button();
            this._buttonCancel = new System.Windows.Forms.Button();
            this._tabControl = new System.Windows.Forms.TabControl();
            this._tabTrafficServer = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this._numServerSecurePort = new System.Windows.Forms.NumericUpDown();
            this.label18 = new System.Windows.Forms.Label();
            this._textForwardingHost = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this._numForwardingPort = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this._boxIp = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._numProxyPort = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this._boxProxyCertPass = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this._proxyCertificate = new TrafficViewerControls.FileSelector();
            this.label23 = new System.Windows.Forms.Label();
            this._gridDynElems = new CommonControls.OptionsGrid();
            this._tabHttpPage = new System.Windows.Forms.TabPage();
            this._textRequestDelayFilter = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this._numericRequestDelay = new System.Windows.Forms.NumericUpDown();
            this.label21 = new System.Windows.Forms.Label();
            this._checkIgnoreInvalidCert = new System.Windows.Forms.CheckBox();
            this._boxTimeout = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this._checkUseProxy = new System.Windows.Forms.CheckBox();
            this._boxProxyHost = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this._boxProxyPort = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this._boxHttpClient = new System.Windows.Forms.ComboBox();
            this._tabHttpVariables = new System.Windows.Forms.TabPage();
            this._gridSessionIds = new CommonControls.OptionsGrid();
            this._gridResponsePatterns = new CommonControls.OptionsGrid();
            this._gridVarDefs = new CommonControls.OptionsGrid();
            this._tabGeneral = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this._checkPrompt = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this._boxStartupParser = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this._boxStartupProfile = new System.Windows.Forms.ComboBox();
            this._fileAutoLoad = new TrafficViewerControls.FileSelector();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this._checkOptSpeed = new System.Windows.Forms.RadioButton();
            this._checkOptMemory = new System.Windows.Forms.RadioButton();
            this._tabGui = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this._swatchHighlightColor = new TrafficViewerControls.Configuration.ColorSwatch();
            this._swatchDiffColor = new TrafficViewerControls.Configuration.ColorSwatch();
            this._swatchTextColor = new TrafficViewerControls.Configuration.ColorSwatch();
            this._swatchBackground = new TrafficViewerControls.Configuration.ColorSwatch();
            this._tabExtensions = new System.Windows.Forms.TabPage();
            this._extensionGrid = new CommonControls.OptionsGrid();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this._dialogOpenFile = new System.Windows.Forms.OpenFileDialog();
            this._saveCertDialog = new System.Windows.Forms.SaveFileDialog();
            this._tabControl.SuspendLayout();
            this._tabTrafficServer.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._numServerSecurePort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._numForwardingPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._numProxyPort)).BeginInit();
            this.groupBox3.SuspendLayout();
            this._tabHttpPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._numericRequestDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._boxTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._boxProxyPort)).BeginInit();
            this._tabHttpVariables.SuspendLayout();
            this._tabGeneral.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this._tabGui.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this._tabExtensions.SuspendLayout();
            this.SuspendLayout();
            // 
            // _buttonOk
            // 
            resources.ApplyResources(this._buttonOk, "_buttonOk");
            this._buttonOk.Name = "_buttonOk";
            this._buttonOk.UseVisualStyleBackColor = true;
            this._buttonOk.Click += new System.EventHandler(this.OkClick);
            // 
            // _buttonCancel
            // 
            resources.ApplyResources(this._buttonCancel, "_buttonCancel");
            this._buttonCancel.Name = "_buttonCancel";
            this._buttonCancel.UseVisualStyleBackColor = true;
            this._buttonCancel.Click += new System.EventHandler(this.CancelClick);
            // 
            // _tabControl
            // 
            resources.ApplyResources(this._tabControl, "_tabControl");
            this._tabControl.Controls.Add(this._tabTrafficServer);
            this._tabControl.Controls.Add(this._tabHttpPage);
            this._tabControl.Controls.Add(this._tabHttpVariables);
            this._tabControl.Controls.Add(this._tabGeneral);
            this._tabControl.Controls.Add(this._tabGui);
            this._tabControl.Controls.Add(this._tabExtensions);
            this._tabControl.Name = "_tabControl";
            this._tabControl.SelectedIndex = 0;
            // 
            // _tabTrafficServer
            // 
            this._tabTrafficServer.Controls.Add(this.groupBox4);
            this._tabTrafficServer.Controls.Add(this.groupBox3);
            this._tabTrafficServer.Controls.Add(this.label23);
            this._tabTrafficServer.Controls.Add(this._gridDynElems);
            resources.ApplyResources(this._tabTrafficServer, "_tabTrafficServer");
            this._tabTrafficServer.Name = "_tabTrafficServer";
            this._tabTrafficServer.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this._numServerSecurePort);
            this.groupBox4.Controls.Add(this.label18);
            this.groupBox4.Controls.Add(this._textForwardingHost);
            this.groupBox4.Controls.Add(this.label16);
            this.groupBox4.Controls.Add(this._numForwardingPort);
            this.groupBox4.Controls.Add(this.label17);
            this.groupBox4.Controls.Add(this._boxIp);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this._numProxyPort);
            this.groupBox4.Controls.Add(this.label2);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // _numServerSecurePort
            // 
            resources.ApplyResources(this._numServerSecurePort, "_numServerSecurePort");
            this._numServerSecurePort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this._numServerSecurePort.Name = "_numServerSecurePort";
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.Name = "label18";
            // 
            // _textForwardingHost
            // 
            resources.ApplyResources(this._textForwardingHost, "_textForwardingHost");
            this._textForwardingHost.Name = "_textForwardingHost";
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            // 
            // _numForwardingPort
            // 
            resources.ApplyResources(this._numForwardingPort, "_numForwardingPort");
            this._numForwardingPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this._numForwardingPort.Name = "_numForwardingPort";
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.Name = "label17";
            // 
            // _boxIp
            // 
            resources.ApplyResources(this._boxIp, "_boxIp");
            this._boxIp.Name = "_boxIp";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // _numProxyPort
            // 
            resources.ApplyResources(this._numProxyPort, "_numProxyPort");
            this._numProxyPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this._numProxyPort.Name = "_numProxyPort";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button2);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this._boxProxyCertPass);
            this.groupBox3.Controls.Add(this.label20);
            this.groupBox3.Controls.Add(this._proxyCertificate);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // button2
            // 
            this.button2.Image = global::TrafficViewerControls.Properties.Resources.Save_6530;
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.SaveBlackopsCA);
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.ResetBlackopsCA);
            // 
            // _boxProxyCertPass
            // 
            resources.ApplyResources(this._boxProxyCertPass, "_boxProxyCertPass");
            this._boxProxyCertPass.Name = "_boxProxyCertPass";
            // 
            // label20
            // 
            resources.ApplyResources(this.label20, "label20");
            this.label20.Name = "label20";
            // 
            // _proxyCertificate
            // 
            this._proxyCertificate.BackColor = System.Drawing.Color.Transparent;
            this._proxyCertificate.CheckFileExists = true;
            this._proxyCertificate.Filter = "PFX Files|*.pfx";
            this._proxyCertificate.Label = "Default Cert:";
            resources.ApplyResources(this._proxyCertificate, "_proxyCertificate");
            this._proxyCertificate.Name = "_proxyCertificate";
            // 
            // label23
            // 
            resources.ApplyResources(this.label23, "label23");
            this.label23.Name = "label23";
            // 
            // _gridDynElems
            // 
            resources.ApplyResources(this._gridDynElems, "_gridDynElems");
            this._gridDynElems.BackColor = System.Drawing.Color.Transparent;
            this._gridDynElems.Columns = "Element";
            this._gridDynElems.LabelText = "Traffic File Proxy: Dynamic Elements:";
            this._gridDynElems.Name = "_gridDynElems";
            // 
            // _tabHttpPage
            // 
            this._tabHttpPage.Controls.Add(this._textRequestDelayFilter);
            this._tabHttpPage.Controls.Add(this.label22);
            this._tabHttpPage.Controls.Add(this._numericRequestDelay);
            this._tabHttpPage.Controls.Add(this.label21);
            this._tabHttpPage.Controls.Add(this._checkIgnoreInvalidCert);
            this._tabHttpPage.Controls.Add(this._boxTimeout);
            this._tabHttpPage.Controls.Add(this.label13);
            this._tabHttpPage.Controls.Add(this._checkUseProxy);
            this._tabHttpPage.Controls.Add(this._boxProxyHost);
            this._tabHttpPage.Controls.Add(this.label11);
            this._tabHttpPage.Controls.Add(this._boxProxyPort);
            this._tabHttpPage.Controls.Add(this.label12);
            this._tabHttpPage.Controls.Add(this.label10);
            this._tabHttpPage.Controls.Add(this._boxHttpClient);
            resources.ApplyResources(this._tabHttpPage, "_tabHttpPage");
            this._tabHttpPage.Name = "_tabHttpPage";
            this._tabHttpPage.UseVisualStyleBackColor = true;
            // 
            // _textRequestDelayFilter
            // 
            resources.ApplyResources(this._textRequestDelayFilter, "_textRequestDelayFilter");
            this._textRequestDelayFilter.Name = "_textRequestDelayFilter";
            // 
            // label22
            // 
            resources.ApplyResources(this.label22, "label22");
            this.label22.Name = "label22";
            // 
            // _numericRequestDelay
            // 
            resources.ApplyResources(this._numericRequestDelay, "_numericRequestDelay");
            this._numericRequestDelay.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this._numericRequestDelay.Name = "_numericRequestDelay";
            // 
            // label21
            // 
            resources.ApplyResources(this.label21, "label21");
            this.label21.Name = "label21";
            // 
            // _checkIgnoreInvalidCert
            // 
            resources.ApplyResources(this._checkIgnoreInvalidCert, "_checkIgnoreInvalidCert");
            this._checkIgnoreInvalidCert.Name = "_checkIgnoreInvalidCert";
            this._checkIgnoreInvalidCert.UseVisualStyleBackColor = true;
            // 
            // _boxTimeout
            // 
            resources.ApplyResources(this._boxTimeout, "_boxTimeout");
            this._boxTimeout.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this._boxTimeout.Name = "_boxTimeout";
            this._boxTimeout.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // _checkUseProxy
            // 
            resources.ApplyResources(this._checkUseProxy, "_checkUseProxy");
            this._checkUseProxy.Name = "_checkUseProxy";
            this._checkUseProxy.UseVisualStyleBackColor = true;
            this._checkUseProxy.CheckedChanged += new System.EventHandler(this._checkUseProxy_CheckedChanged);
            // 
            // _boxProxyHost
            // 
            resources.ApplyResources(this._boxProxyHost, "_boxProxyHost");
            this._boxProxyHost.Name = "_boxProxyHost";
            this._boxProxyHost.ReadOnly = true;
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // _boxProxyPort
            // 
            resources.ApplyResources(this._boxProxyPort, "_boxProxyPort");
            this._boxProxyPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this._boxProxyPort.Name = "_boxProxyPort";
            this._boxProxyPort.ReadOnly = true;
            this._boxProxyPort.Value = new decimal(new int[] {
            8080,
            0,
            0,
            0});
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // _boxHttpClient
            // 
            this._boxHttpClient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._boxHttpClient.FormattingEnabled = true;
            resources.ApplyResources(this._boxHttpClient, "_boxHttpClient");
            this._boxHttpClient.Name = "_boxHttpClient";
            // 
            // _tabHttpVariables
            // 
            this._tabHttpVariables.Controls.Add(this._gridSessionIds);
            this._tabHttpVariables.Controls.Add(this._gridResponsePatterns);
            this._tabHttpVariables.Controls.Add(this._gridVarDefs);
            resources.ApplyResources(this._tabHttpVariables, "_tabHttpVariables");
            this._tabHttpVariables.Name = "_tabHttpVariables";
            this._tabHttpVariables.UseVisualStyleBackColor = true;
            // 
            // _gridSessionIds
            // 
            this._gridSessionIds.BackColor = System.Drawing.Color.Transparent;
            this._gridSessionIds.Columns = "Name regular expression";
            this._gridSessionIds.LabelText = "Session id parameter names:";
            resources.ApplyResources(this._gridSessionIds, "_gridSessionIds");
            this._gridSessionIds.Name = "_gridSessionIds";
            // 
            // _gridResponsePatterns
            // 
            this._gridResponsePatterns.BackColor = System.Drawing.Color.Transparent;
            this._gridResponsePatterns.Columns = "Pattern";
            this._gridResponsePatterns.LabelText = "Parameters response patterns (Used to update parameter values):";
            resources.ApplyResources(this._gridResponsePatterns, "_gridResponsePatterns");
            this._gridResponsePatterns.Name = "_gridResponsePatterns";
            // 
            // _gridVarDefs
            // 
            resources.ApplyResources(this._gridVarDefs, "_gridVarDefs");
            this._gridVarDefs.BackColor = System.Drawing.Color.Transparent;
            this._gridVarDefs.Columns = "Name\r\nLocation:Path,Query,Cookies,Body\r\nDefinition";
            this._gridVarDefs.LabelText = "Variable definitions:";
            this._gridVarDefs.Name = "_gridVarDefs";
            // 
            // _tabGeneral
            // 
            this._tabGeneral.Controls.Add(this.groupBox6);
            this._tabGeneral.Controls.Add(this.groupBox1);
            this._tabGeneral.Controls.Add(this.groupBox2);
            resources.ApplyResources(this._tabGeneral, "_tabGeneral");
            this._tabGeneral.Name = "_tabGeneral";
            this._tabGeneral.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this._checkPrompt);
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // _checkPrompt
            // 
            resources.ApplyResources(this._checkPrompt, "_checkPrompt");
            this._checkPrompt.Name = "_checkPrompt";
            this._checkPrompt.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this._boxStartupParser);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this._boxStartupProfile);
            this.groupBox1.Controls.Add(this._fileAutoLoad);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // _boxStartupParser
            // 
            this._boxStartupParser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._boxStartupParser.FormattingEnabled = true;
            resources.ApplyResources(this._boxStartupParser, "_boxStartupParser");
            this._boxStartupParser.Name = "_boxStartupParser";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // _boxStartupProfile
            // 
            this._boxStartupProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._boxStartupProfile.FormattingEnabled = true;
            resources.ApplyResources(this._boxStartupProfile, "_boxStartupProfile");
            this._boxStartupProfile.Name = "_boxStartupProfile";
            // 
            // _fileAutoLoad
            // 
            this._fileAutoLoad.BackColor = System.Drawing.Color.Transparent;
            this._fileAutoLoad.CheckFileExists = true;
            this._fileAutoLoad.Filter = "";
            this._fileAutoLoad.Label = "Location:";
            resources.ApplyResources(this._fileAutoLoad, "_fileAutoLoad");
            this._fileAutoLoad.Name = "_fileAutoLoad";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this._checkOptSpeed);
            this.groupBox2.Controls.Add(this._checkOptMemory);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // _checkOptSpeed
            // 
            resources.ApplyResources(this._checkOptSpeed, "_checkOptSpeed");
            this._checkOptSpeed.Name = "_checkOptSpeed";
            this._checkOptSpeed.UseVisualStyleBackColor = true;
            this._checkOptSpeed.CheckedChanged += new System.EventHandler(this.OptSpeedCheckedChanged);
            // 
            // _checkOptMemory
            // 
            resources.ApplyResources(this._checkOptMemory, "_checkOptMemory");
            this._checkOptMemory.Checked = true;
            this._checkOptMemory.Name = "_checkOptMemory";
            this._checkOptMemory.TabStop = true;
            this._checkOptMemory.UseVisualStyleBackColor = true;
            this._checkOptMemory.CheckedChanged += new System.EventHandler(this.OptMemoryCheckedChanged);
            // 
            // _tabGui
            // 
            this._tabGui.Controls.Add(this.groupBox5);
            resources.ApplyResources(this._tabGui, "_tabGui");
            this._tabGui.Name = "_tabGui";
            this._tabGui.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this._swatchHighlightColor);
            this.groupBox5.Controls.Add(this._swatchDiffColor);
            this.groupBox5.Controls.Add(this._swatchTextColor);
            this.groupBox5.Controls.Add(this._swatchBackground);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // _swatchHighlightColor
            // 
            this._swatchHighlightColor.Color = System.Drawing.Color.Yellow;
            resources.ApplyResources(this._swatchHighlightColor, "_swatchHighlightColor");
            this._swatchHighlightColor.Name = "_swatchHighlightColor";
            // 
            // _swatchDiffColor
            // 
            this._swatchDiffColor.Color = System.Drawing.Color.Red;
            resources.ApplyResources(this._swatchDiffColor, "_swatchDiffColor");
            this._swatchDiffColor.Name = "_swatchDiffColor";
            // 
            // _swatchTextColor
            // 
            this._swatchTextColor.Color = System.Drawing.Color.Black;
            resources.ApplyResources(this._swatchTextColor, "_swatchTextColor");
            this._swatchTextColor.Name = "_swatchTextColor";
            // 
            // _swatchBackground
            // 
            this._swatchBackground.Color = System.Drawing.Color.LightYellow;
            resources.ApplyResources(this._swatchBackground, "_swatchBackground");
            this._swatchBackground.Name = "_swatchBackground";
            // 
            // _tabExtensions
            // 
            this._tabExtensions.Controls.Add(this._extensionGrid);
            resources.ApplyResources(this._tabExtensions, "_tabExtensions");
            this._tabExtensions.Name = "_tabExtensions";
            this._tabExtensions.UseVisualStyleBackColor = true;
            // 
            // _extensionGrid
            // 
            this._extensionGrid.BackColor = System.Drawing.Color.Transparent;
            this._extensionGrid.Columns = "Function:Disabled,Exploiter,TrafficParser,TrafficExporter,AnalysisModule,HttpClie" +
    "ntFactory,HttpProxyFactory\r\nDll Path";
            this._extensionGrid.LabelText = "Traffic Viewer extensions:";
            resources.ApplyResources(this._extensionGrid, "_extensionGrid");
            this._extensionGrid.Name = "_extensionGrid";
            this._extensionGrid.AddClick += new System.EventHandler(this.AnalysisModulesGridAddClick);
            // 
            // radioButton1
            // 
            resources.ApplyResources(this.radioButton1, "radioButton1");
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            resources.ApplyResources(this.radioButton2, "radioButton2");
            this.radioButton2.Checked = true;
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.TabStop = true;
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // _dialogOpenFile
            // 
            resources.ApplyResources(this._dialogOpenFile, "_dialogOpenFile");
            // 
            // _saveCertDialog
            // 
            this._saveCertDialog.DefaultExt = "cer";
            this._saveCertDialog.FileName = "BlackopsCA.cer";
            resources.ApplyResources(this._saveCertDialog, "_saveCertDialog");
            // 
            // OptionsEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._tabControl);
            this.Controls.Add(this._buttonCancel);
            this.Controls.Add(this._buttonOk);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsEditor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OptionsEditorFormClosing);
            this.Load += new System.EventHandler(this.OptionsEditorLoad);
            this._tabControl.ResumeLayout(false);
            this._tabTrafficServer.ResumeLayout(false);
            this._tabTrafficServer.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._numServerSecurePort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._numForwardingPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._numProxyPort)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this._tabHttpPage.ResumeLayout(false);
            this._tabHttpPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._numericRequestDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._boxTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._boxProxyPort)).EndInit();
            this._tabHttpVariables.ResumeLayout(false);
            this._tabHttpVariables.PerformLayout();
            this._tabGeneral.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this._tabGui.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this._tabExtensions.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button _buttonOk;
		private System.Windows.Forms.Button _buttonCancel;
		private System.Windows.Forms.TabControl _tabControl;
		private System.Windows.Forms.TabPage _tabGeneral;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton _checkOptMemory;
		private System.Windows.Forms.RadioButton _checkOptSpeed;
		private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.GroupBox groupBox1;
		private FileSelector _fileAutoLoad;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox _boxStartupProfile;
		private System.Windows.Forms.TabPage _tabGui;
		private System.Windows.Forms.TabPage _tabExtensions;
		private System.Windows.Forms.GroupBox groupBox5;
		private TrafficViewerControls.Configuration.ColorSwatch _swatchBackground;
		private TrafficViewerControls.Configuration.ColorSwatch _swatchTextColor;
		private TrafficViewerControls.Configuration.ColorSwatch _swatchDiffColor;
		private TrafficViewerControls.Configuration.ColorSwatch _swatchHighlightColor;
		private OptionsGrid _extensionGrid;
		private System.Windows.Forms.OpenFileDialog _dialogOpenFile;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.CheckBox _checkPrompt;
		private System.Windows.Forms.TabPage _tabTrafficServer;
		private System.Windows.Forms.TabPage _tabHttpVariables;
        private OptionsGrid _gridVarDefs;
        private OptionsGrid _gridDynElems;
		private OptionsGrid _gridSessionIds;
		private OptionsGrid _gridResponsePatterns;
		private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox _boxStartupParser;
        private System.Windows.Forms.TabPage _tabHttpPage;
        private System.Windows.Forms.NumericUpDown _boxTimeout;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox _checkUseProxy;
        private System.Windows.Forms.TextBox _boxProxyHost;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown _boxProxyPort;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox _boxHttpClient;
        private System.Windows.Forms.CheckBox _checkIgnoreInvalidCert;
		private System.Windows.Forms.NumericUpDown _numericRequestDelay;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.TextBox _textRequestDelayFilter;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox _boxProxyCertPass;
        private System.Windows.Forms.Label label20;
        private FileSelector _proxyCertificate;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.NumericUpDown _numServerSecurePort;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox _textForwardingHost;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.NumericUpDown _numForwardingPort;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox _boxIp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown _numProxyPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.SaveFileDialog _saveCertDialog;
	}
}