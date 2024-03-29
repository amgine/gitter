﻿#region Copyright Notice
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
using System.Drawing;

using gitter.Framework;
using gitter.Framework.Configuration;

using Resources = gitter.Framework.Properties.Resources;

public class DateColumn : CustomListBoxColumn
{
	public const DateFormat DefaultDateFormat = DateFormat.SystemDefault;

	private DateFormat _dateFormat;
	private bool _convertToLocal;
	private bool _showUTCOffset = true;
	private DateColumnExtender _extender;

	public event EventHandler DateFormatChanged;

	protected virtual void OnDateFormatChanged(EventArgs e)
		=> DateFormatChanged?.Invoke(this, e);

	public event EventHandler ConvertToLocalChanged;

	protected virtual void OnConvertToLocalChanged(EventArgs e)
		=> ConvertToLocalChanged?.Invoke(this, e);

	public event EventHandler ShowUTCOffsetChanged;

	protected virtual void OnShowUTCOffsetChanged(EventArgs e)
		=> ShowUTCOffsetChanged?.Invoke(this, e);

	public DateColumn(int id, string name, bool visible)
		: base(id, name, visible)
	{
		_dateFormat = DefaultDateFormat;
		Width = 106;
	}

	public DateColumn(int id, bool visible)
		: this(id, Resources.StrDate, visible)
	{
	}

	/// <inheritdoc/>
	protected override void OnListBoxAttached()
	{
		base.OnListBoxAttached();
		Extender = new Popup(_extender = new DateColumnExtender(this));
	}

	/// <inheritdoc/>
	protected override void OnListBoxDetached()
	{
		Extender.Dispose();
		Extender = null;
		_extender.Dispose();
		_extender = null;
		base.OnListBoxDetached();
	}

	private static string GetString(CustomListBoxColumn column, DateTimeOffset date)
	{
		bool convertTolocal;
		DateFormat format;
		bool showUTCOffset;
		if(column is DateColumn dc)
		{
			format         = dc.DateFormat;
			convertTolocal = dc.ConvertToLocal;
			showUTCOffset  = dc.ShowUTCOffset;
		}
		else
		{
			convertTolocal = false;
			format         = DefaultDateFormat;
			showUTCOffset  = true;
		}
		if(convertTolocal) date = date.ToLocalTime();
		return Utility.FormatDate(date, format, showUTCOffset);
	}

#if NETCOREAPP

	private static bool TryGetText(CustomListBoxColumn column, Span<char> text, out int charsWritten, DateTimeOffset date)
	{
		bool convertToLocal;
		DateFormat format;
		bool showUTCOffset;
		if(column is DateColumn dc)
		{
			format         = dc.DateFormat;
			convertToLocal = dc.ConvertToLocal;
			showUTCOffset  = dc.ShowUTCOffset;
		}
		else
		{
			format         = DefaultDateFormat;
			convertToLocal = false;
			showUTCOffset  = true;
		}
		if(convertToLocal) date = date.ToLocalTime();
		return Utility.TryFormatDate(date, text, out charsWritten, format, showUTCOffset);
	}

#endif

	public static Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs, DateTimeOffset date)
	{
		Assert.IsNotNull(measureEventArgs);

#if NETCOREAPP
		Span<char> buffer = stackalloc char[64];
		if(TryGetText(measureEventArgs.Column, buffer, out var charsWritten, date))
		{
			buffer = buffer.Slice(0, charsWritten);
			return measureEventArgs.MeasureText(buffer);
		}
#endif
		return measureEventArgs.MeasureText(GetString(measureEventArgs.Column, date));
	}


	public static void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs, DateTimeOffset date)
	{
		Assert.IsNotNull(paintEventArgs);

#if NETCOREAPP
		Span<char> buffer = stackalloc char[64];
		if(TryGetText(paintEventArgs.Column, buffer, out var charsWritten, date))
		{
			buffer = buffer.Slice(0, charsWritten);
			paintEventArgs.PaintText(buffer);
			return;
		}
#endif
		paintEventArgs.PaintText(GetString(paintEventArgs.Column, date));
	}

	public DateFormat DateFormat
	{
		get => _dateFormat;
		set
		{
			if(_dateFormat != value)
			{
				_dateFormat = value;
				var w = Width;
				AutoSize(80);
				if(w != Width)
				{
					ListBox?.Refresh();
				}
				OnDateFormatChanged(EventArgs.Empty);
			}
		}
	}

	public bool ConvertToLocal
	{
		get => _convertToLocal;
		set
		{
			if(_convertToLocal != value)
			{
				_convertToLocal = value;
				ListBox?.Refresh();
				OnConvertToLocalChanged(EventArgs.Empty);
			}
		}
	}

	public bool ShowUTCOffset
	{
		get => _showUTCOffset;
		set
		{
			if(_showUTCOffset != value)
			{
				_showUTCOffset = value;
				var w = Width;
				AutoSize(80);
				if(w != Width)
				{
					ListBox?.Refresh();
				}
				OnShowUTCOffsetChanged(EventArgs.Empty);
			}
		}
	}

	/// <inheritdoc/>
	protected override void LoadMoreFrom(Section section)
	{
		base.LoadMoreFrom(section);
		DateFormat     = section.GetValue("DateFormat",     DateFormat);
		ConvertToLocal = section.GetValue("ConvertToLocal", ConvertToLocal);
		ShowUTCOffset  = section.GetValue("ShowUTCOffset",  ShowUTCOffset);
	}

	/// <inheritdoc/>
	protected override void SaveMoreTo(Section section)
	{
		base.SaveMoreTo(section);
		section.SetValue("DateFormat",     DateFormat);
		section.SetValue("ConvertToLocal", ConvertToLocal);
		section.SetValue("ShowUTCOffset",  ShowUTCOffset);
	}

	/// <inheritdoc/>
	public override string IdentificationString => "Date";
}
