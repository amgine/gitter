namespace gitter.Git.Gui.Dialogs
{
	partial class ConflictsDialog
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
			if(_repository != null)
			{
				DetachFromRepository();
				_repository = null;
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
			this._lstConflicts = new gitter.Git.Gui.Controls.TreeListBox();
			this._lblConflictingFiles = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _lstConflicts
			// 
			this._lstConflicts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._lstConflicts.HeaderStyle = gitter.Framework.Controls.HeaderStyle.Hidden;
			this._lstConflicts.Location = new System.Drawing.Point(3, 18);
			this._lstConflicts.Name = "_lstConflicts";
			this._lstConflicts.Size = new System.Drawing.Size(394, 304);
			this._lstConflicts.TabIndex = 3;
			// 
			// _lblConflictingFiles
			// 
			this._lblConflictingFiles.AutoSize = true;
			this._lblConflictingFiles.Location = new System.Drawing.Point(0, 0);
			this._lblConflictingFiles.Name = "_lblConflictingFiles";
			this._lblConflictingFiles.Size = new System.Drawing.Size(115, 15);
			this._lblConflictingFiles.TabIndex = 4;
			this._lblConflictingFiles.Text = "%Conflicting Files%:";
			// 
			// ConflictsDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._lblConflictingFiles);
			this.Controls.Add(this._lstConflicts);
			this.Name = "ConflictsDialog";
			this.Size = new System.Drawing.Size(400, 325);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private gitter.Git.Gui.Controls.TreeListBox _lstConflicts;
		private System.Windows.Forms.Label _lblConflictingFiles;
	}
}
