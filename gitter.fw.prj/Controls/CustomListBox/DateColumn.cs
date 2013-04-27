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
	using System.Windows.Forms;
	using System.Drawing;

	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Framework.Configuration;

	using Resources = gitter.Framework.Properties.Resources;

	public class DateColumn : CustomListBoxColumn
	{
		public const DateFormat DefaultDateFormat = DateFormat.SystemDefault;

		#region Data

		private DateFormat _dateFormat;
		private DateColumnExtender _extender;

		#endregion

		#region Events

		public event EventHandler DateFormatChanged;

		protected virtual void OnDateFormatChanged()
		{
			var handler = DateFormatChanged;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		#endregion

		#region .ctor

		public DateColumn(int id, string name, bool visible)
			: base(id, name, visible)
		{
			_dateFormat = DefaultDateFormat;
		}

		public DateColumn(int id, bool visible)
			: this(id, Resources.StrDate, visible)
		{
		}

		#endregion

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

		public static Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs, DateTime date)
		{
			var dc = measureEventArgs.Column as DateColumn;
			DateFormat format;
			if(dc != null)
			{
				format = dc.DateFormat;
			}
			else
			{
				format = DateColumn.DefaultDateFormat;
			}
			var strDate = Utility.FormatDate(date, format);
			return measureEventArgs.MeasureText(strDate);
		}

		public static void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs, DateTime date)
		{
			DateFormat format;
			var dc = paintEventArgs.Column as DateColumn;
			if(dc != null)
			{
				format = dc.DateFormat;
			}
			else
			{
				format = DateColumn.DefaultDateFormat;
			}
			var strdate = Utility.FormatDate(date, format);
			paintEventArgs.PaintText(strdate);
		}

		public DateFormat DateFormat
		{
			get { return _dateFormat; }
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
					OnDateFormatChanged();
				}
			}
		}

		protected override void LoadMoreFrom(Section section)
		{
			base.LoadMoreFrom(section);
			DateFormat = section.GetValue("DateFormat", DateFormat);
		}

		protected override void SaveMoreTo(Section section)
		{
			base.SaveMoreTo(section);
			section.SetValue("DateFormat", DateFormat);
		}

		public override string IdentificationString
		{
			get { return "Date"; }
		}
	}
}
