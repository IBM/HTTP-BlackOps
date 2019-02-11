/**
Copyright 2019 Trend Micro, Incorporated, All Rights Reserved.
SPDX-License-Identifier: Apache-2.0
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace CommonControls
{

	public class OptionsGrid : UserControl
	{
		protected System.Windows.Forms.Label _label;
		private System.Windows.Forms.Button _addButton;
		private System.Windows.Forms.Button _deleteButton;
		private System.Windows.Forms.DataGridView _dataGridView;
		private DataGridViewComboBoxColumn Column1;
		private DataGridViewTextBoxColumn Column2;
		private string[] _defaultValues;


		string _columns = "Column1:Value1,Value2\r\nColumn2";
		/// <summary>
		/// Lists the columns of the grid
		/// </summary>
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		Description("The columns of the grid"),
		Category("Appearance"),
		Editor("System.ComponentModel.Design.MultilineStringEditor, " +
			   "System.Design, Version=2.0.0.0, Culture=neutral, " +
			   "PublicKeyToken=b03f5f7f11d50a3a",
				typeof(System.Drawing.Design.UITypeEditor))]
		public string Columns
		{
			get
			{
				return _columns;
			}
			set
			{
				int i, j;
				_columns = value;
				_dataGridView.Columns.Clear();
				string[] lines = _columns.Split(new string[] { "\r\n" },
					StringSplitOptions.RemoveEmptyEntries);
				string[] linecomponents;
				string[] values;
				DataGridViewColumn c;
				_defaultValues = new string[lines.Length];
				for (i = 0; i < lines.Length; i++)
				{
					linecomponents = lines[i].Split(':');
					if (linecomponents.Length == 1)
					{
						_dataGridView.Columns.Add(linecomponents[0], linecomponents[0]);
						_defaultValues[i] = "New " + linecomponents[0];
					}
					else
					{
						if (linecomponents.Length == 2)
						{
                            DataGridViewCell dgvCell;
							values = linecomponents[1].Split(',');
                            if (linecomponents[1].EqualsIgnoreCase("bool") || linecomponents[1].EqualsIgnoreCase("boolean"))
                            {
                                dgvCell = new DataGridViewCheckBoxCell();
                                dgvCell.Style.Alignment = DataGridViewContentAlignment.TopCenter;
                                dgvCell.Value = false;
                                _defaultValues[i] = "false";
                            }
                            else
                            {
                                DataGridViewComboBoxCell comboCell;
                                comboCell = new DataGridViewComboBoxCell();
							    for (j = 0; j < values.Length; j++)
							    {
								    comboCell.Items.Add(values[j]);
							    }
							    comboCell.Value = comboCell.Items[0];
							    _defaultValues[i] = (string)comboCell.Items[0];
                                dgvCell = comboCell;
                            }
                            c = new DataGridViewColumn(dgvCell);
                            c.Name = linecomponents[0];
							c.HeaderText = linecomponents[0];
							_dataGridView.Columns.Add(c);
						}
					}
				}
			}
		}

		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		Description("The text of the label"), Category("Appearance")]
		public string LabelText
		{
			get { return _label.Text; }
			set { if (value != null) _label.Text = value; }
		}

		private void InitializeComponent()
		{
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this._label = new System.Windows.Forms.Label();
            this._addButton = new System.Windows.Forms.Button();
            this._deleteButton = new System.Windows.Forms.Button();
            this._dataGridView = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this._dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // _label
            // 
            this._label.AutoSize = true;
            this._label.Location = new System.Drawing.Point(0, 9);
            this._label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._label.Name = "_label";
            this._label.Size = new System.Drawing.Size(110, 13);
            this._label.TabIndex = 0;
            this._label.Text = "[Enter label text here]:";
            // 
            // _addButton
            // 
            this._addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._addButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this._addButton.FlatAppearance.BorderSize = 0;
            this._addButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._addButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._addButton.Location = new System.Drawing.Point(186, 7);
            this._addButton.Margin = new System.Windows.Forms.Padding(4);
            this._addButton.Name = "_addButton";
            this._addButton.Size = new System.Drawing.Size(32, 28);
            this._addButton.TabIndex = 1;
            this._addButton.Text = "+";
            this._addButton.UseVisualStyleBackColor = true;
            this._addButton.Click += new System.EventHandler(this.AddButtonClick);
            // 
            // _deleteButton
            // 
            this._deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._deleteButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this._deleteButton.FlatAppearance.BorderSize = 0;
            this._deleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._deleteButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._deleteButton.Location = new System.Drawing.Point(218, 1);
            this._deleteButton.Margin = new System.Windows.Forms.Padding(4);
            this._deleteButton.Name = "_deleteButton";
            this._deleteButton.Size = new System.Drawing.Size(30, 35);
            this._deleteButton.TabIndex = 2;
            this._deleteButton.Text = "-";
            this._deleteButton.UseVisualStyleBackColor = true;
            this._deleteButton.Click += new System.EventHandler(this.DeleteButtonClick);
            // 
            // _dataGridView
            // 
            this._dataGridView.AllowUserToAddRows = false;
            this._dataGridView.AllowUserToDeleteRows = false;
            this._dataGridView.AllowUserToResizeRows = false;
            this._dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._dataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this._dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._dataGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this._dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this._dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this._dataGridView.DefaultCellStyle = dataGridViewCellStyle4;
            this._dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this._dataGridView.Location = new System.Drawing.Point(4, 40);
            this._dataGridView.Margin = new System.Windows.Forms.Padding(4);
            this._dataGridView.Name = "_dataGridView";
            this._dataGridView.RowHeadersVisible = false;
            this._dataGridView.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            this._dataGridView.RowTemplate.Height = 20;
            this._dataGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this._dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dataGridView.Size = new System.Drawing.Size(244, 137);
            this._dataGridView.TabIndex = 3;
            this._dataGridView.MouseLeave += new System.EventHandler(this._dataGridView_Leave);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Column1";
            this.Column1.Items.AddRange(new object[] {
            "Value1",
            "Value2"});
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Column2";
            this.Column2.Name = "Column2";
            // 
            // OptionsGrid
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this._dataGridView);
            this.Controls.Add(this._deleteButton);
            this.Controls.Add(this._addButton);
            this.Controls.Add(this._label);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "OptionsGrid";
            this.Size = new System.Drawing.Size(251, 188);
            ((System.ComponentModel.ISupportInitialize)(this._dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		private void AddButtonClick(object sender, EventArgs e)
		{
			AddRow(_defaultValues);
		}

		public void AddRow(params string[] values)
		{
			if (_dataGridView.Columns.Count > 0)
			{
				_dataGridView.ClearSelection();
				_dataGridView.Rows[_dataGridView.Rows.Add(values)].Selected = true;
			}
		}

		private void DeleteButtonClick(object sender, EventArgs e)
		{
			foreach (DataGridViewRow r in _dataGridView.SelectedRows)
				_dataGridView.Rows.Remove(r);
		}

		public event EventHandler AddClick
		{
			add
			{
				//remove the default AddClick evant handler
				_addButton.Click -= new EventHandler(AddButtonClick);
				//add the new event handler
				_addButton.Click += value;
			}
			remove
			{
				_addButton.Click -= value;
			}
		}


		public OptionsGrid()
		{

			InitializeComponent();

		}

		/// <summary>
		/// The values of the Options Grid. Accepts a tCollection of strings with values
		/// separated by tabs
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetValues()
		{
			int i, j;
			string s;
			DataGridViewRow r;
			List<string> values = new List<string>();
			for (i = 0; i < this._dataGridView.Rows.Count; i++)
			{
				s = "";
				r = this._dataGridView.Rows[i];
				for (j = 0; j < r.Cells.Count; j++)
				{
					s += (string)r.Cells[j].Value.ToString();
					if (j < r.Cells.Count - 1) s += "\t";
				}
				values.Add(s);
			}
			return values;
		}

		public void SetValues(IEnumerable<string> value)
		{
			if (value == null) return;
			this._dataGridView.Rows.Clear();
			foreach (string entry in value)
			{
                string[] row = entry.Split('\t');
                _dataGridView.Rows.Add(row);
			}
		}

        /// <summary>
        /// Clears the grid
        /// </summary>
        public void Clear()
        {           
            this._dataGridView.Rows.Clear();
        }

        private void _dataGridView_Leave(object sender, EventArgs e)
        {
            _dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }




	}
}
