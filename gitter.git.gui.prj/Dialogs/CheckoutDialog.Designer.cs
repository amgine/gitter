namespace gitter.Git.Gui.Dialogs
{
	partial class CheckoutDialog
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
			this._txtRevision = new System.Windows.Forms.TextBox();
			this._lblRevision = new System.Windows.Forms.Label();
			this._lstReferences = new gitter.Git.Gui.Controls.ReferencesListBox();
			this.SuspendLayout();
			// 
			// _txtRevision
			// 
			this._txtRevision.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._txtRevision.Location = new System.Drawing.Point(94, 3);
			this._txtRevision.Name = "_txtRevision";
			this._txtRevision.Size = new System.Drawing.Size(288, 23);
			this._txtRevision.TabIndex = 0;
			// 
			// _lblRevision
			// 
			this._lblRevision.AutoSize = true;
			this._lblRevision.Location = new System.Drawing.Point(0, 6);
			this._lblRevision.Name = "_lblRevision";
			this._lblRevision.Size = new System.Drawing.Size(74, 15);
			this._lblRevision.TabIndex = 5;
			this._lblRevision.Text = "%Revision%:";
			// 
			// _lstReferences
			// 
			this._lstReferences.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._lstReferences.DisableContextMenus = true;
			this._lstReferences.HeaderStyle = gitter.Framework.Controls.HeaderStyle.Hidden;
			this._lstReferences.Location = new System.Drawing.Point(3, 28);
			this._lstReferences.Name = "_lstReferences";
			this._lstReferences.ShowTreeLines = true;
			this._lstReferences.Size = new System.Drawing.Size(379, 294);
			this._lstReferences.TabIndex = 1;
			this._lstReferences.SelectionChanged += new System.EventHandler(this.OnSelectionChanged);
			// 
			// CheckoutDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._txtRevision);
			this.Controls.Add(this._lstReferences);
			this.Controls.Add(this._lblRevision);
			this.Name = "CheckoutDialog";
			this.Size = new System.Drawing.Size(385, 325);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private gitter.Git.Gui.Controls.ReferencesListBox _lstReferences;
		private System.Windows.Forms.TextBox _txtRevision;
		private System.Windows.Forms.Label _lblRevision;
	}
}
