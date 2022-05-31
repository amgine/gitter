namespace gitter.Framework.Services
{
	partial class MessageBoxForm
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
				components?.Dispose();
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
			this._pnlContainer = new System.Windows.Forms.Panel();
			this._lblMessage = new System.Windows.Forms.Label();
			this._picIcon = new System.Windows.Forms.PictureBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this._pnlContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._picIcon)).BeginInit();
			this.SuspendLayout();
			// 
			// _pnlContainer
			// 
			this._pnlContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._pnlContainer.BackColor = System.Drawing.SystemColors.Window;
			this._pnlContainer.Controls.Add(this._lblMessage);
			this._pnlContainer.Controls.Add(this._picIcon);
			this._pnlContainer.Controls.Add(this.panel1);
			this._pnlContainer.Location = new System.Drawing.Point(0, 0);
			this._pnlContainer.Name = "_pnlContainer";
			this._pnlContainer.Size = new System.Drawing.Size(481, 137);
			this._pnlContainer.TabIndex = 0;
			// 
			// _lblMessage
			// 
			this._lblMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._lblMessage.Location = new System.Drawing.Point(62, 26);
			this._lblMessage.Name = "_lblMessage";
			this._lblMessage.Size = new System.Drawing.Size(386, 84);
			this._lblMessage.TabIndex = 4;
			// 
			// _picIcon
			// 
			this._picIcon.Location = new System.Drawing.Point(25, 26);
			this._picIcon.Name = "_picIcon";
			this._picIcon.Size = new System.Drawing.Size(32, 32);
			this._picIcon.TabIndex = 3;
			this._picIcon.TabStop = false;
			this._picIcon.Paint += new System.Windows.Forms.PaintEventHandler(this.OnIconPaint);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
			this.panel1.Location = new System.Drawing.Point(0, 136);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(481, 1);
			this.panel1.TabIndex = 2;
			// 
			// MessageBoxForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(481, 176);
			this.Controls.Add(this._pnlContainer);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MessageBoxForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "MessageBoxForm";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MessageBoxForm_KeyDown);
			this._pnlContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._picIcon)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel _pnlContainer;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label _lblMessage;
		private System.Windows.Forms.PictureBox _picIcon;
	}
}
