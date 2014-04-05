namespace gitter.Controls
{
	partial class AddServiceDialog
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
				if(components != null)
				{
					components.Dispose();
				}
				if(_setupControlCache != null)
				{
					foreach(var ctl in _setupControlCache.Values)
					{
						if(ctl != null)
						{
							ctl.Dispose();
						}
					}
					_setupControlCache.Clear();
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
			this._lblProvider = new System.Windows.Forms.Label();
			this._servicePicker = new gitter.Controls.ServicePicker();
			this.SuspendLayout();
			// 
			// _lblProvider
			// 
			this._lblProvider.AutoSize = true;
			this._lblProvider.Location = new System.Drawing.Point(3, 7);
			this._lblProvider.Name = "_lblProvider";
			this._lblProvider.Size = new System.Drawing.Size(54, 15);
			this._lblProvider.TabIndex = 1;
			this._lblProvider.Text = "Provider:";
			// 
			// _servicePicker
			// 
			this._servicePicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._servicePicker.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this._servicePicker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._servicePicker.FormattingEnabled = true;
			this._servicePicker.Location = new System.Drawing.Point(100, 3);
			this._servicePicker.Name = "_servicePicker";
			this._servicePicker.SelectedIssueTracker = null;
			this._servicePicker.Size = new System.Drawing.Size(273, 24);
			this._servicePicker.TabIndex = 0;
			// 
			// AddServiceDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._lblProvider);
			this.Controls.Add(this._servicePicker);
			this.Name = "AddServiceDialog";
			this.Size = new System.Drawing.Size(376, 30);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ServicePicker _servicePicker;
		private System.Windows.Forms.Label _lblProvider;
	}
}
