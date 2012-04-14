namespace gitter.Git.Gui.Dialogs
{
	partial class ResolveCheckoutDialog
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
			this._chkDontShowAgain = new System.Windows.Forms.CheckBox();
			this._btnCheckoutCommit = new gitter.Framework.Controls.CommandLink();
			this._btnCheckoutBranch = new gitter.Framework.Controls.CommandLink();
			this._references = new gitter.Git.Gui.Controls.ReferencesListBox();
			this._lblSelectOther = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _chkDontShowAgain
			// 
			this._chkDontShowAgain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._chkDontShowAgain.AutoSize = true;
			this._chkDontShowAgain.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._chkDontShowAgain.Location = new System.Drawing.Point(16, 261);
			this._chkDontShowAgain.Name = "_chkDontShowAgain";
			this._chkDontShowAgain.Size = new System.Drawing.Size(256, 20);
			this._chkDontShowAgain.TabIndex = 3;
			this._chkDontShowAgain.Text = "Don\'t ask again, always checkout commits";
			this._chkDontShowAgain.UseVisualStyleBackColor = true;
			// 
			// _btnCheckoutCommit
			// 
			this._btnCheckoutCommit.Description = "This will detach HEAD and make you unable to create new commits, merge, revert, e" +
				"tc.";
			this._btnCheckoutCommit.Location = new System.Drawing.Point(16, 16);
			this._btnCheckoutCommit.Name = "_btnCheckoutCommit";
			this._btnCheckoutCommit.Size = new System.Drawing.Size(319, 66);
			this._btnCheckoutCommit.TabIndex = 0;
			this._btnCheckoutCommit.Text = "Checkout commit";
			this._btnCheckoutCommit.Click += new System.EventHandler(this._btnCheckoutCommit_Click);
			// 
			// _btnCheckoutBranch
			// 
			this._btnCheckoutBranch.Description = "This will bring working tree to the same state, but HEAD will point to selected b" +
				"ranch.";
			this._btnCheckoutBranch.Location = new System.Drawing.Point(16, 98);
			this._btnCheckoutBranch.Name = "_btnCheckoutBranch";
			this._btnCheckoutBranch.Size = new System.Drawing.Size(319, 66);
			this._btnCheckoutBranch.TabIndex = 1;
			this._btnCheckoutBranch.Text = "Checkout \'%BRANCH NAME%\'";
			this._btnCheckoutBranch.Click += new System.EventHandler(this._btnCheckoutBranch_Click);
			// 
			// _references
			// 
			this._references.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._references.HeaderStyle = gitter.Framework.Controls.HeaderStyle.Hidden;
			this._references.ItemActivation = gitter.Framework.Controls.ItemActivation.SingleClick;
			this._references.Location = new System.Drawing.Point(16, 193);
			this._references.Name = "_references";
			this._references.ShowTreeLines = true;
			this._references.Size = new System.Drawing.Size(319, 65);
			this._references.TabIndex = 2;
			this._references.ItemActivated += new System.EventHandler<gitter.Framework.Controls.ItemEventArgs>(this.OnItemActivated);
			// 
			// _lblSelectOther
			// 
			this._lblSelectOther.AutoSize = true;
			this._lblSelectOther.Location = new System.Drawing.Point(13, 177);
			this._lblSelectOther.Name = "_lblSelectOther";
			this._lblSelectOther.Size = new System.Drawing.Size(112, 15);
			this._lblSelectOther.TabIndex = 4;
			this._lblSelectOther.Text = "Select other branch:";
			// 
			// ResolveCheckoutDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._lblSelectOther);
			this.Controls.Add(this._references);
			this.Controls.Add(this._btnCheckoutBranch);
			this.Controls.Add(this._btnCheckoutCommit);
			this.Controls.Add(this._chkDontShowAgain);
			this.Name = "ResolveCheckoutDialog";
			this.Size = new System.Drawing.Size(350, 284);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox _chkDontShowAgain;
		private gitter.Framework.Controls.CommandLink _btnCheckoutCommit;
		private gitter.Framework.Controls.CommandLink _btnCheckoutBranch;
		private gitter.Git.Gui.Controls.ReferencesListBox _references;
		private System.Windows.Forms.Label _lblSelectOther;
	}
}
