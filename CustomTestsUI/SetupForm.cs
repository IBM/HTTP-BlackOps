using CommonControls;
using CustomTestsUI.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Testing;
using TrafficViewerSDK.Http;

namespace CustomTestsUI
{
    public partial class SetupForm : Form
    {


        private const string CUSTOM_TESTS = "CustomTests";
        private const string DEFAULT_CUSTOM_TESTS_FILE = CUSTOM_TESTS + ".xml";
        private static string _defaultCustomTestsFilePath = String.Empty;
        private CustomTestsFile _testFile;
        public static string GetDefaultCustomTestsFile(string extensionDir)
        {
            return Path.Combine(extensionDir, DEFAULT_CUSTOM_TESTS_FILE);
        }

        private ITestRunner _testRunner;
        private INetworkSettings _networkSettings;

        public SetupForm(ITestRunner testRunner, string workingDir, INetworkSettings networkSettings, bool enableAppScanFeatures = false)
        {
            _testRunner = testRunner;
            _networkSettings = networkSettings;
            _defaultCustomTestsFilePath = GetDefaultCustomTestsFile(workingDir);
            InitializeComponent();

        }



        private void SetupForm_Load(object sender, EventArgs e)
        {
            LoadFile(_defaultCustomTestsFilePath);
            UpdateAttackTargets();
        }

        private void LoadFile(string path)
        {
            bool loaded = true;
            _testFile = new CustomTestsFile();
            CustomTestsFile file = new CustomTestsFile();
            if (File.Exists(path))
            {
                loaded = file.Load(path);
            }


            if (!loaded)
            {
                ErrorBox.ShowDialog("Could not load file");
                return;
            }
            _testFile = file;
            _testFile.SetCustomTests(file.GetCustomTests());
            _grid.SetValues((List<string>)_testFile.GetOption(CUSTOM_TESTS));
            runAutomaticallyToolStripMenuItem.Checked = _testFile.AutoRunTests;
            _testRunner.SetTestFile(_testFile);
        }


        private void SaveCurrent()
        {
            _testFile.SetMultiValueOption(CUSTOM_TESTS, _grid.GetValues());
            _testFile.Save();
        }


        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveCurrent();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = saveFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                _testFile.SetMultiValueOption(CUSTOM_TESTS, _grid.GetValues());
                _testFile.SaveAs(saveFileDialog1.FileName);
            }

        }

        private void saveAsDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _testFile.SetMultiValueOption(CUSTOM_TESTS, _grid.GetValues());
            _testFile.SaveAs(_defaultCustomTestsFilePath);
        }

        private void runAutomaticallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            runAutomaticallyToolStripMenuItem.Checked = !runAutomaticallyToolStripMenuItem.Checked;
            _testFile.AutoRunTests = runAutomaticallyToolStripMenuItem.Checked;
            _testFile.Save();
        }

        private void runToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var attackTargetList = _testFile.GetAttackTargetList(); 
           
            if (attackTargetList.Count == 0) //if still not set
            {
                string pattern = _testRunner.GetPatternOfRequestsToTest();

                if (String.IsNullOrWhiteSpace(pattern))
                {
                    ErrorBox.ShowDialog(Resources.ErrorTestPatternMissing);
                    return;
                }
                else
                { 
                    attackTargetList.Add("Default", new AttackTarget("Default","Enabled", pattern));
                    _testFile.SetAttackTargetList(attackTargetList);
                }
            }

            SaveCurrent();
            StartUI();
            backgroundWorker1.RunWorkerAsync();
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopUI();
            _testRunner.Pause();
        }

        private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopUI();
            _testRunner.Cancel();
        }

        private void StartUI()
        {
            toolStripProgressBar1.Visible = true;
            toolStripStatusLabel1.Visible = true;
            toolStripStatusLabel2.Visible = true;
            fileToolStripMenuItem.Enabled = false;
            _grid.Enabled = false;
            timer1.Start();
        }

        private void StopUI()
        {
            timer1.Stop();
            toolStripProgressBar1.Visible = false;
            toolStripStatusLabel1.Visible = false;
            toolStripStatusLabel2.Visible = false;

            fileToolStripMenuItem.Enabled = true;
            _grid.Enabled = true;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (toolStripProgressBar1.Value < 10)
            {
                toolStripProgressBar1.Value += 1;
            }
            else
            {
                toolStripProgressBar1.Value = 0;
            }
            toolStripStatusLabel2.Text = String.Format("Requests and multi-steps left: {0}", _testRunner.LeftToTest);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            _testRunner.Run();


        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StopUI();
        }

        private void SetupForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = _testRunner.IsRunning;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
        }



        private void actionBasedMultiStepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new ActionBasedMultiStepSetup(_testFile);
            form.ShowDialog();
        }

        private void startTestSequentialProxyToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (_testFile.GetAttackTargetList().Count == 0)
            {
                ErrorBox.ShowDialog(Resources.ErrorTestPatternMissing);
                return;
            }
            Form form = new ProxyForm(_testRunner, _networkSettings, true);
            form.Show(this);
        }


        private void fromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                SaveCurrent();

                LoadFile(openFileDialog1.FileName);

            }
        }

        private void configureToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void driveByAttackProxyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_testFile.GetAttackTargetList().Count == 0)
            {
                ErrorBox.ShowDialog(Resources.ErrorTestPatternMissing);
                return;
            }
            Form form = new ProxyForm(_testRunner, _networkSettings, false);
            form.Show(this);
        }

        private void numberOfThreadsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InputMessageBox box = new InputMessageBox();
            string numThreadsVal = box.ShowDialog("Enter the number of threads", _testFile.NumberOfThreads.ToString());
            int numVal = 10;
            if (!String.IsNullOrEmpty(numThreadsVal) && int.TryParse(numThreadsVal, out numVal))
            {
                _testFile.NumberOfThreads = numVal;
                _testFile.Save();
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form = new TestOptions(_testFile);
            form.ShowDialog();
        }

        private void hTDMultiStepOperationsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void LoadServerTests(object sender, EventArgs e)
        {
            ServerTests form = new ServerTests();
            if (form.ShowDialog() == DialogResult.OK)
            {
                DataGridView serverGrid = form.GetDataGrid();
                foreach (DataGridViewRow row in serverGrid.SelectedRows)
                {
                    if (row.Visible)
                    {
                        _grid.AddRow(row.Cells["_variantName"].Value.ToString(),
                            row.Cells["_issueType"].Value.ToString(),
                            row.Cells["_mutation"].Value.ToString(),
                            row.Cells["_validation"].Value.ToString());
                    }
                }
            }
        }

        private void AttackTargetEdit(object sender, EventArgs e)
        {
            var form = new AttackTargetsSetup(_testFile);
            form.ShowDialog();
            UpdateAttackTargets();
        }

        private void UpdateAttackTargets()
        {
            var targets = _testFile.GetAttackTargetList();
            if (targets.Count > 0)
            {
                string labelVal = String.Empty;
                foreach (var target in targets.Values)
                {
                    if (target.Status == AttackTargetStatus.Enabled)
                    {
                        labelVal += target.Name + ",";
                    }
                }

                _targetsLabel.Text = labelVal.TrimEnd(',');
            }
            else
            {
                _targetsLabel.Text = Resources.NoTargetsDefined;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HelpForm form = new HelpForm();
            form.Show();
        }

       
    }
}

