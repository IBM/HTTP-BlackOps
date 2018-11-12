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
