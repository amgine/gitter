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
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>"Value" column.</summary>
	public class ConfigParameterValueColumn : CustomListBoxColumn
	{
		public ConfigParameterValueColumn(bool visible)
			: base((int)ColumnId.Value, Resources.StrValue, visible)
		{
			Width = 250;
			SizeMode = ColumnSizeMode.Fill;
		}

		public ConfigParameterValueColumn()
			: this(true)
		{
		}

		private static bool TryGetContent(CustomListBoxItem item, out string value)
		{
			if(item is ConfigParameterListItem cpli)
			{
				value = cpli.DataContext.Value;
				return true;
			}
			value = default;
			return false;
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			Assert.IsNotNull(measureEventArgs);

			if(TryGetContent(measureEventArgs.Item, out var content))
			{
				return measureEventArgs.MeasureText(content);
			}
			return base.OnMeasureSubItem(measureEventArgs);
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			Assert.IsNotNull(paintEventArgs);

			if(TryGetContent(paintEventArgs.Item, out var content))
			{
				paintEventArgs.PaintText(content);
				return;
			}
			base.OnPaintSubItem(paintEventArgs);
		}

		public static Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs, string value)
			=> measureEventArgs.MeasureText(value);

		public static void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs, string value)
			=> paintEventArgs.PaintText(value);

		public override string IdentificationString => "Value";
	}
}
