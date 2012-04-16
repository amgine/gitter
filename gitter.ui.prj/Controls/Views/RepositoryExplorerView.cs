namespace gitter
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Properties.Resources;

	public partial class RepositoryExplorerView : ViewBase
	{
		public RepositoryExplorerView(IWorkingEnvironment environment, IDictionary<string, object> parameters)
			: base(Guids.RepositoryExplorerView, environment, parameters)
		{
			InitializeComponent();

			Text = Resources.StrRepositoryExplorer;
		}

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgRepositoryExplorer"]; }
		}

		private void OnItemActivated(object sender, ItemEventArgs e)
		{
			var item = e.Item as ViewListItem;
			if(item != null)
			{
				WorkingEnvironment.ViewDockService.ShowView(item.Data.Guid);
			}
		}

		public void AddItem(CustomListBoxItem item)
		{
			_lstRepositoryExplorer.Items.Add(item);
		}

		public void RemoveItem(CustomListBoxItem item)
		{
			_lstRepositoryExplorer.Items.Remove(item);
		}
	}
}
