/******************************************************************
* IBM Confidential
* OCO Source Materials
* IBM Rational Traffic Viewer
* (c) Copyright IBM Corp. 2010 All Rights Reserved.
* 
* The source code for this program is not published or otherwise
* divested of its trade secrets, irrespective of what has been
* deposited with the U.S. Copyright Office.
******************************************************************/
namespace TrafficViewerControls
{
	partial class ImportFileForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportFileForm));
			this._buttonImport = new System.Windows.Forms.Button();
			this._buttonCancelDialog = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this._boxTargetPath = new System.Windows.Forms.TextBox();
			this._buttonBrowseToTarget = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this._checkAutoLoad = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label9 = new System.Windows.Forms.Label();
			this._checkAppendToFile = new System.Windows.Forms.CheckBox();
			this._buttonEditParserProfile = new System.Windows.Forms.Button();
			this.label8 = new System.Windows.Forms.Label();
			this._checkExclusions = new System.Windows.Forms.CheckBox();
			this.label7 = new System.Windows.Forms.Label();
			this._buttonRemoveParserProfile = new System.Windows.Forms.Button();
			this._buttonAddParserProfile = new System.Windows.Forms.Button();
			this._boxParserProfile = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this._boxParserDll = new System.Windows.Forms.ComboBox();
			this._buttonRemoveParserDll = new System.Windows.Forms.Button();
			this._buttonAddParserDll = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this._dialogSelectFile = new System.Windows.Forms.OpenFileDialog();
			this._dialogSelectParser = new System.Windows.Forms.OpenFileDialog();
			this._dialogSelectProfile = new System.Windows.Forms.OpenFileDialog();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// _buttonImport
			// 
			resources.ApplyResources(this._buttonImport, "_buttonImport");
			this._buttonImport.Name = "_buttonImport";
			this._buttonImport.UseVisualStyleBackColor = true;
			this._buttonImport.Click += new System.EventHandler(this.ImportClick);
			// 
			// _buttonCancelDialog
			// 
			resources.ApplyResources(this._buttonCancelDialog, "_buttonCancelDialog");
			this._buttonCancelDialog.Name = "_buttonCancelDialog";
			this._buttonCancelDialog.UseVisualStyleBackColor = true;
			this._buttonCancelDialog.Click += new System.EventHandler(this.CancelClick);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// _boxTargetPath
			// 
			this._boxTargetPath.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this._boxTargetPath.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.RecentlyUsedList;
			resources.ApplyResources(this._boxTargetPath, "_boxTargetPath");
			this._boxTargetPath.Name = "_boxTargetPath";
			// 
			// _buttonBrowseToTarget
			// 
			resources.ApplyResources(this._buttonBrowseToTarget, "_buttonBrowseToTarget");
			this._buttonBrowseToTarget.Name = "_buttonBrowseToTarget";
			this._buttonBrowseToTarget.UseVisualStyleBackColor = true;
			this._buttonBrowseToTarget.Click += new System.EventHandler(this.BrowseToTargetClick);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this._checkAutoLoad);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this._boxTargetPath);
			this.groupBox1.Controls.Add(this._buttonBrowseToTarget);
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// _checkAutoLoad
			// 
			resources.ApplyResources(this._checkAutoLoad, "_checkAutoLoad");
			this._checkAutoLoad.Name = "_checkAutoLoad";
			this._checkAutoLoad.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this._checkAppendToFile);
			this.groupBox2.Controls.Add(this._buttonEditParserProfile);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this._checkExclusions);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this._buttonRemoveParserProfile);
			this.groupBox2.Controls.Add(this._buttonAddParserProfile);
			this.groupBox2.Controls.Add(this._boxParserProfile);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this._boxParserDll);
			this.groupBox2.Controls.Add(this._buttonRemoveParserDll);
			this.groupBox2.Controls.Add(this._buttonAddParserDll);
			this.groupBox2.Controls.Add(this.label2);
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// label9
			// 
			resources.ApplyResources(this.label9, "label9");
			this.label9.Name = "label9";
			// 
			// _checkAppendToFile
			// 
			resources.ApplyResources(this._checkAppendToFile, "_checkAppendToFile");
			this._checkAppendToFile.Name = "_checkAppendToFile";
			this._checkAppendToFile.UseVisualStyleBackColor = true;
			// 
			// _buttonEditParserProfile
			// 
			resources.ApplyResources(this._buttonEditParserProfile, "_buttonEditParserProfile");
			this._buttonEditParserProfile.Name = "_buttonEditParserProfile";
			this._buttonEditParserProfile.UseVisualStyleBackColor = true;
			this._buttonEditParserProfile.Click += new System.EventHandler(this.EditParserProfileClick);
			// 
			// label8
			// 
			resources.ApplyResources(this.label8, "label8");
			this.label8.Name = "label8";
			// 
			// _checkExclusions
			// 
			resources.ApplyResources(this._checkExclusions, "_checkExclusions");
			this._checkExclusions.Checked = true;
			this._checkExclusions.CheckState = System.Windows.Forms.CheckState.Checked;
			this._checkExclusions.Name = "_checkExclusions";
			this._checkExclusions.UseVisualStyleBackColor = true;
			// 
			// label7
			// 
			resources.ApplyResources(this.label7, "label7");
			this.label7.Name = "label7";
			// 
			// _buttonRemoveParserProfile
			// 
			resources.ApplyResources(this._buttonRemoveParserProfile, "_buttonRemoveParserProfile");
			this._buttonRemoveParserProfile.Name = "_buttonRemoveParserProfile";
			this._buttonRemoveParserProfile.UseVisualStyleBackColor = true;
			this._buttonRemoveParserProfile.Click += new System.EventHandler(this.RemoveProfileDllClick);
			// 
			// _buttonAddParserProfile
			// 
			resources.ApplyResources(this._buttonAddParserProfile, "_buttonAddParserProfile");
			this._buttonAddParserProfile.Name = "_buttonAddParserProfile";
			this._buttonAddParserProfile.UseVisualStyleBackColor = true;
			this._buttonAddParserProfile.Click += new System.EventHandler(this.AddParserProfileClick);
			// 
			// _boxParserProfile
			// 
			this._boxParserProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._boxParserProfile.FormattingEnabled = true;
			resources.ApplyResources(this._boxParserProfile, "_boxParserProfile");
			this._boxParserProfile.Name = "_boxParserProfile";
			this._boxParserProfile.SelectedIndexChanged += new System.EventHandler(this.ParserProfileSelectedIndexChanged);
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.Name = "label6";
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// _boxParserDll
			// 
			this._boxParserDll.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._boxParserDll.FormattingEnabled = true;
			resources.ApplyResources(this._boxParserDll, "_boxParserDll");
			this._boxParserDll.Name = "_boxParserDll";
			// 
			// _buttonRemoveParserDll
			// 
			resources.ApplyResources(this._buttonRemoveParserDll, "_buttonRemoveParserDll");
			this._buttonRemoveParserDll.Name = "_buttonRemoveParserDll";
			this._buttonRemoveParserDll.UseVisualStyleBackColor = true;
			this._buttonRemoveParserDll.Click += new System.EventHandler(this.RemoveParserDllClick);
			// 
			// _buttonAddParserDll
			// 
			resources.ApplyResources(this._buttonAddParserDll, "_buttonAddParserDll");
			this._buttonAddParserDll.Name = "_buttonAddParserDll";
			this._buttonAddParserDll.UseVisualStyleBackColor = true;
			this._buttonAddParserDll.Click += new System.EventHandler(this.AddParserDllClick);
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// _dialogSelectFile
			// 
			resources.ApplyResources(this._dialogSelectFile, "_dialogSelectFile");
			this._dialogSelectFile.RestoreDirectory = true;
			// 
			// _dialogSelectParser
			// 
			resources.ApplyResources(this._dialogSelectParser, "_dialogSelectParser");
			this._dialogSelectParser.RestoreDirectory = true;
			// 
			// _dialogSelectProfile
			// 
			resources.ApplyResources(this._dialogSelectProfile, "_dialogSelectProfile");
			this._dialogSelectProfile.RestoreDirectory = true;
			// 
			// ImportFileForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this._buttonCancelDialog);
			this.Controls.Add(this._buttonImport);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ImportFileForm";
			this.ShowInTaskbar = false;
			this.Load += new System.EventHandler(this.FileImportFormLoad);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button _buttonImport;
		private System.Windows.Forms.Button _buttonCancelDialog;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox _boxTargetPath;
		private System.Windows.Forms.Button _buttonBrowseToTarget;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox _checkAutoLoad;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ComboBox _boxParserDll;
		private System.Windows.Forms.Button _buttonRemoveParserDll;
		private System.Windows.Forms.Button _buttonAddParserDll;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button _buttonRemoveParserProfile;
		private System.Windows.Forms.Button _buttonAddParserProfile;
		private System.Windows.Forms.ComboBox _boxParserProfile;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.CheckBox _checkExclusions;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Button _buttonEditParserProfile;
		private System.Windows.Forms.OpenFileDialog _dialogSelectFile;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.CheckBox _checkAppendToFile;
		private System.Windows.Forms.OpenFileDialog _dialogSelectParser;
		private System.Windows.Forms.OpenFileDialog _dialogSelectProfile;
	}
}