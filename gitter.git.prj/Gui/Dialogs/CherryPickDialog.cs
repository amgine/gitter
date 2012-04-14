namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.ComponentModel;

	using gitter.Framework;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	public partial class CherryPickDialog : GitDialogBase
	{
		public CherryPickDialog()
		{
			InitializeComponent();

			Text = Resources.StrCherryPick;
		}

		protected override string ActionVerb
		{
			get { return Resources.StrCherryPick; }
		}
	}
}
