namespace gitter.Git.Gui.Views
{
	partial class CommitView
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
				_chkAmend.Dispose();
				_btnCommit.Dispose();
				_speller?.Dispose();
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
			this._splitContainer = new System.Windows.Forms.SplitContainer();
			this._tableChanges = new System.Windows.Forms.TableLayoutPanel();
			this._lblUnstaged = new System.Windows.Forms.Label();
			this._lblStaged = new System.Windows.Forms.Label();
			this._lstUnstaged = new gitter.Git.Gui.Controls.TreeListBox();
			this._lstStaged = new gitter.Git.Gui.Controls.TreeListBox();
			this._lblMessage = new System.Windows.Forms.Label();
			this._txtMessage = new System.Windows.Forms.TextBox();
			this._chkAmend = gitter.Framework.GitterApplication.Style.CreateCheckBox();
			this._btnCommit = gitter.Framework.GitterApplication.Style.CreateButton();
			((System.ComponentModel.ISupportInitialize)(this._splitContainer)).BeginInit();
			this._splitContainer.Panel1.SuspendLayout();
			this._splitContainer.Panel2.SuspendLayout();
			this._splitContainer.SuspendLayout();
			this._tableChanges.SuspendLayout();
			this.SuspendLayout();
			// 
			// _splitContainer
			// 
			this._splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this._splitContainer.Location = new System.Drawing.Point(0, 0);
			this._splitContainer.Name = "_splitContainer";
			this._splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// _splitContainer.Panel1
			// 
			this._splitContainer.Panel1.Controls.Add(this._tableChanges);
			this._splitContainer.Panel1MinSize = 150;
			// 
			// _splitContainer.Panel2
			// 
			this._splitContainer.Panel2.Controls.Add(this._lblMessage);
			this._splitContainer.Panel2.Controls.Add(this._txtMessage);
			this._splitContainer.Size = new System.Drawing.Size(555, 362);
			this._splitContainer.SplitterDistance = 239;
			this._splitContainer.TabIndex = 11;
			// 
			// _tableChanges
			// 
			this._tableChanges.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._tableChanges.ColumnCount = 2;
			this._tableChanges.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._tableChanges.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._tableChanges.Controls.Add(this._lblUnstaged, 0, 0);
			this._tableChanges.Controls.Add(this._lblStaged, 1, 0);
			this._tableChanges.Controls.Add(this._lstUnstaged, 0, 1);
			this._tableChanges.Controls.Add(this._lstStaged, 1, 1);
			this._tableChanges.Location = new System.Drawing.Point(3, 0);
			this._tableChanges.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this._tableChanges.Name = "_tableChanges";
			this._tableChanges.RowCount = 2;
			this._tableChanges.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
			this._tableChanges.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableChanges.Size = new System.Drawing.Size(549, 239);
			this._tableChanges.TabIndex = 0;
			// 
			// _lblUnstaged
			// 
			this._lblUnstaged.Dock = System.Windows.Forms.DockStyle.Fill;
			this._lblUnstaged.Location = new System.Drawing.Point(0, 0);
			this._lblUnstaged.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this._lblUnstaged.Name = "_lblUnstaged";
			this._lblUnstaged.Size = new System.Drawing.Size(271, 21);
			this._lblUnstaged.TabIndex = 0;
			this._lblUnstaged.Text = "%Unstaged Changes%:";
			this._lblUnstaged.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _lblStaged
			// 
			this._lblStaged.Dock = System.Windows.Forms.DockStyle.Fill;
			this._lblStaged.Location = new System.Drawing.Point(274, 0);
			this._lblStaged.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this._lblStaged.Name = "_lblStaged";
			this._lblStaged.Size = new System.Drawing.Size(272, 21);
			this._lblStaged.TabIndex = 1;
			this._lblStaged.Text = "%Staged Changes%:";
			this._lblStaged.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _lstUnstaged
			// 
			this._lstUnstaged.Dock = System.Windows.Forms.DockStyle.Fill;
			this._lstUnstaged.ForeColor = System.Drawing.SystemColors.WindowText;
			this._lstUnstaged.HeaderStyle = gitter.Framework.Controls.HeaderStyle.Hidden;
			this._lstUnstaged.Location = new System.Drawing.Point(0, 21);
			this._lstUnstaged.Margin = new System.Windows.Forms.Padding(0, 0, 2, 4);
			this._lstUnstaged.Multiselect = true;
			this._lstUnstaged.Name = "_lstUnstaged";
			this._lstUnstaged.ShowTreeLines = true;
			this._lstUnstaged.Size = new System.Drawing.Size(272, 214);
			this._lstUnstaged.TabIndex = 2;
			this._lstUnstaged.Text = "%No unstaged changes%";
			// 
			// _lstStaged
			// 
			this._lstStaged.Dock = System.Windows.Forms.DockStyle.Fill;
			this._lstStaged.ForeColor = System.Drawing.SystemColors.WindowText;
			this._lstStaged.HeaderStyle = gitter.Framework.Controls.HeaderStyle.Hidden;
			this._lstStaged.Location = new System.Drawing.Point(276, 21);
			this._lstStaged.Margin = new System.Windows.Forms.Padding(2, 0, 0, 4);
			this._lstStaged.Multiselect = true;
			this._lstStaged.Name = "_lstStaged";
			this._lstStaged.ShowTreeLines = true;
			this._lstStaged.Size = new System.Drawing.Size(273, 214);
			this._lstStaged.TabIndex = 2;
			this._lstStaged.Text = "%No staged changes%";
			// 
			// _lblMessage
			// 
			this._lblMessage.AutoSize = true;
			this._lblMessage.Location = new System.Drawing.Point(0, 0);
			this._lblMessage.Name = "_lblMessage";
			this._lblMessage.Size = new System.Drawing.Size(76, 15);
			this._lblMessage.TabIndex = 13;
			this._lblMessage.Text = "%Message%:";
			// 
			// _txtMessage
			// 
			this._txtMessage.AcceptsReturn = true;
			this._txtMessage.AcceptsTab = true;
			this._txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._txtMessage.Font = new System.Drawing.Font("Consolas", 10F);
			this._txtMessage.Location = new System.Drawing.Point(3, 18);
			this._txtMessage.Multiline = true;
			this._txtMessage.Name = "_txtMessage";
			this._txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this._txtMessage.Size = new System.Drawing.Size(468, 98);
			this._txtMessage.TabIndex = 10;
			//
			// _chkAmend
			//
			this._chkAmend.Control.Bounds = new System.Drawing.Rectangle(477, 67, 71, 20);
			this._chkAmend.Control.Anchor = System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom;
			this._chkAmend.Control.Parent = _splitContainer.Panel2;
			this._chkAmend.Control.Enabled = false;
			this._chkAmend.Control.TabIndex = 12;
			this._chkAmend.IsCheckedChanged += OnAmendCheckedChanged;
			//
			// _btnCommit
			//
			this._btnCommit.Control.Bounds = new System.Drawing.Rectangle(477, 93, 75, 23);
			this._btnCommit.Control.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this._btnCommit.Control.Parent = _splitContainer.Panel2;
			this._btnCommit.Control.TabIndex = 11;
			this._btnCommit.Click += OnCommitClick;
			// 
			// CommitView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this._splitContainer);
			this.Name = "CommitView";
			this._splitContainer.Panel1.ResumeLayout(false);
			this._splitContainer.Panel2.ResumeLayout(false);
			this._splitContainer.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._splitContainer)).EndInit();
			this._splitContainer.ResumeLayout(false);
			this._tableChanges.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.SplitContainer _splitContainer;
		private System.Windows.Forms.TextBox _txtMessage;
		private System.Windows.Forms.TableLayoutPanel _tableChanges;
		private System.Windows.Forms.Label _lblUnstaged;
		private System.Windows.Forms.Label _lblStaged;
		private Controls.TreeListBox _lstUnstaged;
		private Controls.TreeListBox _lstStaged;
		private System.Windows.Forms.Label _lblMessage;
		private gitter.Framework.Controls.ICheckBoxWidget _chkAmend;
		private gitter.Framework.Controls.IButtonWidget _btnCommit;
	}
}
