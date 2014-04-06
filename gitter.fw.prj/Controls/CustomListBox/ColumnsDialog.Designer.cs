namespace gitter.Framework.Controls
{
	partial class ColumnsDialog
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
			this._lblVisibleColumns = new System.Windows.Forms.Label();
			this._lstColumns = new gitter.Framework.Controls.CustomListBox();
			this._btnHide = new System.Windows.Forms.Button();
			this._btnUp = new System.Windows.Forms.Button();
			this._btnShow = new System.Windows.Forms.Button();
			this._btnDown = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _lblVisibleColumns
			// 
			this._lblVisibleColumns.AutoSize = true;
			this._lblVisibleColumns.Location = new System.Drawing.Point(0, 0);
			this._lblVisibleColumns.Name = "_lblVisibleColumns";
			this._lblVisibleColumns.Size = new System.Drawing.Size(115, 15);
			this._lblVisibleColumns.TabIndex = 19;
			this._lblVisibleColumns.Text = "%Visible Columns%:";
			// 
			// _lstColumns
			// 
			this._lstColumns.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._lstColumns.Location = new System.Drawing.Point(3, 16);
			this._lstColumns.Name = "_lstColumns";
			this._lstColumns.ShowCheckBoxes = true;
			this._lstColumns.Size = new System.Drawing.Size(213, 228);
			this._lstColumns.TabIndex = 18;
			this._lstColumns.SelectionChanged += new System.EventHandler(this._lstColumns_SelectionChanged);
			// 
			// _btnHide
			// 
			this._btnHide.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnHide.Enabled = false;
			this._btnHide.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnHide.Location = new System.Drawing.Point(222, 103);
			this._btnHide.Name = "_btnHide";
			this._btnHide.Size = new System.Drawing.Size(75, 23);
			this._btnHide.TabIndex = 23;
			this._btnHide.Text = "%Hide%";
			this._btnHide.UseVisualStyleBackColor = true;
			this._btnHide.Click += new System.EventHandler(this._btnHide_Click);
			// 
			// _btnUp
			// 
			this._btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnUp.Enabled = false;
			this._btnUp.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnUp.Location = new System.Drawing.Point(222, 16);
			this._btnUp.Name = "_btnUp";
			this._btnUp.Size = new System.Drawing.Size(75, 23);
			this._btnUp.TabIndex = 20;
			this._btnUp.Text = "%Up%";
			this._btnUp.UseVisualStyleBackColor = true;
			this._btnUp.Click += new System.EventHandler(this._btnUp_Click);
			// 
			// _btnShow
			// 
			this._btnShow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnShow.Enabled = false;
			this._btnShow.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnShow.Location = new System.Drawing.Point(222, 74);
			this._btnShow.Name = "_btnShow";
			this._btnShow.Size = new System.Drawing.Size(75, 23);
			this._btnShow.TabIndex = 22;
			this._btnShow.Text = "%Show%";
			this._btnShow.UseVisualStyleBackColor = true;
			this._btnShow.Click += new System.EventHandler(this._btnShow_Click);
			// 
			// _btnDown
			// 
			this._btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnDown.Enabled = false;
			this._btnDown.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._btnDown.Location = new System.Drawing.Point(222, 45);
			this._btnDown.Name = "_btnDown";
			this._btnDown.Size = new System.Drawing.Size(75, 23);
			this._btnDown.TabIndex = 21;
			this._btnDown.Text = "%Down%";
			this._btnDown.UseVisualStyleBackColor = true;
			this._btnDown.Click += new System.EventHandler(this._btnDown_Click);
			// 
			// ColumnsDialog
			// 
			this.Controls.Add(this._lblVisibleColumns);
			this.Controls.Add(this._lstColumns);
			this.Controls.Add(this._btnHide);
			this.Controls.Add(this._btnUp);
			this.Controls.Add(this._btnShow);
			this.Controls.Add(this._btnDown);
			this.Name = "ColumnsDialog";
			this.Size = new System.Drawing.Size(300, 247);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private CustomListBox _lstColumns;
		private System.Windows.Forms.Button _btnHide;
		private System.Windows.Forms.Button _btnUp;
		private System.Windows.Forms.Button _btnShow;
		private System.Windows.Forms.Button _btnDown;
		private System.Windows.Forms.Label _lblVisibleColumns;

	}
}
