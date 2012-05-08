namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Framework.Properties.Resources;

	/// <summary>Extender for <see cref="DateColumn"/>.</summary>
	[ToolboxItem(false)]
	public partial class DateColumnExtender : BaseExtender
	{
		private readonly DateColumn _column;
		private bool _disableEvents;

		/// <summary>Create <see cref="DateColumnExtender"/>.</summary>
		/// <param name="column">Host <see cref="DateColumn"/>.</param>
		public DateColumnExtender(DateColumn column)
		{
			if(column == null) throw new ArgumentNullException("column");
			_column = column;

			InitializeComponent();

			_lblDateFormat.Text		= Resources.StrDateFormat.AddColon();
			_lblExample.Text		= Resources.StrExample.AddColon();
			
			_radUnixTimestamp.Text	= Resources.StrUNIXTimestamp;
			_radRelative.Text		= Resources.StrRelative;
			_radSystemDefault.Text	= Resources.StrDefaultFormat;
			_radRFC2822.Text		= Resources.StrRFC2822;
			_radISO8601.Text		= Resources.StrISO8601;

			var date = DateTime.Now;
			_lblUnixTimestamp.Text	= Utility.FormatDate(date, DateFormat.UnixTimestamp);
			_lblRelative.Text		= Utility.FormatDate(date, DateFormat.Relative);
			_lblSystemDefault.Text	= Utility.FormatDate(date, DateFormat.SystemDefault);
			_lblRFC2822.Text		= Utility.FormatDate(date, DateFormat.RFC2822);
			_lblISO8601.Text		= Utility.FormatDate(date, DateFormat.ISO8601);

			DateFormat = column.DateFormat;

			SubscribeToColumnEvents();
		}

		private void SubscribeToColumnEvents()
		{
			_column.DateFormatChanged += OnColumnDateFormatChanged;
		}

		private void UnsubscribeFromColumnEvents()
		{
			_column.DateFormatChanged -= OnColumnDateFormatChanged;
		}

		private void OnColumnDateFormatChanged(object sender, EventArgs e)
		{
			DateFormat = _column.DateFormat;
		}

		public DateFormat DateFormat
		{
			get
			{
				if(_radUnixTimestamp.Checked)
				{
					return DateFormat.UnixTimestamp;
				}
				if(_radRelative.Checked)
				{
					return DateFormat.Relative;
				}
				if(_radSystemDefault.Checked)
				{
					return DateFormat.SystemDefault;
				}
				if(_radRFC2822.Checked)
				{
					return DateFormat.RFC2822;
				}
				if(_radISO8601.Checked)
				{
					return DateFormat.ISO8601;
				}
				return DateFormat.SystemDefault;
			}
			set
			{
				_disableEvents = true;
				switch(value)
				{
					case DateFormat.UnixTimestamp:
						_radUnixTimestamp.Checked = true;
						break;
					case DateFormat.Relative:
						_radRelative.Checked = true;
						break;
					case DateFormat.SystemDefault:
						_radSystemDefault.Checked = true;
						break;
					case DateFormat.RFC2822:
						_radRFC2822.Checked = true;
						break;
					case DateFormat.ISO8601:
						_radISO8601.Checked = true;
						break;
				}
				_disableEvents = false;
			}
		}

		private void OnCheckedChanged(object sender, EventArgs e)
		{
			if(!_disableEvents && ((RadioButton)sender).Checked)
			{
				_column.DateFormat = DateFormat;
			}
		}
	}
}
