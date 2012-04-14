namespace gitter.Git.Gui.Controls
{
	partial class SubjectColumnExtender
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
				UnsubscribeFromColumnEvents();
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
			this._chkLocalBranches = new System.Windows.Forms.CheckBox();
			this._chkRemoteBranches = new System.Windows.Forms.CheckBox();
			this._chkTags = new System.Windows.Forms.CheckBox();
			this._chkStash = new System.Windows.Forms.CheckBox();
			this._chkAlignToGraph = new System.Windows.Forms.CheckBox();
			this._grpVisibleReferences = new gitter.Framework.Controls.GroupSeparator();
			this.SuspendLayout();
			// 
			// _chkLocalBranches
			// 
			this._chkLocalBranches.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._chkLocalBranches.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._chkLocalBranches.Location = new System.Drawing.Point(6, 44);
			this._chkLocalBranches.Name = "_chkLocalBranches";
			this._chkLocalBranches.Size = new System.Drawing.Size(202, 27);
			this._chkLocalBranches.TabIndex = 0;
			this._chkLocalBranches.Text = "%Local Branches%";
			this._chkLocalBranches.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._chkLocalBranches.UseVisualStyleBackColor = true;
			this._chkLocalBranches.CheckedChanged += new System.EventHandler(this.OnLocalBranchesCheckedChanged);
			// 
			// _chkRemoteBranches
			// 
			this._chkRemoteBranches.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._chkRemoteBranches.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._chkRemoteBranches.Location = new System.Drawing.Point(6, 65);
			this._chkRemoteBranches.Name = "_chkRemoteBranches";
			this._chkRemoteBranches.Size = new System.Drawing.Size(202, 27);
			this._chkRemoteBranches.TabIndex = 1;
			this._chkRemoteBranches.Text = "%Remote Branches%";
			this._chkRemoteBranches.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._chkRemoteBranches.UseVisualStyleBackColor = true;
			this._chkRemoteBranches.CheckedChanged += new System.EventHandler(this.OnRemoteBranchesCheckedChanged);
			// 
			// _chkTags
			// 
			this._chkTags.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._chkTags.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._chkTags.Location = new System.Drawing.Point(6, 86);
			this._chkTags.Name = "_chkTags";
			this._chkTags.Size = new System.Drawing.Size(202, 27);
			this._chkTags.TabIndex = 2;
			this._chkTags.Text = "%Tags%";
			this._chkTags.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._chkTags.UseVisualStyleBackColor = true;
			this._chkTags.CheckedChanged += new System.EventHandler(this.OnTagsCheckedChanged);
			// 
			// _chkStash
			// 
			this._chkStash.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._chkStash.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._chkStash.Location = new System.Drawing.Point(6, 107);
			this._chkStash.Name = "_chkStash";
			this._chkStash.Size = new System.Drawing.Size(202, 27);
			this._chkStash.TabIndex = 3;
			this._chkStash.Text = "%Stash%";
			this._chkStash.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._chkStash.UseVisualStyleBackColor = true;
			this._chkStash.CheckedChanged += new System.EventHandler(this.OnStashCheckedChanged);
			// 
			// _chkAlignToGraph
			// 
			this._chkAlignToGraph.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._chkAlignToGraph.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._chkAlignToGraph.Location = new System.Drawing.Point(6, 0);
			this._chkAlignToGraph.Name = "_chkAlignToGraph";
			this._chkAlignToGraph.Size = new System.Drawing.Size(202, 27);
			this._chkAlignToGraph.TabIndex = 0;
			this._chkAlignToGraph.Text = "%Align to Graph%";
			this._chkAlignToGraph.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._chkAlignToGraph.UseVisualStyleBackColor = true;
			this._chkAlignToGraph.CheckedChanged += new System.EventHandler(this.OnAlignToGraphCheckedChanged);
			// 
			// _grpVisibleReferences
			// 
			this._grpVisibleReferences.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._grpVisibleReferences.Location = new System.Drawing.Point(3, 24);
			this._grpVisibleReferences.Name = "_grpVisibleReferences";
			this._grpVisibleReferences.Size = new System.Drawing.Size(208, 19);
			this._grpVisibleReferences.TabIndex = 4;
			this._grpVisibleReferences.Text = "%Visible References%";
			// 
			// SubjectColumnExtender
			// 
			this.Controls.Add(this._chkTags);
			this.Controls.Add(this._grpVisibleReferences);
			this.Controls.Add(this._chkStash);
			this.Controls.Add(this._chkRemoteBranches);
			this.Controls.Add(this._chkLocalBranches);
			this.Controls.Add(this._chkAlignToGraph);
			this.Name = "SubjectColumnExtender";
			this.Size = new System.Drawing.Size(214, 135);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.CheckBox _chkLocalBranches;
		private System.Windows.Forms.CheckBox _chkRemoteBranches;
		private System.Windows.Forms.CheckBox _chkTags;
		private System.Windows.Forms.CheckBox _chkStash;
		private System.Windows.Forms.CheckBox _chkAlignToGraph;
		private gitter.Framework.Controls.GroupSeparator _grpVisibleReferences;
	}
}
