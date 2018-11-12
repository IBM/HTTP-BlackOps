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
    public partial class AttackTargetsSetup : Form
    {
        private CustomTestsFile _testFile;
        public AttackTargetsSetup(CustomTestsFile file)
        {
            _testFile = file;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void _buttonOK_Click(object sender, EventArgs e)
        {
            _testFile.SetAttackTargetList(_attackTargetListGrid.GetValues());
            _testFile.Save();
            this.Hide();
        }

       

        private void ActionBasedMultiStepSetup_Load(object sender, EventArgs e)
        {
            _attackTargetListGrid.SetValues(_testFile.GetAttackTargetListRaw());
        }
    }
}
