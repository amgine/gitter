namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.IO;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Options;

	using gitter.Git.AccessLayer.CLI;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class GitOptionsPage : PropertyPage, IExecutableDialog
	{
		public static readonly new Guid Guid = new Guid("22102F21-350D-426A-AE8A-685928EAABE5");

		private CliOptionsPage _cliOptions;

		public GitOptionsPage(IWorkingEnvironment environment)
			: base(Guid)
		{
			if(environment == null) throw new ArgumentNullException("environment");

			InitializeComponent();

			Text = Resources.StrGit;

			var provider = environment.GetRepositoryProvider<RepositoryProvider>();

			_cliOptions = new CliOptionsPage(RepositoryProvider.Git);
			_cliOptions.Parent = this;
			_cliOptions.SetBounds(0, _cmbAccessMethod.Bottom + 9, Width, 0,
				BoundsSpecified.X | BoundsSpecified.Y | BoundsSpecified.Width);
			_cliOptions.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

			_cmbAccessMethod.SelectedIndex = 0;

			_grpRepositoryAccessor.Text = Resources.StrsRepositoryAccessMethod;
			_lblAccessmethod.Text = Resources.StrAccessMethod.AddColon();
		}

		protected override void OnShown()
		{
			base.OnShown();

			_cliOptions.InvokeOnShown();
		}

		#region IExecutableDialog Members

		public bool Execute()
		{
			return _cliOptions.Execute();
		}

		#endregion
	}
}
