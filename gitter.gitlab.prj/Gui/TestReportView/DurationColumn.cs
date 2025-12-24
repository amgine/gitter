#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.GitLab.Gui;

using System;
using System.Drawing;

using gitter.Framework.Controls;

sealed class DurationColumn : CustomListBoxColumn
{
	public DurationColumn() : base(1, "Duration", visible: true)
	{
		Width = 54;
	}

	private static string DurationToString(double value)
	{
		var ts = TimeSpan.FromSeconds(value);
		if(value < 1)            return $"{(int)ts.TotalMilliseconds} ms";
		if(ts.TotalSeconds < 60) return $"{(int)ts.TotalSeconds} s";
		if(ts.TotalMinutes < 60) return $"{(int)ts.TotalMinutes} m";
		                         return $"{(int)ts.TotalHours} h";
	}

	private static string? TryGetDuration(CustomListBoxItem item)
		=> item switch
		{
			TestCaseListBoxItem testCase => DurationToString(testCase.DataContext.ExecutionTime),
			_ => default,
		};

	/// <inheritdoc/>
	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(paintEventArgs);

		var duration = TryGetDuration(paintEventArgs.Item);
		if(duration is not null)
		{
			paintEventArgs.PaintText(duration, StringAlignment.Far);
		}
	}

	/// <inheritdoc/>
	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		Assert.IsNotNull(measureEventArgs);

		var duration = TryGetDuration(measureEventArgs.Item);
		if(duration is not null)
		{
			return measureEventArgs.MeasureText(duration);
		}
		return base.OnMeasureSubItem(measureEventArgs);
	}
}
