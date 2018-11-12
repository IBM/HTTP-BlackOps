using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CommonControls
{
	public partial class ProgressDialog : Form
	{
        private object _lock= new object();
        private bool _stop = false;
		/// <summary>
		/// Constructor
		/// </summary>
		public ProgressDialog()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Gets/sets a message for the progress bar
		/// </summary>
		public string Label
		{
			get
			{
				return _label.Text;
			}
			set
			{
				_label.Text = value;
			}
		}


		/// <summary>
		/// Shows and starts the progress bar
		/// </summary>
		public void Start(IWin32Window owner =  null)
		{
            lock (_lock)
            {
                _stop = false;
                _progressBar.Value = 0;
                _timer.Start();
                if (owner == null)
                {
                    this.ShowDialog();
                }
                else
                {
                    this.ShowDialog(owner);
                }
            }
		}

		/// <summary>
		/// Stops the progress and hides the dialog
		/// </summary>
		public void Stop()
		{
            lock (_lock)
            {
                _stop = true;
                if (_timer.Enabled)
                {
                    _timer.Stop();
                    this.Hide();
                }
            }
		}


		private void ProgressDialogFormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
		}

		private void TimerTick(object sender, EventArgs e)
		{
			_timer.Stop();
			if (_progressBar.Value + _progressBar.Step <= 100)
			{
				_progressBar.Value += _progressBar.Step;
			}
			else
			{
				_progressBar.Value = 0;
			}
            if (!_stop)
            {
                _timer.Start();
            }
		}
	}
}