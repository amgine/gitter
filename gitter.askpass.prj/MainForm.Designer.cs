namespace gitter;

partial class MainForm
{
	/// <summary>
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <inheritdoc/>
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
		this._lblField = new System.Windows.Forms.Label();
		this._lblPrompt = new System.Windows.Forms.Label();
		this._txtPassword = new System.Windows.Forms.TextBox();
		this._btnCancel = new System.Windows.Forms.Button();
		this._btnOk = new System.Windows.Forms.Button();
		this._pnlContainer = new System.Windows.Forms.Panel();
		this.pictureBox1 = new System.Windows.Forms.PictureBox();
		this._pnlLine = new System.Windows.Forms.Panel();
		this._pnlContainer.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
		this.SuspendLayout();
		// 
		// _lblField
		// 
		this._lblField.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
		this._lblField.AutoSize = true;
		this._lblField.Location = new System.Drawing.Point(49, 51);
		this._lblField.Name = "_lblField";
		this._lblField.Size = new System.Drawing.Size(56, 13);
		this._lblField.TabIndex = 0;
		this._lblField.Text = "Password:";
		// 
		// _lblPrompt
		// 
		this._lblPrompt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
		this._lblPrompt.Location = new System.Drawing.Point(49, 12);
		this._lblPrompt.Name = "_lblPrompt";
		this._lblPrompt.Size = new System.Drawing.Size(341, 33);
		this._lblPrompt.TabIndex = 1;
		this._lblPrompt.Text = "Requested operation requires your password.";
		// 
		// _txtPassword
		// 
		this._txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
		this._txtPassword.Location = new System.Drawing.Point(111, 48);
		this._txtPassword.Name = "_txtPassword";
		this._txtPassword.PasswordChar = '*';
		this._txtPassword.Size = new System.Drawing.Size(279, 20);
		this._txtPassword.TabIndex = 2;
		this._txtPassword.UseSystemPasswordChar = true;
		// 
		// _btnCancel
		// 
		this._btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
		this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this._btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
		this._btnCancel.Location = new System.Drawing.Point(321, 91);
		this._btnCancel.Name = "_btnCancel";
		this._btnCancel.Size = new System.Drawing.Size(75, 23);
		this._btnCancel.TabIndex = 8;
		this._btnCancel.Text = "Cancel";
		this._btnCancel.UseVisualStyleBackColor = true;
		this._btnCancel.Click += new System.EventHandler(this._btnCancel_Click);
		// 
		// _btnOk
		// 
		this._btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
		this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
		this._btnOk.FlatStyle = System.Windows.Forms.FlatStyle.System;
		this._btnOk.Location = new System.Drawing.Point(240, 91);
		this._btnOk.Name = "_btnOk";
		this._btnOk.Size = new System.Drawing.Size(75, 23);
		this._btnOk.TabIndex = 6;
		this._btnOk.Text = "OK";
		this._btnOk.UseVisualStyleBackColor = true;
		this._btnOk.Click += new System.EventHandler(this._btnOk_Click);
		// 
		// _pnlContainer
		// 
		this._pnlContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
		this._pnlContainer.BackColor = System.Drawing.SystemColors.Window;
		this._pnlContainer.Controls.Add(this.pictureBox1);
		this._pnlContainer.Controls.Add(this._pnlLine);
		this._pnlContainer.Controls.Add(this._lblPrompt);
		this._pnlContainer.Controls.Add(this._lblField);
		this._pnlContainer.Controls.Add(this._txtPassword);
		this._pnlContainer.Location = new System.Drawing.Point(1, 0);
		this._pnlContainer.Name = "_pnlContainer";
		this._pnlContainer.Size = new System.Drawing.Size(402, 83);
		this._pnlContainer.TabIndex = 5;
		// 
		// pictureBox1
		// 
		this.pictureBox1.Location = new System.Drawing.Point(11, 12);
		this.pictureBox1.Name = "pictureBox1";
		this.pictureBox1.Size = new System.Drawing.Size(32, 32);
		this.pictureBox1.TabIndex = 9;
		this.pictureBox1.TabStop = false;
		// 
		// _pnlLine
		// 
		this._pnlLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
		this._pnlLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
		this._pnlLine.Location = new System.Drawing.Point(0, 82);
		this._pnlLine.Name = "_pnlLine";
		this._pnlLine.Size = new System.Drawing.Size(402, 1);
		this._pnlLine.TabIndex = 1;
		// 
		// MainForm
		// 
		this.AcceptButton = this._btnOk;
		this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
		this.CancelButton = this._btnCancel;
		this.ClientSize = new System.Drawing.Size(403, 122);
		this.Controls.Add(this._btnCancel);
		this.Controls.Add(this._btnOk);
		this.Controls.Add(this._pnlContainer);
		this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		this.MaximizeBox = false;
		this.MinimizeBox = false;
		this.Name = "MainForm";
		this.ShowIcon = false;
		this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "gitter";
		this._pnlContainer.ResumeLayout(false);
		this._pnlContainer.PerformLayout();
		((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
		this.ResumeLayout(false);

	}

	#endregion

	private System.Windows.Forms.Label _lblField;
	private System.Windows.Forms.Label _lblPrompt;
	private System.Windows.Forms.TextBox _txtPassword;
	private System.Windows.Forms.Button _btnCancel;
	private System.Windows.Forms.Button _btnOk;
	private System.Windows.Forms.Panel _pnlContainer;
	private System.Windows.Forms.Panel _pnlLine;
	private System.Windows.Forms.PictureBox pictureBox1;
}
