namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;
	using System.Xml;
	using gitter.Framework.Configuration;
	using gitter.Git.Gui.Controls;
	using Resources = gitter.Git.Properties.Resources;
	[ToolboxItem(false)]
	sealed class ReferencesSearchToolBar : SearchToolBar<ReferencesView, SearchOptions>
	{
		public ReferencesSearchToolBar(ReferencesView tool)
			: base(tool)
		{
		}

		protected override SearchOptions CreateSearchOptions()
		{
			return new SearchOptions()
			{
				Text = SearchText,
			};
		}
	}
}
