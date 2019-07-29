using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Testing;

namespace CustomTestsUI
{
    public partial class TestOptions : Form
    {
        private CustomTestsFile _testFile;
        public TestOptions(CustomTestsFile testFile)
        {
            _testFile = testFile;
            InitializeComponent();
        }

        

        private void TestOptions_Load(object sender, EventArgs e)
        {
            _checkLoginBeforeEachTest.Checked = _testFile.LoginBeforeTests;
            _checkTestOnlyParameters.Checked = _testFile.TestOnlyParameters;
            _checkVerbose.Checked = _testFile.Verbose;
            _textNumThreads.Text = _testFile.NumberOfThreads.ToString();
            _checkGenerateAllEncodings.Checked = _testFile.GenerateAllEncodings;
            _textPatternOfFirstTestRequest.Text = _testFile.PatternOfFirstRequestToTest;
            _textPatternEntityExclusion.Text = _testFile.PatternEntityExclusion;
            _textPatternRequestExclusion.Text = _testFile.PatternRequestExclusion;
            
        }

        private void CancelClick(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void OKClick(object sender, EventArgs e)
        {
            _testFile.LoginBeforeTests = _checkLoginBeforeEachTest.Checked;
            _testFile.TestOnlyParameters = _checkTestOnlyParameters.Checked;
            _testFile.Verbose = _checkVerbose.Checked;
            int numThreads;
            if (int.TryParse(_textNumThreads.Text, out numThreads)) 
            {
                _testFile.NumberOfThreads = numThreads;
            }
            _testFile.PatternEntityExclusion = _textPatternEntityExclusion.Text;
            _testFile.PatternRequestExclusion = _textPatternRequestExclusion.Text;
            _testFile.PatternOfFirstRequestToTest = _textPatternOfFirstTestRequest.Text;
            _testFile.GenerateAllEncodings = _checkGenerateAllEncodings.Checked;
            _testFile.Save();
            this.Hide();
        }

        private void _checkGenerateAllEncodings_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
