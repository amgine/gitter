namespace gitter.Framework.Controls
{
	using System;
	using System.Windows.Forms;

	public class ViewMenuItem : ToolStripMenuItem
	{
		private readonly IWorkingEnvironment _environment;
		private readonly IViewFactory _factory;

		public ViewMenuItem(IViewFactory factory, IWorkingEnvironment environment)
			: base(factory.Name, factory.Image, OnClick)
		{
			_factory = factory;
			_environment = environment;
		}

		private static void OnClick(object sender, EventArgs e)
		{
			var item = (ViewMenuItem)sender;
			item._environment.ViewDockService.ShowView(item._factory.Guid, true);
		}
	}
}
