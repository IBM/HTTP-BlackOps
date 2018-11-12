using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace TrafficViewerControls
{
    public class ColumnSelectMenuItem:ToolStripMenuItem
    {
        DataGridViewColumn _column;

        public DataGridViewColumn Column
        {
            get { return _column; }
            set 
            { 
                _column = value;
                this.Checked = value.Visible;
                this.Text = value.HeaderText;
            }
        }

    }
}
