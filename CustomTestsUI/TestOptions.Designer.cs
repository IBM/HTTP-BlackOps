namespace CustomTestsUI
{
    partial class TestOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestOptions));
            this._checkLoginBeforeEachTest = new System.Windows.Forms.CheckBox();
            this._checkTestOnlyParameters = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this._textNumThreads = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this._textPatternOfFirstTestRequest = new System.Windows.Forms.TextBox();
            this._labelPatternOfFirstRequest = new System.Windows.Forms.Label();
            this._textPatternEntityExclusion = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._textPatternRequestExclusion = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this._checkGenerateAllEncodings = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _checkLoginBeforeEachTest
            // 
            this._checkLoginBeforeEachTest.AutoSize = true;
            this._checkLoginBeforeEachTest.Location = new System.Drawing.Point(26, 27);
            this._checkLoginBeforeEachTest.Name = "_checkLoginBeforeEachTest";
            this._checkLoginBeforeEachTest.Size = new System.Drawing.Size(209, 17);
            this._checkLoginBeforeEachTest.TabIndex = 0;
            this._checkLoginBeforeEachTest.Text = "Login before each test (AppScan Only)";
            this._checkLoginBeforeEachTest.UseVisualStyleBackColor = true;
            // 
            // _checkTestOnlyParameters
            // 
            this._checkTestOnlyParameters.AutoSize = true;
            this._checkTestOnlyParameters.Location = new System.Drawing.Point(26, 112);
            this._checkTestOnlyParameters.Name = "_checkTestOnlyParameters";
            this._checkTestOnlyParameters.Size = new System.Drawing.Size(173, 17);
            this._checkTestOnlyParameters.TabIndex = 1;
            this._checkTestOnlyParameters.Text = "Test only parameters && cookies";
            this._checkTestOnlyParameters.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 203);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Number of threads";
            // 
            // _textNumThreads
            // 
            this._textNumThreads.Location = new System.Drawing.Point(187, 200);
            this._textNumThreads.Name = "_textNumThreads";
            this._textNumThreads.Size = new System.Drawing.Size(109, 20);
            this._textNumThreads.TabIndex = 3;
            this._textNumThreads.Text = "10";
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button1.Location = new System.Drawing.Point(75, 380);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OKClick);
            // 
            // button2
            // 
            this.button2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button2.Location = new System.Drawing.Point(156, 380);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.CancelClick);
            // 
            // _textPatternOfFirstTestRequest
            // 
            this._textPatternOfFirstTestRequest.Location = new System.Drawing.Point(26, 330);
            this._textPatternOfFirstTestRequest.Name = "_textPatternOfFirstTestRequest";
            this._textPatternOfFirstTestRequest.Size = new System.Drawing.Size(270, 20);
            this._textPatternOfFirstTestRequest.TabIndex = 9;
            this._textPatternOfFirstTestRequest.Text = ".*";
            // 
            // _labelPatternOfFirstRequest
            // 
            this._labelPatternOfFirstRequest.AutoSize = true;
            this._labelPatternOfFirstRequest.Location = new System.Drawing.Point(23, 303);
            this._labelPatternOfFirstRequest.Name = "_labelPatternOfFirstRequest";
            this._labelPatternOfFirstRequest.Size = new System.Drawing.Size(264, 13);
            this._labelPatternOfFirstRequest.TabIndex = 8;
            this._labelPatternOfFirstRequest.Text = "Pattern of first request to start testing (sequential proxy)";
            // 
            // _textPatternEntityExclusion
            // 
            this._textPatternEntityExclusion.Location = new System.Drawing.Point(26, 174);
            this._textPatternEntityExclusion.Name = "_textPatternEntityExclusion";
            this._textPatternEntityExclusion.Size = new System.Drawing.Size(270, 20);
            this._textPatternEntityExclusion.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 149);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(242, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Do not test entities matching the following pattern:";
            // 
            // _textPatternRequestExclusion
            // 
            this._textPatternRequestExclusion.Location = new System.Drawing.Point(26, 266);
            this._textPatternRequestExclusion.Name = "_textPatternRequestExclusion";
            this._textPatternRequestExclusion.Size = new System.Drawing.Size(270, 20);
            this._textPatternRequestExclusion.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 241);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(249, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Do not test requests matching the following pattern:";
            // 
            // _checkGenerateAllEncodings
            // 
            this._checkGenerateAllEncodings.AutoSize = true;
            this._checkGenerateAllEncodings.Location = new System.Drawing.Point(26, 71);
            this._checkGenerateAllEncodings.Name = "_checkGenerateAllEncodings";
            this._checkGenerateAllEncodings.Size = new System.Drawing.Size(137, 17);
            this._checkGenerateAllEncodings.TabIndex = 16;
            this._checkGenerateAllEncodings.Text = "Generate All Encodings";
            this._checkGenerateAllEncodings.UseVisualStyleBackColor = true;
            // 
            // TestOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(321, 415);
            this.Controls.Add(this._checkGenerateAllEncodings);
            this.Controls.Add(this._textPatternRequestExclusion);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._textPatternEntityExclusion);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._textPatternOfFirstTestRequest);
            this.Controls.Add(this._labelPatternOfFirstRequest);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this._textNumThreads);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._checkTestOnlyParameters);
            this.Controls.Add(this._checkLoginBeforeEachTest);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(337, 403);
            this.Name = "TestOptions";
            this.Text = "Test Options";
            this.Load += new System.EventHandler(this.TestOptions_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox _checkLoginBeforeEachTest;
        private System.Windows.Forms.CheckBox _checkTestOnlyParameters;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _textNumThreads;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox _textPatternOfFirstTestRequest;
        private System.Windows.Forms.Label _labelPatternOfFirstRequest;
        private System.Windows.Forms.TextBox _textPatternEntityExclusion;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _textPatternRequestExclusion;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox _checkGenerateAllEncodings;
    }
}