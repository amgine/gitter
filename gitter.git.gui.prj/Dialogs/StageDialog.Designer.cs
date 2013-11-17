namespace gitter.Git.Gui.Dialogs
{
	partial class StageDialog
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
			if(disposing)
			{
				DataBinding = null;
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._txtPattern = new System.Windows.Forms.TextBox();
			this._lblPattern = new System.Windows.Forms.Label();
			this._lstUnstaged = new gitter.Git.Gui.Controls.TreeListBox();
			this._chkIncludeUntracked = new System.Windows.Forms.CheckBox();
			this._chkIncludeIgnored = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// _txtPattern
			// 
			this._txtPattern.Location = new System.Drawing.Point(63, 3);
			this._txtPattern.Name = "_txtPattern";
			this._txtPattern.Size = new System.Drawing.Size(319, 23);
			this._txtPattern.TabIndex = 0;
			this._txtPattern.TextChanged += new System.EventHandler(this.OnPatternTextChanged);
			// 
			// _lblPattern
			// 
			this._lblPattern.AutoSize = true;
			this._lblPattern.Location = new System.Drawing.Point(0, 6);
			this._lblPattern.Name = "_lblPattern";
			this._lblPattern.Size = new System.Drawing.Size(68, 15);
			this._lblPattern.TabIndex = 1;
			this._lblPattern.Text = "%Pattern%:";
			// 
			// _lstUnstaged
			// 
			this._lstUnstaged.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._lstUnstaged.HeaderStyle = gitter.Framework.Controls.HeaderStyle.Hidden;
			this._lstUnstaged.Location = new System.Drawing.Point(3, 52);
			this._lstUnstaged.Name = "_lstUnstaged";
			this._lstUnstaged.ShowTreeLines = true;
			this._lstUnstaged.Size = new System.Drawing.Size(379, 270);
			this._lstUnstaged.TabIndex = 2;
			this._lstUnstaged.ItemActivated += new System.EventHandler<gitter.Framework.Controls.ItemEventArgs>(this.OnFilesItemActivated);
			// 
			// _chkIncludeUntracked
			// 
			this._chkIncludeUntracked.AutoSize = true;
			this._chkIncludeUntracked.Checked = true;
			this._chkIncludeUntracked.CheckState = System.Windows.Forms.CheckState.Checked;
			this._chkIncludeUntracked.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkIncludeUntracked.Location = new System.Drawing.Point(63, 29);
			this._chkIncludeUntracked.Name = "_chkIncludeUntracked";
			this._chkIncludeUntracked.Size = new System.Drawing.Size(127, 20);
			this._chkIncludeUntracked.TabIndex = 3;
			this._chkIncludeUntracked.Text = "Include untracked";
			this._chkIncludeUntracked.UseVisualStyleBackColor = true;
			this._chkIncludeUntracked.CheckedChanged += new System.EventHandler(this.OnIncludeUntrackedCheckedChanged);
			// 
			// _chkIncludeIgnored
			// 
			this._chkIncludeIgnored.AutoSize = true;
			this._chkIncludeIgnored.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkIncludeIgnored.Location = new System.Drawing.Point(211, 29);
			this._chkIncludeIgnored.Name = "_chkIncludeIgnored";
			this._chkIncludeIgnored.Size = new System.Drawing.Size(115, 20);
			this._chkIncludeIgnored.TabIndex = 4;
			this._chkIncludeIgnored.Text = "Include ignored";
			this._chkIncludeIgnored.UseVisualStyleBackColor = true;
			this._chkIncludeIgnored.CheckedChanged += new System.EventHandler(this.OnIncludeIgnoredCheckedChanged);
			// 
			// StageDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._chkIncludeIgnored);
			this.Controls.Add(this._chkIncludeUntracked);
			this.Controls.Add(this._lstUnstaged);
			this.Controls.Add(this._txtPattern);
			this.Controls.Add(this._lblPattern);
			this.Name = "StageDialog";
			this.Size = new System.Drawing.Size(385, 325);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _txtPattern;
		private System.Windows.Forms.Label _lblPattern;
		private gitter.Git.Gui.Controls.TreeListBox _lstUnstaged;
		private System.Windows.Forms.CheckBox _chkIncludeUntracked;
		private System.Windows.Forms.CheckBox _chkIncludeIgnored;
	}
}
