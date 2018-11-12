using ASMRest;
using CommonControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrafficViewerSDK;

namespace CustomTestsUI
{
    public partial class ServerTests : Form
    {
        const string ATTACKS_APP_ID = "75";
        private Dictionary<string, string>[] _issues;

        

        private ProgressDialog _progressDialog;
        public ServerTests()
        {
            InitializeComponent();
        }

        public DataGridView GetDataGrid()
        {
            return _dataGrid;
        }

        private void ServerTests_Load(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(ASMRestSettingsInstance.Instance.AscSessionId))
            {
                //login to ase and initialize settings
                ASMLoginForm loginForm = new ASMLoginForm();

                loginForm.ShowDialog(this);
                if (loginForm.DialogResult != System.Windows.Forms.DialogResult.OK)
                {
                    this.Hide();
                    return;
                }
            }

            /*
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += InitAsync;
            worker.RunWorkerCompleted += DoPostInit;
            worker.RunWorkerAsync();
            _progressDialog = new ProgressDialog();
            _progressDialog.Start();

            ErrorBox.ShowDialog("Muci");*/

            Init();
            DoPostInit();
        }

        void Init()
        {

            if (ASMRestSettingsInstance.Instance.IssueAttributeMap.Count == 0)
            {
                IssueAttributeDefinitionListCall defsCall = new IssueAttributeDefinitionListCall(ASMRestSettingsInstance.Instance);
                var defs = defsCall.Get();
                foreach (IssueAttributeDefinitionEx def in defs.attributeDefColl)
                {
                    ASMRestSettingsInstance.Instance.IssueAttributeMap.Add(def.id.ToString(), def.name);
                }
            }

            AppCall appCall = new AppCall(ASMRestSettingsInstance.Instance);
            var app = appCall.Get(ATTACKS_APP_ID);
            IssueListCall issuesCall = new IssueListCall(app, ASMRestSettingsInstance.Instance);
            issuesCall.SkipHtmlEncoding = true;
            issuesCall.IssueAttributesMap = ASMRestSettingsInstance.Instance.IssueAttributeMap;
            _issues = issuesCall.Fetch("+issuetype");
        }

        private void DoPostInit()
        {
            
            if (_issues != null)
            {
                foreach (var item in _issues)
                {
                    _dataGrid.Rows.Add(
                        item.ContainsKey("Issue Type") ? item["Issue Type"] : String.Empty,
                        item.ContainsKey("Attack Variant Name") ? item["Attack Variant Name"] : String.Empty,
                        item.ContainsKey("Attack Hits") ? item["Attack Hits"] : String.Empty,
                        item.ContainsKey("Attack Mutation") ? item["Attack Mutation"] : String.Empty,
                        item.ContainsKey("Attack Validation") ? item["Attack Validation"] : String.Empty,
                        item.ContainsKey("Issue Tags") ? item["Issue Tags"] : String.Empty);

                }
            }
        }

       

        private void CancelClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void ApplyFilterClick(object sender, EventArgs e)
        {
            string filter = _filterBox.Text;
            
            foreach (DataGridViewRow row in _dataGrid.Rows)
            { 
                string rowString = String.Format("Type={0},Variant={1},Mutation={2},Validation={3},Issue Tags={4}",
                    row.Cells["_issueType"].Value, row.Cells["_variantName"].Value, row.Cells["_mutation"].Value, row.Cells["_validation"].Value, row.Cells["_tags"].Value);
                
                row.Visible = String.IsNullOrWhiteSpace(filter) || Utils.IsMatch(rowString, filter);
                
            }
        }

        private void LoadClick(object sender, EventArgs e)
        {
            if (_dataGrid.SelectedRows.Count == 0)
            {
                ErrorBox.ShowDialog("No Items Selected");
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void FilterBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ApplyFilterClick(sender, e);
            }
        }
    }
}
