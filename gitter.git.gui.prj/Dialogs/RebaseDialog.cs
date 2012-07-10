namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.ComponentModel;

	using gitter.Framework;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class RebaseDialog : GitDialogBase
	{
		public RebaseDialog()
		{
			InitializeComponent();

			Text = Resources.StrRebase;
		}

		protected override string ActionVerb
		{
			get { return Resources.StrRebase; }
		}
	}
}
