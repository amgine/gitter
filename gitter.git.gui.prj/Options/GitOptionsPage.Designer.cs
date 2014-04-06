namespace gitter.Git
{
	partial class GitOptionsPage
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
				if(_cachedControls != null)
				{
					foreach(var pair in _cachedControls.Values)
					{
						pair.Item2.Dispose();
					}
				}
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
			this._grpRepositoryAccessor = new gitter.Framework.Controls.GroupSeparator();
			this._cmbAccessorProvider = new System.Windows.Forms.ComboBox();
			this._lblAccessmethod = new System.Windows.Forms.Label();
			this._openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.SuspendLayout();
			// 
			// _grpRepositoryAccessor
			// 
			this._grpRepositoryAccessor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._grpRepositoryAccessor.Location = new System.Drawing.Point(0, 0);
			this._grpRepositoryAccessor.Name = "_grpRepositoryAccessor";
			this._grpRepositoryAccessor.Size = new System.Drawing.Size(476, 19);
			this._grpRepositoryAccessor.TabIndex = 0;
			this._grpRepositoryAccessor.Text = "%Repository access method%";
			// 
			// _cmbAccessorProvider
			// 
			this._cmbAccessorProvider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._cmbAccessorProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._cmbAccessorProvider.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._cmbAccessorProvider.FormattingEnabled = true;
			this._cmbAccessorProvider.Location = new System.Drawing.Point(117, 25);
			this._cmbAccessorProvider.Name = "_cmbAccessorProvider";
			this._cmbAccessorProvider.Size = new System.Drawing.Size(359, 23);
			this._cmbAccessorProvider.TabIndex = 4;
			// 
			// _lblAccessmethod
			// 
			this._lblAccessmethod.AutoSize = true;
			this._lblAccessmethod.Location = new System.Drawing.Point(0, 28);
			this._lblAccessmethod.Name = "_lblAccessmethod";
			this._lblAccessmethod.Size = new System.Drawing.Size(111, 15);
			this._lblAccessmethod.TabIndex = 8;
			this._lblAccessmethod.Text = "%Access Method%:";
			// 
			// GitOptionsPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._lblAccessmethod);
			this.Controls.Add(this._cmbAccessorProvider);
			this.Controls.Add(this._grpRepositoryAccessor);
			this.Name = "GitOptionsPage";
			this.Size = new System.Drawing.Size(479, 218);
			this.ResumeLayout(false);

		}

		#endregion

		private gitter.Framework.Controls.GroupSeparator _grpRepositoryAccessor;
		private System.Windows.Forms.ComboBox _cmbAccessorProvider;
		private System.Windows.Forms.Label _lblAccessmethod;
		private System.Windows.Forms.OpenFileDialog _openFileDialog;
	}
}
