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
				BehaviorPage.Guid,
				Resources.StrBehavior,
				null,
				PropertyPageFactory.RootGroupGuid,
				env => new BehaviorPage()));

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
			Verify.Argument.IsNotNull(color, "color");

			_colors.Add(color.Id, color);
		}

		public static void RegisterSelectableColorCategory(SelectableColorCategory category)
		{
			Verify.Argument.IsNotNull(category, "category");

			_colorCategories.Add(category.Id, category);
		}

		public static void RegisterPropertyPageFactory(PropertyPageFactory description)
		{
			Verify.Argument.IsNotNull(description, "description");

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
					list.Add(item);
			}
			foreach(var item in list)
			{
				PropertyPageItem parent;
				if(dic.TryGetValue(item.DataContext.GroupGuid, out parent))
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
						return false;
					}
				}
				catch
				{
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
				var gravatarNode = servicesNode.TryGetSection("Gravatar");
				if(gravatarNode != null)
				{
				}
				var spellingNode = servicesNode.TryGetSection("Spelling");
				if(spellingNode != null)
				{
					SpellingService.LoadFrom(spellingNode);
				}
			}
		}

		public static void SaveTo(Section section)
		{
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
			var gravatarNode = servicesNode.GetCreateSection("Gravatar");
			var spellingNode = servicesNode.GetCreateSection("Spelling");
			SpellingService.SaveTo(spellingNode);
		}
	}
}
