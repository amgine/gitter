namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.ComponentModel;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class FetchDialog : GitDialogBase
	{
		private readonly Repository _repository;

		public FetchDialog(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			InitializeComponent();

			Text = Resources.StrFetch;
		}

		protected override string ActionVerb
		{
			get { return Resources.StrFetch; }
		}
	}
}
