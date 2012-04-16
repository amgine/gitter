namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	internal partial class BlameView : GitViewBase
	{
		public BlameView(IDictionary<string, object> parameters, GuiProvider gui)
			: base(Guids.BlameViewGuid, gui, parameters)
		{
			InitializeComponent();

			Text = Resources.StrlBlame;
			ApplyParameters(parameters);
		}

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgBlame"]; }
		}

		private static string GetText(BlameFile file)
		{
			return Resources.StrlBlame + ": " + file.Name;
		}

		public override void ApplyParameters(IDictionary<string, object> parameters)
		{
			base.ApplyParameters(parameters);

			var blame = (IBlameSource)parameters["blame"];

			Text = Resources.StrlBlame + ": " + blame.ToString();

			_blamePanel.Panels.Clear();
			_blamePanel.Panels.Add(new BlameFilePanel(blame.GetBlame()));
		}
	}
}
