namespace gitter.Git.Gui.Dialogs
{
	partial class AddParameterDialog
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
			this._lblName = new System.Windows.Forms.Label();
			this._lblValue = new System.Windows.Forms.Label();
			this._txtName = new System.Windows.Forms.TextBox();
			this._txtValue = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// _lblName
			// 
			this._lblName.AutoSize = true;
			this._lblName.Location = new System.Drawing.Point(0, 6);
			this._lblName.Name = "_lblName";
			this._lblName.Size = new System.Drawing.Size(62, 15);
			this._lblName.TabIndex = 0;
			this._lblName.Text = "%Name%:";
			// 
			// _lblValue
			// 
			this._lblValue.AutoSize = true;
			this._lblValue.Location = new System.Drawing.Point(0, 32);
			this._lblValue.Name = "_lblValue";
			this._lblValue.Size = new System.Drawing.Size(59, 15);
			this._lblValue.TabIndex = 1;
			this._lblValue.Text = "%Value%:";
			// 
			// _txtName
			// 
			this._txtName.Location = new System.Drawing.Point(94, 3);
			this._txtName.Name = "_txtName";
			this._txtName.Size = new System.Drawing.Size(288, 23);
			this._txtName.TabIndex = 2;
			// 
			// _txtValue
			// 
			this._txtValue.Location = new System.Drawing.Point(94, 29);
			this._txtValue.Name = "_txtValue";
			this._txtValue.Size = new System.Drawing.Size(288, 23);
			this._txtValue.TabIndex = 3;
			// 
			// AddParameterDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._txtValue);
			this.Controls.Add(this._txtName);
			this.Controls.Add(this._lblValue);
			this.Controls.Add(this._lblName);
			this.Name = "AddParameterDialog";
			this.Size = new System.Drawing.Size(385, 55);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _lblName;
		private System.Windows.Forms.Label _lblValue;
		private System.Windows.Forms.TextBox _txtName;
		private System.Windows.Forms.TextBox _txtValue;
	}
}
