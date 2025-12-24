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

namespace gitter.Git.Gui;

using gitter.Framework.Configuration;

public record class GraphStyleOptions(
	bool BreakLinesWithDot = false,
	bool RoundedCorners    = false,
	bool ColorNodes        = false,
	int  BaseLineWidth     = 1,
	int  NodeRadius        = 3)
{
	public static GraphStyleOptions Default { get; } = new();

	public static void SaveTo(GraphStyleOptions options, Section section)
	{
		Verify.Argument.IsNotNull(options);
		Verify.Argument.IsNotNull(section);

		section.SetValue("BreakLinesWithDot", options.BreakLinesWithDot);
		section.SetValue("RoundedCorners",    options.RoundedCorners);
		section.SetValue("ColorNodes",        options.ColorNodes);
		section.SetValue("BaseLineWidth",     options.BaseLineWidth);
		section.SetValue("NodeRadius",        options.NodeRadius);
	}

	public static GraphStyleOptions LoadFrom(Section section)
	{
		Verify.Argument.IsNotNull(section);

		return new GraphStyleOptions(
			BreakLinesWithDot: section.GetValue("BreakLinesWithDot", Default.BreakLinesWithDot),
			RoundedCorners:    section.GetValue("RoundedCorners",    Default.RoundedCorners),
			ColorNodes:        section.GetValue("ColorNodes",        Default.ColorNodes),
			BaseLineWidth:     section.GetValue("BaseLineWidth",     Default.BaseLineWidth),
			NodeRadius:        section.GetValue("NodeRadius",        Default.NodeRadius));
	}
}
