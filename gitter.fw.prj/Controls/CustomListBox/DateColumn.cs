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

namespace gitter.Framework.Controls
{
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
		private DateColumnExtender _extender;

		public event EventHandler DateFormatChanged;

		protected virtual void OnDateFormatChanged(EventArgs e)
			=> DateFormatChanged?.Invoke(this, e);

		public event EventHandler ConvertToLocalChanged;

		protected virtual void OnConvertToLocalChanged(EventArgs e)
			=> ConvertToLocalChanged?.Invoke(this, e);

		public DateColumn(int id, string name, bool visible)
			: base(id, name, visible)
		{
			_dateFormat = DefaultDateFormat;
		}

		public DateColumn(int id, bool visible)
			: this(id, Resources.StrDate, visible)
		{
		}

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			_extender = new DateColumnExtender(this);
			Extender = new Popup(_extender);
		}

		protected override void OnListBoxDetached()
		{
			Extender.Dispose();
			Extender = null;
			_extender.Dispose();
			_extender = null;
			base.OnListBoxDetached();
		}

		public static Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs, DateTimeOffset date)
		{
			bool convertTolocal;
			DateFormat format;
			if(measureEventArgs.Column is DateColumn dc)
			{
				format = dc.DateFormat;
				convertTolocal = dc.ConvertToLocal;
			}
			else
			{
				convertTolocal = false;
				format = DefaultDateFormat;
			}
			if(convertTolocal) date = date.ToLocalTime();
			var strDate = Utility.FormatDate(date, format);
			return measureEventArgs.MeasureText(strDate);
		}

		public static void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs, DateTimeOffset date)
		{
			bool convertTolocal;
			DateFormat format;
			if(paintEventArgs.Column is DateColumn dc)
			{
				format = dc.DateFormat;
				convertTolocal = dc.ConvertToLocal;
			}
			else
			{
				convertTolocal = false;
				format = DefaultDateFormat;
			}
			if(convertTolocal) date = date.ToLocalTime();
			var strdate = Utility.FormatDate(date, format);
			paintEventArgs.PaintText(strdate);
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
					if(w != Width && ListBox != null)
					{
						ListBox.Refresh();
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
					if(ListBox != null)
					{
						ListBox.Refresh();
					}
					OnConvertToLocalChanged(EventArgs.Empty);
				}
			}
		}

		protected override void LoadMoreFrom(Section section)
		{
			base.LoadMoreFrom(section);
			DateFormat     = section.GetValue("DateFormat",     DateFormat);
			ConvertToLocal = section.GetValue("ConvertToLocal", ConvertToLocal);
		}

		protected override void SaveMoreTo(Section section)
		{
			base.SaveMoreTo(section);
			section.SetValue("DateFormat", DateFormat);
			section.SetValue("ConvertToLocal", ConvertToLocal);
		}

		public override string IdentificationString => "Date";
	}
}
