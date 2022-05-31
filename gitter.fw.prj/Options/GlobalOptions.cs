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
	private static readonly Dictionary<string, SelectableColorCategory> _colorCategories = new();
	private static readonly Dictionary<string, SelectableColor> _colors = new();

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

		var appearanceNode = section.TryGetSection("Appearance");
		if(appearanceNode != null)
		{
			var textRenderer = appearanceNode.TryGetParameter("TextRenderer");
			if(textRenderer != null)
			{
				switch(textRenderer.Value as string)
				{
					case "GDI":
						GitterApplication.TextRenderer = GitterApplication.GdiTextRenderer;
						break;
					case "GDI+":
						GitterApplication.TextRenderer = GitterApplication.GdiPlusTextRenderer;
						break;
				}
			}
		}
		var servicesNode = section.TryGetSection("Services");
		if(servicesNode != null)
		{
			var spellingSection = servicesNode.TryGetSection("Spelling");
			if(spellingSection != null)
			{
				SpellingService.LoadFrom(spellingSection);
			}
		}
		var featuresSection = section.TryGetSection("IntegrationFeatures");
		if(featuresSection != null)
		{
			GitterApplication.IntegrationFeatures.LoadFrom(featuresSection);
		}
	}

	public static void SaveTo(Section section)
	{
		Verify.Argument.IsNotNull(section);

		var appearanceNode = section.GetCreateSection("Appearance");
		if(GitterApplication.TextRenderer == GitterApplication.GdiTextRenderer)
		{
			appearanceNode.SetValue("TextRenderer", "GDI");
		}
		else if(GitterApplication.TextRenderer == GitterApplication.GdiPlusTextRenderer)
		{
			appearanceNode.SetValue("TextRenderer", "GDI+");
		}
		var servicesNode = section.GetCreateSection("Services");
		var spellingNode = servicesNode.GetCreateSection("Spelling");
		SpellingService.SaveTo(spellingNode);
		var featuresSection = section.GetCreateSection("IntegrationFeatures");
		GitterApplication.IntegrationFeatures.SaveTo(featuresSection);
	}
}
