namespace gitter.Git.Gui.Dialogs
{
	partial class RemoveRemoteBranchDialog
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
			this._cmdRemoveLocalOnly = new gitter.Framework.Controls.CommandLink();
			this._cmdRemoveFromRemote = new gitter.Framework.Controls.CommandLink();
			this._lblRemoveBranch = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _cmdRemoveLocalOnly
			// 
			this._cmdRemoveLocalOnly.Description = "%Remove tracking branch from local repository without touching real branch on rem" +
				"ote repository%";
			this._cmdRemoveLocalOnly.Location = new System.Drawing.Point(15, 38);
			this._cmdRemoveLocalOnly.Name = "_cmdRemoveLocalOnly";
			this._cmdRemoveLocalOnly.Size = new System.Drawing.Size(319, 66);
			this._cmdRemoveLocalOnly.TabIndex = 0;
			this._cmdRemoveLocalOnly.Text = "%Local repository only%";
			this._cmdRemoveLocalOnly.Click += new System.EventHandler(this.OnRemoveLocalOnlyClick);
			// 
			// _cmdRemoveFromRemote
			// 
			this._cmdRemoveFromRemote.Description = "Remove branch from \'{0}\' and local repository (this is a potentially dangerous op" +
				"eration)";
			this._cmdRemoveFromRemote.Location = new System.Drawing.Point(15, 120);
			this._cmdRemoveFromRemote.Name = "_cmdRemoveFromRemote";
			this._cmdRemoveFromRemote.Size = new System.Drawing.Size(319, 66);
			this._cmdRemoveFromRemote.TabIndex = 1;
			this._cmdRemoveFromRemote.Text = "%Local and remote repository%";
			this._cmdRemoveFromRemote.Click += new System.EventHandler(this.OnRemoveFromRemoteClick);
			// 
			// _lblRemoveBranch
			// 
			this._lblRemoveBranch.AutoSize = true;
			this._lblRemoveBranch.Location = new System.Drawing.Point(12, 10);
			this._lblRemoveBranch.Name = "_lblRemoveBranch";
			this._lblRemoveBranch.Size = new System.Drawing.Size(165, 15);
			this._lblRemoveBranch.TabIndex = 2;
			this._lblRemoveBranch.Text = "%Remove branch \'{0}\' from%:";
			// 
			// RemoveRemoteBranchDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._lblRemoveBranch);
			this.Controls.Add(this._cmdRemoveFromRemote);
			this.Controls.Add(this._cmdRemoveLocalOnly);
			this.Name = "RemoveRemoteBranchDialog";
			this.Size = new System.Drawing.Size(350, 202);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private gitter.Framework.Controls.CommandLink _cmdRemoveLocalOnly;
		private gitter.Framework.Controls.CommandLink _cmdRemoveFromRemote;
		private System.Windows.Forms.Label _lblRemoveBranch;
	}
}
