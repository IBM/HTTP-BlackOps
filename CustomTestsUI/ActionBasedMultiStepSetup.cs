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
    public partial class ActionBasedMultiStepSetup : Form
    {
        private CustomTestsFile _testFile;
        public ActionBasedMultiStepSetup(CustomTestsFile file)
        {
            _testFile = file;
            InitializeComponent();
        }

        private void _multiStepGrid_AddClick(object sender, EventArgs e)
        {
            OpenFileDialog dialogOpenFile = new OpenFileDialog();
            if (dialogOpenFile.ShowDialog() == DialogResult.OK)
            {
                _multiStepsGrid.AddRow(dialogOpenFile.FileName);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void _buttonOK_Click(object sender, EventArgs e)
        {
            _testFile.SetMultiStepList(_multiStepsGrid.GetValues());
            _testFile.Save();
            this.Hide();
        }


        private void ActionBasedMultiStepSetup_Load(object sender, EventArgs e)
        {
            _multiStepsGrid.SetValues(_testFile.GetMultiStepList());
        }
    }
}
