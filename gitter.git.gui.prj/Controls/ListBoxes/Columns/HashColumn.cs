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

namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Controls;
	using gitter.Framework.Configuration;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>"Hash" column.</summary>
	public class HashColumn : CustomListBoxColumn
	{
		public const bool DefaultAbbreviate = true;
		public const int DefaultAbbrevLength = 7;

		public static readonly Font Font = new Font("Consolas", 9.0f, FontStyle.Regular, GraphicsUnit.Point);

		#region Data

		private bool _abbreviate;
		private HashColumnExtender _extender;

		#endregion

		#region Events

		public event EventHandler AbbreviateChanged;

		#endregion

		public HashColumn(int id, string name, bool visible)
			: base(id, name, visible)
		{
			Width = 56;
			ContentFont = Font;

			_abbreviate = DefaultAbbreviate;
		}

		public HashColumn()
			: this((int)ColumnId.Hash, Resources.StrHash, false)
		{
		}

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			_extender = new HashColumnExtender(this);
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

		public bool Abbreviate
		{
			get { return _abbreviate; }
			set
			{
				if(_abbreviate != value)
				{
					_abbreviate = value;
					var w = Width;
					AutoSize();
					if(Width != w && ListBox != null)
					{
						ListBox.Refresh();
					}
					AbbreviateChanged.Raise(this);
				}
			}
		}

		private static bool IsAbbreviateEnabled(CustomListBoxColumn column)
		{
			var rhc = column as HashColumn;
			if(rhc != null)
			{
				return rhc.Abbreviate;
			}
			else
			{
				return HashColumn.DefaultAbbreviate;
			}
		}

		public static Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs, string data)
		{
			Assert.IsNotNull(measureEventArgs);
			Assert.IsNotNull(data);

			bool abbreviate = IsAbbreviateEnabled(measureEventArgs.Column);
			return measureEventArgs.MeasureText(
				abbreviate ? data.Substring(0, HashColumn.DefaultAbbrevLength) : (data),
				HashColumn.Font);
		}

		public static Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs, HashStringCache data)
		{
			Assert.IsNotNull(measureEventArgs);
			Assert.IsNotNull(data);

			bool abbreviate = IsAbbreviateEnabled(measureEventArgs.Column);
			return measureEventArgs.MeasureText(
				data.GetValue(abbreviate ? HashColumn.DefaultAbbrevLength : 40),
				HashColumn.Font);
		}

		public static void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs, string data)
		{
			Assert.IsNotNull(paintEventArgs);
			Assert.IsNotNull(data);

			bool abbreviate = IsAbbreviateEnabled(paintEventArgs.Column);
			paintEventArgs.PaintText(
				abbreviate ? data.Substring(0, HashColumn.DefaultAbbrevLength) : (data), paintEventArgs.Font);
		}

		public static void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs, HashStringCache data)
		{
			Assert.IsNotNull(paintEventArgs);
			Assert.IsNotNull(data);

			bool abbreviate = IsAbbreviateEnabled(paintEventArgs.Column);
			paintEventArgs.PaintText(data.GetValue(abbreviate ? HashColumn.DefaultAbbrevLength : 40));
		}

		public static void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs, string data, Brush brush)
		{
			Assert.IsNotNull(paintEventArgs);
			Assert.IsNotNull(data);

			bool abbreviate = IsAbbreviateEnabled(paintEventArgs.Column);
			paintEventArgs.PaintText(
				abbreviate ? data.Substring(0, HashColumn.DefaultAbbrevLength) : (data), paintEventArgs.Font, brush);
		}

		protected override void SaveMoreTo(Section section)
		{
			base.SaveMoreTo(section);
			section.SetValue("Abbreviate", Abbreviate);
		}

		protected override void LoadMoreFrom(Section section)
		{
			base.LoadMoreFrom(section);
			Abbreviate = section.GetValue("Abbreviate", Abbreviate);
		}

		public override string IdentificationString
		{
			get { return "Hash"; }
		}
	}
}
