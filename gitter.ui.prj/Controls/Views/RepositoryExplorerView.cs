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
		private readonly IWorkingEnvironment _environment;

		public RepositoryExplorerView(IDictionary<string, object> parameters, IWorkingEnvironment environment)
			: base(Guids.RepositoryExplorerView, parameters)
		{
			if(environment == null) throw new ArgumentNullException("environment");
			_environment = environment;

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
			if(item != null) _environment.ViewDockService.ShowView(item.Data.Guid);
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
