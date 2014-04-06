namespace gitter.Framework
{
	partial class ProgressForm
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
			if(_cancellationTokenSource != null)
			{
				_cancellationTokenSource.Dispose();
				_cancellationTokenSource = null;
			}
			if(disposing && (components != null))
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
			this.panel1 = new System.Windows.Forms.Panel();
			this._btnCancel = new System.Windows.Forms.Button();
			this._pnlLine = new System.Windows.Forms.Panel();
			this._progressBar = new System.Windows.Forms.ProgressBar();
			this._lblAction = new System.Windows.Forms.Label();
			this._pnlContainer = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			this._pnlContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._btnCancel);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 64);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(389, 40);
			this.panel1.TabIndex = 4;
			// 
			// _btnCancel
			// 
			this._btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnCancel.Location = new System.Drawing.Point(307, 9);
			this._btnCancel.Name = "_btnCancel";
			this._btnCancel.Size = new System.Drawing.Size(75, 23);
			this._btnCancel.TabIndex = 3;
			this._btnCancel.Text = "Cancel";
			this._btnCancel.UseVisualStyleBackColor = true;
			this._btnCancel.Click += new System.EventHandler(this.OnCancelClick);
			// 
			// _pnlLine
			// 
			this._pnlLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._pnlLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
			this._pnlLine.Location = new System.Drawing.Point(0, 64);
			this._pnlLine.Name = "_pnlLine";
			this._pnlLine.Size = new System.Drawing.Size(389, 1);
			this._pnlLine.TabIndex = 2;
			// 
			// _progressBar
			// 
			this._progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._progressBar.Location = new System.Drawing.Point(12, 31);
			this._progressBar.Name = "_progressBar";
			this._progressBar.Size = new System.Drawing.Size(365, 18);
			this._progressBar.TabIndex = 0;
			// 
			// _lblAction
			// 
			this._lblAction.AutoSize = true;
			this._lblAction.Location = new System.Drawing.Point(10, 10);
			this._lblAction.Name = "_lblAction";
			this._lblAction.Size = new System.Drawing.Size(48, 13);
			this._lblAction.TabIndex = 1;
			this._lblAction.Text = "<action>";
			// 
			// _pnlContainer
			// 
			this._pnlContainer.BackColor = System.Drawing.SystemColors.Window;
			this._pnlContainer.Controls.Add(this._pnlLine);
			this._pnlContainer.Controls.Add(this._lblAction);
			this._pnlContainer.Controls.Add(this._progressBar);
			this._pnlContainer.Dock = System.Windows.Forms.DockStyle.Top;
			this._pnlContainer.Location = new System.Drawing.Point(0, 0);
			this._pnlContainer.Name = "_pnlContainer";
			this._pnlContainer.Size = new System.Drawing.Size(389, 65);
			this._pnlContainer.TabIndex = 3;
			// 
			// ProgressForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this._btnCancel;
			this.ClientSize = new System.Drawing.Size(389, 104);
			this.ControlBox = false;
			this.Controls.Add(this._pnlContainer);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProgressForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Progress";
			this.panel1.ResumeLayout(false);
			this._pnlContainer.ResumeLayout(false);
			this._pnlContainer.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ProgressBar _progressBar;
		private System.Windows.Forms.Label _lblAction;
		private System.Windows.Forms.Panel _pnlContainer;
		private System.Windows.Forms.Panel _pnlLine;
		private System.Windows.Forms.Button _btnCancel;
		private System.Windows.Forms.Panel panel1;
	}
}