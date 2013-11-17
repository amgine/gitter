namespace gitter.Git.Gui.Dialogs
{
	partial class CleanDialog
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
			this._lblIncludePattern = new System.Windows.Forms.Label();
			this._radIncludeUntracked = new System.Windows.Forms.RadioButton();
			this._radIncludeIgnored = new System.Windows.Forms.RadioButton();
			this._radIncludeBoth = new System.Windows.Forms.RadioButton();
			this._lblExcludePattern = new System.Windows.Forms.Label();
			this._txtExclude = new System.Windows.Forms.TextBox();
			this._lblType = new System.Windows.Forms.Label();
			this._chkRemoveDirectories = new System.Windows.Forms.CheckBox();
			this._lstFilesToClear = new gitter.Git.Gui.Controls.TreeListBox();
			this._lblObjectList = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _txtPattern
			// 
			this._txtPattern.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtPattern.Location = new System.Drawing.Point(127, 3);
			this._txtPattern.Name = "_txtPattern";
			this._txtPattern.Size = new System.Drawing.Size(307, 23);
			this._txtPattern.TabIndex = 0;
			this._txtPattern.TextChanged += new System.EventHandler(this.OnPatternTextChanged);
			// 
			// _lblIncludePattern
			// 
			this._lblIncludePattern.AutoSize = true;
			this._lblIncludePattern.Location = new System.Drawing.Point(3, 6);
			this._lblIncludePattern.Name = "_lblIncludePattern";
			this._lblIncludePattern.Size = new System.Drawing.Size(110, 15);
			this._lblIncludePattern.TabIndex = 1;
			this._lblIncludePattern.Text = "%Include Pattern%:";
			// 
			// _radIncludeUntracked
			// 
			this._radIncludeUntracked.AutoSize = true;
			this._radIncludeUntracked.Checked = true;
			this._radIncludeUntracked.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._radIncludeUntracked.Location = new System.Drawing.Point(127, 61);
			this._radIncludeUntracked.Name = "_radIncludeUntracked";
			this._radIncludeUntracked.Size = new System.Drawing.Size(85, 20);
			this._radIncludeUntracked.TabIndex = 2;
			this._radIncludeUntracked.TabStop = true;
			this._radIncludeUntracked.Text = "Untracked";
			this._radIncludeUntracked.UseVisualStyleBackColor = true;
			this._radIncludeUntracked.CheckedChanged += new System.EventHandler(this.OnRadioButtonCheckedChanged);
			// 
			// _radIncludeIgnored
			// 
			this._radIncludeIgnored.AutoSize = true;
			this._radIncludeIgnored.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._radIncludeIgnored.Location = new System.Drawing.Point(239, 61);
			this._radIncludeIgnored.Name = "_radIncludeIgnored";
			this._radIncludeIgnored.Size = new System.Drawing.Size(72, 20);
			this._radIncludeIgnored.TabIndex = 3;
			this._radIncludeIgnored.Text = "Ignored";
			this._radIncludeIgnored.UseVisualStyleBackColor = true;
			this._radIncludeIgnored.CheckedChanged += new System.EventHandler(this.OnRadioButtonCheckedChanged);
			// 
			// _radIncludeBoth
			// 
			this._radIncludeBoth.AutoSize = true;
			this._radIncludeBoth.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._radIncludeBoth.Location = new System.Drawing.Point(349, 61);
			this._radIncludeBoth.Name = "_radIncludeBoth";
			this._radIncludeBoth.Size = new System.Drawing.Size(56, 20);
			this._radIncludeBoth.TabIndex = 4;
			this._radIncludeBoth.Text = "Both";
			this._radIncludeBoth.UseVisualStyleBackColor = true;
			this._radIncludeBoth.CheckedChanged += new System.EventHandler(this.OnRadioButtonCheckedChanged);
			// 
			// _lblExcludePattern
			// 
			this._lblExcludePattern.AutoSize = true;
			this._lblExcludePattern.Location = new System.Drawing.Point(3, 35);
			this._lblExcludePattern.Name = "_lblExcludePattern";
			this._lblExcludePattern.Size = new System.Drawing.Size(111, 15);
			this._lblExcludePattern.TabIndex = 1;
			this._lblExcludePattern.Text = "%Exclude Pattern%:";
			// 
			// _txtExclude
			// 
			this._txtExclude.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtExclude.Location = new System.Drawing.Point(127, 32);
			this._txtExclude.Name = "_txtExclude";
			this._txtExclude.Size = new System.Drawing.Size(307, 23);
			this._txtExclude.TabIndex = 1;
			this._txtExclude.TextChanged += new System.EventHandler(this.OnPatternTextChanged);
			// 
			// _lblType
			// 
			this._lblType.AutoSize = true;
			this._lblType.Location = new System.Drawing.Point(3, 63);
			this._lblType.Name = "_lblType";
			this._lblType.Size = new System.Drawing.Size(56, 15);
			this._lblType.TabIndex = 1;
			this._lblType.Text = "%Type%:";
			// 
			// _chkRemoveDirectories
			// 
			this._chkRemoveDirectories.AutoSize = true;
			this._chkRemoveDirectories.Checked = true;
			this._chkRemoveDirectories.CheckState = System.Windows.Forms.CheckState.Checked;
			this._chkRemoveDirectories.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkRemoveDirectories.Location = new System.Drawing.Point(127, 81);
			this._chkRemoveDirectories.Name = "_chkRemoveDirectories";
			this._chkRemoveDirectories.Size = new System.Drawing.Size(156, 20);
			this._chkRemoveDirectories.TabIndex = 5;
			this._chkRemoveDirectories.Text = "Also remove directories";
			this._chkRemoveDirectories.UseVisualStyleBackColor = true;
			this._chkRemoveDirectories.CheckedChanged += new System.EventHandler(this.OnRemoveDirectoriesCheckedChanged);
			// 
			// _lstFilesToClear
			// 
			this._lstFilesToClear.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._lstFilesToClear.DisableContextMenus = true;
			this._lstFilesToClear.HeaderStyle = gitter.Framework.Controls.HeaderStyle.Hidden;
			this._lstFilesToClear.Location = new System.Drawing.Point(3, 122);
			this._lstFilesToClear.Name = "_lstFilesToClear";
			this._lstFilesToClear.ShowTreeLines = true;
			this._lstFilesToClear.Size = new System.Drawing.Size(431, 270);
			this._lstFilesToClear.TabIndex = 6;
			this._lstFilesToClear.ItemActivated += new System.EventHandler<gitter.Framework.Controls.ItemEventArgs>(this.OnFilesToClearItemActivated);
			// 
			// _lblObjectList
			// 
			this._lblObjectList.AutoSize = true;
			this._lblObjectList.Location = new System.Drawing.Point(0, 104);
			this._lblObjectList.Name = "_lblObjectList";
			this._lblObjectList.Size = new System.Drawing.Size(181, 15);
			this._lblObjectList.TabIndex = 7;
			this._lblObjectList.Text = "%Objects that will be removed%:";
			// 
			// CleanDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._lblObjectList);
			this.Controls.Add(this._chkRemoveDirectories);
			this.Controls.Add(this._radIncludeBoth);
			this.Controls.Add(this._radIncludeIgnored);
			this.Controls.Add(this._radIncludeUntracked);
			this.Controls.Add(this._lstFilesToClear);
			this.Controls.Add(this._txtExclude);
			this.Controls.Add(this._lblExcludePattern);
			this.Controls.Add(this._txtPattern);
			this.Controls.Add(this._lblType);
			this.Controls.Add(this._lblIncludePattern);
			this.Name = "CleanDialog";
			this.Size = new System.Drawing.Size(437, 395);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _txtPattern;
		private System.Windows.Forms.Label _lblIncludePattern;
		private gitter.Git.Gui.Controls.TreeListBox _lstFilesToClear;
		private System.Windows.Forms.RadioButton _radIncludeUntracked;
		private System.Windows.Forms.RadioButton _radIncludeIgnored;
		private System.Windows.Forms.RadioButton _radIncludeBoth;
		private System.Windows.Forms.Label _lblExcludePattern;
		private System.Windows.Forms.TextBox _txtExclude;
		private System.Windows.Forms.Label _lblType;
		private System.Windows.Forms.CheckBox _chkRemoveDirectories;
		private System.Windows.Forms.Label _lblObjectList;
	}
}
