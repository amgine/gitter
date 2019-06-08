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

namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;
	using System.IO;
	using System.Windows.Forms;
	using System.Xml;

	using Microsoft.Win32;

	using gitter.Framework.Options;
	using gitter.Framework.Services;
	using gitter.Framework.Configuration;

	using Resources = gitter.Framework.Properties.Resources;

	public static class GlobalOptions
	{
		private static readonly Dictionary<Guid, PropertyPageFactory> _propertyPages;
		private static readonly Dictionary<string, SelectableColorCategory> _colorCategories;
		private static readonly Dictionary<string, SelectableColor> _colors;

		static GlobalOptions()
		{
			_propertyPages = new Dictionary<Guid, PropertyPageFactory>();
			_colorCategories = new Dictionary<string, SelectableColorCategory>();
			_colors = new Dictionary<string, SelectableColor>();

			RegisterPropertyPageFactory(new PropertyPageFactory(
				IntegrationOptionsPage.Guid,
				Resources.StrIntegration,
				null,
				PropertyPageFactory.RootGroupGuid,
				env => new IntegrationOptionsPage()));

			RegisterPropertyPageFactory(new PropertyPageFactory(
				SpellingPage.Guid,
				Resources.StrSpelling,
				null,
				PropertyPageFactory.RootGroupGuid,
				env => new SpellingPage()));

			RegisterPropertyPageFactory(new PropertyPageFactory(
				PropertyPageFactory.AppearanceGroupGuid,
				Resources.StrAppearance,
				null,
				PropertyPageFactory.RootGroupGuid,
				env => new AppearancePage()));

			RegisterPropertyPageFactory(new PropertyPageFactory(
				FontsPage.Guid,
				Resources.StrFonts,
				null,
				PropertyPageFactory.AppearanceGroupGuid,
				env => new FontsPage()));

			RegisterPropertyPageFactory(new PropertyPageFactory(
				ColorsPage.Guid,
				Resources.StrColors,
				null,
				PropertyPageFactory.AppearanceGroupGuid,
				env => new ColorsPage()));
		}

		public static void RegisterSelectableColor(SelectableColor color)
		{
			Verify.Argument.IsNotNull(color, nameof(color));

			_colors.Add(color.Id, color);
		}

		public static void RegisterSelectableColorCategory(SelectableColorCategory category)
		{
			Verify.Argument.IsNotNull(category, nameof(category));

			_colorCategories.Add(category.Id, category);
		}

		public static void RegisterPropertyPageFactory(PropertyPageFactory description)
		{
			Verify.Argument.IsNotNull(description, nameof(description));

			_propertyPages.Add(description.Guid, description);
		}

		public static IList<PropertyPageItem> GetListBoxItems()
		{
			var list = new List<PropertyPageItem>(_propertyPages.Count);
			var dic = new Dictionary<Guid, PropertyPageItem>(_propertyPages.Count);
			foreach(var kvp in _propertyPages)
			{
				var item = new PropertyPageItem(kvp.Value);
				dic.Add(kvp.Key, item);
				if(kvp.Value.GroupGuid != PropertyPageFactory.RootGroupGuid)
				{
					list.Add(item);
				}
			}
			foreach(var item in list)
			{
				if(dic.TryGetValue(item.DataContext.GroupGuid, out var parent))
				{
					parent.Items.Add(item);
					parent.IsExpanded = true;
					dic.Remove(item.DataContext.Guid);
				}
			}
			list.Clear();
			foreach(var kvp in dic)
			{
				list.Add(kvp.Value);
			}
			return list;
		}

		public static bool IsIntegratedInExplorerContextMenu
		{
			get
			{
				try
				{
					using(var key = Registry.ClassesRoot.OpenSubKey(@"Directory\shell\gitter\command", false))
					{
						if(key != null)
						{
							var value = (string)key.GetValue(null, string.Empty);
							if(value == string.Empty) return false;
							if(value.EndsWith(" \"%1\""))
							{
								value = value.Substring(0, value.Length - 5);
								if(value.StartsWith("\"") && value.EndsWith("\""))
								{
									value = value.Substring(1, value.Length - 2);
									value = Path.GetFullPath(value);
									var appPath = System.IO.Path.GetFullPath(Application.ExecutablePath);
									return value == appPath;
								}
							}
						}
						return false;
					}
				}
				catch(Exception exc)
				{
					if(exc.IsCritical())
					{
						throw;
					}
					return false;
				}
			}
		}

		public static void IntegrateInExplorerContextMenu()
		{
			using(var key = Registry.ClassesRoot.OpenSubKey(@"Directory\shell", true))
			{
				using(var gitterKey = key.CreateSubKey("gitter", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryOptions.None))
				{
					gitterKey.SetValue(null, @"Open With gitter");
					using(var commandKey = gitterKey.CreateSubKey("command", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryOptions.None))
					{
						var appPath = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(Application.ExecutablePath)), "gitter.exe");
						if(!appPath.StartsWith("\"") || !appPath.EndsWith("\""))
							appPath = appPath.SurroundWithDoubleQuotes();
						appPath += " \"%1\"";
						commandKey.SetValue(null, appPath);
					}
				}
			}
		}

		public static void RemoveFromExplorerContextMenu()
		{
			using(var key = Registry.ClassesRoot.OpenSubKey(@"Directory\shell", true))
			{
				key.DeleteSubKeyTree("gitter", false);
			}
		}

		public static void LoadFrom(Section section)
		{
			Verify.Argument.IsNotNull(section, nameof(section));

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
			Verify.Argument.IsNotNull(section, nameof(section));

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
}
