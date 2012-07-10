namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public sealed class ConfigParameterMenu : ContextMenuStrip
	{
		private readonly ConfigParameterListItem _listItem;
		private readonly ConfigParameter _parameter;

		public ConfigParameterMenu(ConfigParameterListItem listItem)
		{
			if(listItem == null) throw new ArgumentNullException("listItem");
			_listItem = listItem;
			_parameter = listItem.DataContext;
			if(_parameter.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "ConfigParameter"), "listItem");

			Items.Add(new ToolStripMenuItem(Resources.StrEditValue, CachedResources.Bitmaps["ImgConfigEdit"], (s, e) => listItem.StartValueEditor()));
			Items.Add(GuiItemFactory.GetUnsetParameterItem<ToolStripMenuItem>(_parameter));
		}

		public ConfigParameterMenu(ConfigParameter parameter)
		{
			if(parameter == null) throw new ArgumentNullException("parameter");
			if(parameter.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "ConfigParameter"), "parameter");
			_parameter = parameter;

			Items.Add(GuiItemFactory.GetUnsetParameterItem<ToolStripMenuItem>(parameter));
		}

		public ConfigParameter ConfigParameter
		{
			get { return _parameter; }
		}
	}
}
