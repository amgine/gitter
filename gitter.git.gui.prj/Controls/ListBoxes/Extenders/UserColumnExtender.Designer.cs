namespace gitter.Git.Gui.Controls
{
	partial class UserColumnExtender
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._chkShowEmail = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// _chkShowEmail
			// 
			this._chkShowEmail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._chkShowEmail.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._chkShowEmail.Location = new System.Drawing.Point(6, 0);
			this._chkShowEmail.Name = "_chkShowEmail";
			this._chkShowEmail.Size = new System.Drawing.Size(127, 27);
			this._chkShowEmail.TabIndex = 0;
			this._chkShowEmail.Text = "%Show Email%";
			this._chkShowEmail.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._chkShowEmail.UseVisualStyleBackColor = true;
			this._chkShowEmail.CheckedChanged += new System.EventHandler(this.OnShowEmailCheckedChanged);
			// 
			// UserColumnExtender
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._chkShowEmail);
			this.Name = "UserColumnExtender";
			this.Size = new System.Drawing.Size(138, 28);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.CheckBox _chkShowEmail;
	}
}
