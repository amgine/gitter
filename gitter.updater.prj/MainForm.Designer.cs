namespace gitter.Updater
{
	partial class MainForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this._btnCancel = new System.Windows.Forms.Button();
			this._updateProgress = new System.Windows.Forms.ProgressBar();
			this._lblStage = new System.Windows.Forms.Label();
			this._pnlContainer = new System.Windows.Forms.Panel();
			this._pnlLine = new System.Windows.Forms.Panel();
			this._pnlContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// _btnCancel
			// 
			this._btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnCancel.Location = new System.Drawing.Point(371, 84);
			this._btnCancel.Name = "_btnCancel";
			this._btnCancel.Size = new System.Drawing.Size(75, 23);
			this._btnCancel.TabIndex = 0;
			this._btnCancel.Text = "Cancel";
			this._btnCancel.UseVisualStyleBackColor = true;
			this._btnCancel.Click += new System.EventHandler(this.OnCancelClick);
			// 
			// _updateProgress
			// 
			this._updateProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._updateProgress.Location = new System.Drawing.Point(15, 33);
			this._updateProgress.Name = "_updateProgress";
			this._updateProgress.Size = new System.Drawing.Size(426, 23);
			this._updateProgress.TabIndex = 1;
			// 
			// _lblStage
			// 
			this._lblStage.AutoSize = true;
			this._lblStage.Location = new System.Drawing.Point(12, 14);
			this._lblStage.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this._lblStage.Name = "_lblStage";
			this._lblStage.Size = new System.Drawing.Size(0, 13);
			this._lblStage.TabIndex = 2;
			// 
			// _pnlContainer
			// 
			this._pnlContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._pnlContainer.BackColor = System.Drawing.SystemColors.Window;
			this._pnlContainer.Controls.Add(this._pnlLine);
			this._pnlContainer.Controls.Add(this._lblStage);
			this._pnlContainer.Controls.Add(this._updateProgress);
			this._pnlContainer.Location = new System.Drawing.Point(0, 0);
			this._pnlContainer.Margin = new System.Windows.Forms.Padding(0);
			this._pnlContainer.Name = "_pnlContainer";
			this._pnlContainer.Size = new System.Drawing.Size(455, 76);
			this._pnlContainer.TabIndex = 3;
			// 
			// _pnlLine
			// 
			this._pnlLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._pnlLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
			this._pnlLine.Location = new System.Drawing.Point(0, 75);
			this._pnlLine.Name = "_pnlLine";
			this._pnlLine.Size = new System.Drawing.Size(455, 1);
			this._pnlLine.TabIndex = 1;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this._btnCancel;
			this.ClientSize = new System.Drawing.Size(453, 115);
			this.Controls.Add(this._pnlContainer);
			this.Controls.Add(this._btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "gitter Update";
			this._pnlContainer.ResumeLayout(false);
			this._pnlContainer.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button _btnCancel;
		private System.Windows.Forms.ProgressBar _updateProgress;
		private System.Windows.Forms.Label _lblStage;
		private System.Windows.Forms.Panel _pnlContainer;
		private System.Windows.Forms.Panel _pnlLine;
	}
}

