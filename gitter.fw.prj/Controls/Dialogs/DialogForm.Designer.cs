namespace gitter.Framework
{
	partial class DialogForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._pnlContainer = new System.Windows.Forms.Panel();
			this._pnlLine = new System.Windows.Forms.Panel();
			this._btnCancel = new System.Windows.Forms.Button();
			this._btnOK = new System.Windows.Forms.Button();
			this._picAdvanced = new System.Windows.Forms.PictureBox();
			this._btnApply = new System.Windows.Forms.Button();
			this._pnlContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._picAdvanced)).BeginInit();
			this.SuspendLayout();
			// 
			// _pnlContainer
			// 
			this._pnlContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._pnlContainer.BackColor = System.Drawing.SystemColors.Window;
			this._pnlContainer.Controls.Add(this._pnlLine);
			this._pnlContainer.Location = new System.Drawing.Point(0, 0);
			this._pnlContainer.Name = "_pnlContainer";
			this._pnlContainer.Size = new System.Drawing.Size(423, 137);
			this._pnlContainer.TabIndex = 0;
			// 
			// _pnlLine
			// 
			this._pnlLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._pnlLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
			this._pnlLine.Location = new System.Drawing.Point(0, 136);
			this._pnlLine.Name = "_pnlLine";
			this._pnlLine.Size = new System.Drawing.Size(423, 1);
			this._pnlLine.TabIndex = 1;
			// 
			// _btnCancel
			// 
			this._btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnCancel.Location = new System.Drawing.Point(260, 145);
			this._btnCancel.Name = "_btnCancel";
			this._btnCancel.Size = new System.Drawing.Size(75, 23);
			this._btnCancel.TabIndex = 2;
			this._btnCancel.Text = global::gitter.Framework.Properties.Resources.StrCancel;
			this._btnCancel.UseVisualStyleBackColor = true;
			this._btnCancel.Click += new System.EventHandler(this._btnCancel_Click);
			// 
			// _btnOK
			// 
			this._btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnOK.Location = new System.Drawing.Point(179, 145);
			this._btnOK.Name = "_btnOK";
			this._btnOK.Size = new System.Drawing.Size(75, 23);
			this._btnOK.TabIndex = 2;
			this._btnOK.Text = global::gitter.Framework.Properties.Resources.StrOk;
			this._btnOK.UseVisualStyleBackColor = true;
			this._btnOK.Click += new System.EventHandler(this._btnOK_Click);
			// 
			// _picAdvanced
			// 
			this._picAdvanced.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._picAdvanced.Cursor = System.Windows.Forms.Cursors.Hand;
			this._picAdvanced.Location = new System.Drawing.Point(7, 146);
			this._picAdvanced.Name = "_picAdvanced";
			this._picAdvanced.Size = new System.Drawing.Size(150, 21);
			this._picAdvanced.TabIndex = 3;
			this._picAdvanced.TabStop = false;
			this._picAdvanced.Visible = false;
			this._picAdvanced.Click += new System.EventHandler(this._picAdvanced_Click);
			this._picAdvanced.MouseEnter += new System.EventHandler(this._picAdvanced_MouseEnter);
			this._picAdvanced.MouseLeave += new System.EventHandler(this._picAdvanced_MouseLeave);
			// 
			// _btnApply
			// 
			this._btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._btnApply.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnApply.Location = new System.Drawing.Point(341, 145);
			this._btnApply.Name = "_btnApply";
			this._btnApply.Size = new System.Drawing.Size(75, 23);
			this._btnApply.TabIndex = 4;
			this._btnApply.Text = global::gitter.Framework.Properties.Resources.StrApply;
			this._btnApply.UseVisualStyleBackColor = true;
			this._btnApply.Click += new System.EventHandler(this._btnApply_Click);
			// 
			// DialogForm
			// 
			this.AcceptButton = this._btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this._btnCancel;
			this.ClientSize = new System.Drawing.Size(423, 176);
			this.Controls.Add(this._btnApply);
			this.Controls.Add(this._picAdvanced);
			this.Controls.Add(this._btnOK);
			this.Controls.Add(this._btnCancel);
			this.Controls.Add(this._pnlContainer);
			this.Font = gitter.Framework.GitterApplication.FontManager.UIFont;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DialogForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "DialogForm";
			this._pnlContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._picAdvanced)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel _pnlLine;
		private System.Windows.Forms.Panel _pnlContainer;
		private System.Windows.Forms.Button _btnCancel;
		private System.Windows.Forms.Button _btnOK;
		private System.Windows.Forms.PictureBox _picAdvanced;
		private System.Windows.Forms.Button _btnApply;
	}
}