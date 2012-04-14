namespace gitter.Git.Gui.Dialogs
{
	partial class RemoteReferencesDialog
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._lstRemotes = new gitter.Git.Gui.Controls.RemoteReferencesListBox();
			this._lblRemoteReferences = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _lstRemotes
			// 
			this._lstRemotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._lstRemotes.Location = new System.Drawing.Point(3, 18);
			this._lstRemotes.Name = "_lstRemotes";
			this._lstRemotes.Size = new System.Drawing.Size(379, 304);
			this._lstRemotes.TabIndex = 0;
			// 
			// _lblRemoteReferences
			// 
			this._lblRemoteReferences.AutoSize = true;
			this._lblRemoteReferences.Location = new System.Drawing.Point(0, 0);
			this._lblRemoteReferences.Name = "_lblRemoteReferences";
			this._lblRemoteReferences.Size = new System.Drawing.Size(131, 15);
			this._lblRemoteReferences.TabIndex = 1;
			this._lblRemoteReferences.Text = "%Remote References%:";
			// 
			// RemoteDialog
			// 
			this.Controls.Add(this._lblRemoteReferences);
			this.Controls.Add(this._lstRemotes);
			this.Name = "RemoteDialog";
			this.Size = new System.Drawing.Size(385, 325);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private gitter.Git.Gui.Controls.RemoteReferencesListBox _lstRemotes;
		private System.Windows.Forms.Label _lblRemoteReferences;

	}
}
