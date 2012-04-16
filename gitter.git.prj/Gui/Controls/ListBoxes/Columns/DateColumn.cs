namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Globalization;
	using System.Text;
	using System.Windows.Forms;
	using System.Drawing;
	using System.Xml;

	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Framework.Configuration;

	using Resources = gitter.Git.Properties.Resources;

	public class DateColumn : CustomListBoxColumn
	{
		public const DateFormat DefaultDateFormat = DateFormat.SystemDefault;

		public static readonly Font Font = new Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point);

		#region Data

		private DateFormat _dateFormat;
		private DateColumnExtender _extender;

		#endregion

		#region Events

		public event EventHandler DateFormatChanged;

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
			return measureEventArgs.MeasureText(strDate, DateColumn.Font);
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
			paintEventArgs.PaintText(strdate, DateColumn.Font);
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
					DateFormatChanged.Raise(this);
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
