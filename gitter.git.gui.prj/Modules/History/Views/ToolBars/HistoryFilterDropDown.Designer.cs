namespace gitter.Git.Gui.Views
{
	partial class HistoryFilterDropDown
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
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.radioButton2 = new System.Windows.Forms.RadioButton();
			this.radioButton3 = new System.Windows.Forms.RadioButton();
			this._txtSearch = new System.Windows.Forms.TextBox();
			this._lstReferences = new gitter.Git.Gui.Controls.ReferencesListBox();
			this.SuspendLayout();
			// 
			// radioButton1
			// 
			this.radioButton1.AutoSize = true;
			this.radioButton1.Checked = true;
			this.radioButton1.Location = new System.Drawing.Point(3, 3);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(89, 17);
			this.radioButton1.TabIndex = 1;
			this.radioButton1.TabStop = true;
			this.radioButton1.Text = "All references";
			this.radioButton1.UseVisualStyleBackColor = true;
			this.radioButton1.CheckedChanged += new System.EventHandler(this.OnFilterTypeCheckedChanged);
			// 
			// radioButton2
			// 
			this.radioButton2.AutoSize = true;
			this.radioButton2.Location = new System.Drawing.Point(142, 3);
			this.radioButton2.Name = "radioButton2";
			this.radioButton2.Size = new System.Drawing.Size(55, 17);
			this.radioButton2.TabIndex = 2;
			this.radioButton2.Text = "HEAD";
			this.radioButton2.UseVisualStyleBackColor = true;
			this.radioButton2.CheckedChanged += new System.EventHandler(this.OnFilterTypeCheckedChanged);
			// 
			// radioButton3
			// 
			this.radioButton3.AutoSize = true;
			this.radioButton3.Location = new System.Drawing.Point(3, 25);
			this.radioButton3.Name = "radioButton3";
			this.radioButton3.Size = new System.Drawing.Size(145, 17);
			this.radioButton3.TabIndex = 3;
			this.radioButton3.Text = "Only selected references:";
			this.radioButton3.UseVisualStyleBackColor = true;
			this.radioButton3.CheckedChanged += new System.EventHandler(this.OnFilterTypeCheckedChanged);
			//
			//_searchToolBar
			//
			this._txtSearch.Anchor= ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._txtSearch.Name = "_txtSearch";
			this._txtSearch.Size = new System.Drawing.Size(233, 23);
			this._txtSearch.Location = new System.Drawing.Point(3, 49);
			this._txtSearch.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this._txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			// 
			// _lstReferences
			// 
			this._lstReferences.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._lstReferences.DisableContextMenus = true;
			this._lstReferences.ForeColor = System.Drawing.SystemColors.WindowText;
			this._lstReferences.HeaderStyle = gitter.Framework.Controls.HeaderStyle.Hidden;
			this._lstReferences.Location = new System.Drawing.Point(3, 78);
			this._lstReferences.Name = "_lstReferences";
			this._lstReferences.ShowCheckBoxes = true;
			this._lstReferences.ShowTreeLines = true;
			this._lstReferences.Size = new System.Drawing.Size(233, 196);
			this._lstReferences.TabIndex = 0;
			// 
			// HistoryFilterDropDown
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.radioButton3);
			this.Controls.Add(this.radioButton2);
			this.Controls.Add(this.radioButton1);
			this.Controls.Add(this._txtSearch);
			this.Controls.Add(this._lstReferences);
			this.MaximumSize = new System.Drawing.Size(241, 500);
			this.MinimumSize = new System.Drawing.Size(241, 279);
			this.Name = "HistoryFilterDropDown";
			this.Size = new System.Drawing.Size(239, 279);
			this.VisibleChanged += HistoryFilterDropDown_VisibleChanged;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _txtSearch;
		private Controls.ReferencesListBox _lstReferences;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.RadioButton radioButton3;
	}
}
