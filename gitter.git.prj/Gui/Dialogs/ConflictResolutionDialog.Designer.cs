namespace gitter.Git.Gui.Dialogs
{
	partial class ConflictResolutionDialog
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
			this._btnResolution2 = new gitter.Framework.Controls.CommandLink();
			this._btnResolution1 = new gitter.Framework.Controls.CommandLink();
			this.label1 = new System.Windows.Forms.Label();
			this._lblFileName = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this._lblOursStatus = new System.Windows.Forms.Label();
			this._lblTheirsStatus = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _btnResolution2
			// 
			this._btnResolution2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._btnResolution2.Description = null;
			this._btnResolution2.Location = new System.Drawing.Point(19, 94);
			this._btnResolution2.Name = "_btnResolution2";
			this._btnResolution2.Size = new System.Drawing.Size(319, 26);
			this._btnResolution2.TabIndex = 3;
			this._btnResolution2.Text = "Delete file";
			this._btnResolution2.Click += new System.EventHandler(this._btnResolution2_Click);
			// 
			// _btnResolution1
			// 
			this._btnResolution1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._btnResolution1.Description = null;
			this._btnResolution1.Location = new System.Drawing.Point(19, 61);
			this._btnResolution1.Name = "_btnResolution1";
			this._btnResolution1.Size = new System.Drawing.Size(319, 26);
			this._btnResolution1.TabIndex = 2;
			this._btnResolution1.Text = "Keep modified file";
			this._btnResolution1.Click += new System.EventHandler(this._btnResolution1_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(16, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(28, 15);
			this.label1.TabIndex = 4;
			this.label1.Text = "File:";
			// 
			// _lblFileName
			// 
			this._lblFileName.AutoSize = true;
			this._lblFileName.Location = new System.Drawing.Point(16, 15);
			this._lblFileName.Name = "_lblFileName";
			this._lblFileName.Size = new System.Drawing.Size(75, 15);
			this._lblFileName.TabIndex = 5;
			this._lblFileName.Text = "[FILE_NAME]";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(16, 35);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(69, 15);
			this.label3.TabIndex = 6;
			this.label3.Text = "Ours status:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(169, 35);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(76, 15);
			this.label4.TabIndex = 7;
			this.label4.Text = "Theirs status:";
			// 
			// _lblOursStatus
			// 
			this._lblOursStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._lblOursStatus.Location = new System.Drawing.Point(91, 33);
			this._lblOursStatus.Name = "_lblOursStatus";
			this._lblOursStatus.Size = new System.Drawing.Size(71, 18);
			this._lblOursStatus.TabIndex = 8;
			this._lblOursStatus.Text = "modified";
			this._lblOursStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// _lblTheirsStatus
			// 
			this._lblTheirsStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._lblTheirsStatus.Location = new System.Drawing.Point(251, 33);
			this._lblTheirsStatus.Name = "_lblTheirsStatus";
			this._lblTheirsStatus.Size = new System.Drawing.Size(71, 18);
			this._lblTheirsStatus.TabIndex = 8;
			this._lblTheirsStatus.Text = "modified";
			this._lblTheirsStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ConflictResolutionDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._lblTheirsStatus);
			this.Controls.Add(this._lblOursStatus);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this._lblFileName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._btnResolution2);
			this.Controls.Add(this._btnResolution1);
			this.Name = "ConflictResolutionDialog";
			this.Size = new System.Drawing.Size(350, 133);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Framework.Controls.CommandLink _btnResolution2;
		private Framework.Controls.CommandLink _btnResolution1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label _lblFileName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label _lblOursStatus;
		private System.Windows.Forms.Label _lblTheirsStatus;


	}
}
