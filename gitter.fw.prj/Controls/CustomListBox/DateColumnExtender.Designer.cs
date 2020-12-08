namespace gitter.Framework.Controls
{
	partial class DateColumnExtender
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
			this._radUnixTimestamp = new System.Windows.Forms.RadioButton();
			this._radRelative = new System.Windows.Forms.RadioButton();
			this._radSystemDefault = new System.Windows.Forms.RadioButton();
			this._radISO8601 = new System.Windows.Forms.RadioButton();
			this._radRFC2822 = new System.Windows.Forms.RadioButton();
			this._lblUnixTimestamp = new System.Windows.Forms.Label();
			this._lblRelative = new System.Windows.Forms.Label();
			this._lblSystemDefault = new System.Windows.Forms.Label();
			this._lblISO8601 = new System.Windows.Forms.Label();
			this._lblRFC2822 = new System.Windows.Forms.Label();
			this._lblDateFormat = new System.Windows.Forms.Label();
			this._lblExample = new System.Windows.Forms.Label();
			this._chkConvertToLocal = new System.Windows.Forms.CheckBox();
			this._chkShowUTCOffset = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// _radUnixTimestamp
			// 
			this._radUnixTimestamp.AutoSize = true;
			this._radUnixTimestamp.Location = new System.Drawing.Point(6, 22);
			this._radUnixTimestamp.Name = "_radUnixTimestamp";
			this._radUnixTimestamp.Size = new System.Drawing.Size(94, 19);
			this._radUnixTimestamp.TabIndex = 0;
			this._radUnixTimestamp.TabStop = true;
			this._radUnixTimestamp.Text = "radioButton1";
			this._radUnixTimestamp.UseVisualStyleBackColor = true;
			this._radUnixTimestamp.CheckedChanged += new System.EventHandler(this.OnCheckedChanged);
			// 
			// _radRelative
			// 
			this._radRelative.AutoSize = true;
			this._radRelative.Location = new System.Drawing.Point(6, 42);
			this._radRelative.Name = "_radRelative";
			this._radRelative.Size = new System.Drawing.Size(94, 19);
			this._radRelative.TabIndex = 1;
			this._radRelative.TabStop = true;
			this._radRelative.Text = "radioButton2";
			this._radRelative.UseVisualStyleBackColor = true;
			this._radRelative.CheckedChanged += new System.EventHandler(this.OnCheckedChanged);
			// 
			// _radSystemDefault
			// 
			this._radSystemDefault.AutoSize = true;
			this._radSystemDefault.Checked = true;
			this._radSystemDefault.Location = new System.Drawing.Point(6, 62);
			this._radSystemDefault.Name = "_radSystemDefault";
			this._radSystemDefault.Size = new System.Drawing.Size(94, 19);
			this._radSystemDefault.TabIndex = 2;
			this._radSystemDefault.TabStop = true;
			this._radSystemDefault.Text = "radioButton3";
			this._radSystemDefault.UseVisualStyleBackColor = true;
			this._radSystemDefault.CheckedChanged += new System.EventHandler(this.OnCheckedChanged);
			// 
			// _radISO8601
			// 
			this._radISO8601.AutoSize = true;
			this._radISO8601.Location = new System.Drawing.Point(6, 82);
			this._radISO8601.Name = "_radISO8601";
			this._radISO8601.Size = new System.Drawing.Size(94, 19);
			this._radISO8601.TabIndex = 3;
			this._radISO8601.TabStop = true;
			this._radISO8601.Text = "radioButton4";
			this._radISO8601.UseVisualStyleBackColor = true;
			this._radISO8601.CheckedChanged += new System.EventHandler(this.OnCheckedChanged);
			// 
			// _radRFC2822
			// 
			this._radRFC2822.AutoSize = true;
			this._radRFC2822.Location = new System.Drawing.Point(6, 102);
			this._radRFC2822.Name = "_radRFC2822";
			this._radRFC2822.Size = new System.Drawing.Size(94, 19);
			this._radRFC2822.TabIndex = 4;
			this._radRFC2822.TabStop = true;
			this._radRFC2822.Text = "radioButton5";
			this._radRFC2822.UseVisualStyleBackColor = true;
			this._radRFC2822.CheckedChanged += new System.EventHandler(this.OnCheckedChanged);
			// 
			// _lblUnixTimestamp
			// 
			this._lblUnixTimestamp.AutoSize = true;
			this._lblUnixTimestamp.Location = new System.Drawing.Point(137, 24);
			this._lblUnixTimestamp.Name = "_lblUnixTimestamp";
			this._lblUnixTimestamp.Size = new System.Drawing.Size(38, 15);
			this._lblUnixTimestamp.TabIndex = 5;
			this._lblUnixTimestamp.Text = "label1";
			// 
			// _lblRelative
			// 
			this._lblRelative.AutoSize = true;
			this._lblRelative.Location = new System.Drawing.Point(137, 44);
			this._lblRelative.Name = "_lblRelative";
			this._lblRelative.Size = new System.Drawing.Size(38, 15);
			this._lblRelative.TabIndex = 6;
			this._lblRelative.Text = "label2";
			// 
			// _lblSystemDefault
			// 
			this._lblSystemDefault.AutoSize = true;
			this._lblSystemDefault.Location = new System.Drawing.Point(137, 64);
			this._lblSystemDefault.Name = "_lblSystemDefault";
			this._lblSystemDefault.Size = new System.Drawing.Size(38, 15);
			this._lblSystemDefault.TabIndex = 7;
			this._lblSystemDefault.Text = "label3";
			// 
			// _lblISO8601
			// 
			this._lblISO8601.AutoSize = true;
			this._lblISO8601.Location = new System.Drawing.Point(137, 84);
			this._lblISO8601.Name = "_lblISO8601";
			this._lblISO8601.Size = new System.Drawing.Size(38, 15);
			this._lblISO8601.TabIndex = 8;
			this._lblISO8601.Text = "label4";
			// 
			// _lblRFC2822
			// 
			this._lblRFC2822.AutoSize = true;
			this._lblRFC2822.Location = new System.Drawing.Point(137, 104);
			this._lblRFC2822.Name = "_lblRFC2822";
			this._lblRFC2822.Size = new System.Drawing.Size(38, 15);
			this._lblRFC2822.TabIndex = 9;
			this._lblRFC2822.Text = "label5";
			// 
			// _lblDateFormat
			// 
			this._lblDateFormat.AutoSize = true;
			this._lblDateFormat.Location = new System.Drawing.Point(3, 4);
			this._lblDateFormat.Name = "_lblDateFormat";
			this._lblDateFormat.Size = new System.Drawing.Size(95, 15);
			this._lblDateFormat.TabIndex = 10;
			this._lblDateFormat.Text = "%Date Format%:";
			// 
			// _lblExample
			// 
			this._lblExample.AutoSize = true;
			this._lblExample.Location = new System.Drawing.Point(137, 4);
			this._lblExample.Name = "_lblExample";
			this._lblExample.Size = new System.Drawing.Size(75, 15);
			this._lblExample.TabIndex = 11;
			this._lblExample.Text = "%Example%:";
			// 
			// _chkConvertToLocal
			// 
			this._chkConvertToLocal.AutoSize = true;
			this._chkConvertToLocal.Location = new System.Drawing.Point(6, 127);
			this._chkConvertToLocal.Name = "_chkConvertToLocal";
			this._chkConvertToLocal.Size = new System.Drawing.Size(130, 19);
			this._chkConvertToLocal.TabIndex = 12;
			this._chkConvertToLocal.Text = "%Convert to local%";
			this._chkConvertToLocal.UseVisualStyleBackColor = true;
			this._chkConvertToLocal.CheckedChanged += new System.EventHandler(this._chkConvertToLocal_CheckedChanged);
			// 
			// _chkShowUTCOffset
			// 
			this._chkShowUTCOffset.AutoSize = true;
			this._chkShowUTCOffset.Location = new System.Drawing.Point(6, 149);
			this._chkShowUTCOffset.Name = "_chkShowUTCOffset";
			this._chkShowUTCOffset.Size = new System.Drawing.Size(132, 19);
			this._chkShowUTCOffset.TabIndex = 13;
			this._chkShowUTCOffset.Text = "%Show UTC offset%";
			this._chkShowUTCOffset.UseVisualStyleBackColor = true;
			this._chkShowUTCOffset.CheckedChanged += new System.EventHandler(this._chkShowUTCOffset_CheckedChanged);
			// 
			// DateColumnExtender
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._chkShowUTCOffset);
			this.Controls.Add(this._chkConvertToLocal);
			this.Controls.Add(this._lblExample);
			this.Controls.Add(this._lblDateFormat);
			this.Controls.Add(this._lblRFC2822);
			this.Controls.Add(this._lblISO8601);
			this.Controls.Add(this._lblSystemDefault);
			this.Controls.Add(this._lblRelative);
			this.Controls.Add(this._lblUnixTimestamp);
			this.Controls.Add(this._radRFC2822);
			this.Controls.Add(this._radISO8601);
			this.Controls.Add(this._radSystemDefault);
			this.Controls.Add(this._radRelative);
			this.Controls.Add(this._radUnixTimestamp);
			this.Name = "DateColumnExtender";
			this.Size = new System.Drawing.Size(327, 172);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton _radUnixTimestamp;
		private System.Windows.Forms.RadioButton _radRelative;
		private System.Windows.Forms.RadioButton _radSystemDefault;
		private System.Windows.Forms.RadioButton _radISO8601;
		private System.Windows.Forms.RadioButton _radRFC2822;
		private System.Windows.Forms.Label _lblUnixTimestamp;
		private System.Windows.Forms.Label _lblRelative;
		private System.Windows.Forms.Label _lblSystemDefault;
		private System.Windows.Forms.Label _lblISO8601;
		private System.Windows.Forms.Label _lblRFC2822;
		private System.Windows.Forms.Label _lblDateFormat;
		private System.Windows.Forms.Label _lblExample;
		private System.Windows.Forms.CheckBox _chkConvertToLocal;
		private System.Windows.Forms.CheckBox _chkShowUTCOffset;
	}
}
