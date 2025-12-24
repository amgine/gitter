#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework;

using System;
using System.Collections.Generic;

using gitter.Framework.Options;
using gitter.Framework.Services;
using gitter.Framework.Configuration;

public static class GlobalOptions
{
	private static readonly Dictionary<string, SelectableColorCategory> _colorCategories = [];
	private static readonly Dictionary<string, SelectableColor> _colors = [];

	public static void RegisterSelectableColor(SelectableColor color)
	{
		Verify.Argument.IsNotNull(color);

		_colors.Add(color.Id, color);
	}

	public static void RegisterSelectableColorCategory(SelectableColorCategory category)
	{
		Verify.Argument.IsNotNull(category);

		_colorCategories.Add(category.Id, category);
	}

	public static void LoadFrom(Section section)
	{
		Verify.Argument.IsNotNull(section);

		var servicesNode = section.TryGetSection("Services");
		if(servicesNode is not null)
		{
			var spellingSection = servicesNode.TryGetSection("Spelling");
			if(spellingSection is not null)
			{
				SpellingService.LoadFrom(spellingSection);
			}
		}
		var featuresSection = section.TryGetSection("IntegrationFeatures");
		if(featuresSection is not null)
		{
			GitterApplication.IntegrationFeatures.LoadFrom(featuresSection);
		}
	}

	public static void SaveTo(Section section)
	{
		Verify.Argument.IsNotNull(section);

		var servicesNode = section.GetCreateSection("Services");
		var spellingNode = servicesNode.GetCreateSection("Spelling");
		SpellingService.SaveTo(spellingNode);
		var featuresSection = section.GetCreateSection("IntegrationFeatures");
		GitterApplication.IntegrationFeatures.SaveTo(featuresSection);
	}
}
