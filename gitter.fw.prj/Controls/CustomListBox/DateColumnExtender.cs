#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace gitter.Framework.Controls;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;

using Resources = gitter.Framework.Properties.Resources;

/// <summary>Extender for <see cref="DateColumn"/>.</summary>
[ToolboxItem(false)]
[DesignerCategory("")]
public partial class DateColumnExtender : ExtenderBase
{
	private readonly DateColumn _column;
	private bool _disableEvents;

	/// <summary>Create <see cref="DateColumnExtender"/>.</summary>
	/// <param name="column">Host <see cref="DateColumn"/>.</param>
	public DateColumnExtender(DateColumn column)
	{
		Verify.Argument.IsNotNull(column);

		_column = column;

		InitializeComponent();

		_lblDateFormat.Text		= Resources.StrDateFormat.AddColon();
		_lblExample.Text		= Resources.StrExample.AddColon();
			
		_radUnixTimestamp.Text	= Resources.StrUNIXTimestamp;
		_radRelative.Text		= Resources.StrRelative;
		_radSystemDefault.Text	= Resources.StrDefaultFormat;
		_radRFC2822.Text		= Resources.StrRFC2822;
		_radISO8601.Text		= Resources.StrISO8601;

		var date = DateTimeOffset.Now;
		_lblUnixTimestamp.Text	= Utility.FormatDate(date, DateFormat.UnixTimestamp);
		_lblRelative.Text		= Utility.FormatDate(date, DateFormat.Relative);
		_lblSystemDefault.Text	= Utility.FormatDate(date, DateFormat.SystemDefault);
		_lblRFC2822.Text		= Utility.FormatDate(date, DateFormat.RFC2822);
		_lblISO8601.Text		= Utility.FormatDate(date, DateFormat.ISO8601);

		_chkConvertToLocal.Text = Resources.StrConvertDateTimeToLocal;
		_chkShowUTCOffset.Text  = Resources.StrShowUTCOffset;

		DateFormat     = column.DateFormat;
		ConvertToLocal = column.ConvertToLocal;
		ShowUTCOffset  = column.ShowUTCOffset;

		SubscribeToColumnEvents();
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(327, 172));

	private void SubscribeToColumnEvents()
	{
		_column.DateFormatChanged     += OnColumnDateFormatChanged;
		_column.ConvertToLocalChanged += OnConvertToLocalChanged;
		_column.ShowUTCOffsetChanged  += OnShowUTCOffsetChanged;
	}

	private void UnsubscribeFromColumnEvents()
	{
		_column.DateFormatChanged     -= OnColumnDateFormatChanged;
		_column.ConvertToLocalChanged -= OnConvertToLocalChanged;
		_column.ShowUTCOffsetChanged  -= OnShowUTCOffsetChanged;
	}

	private void OnColumnDateFormatChanged(object sender, EventArgs e)
		=> DateFormat = _column.DateFormat;

	private void OnConvertToLocalChanged(object sender, EventArgs e)
		=> ConvertToLocal = _column.ConvertToLocal;

	private void OnShowUTCOffsetChanged(object sender, EventArgs e)
		=> ShowUTCOffset = _column.ShowUTCOffset;

	public DateFormat DateFormat
	{
		get
		{
			if(_radUnixTimestamp.Checked) return DateFormat.UnixTimestamp;
			if(_radRelative.Checked)      return DateFormat.Relative;
			if(_radSystemDefault.Checked) return DateFormat.SystemDefault;
			if(_radRFC2822.Checked)       return DateFormat.RFC2822;
			if(_radISO8601.Checked)       return DateFormat.ISO8601;
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

	public bool ConvertToLocal
	{
		get => _chkConvertToLocal.Checked;
		set
		{
			_disableEvents = true;
			try
			{
				_chkConvertToLocal.Checked = value;
			}
			finally
			{
				_disableEvents = false;
			}
		}
	}

	public bool ShowUTCOffset
	{
		get => _chkShowUTCOffset.Checked;
		set
		{
			_disableEvents = true;
			try
			{
				_chkShowUTCOffset.Checked = value;
			}
			finally
			{
				_disableEvents = false;
			}
		}
	}

	private void OnCheckedChanged(object sender, EventArgs e)
	{
		if(!_disableEvents && ((RadioButton)sender).Checked)
		{
			_column.DateFormat = DateFormat;
		}
	}

	private void _chkConvertToLocal_CheckedChanged(object sender, EventArgs e)
	{
		if(!_disableEvents)
		{
			_column.ConvertToLocal = ConvertToLocal;
		}
	}

	private void _chkShowUTCOffset_CheckedChanged(object sender, EventArgs e)
	{
		if(!_disableEvents)
		{
			_column.ShowUTCOffset = ShowUTCOffset;
		}
	}
}
