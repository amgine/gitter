namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Options;

	using gitter.Git.AccessLayer;
	using gitter.Git.AccessLayer.CLI;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class GitOptionsPage : PropertyPage, IExecutableDialog
	{
		public static readonly new Guid Guid = new Guid("22102F21-350D-426A-AE8A-685928EAABE5");

		private DialogBase _gitAccessorOptions;

		private static Dictionary<Type, Func<IGitAccessor, DialogBase>> _gitAcessorSetupControls =
			new Dictionary<Type, Func<IGitAccessor, DialogBase>>()
			{
				{ typeof(MSysGitAccessorProvider), accessor => new CliOptionsPage(accessor) },
			};

		private Dictionary<Type, Tuple<IGitAccessor, DialogBase>> _cachedControls =
			new Dictionary<Type, Tuple<IGitAccessor, DialogBase>>();

		private readonly IGitRepositoryProvider _repositoryProvider;
		private IGitAccessorProvider _selectedAccessorProvder;
		private IGitAccessor _selectedAccessor;

		public GitOptionsPage(IWorkingEnvironment environment)
			: base(Guid)
		{
			if(environment == null) throw new ArgumentNullException("environment");

			InitializeComponent();

			Text = Resources.StrGit;

			_repositoryProvider = environment.GetRepositoryProvider<RepositoryProvider>();
			ShowGitAccessorProviders();

			_grpRepositoryAccessor.Text = Resources.StrsRepositoryAccessMethod;
			_lblAccessmethod.Text = Resources.StrAccessMethod.AddColon();
		}

		private void ShowGitAccessorProviders()
		{
			_cmbAccessorProvider.SelectedIndexChanged -= OnGitAccessorChanged;
			_cmbAccessorProvider.BeginUpdate();
			_cmbAccessorProvider.Items.Clear();
			int index = 0;
			int selectedIndex = -1;
			foreach(var accessorProvider in _repositoryProvider.GitAccessorProviders)
			{
				_cmbAccessorProvider.Items.Add(accessorProvider);
				if(accessorProvider == _repositoryProvider.ActiveGitAccessorProvider)
				{
					selectedIndex = index;
				}
				++index;
			}
			_cmbAccessorProvider.DisplayMember = "DisplayName";
			_cmbAccessorProvider.SelectedIndex = selectedIndex;
			_cmbAccessorProvider.EndUpdate();
			_cmbAccessorProvider.SelectedIndexChanged += OnGitAccessorChanged;
			ShowGitAccessorSetupControl(
				_repositoryProvider.ActiveGitAccessorProvider,
				_repositoryProvider.GitAccessor);
		}

		private void ShowGitAccessorSetupControl(IGitAccessorProvider accessorProvider, IGitAccessor accessor)
		{
			if(accessorProvider != null)
			{
				var type = accessorProvider.GetType();
				Tuple<IGitAccessor, DialogBase> cachedControl;
				if(_cachedControls.TryGetValue(type, out cachedControl))
				{
					ShowGitAccessorSetupControl(cachedControl.Item2);
					_selectedAccessorProvder = accessorProvider;
					_selectedAccessor = cachedControl.Item1;
				}
				else
				{
					if(accessor == null)
					{
						accessor = accessorProvider.CreateAccessor();
					}
					Func<IGitAccessor, DialogBase> setupControlFactory;
					if(_gitAcessorSetupControls.TryGetValue(type, out setupControlFactory))
					{
						var setupControl = setupControlFactory(accessor);
						ShowGitAccessorSetupControl(setupControl);
						_cachedControls.Add(type, Tuple.Create(accessor, setupControl));
					}
					_selectedAccessorProvder = accessorProvider;
					_selectedAccessor = accessor;
				}
			}
		}

		private void ShowGitAccessorSetupControl(DialogBase setupControl)
		{
			if(setupControl != _gitAccessorOptions)
			{
				if(setupControl != null)
				{
					setupControl.SetBounds(0, _cmbAccessorProvider.Bottom + 9, Width, 0,
						BoundsSpecified.X | BoundsSpecified.Y | BoundsSpecified.Width);
					setupControl.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
					setupControl.Parent = this;
				}
				if(_gitAccessorOptions != null)
				{
					_gitAccessorOptions.Parent = null;
				}
				_gitAccessorOptions = setupControl;
			}
		}

		private void OnGitAccessorChanged(object sender, EventArgs e)
		{
			var selectedAccessor = (IGitAccessorProvider)_cmbAccessorProvider.SelectedItem;

			ShowGitAccessorSetupControl(selectedAccessor, null);
		}

		protected override void OnShown()
		{
			base.OnShown();

			_gitAccessorOptions.InvokeOnShown();
		}

		#region IExecutableDialog Members

		public bool Execute()
		{
			var executableDialog = _gitAccessorOptions as IExecutableDialog;
			if(executableDialog != null)
			{
				if(executableDialog.Execute())
				{
					_repositoryProvider.GitAccessor = _selectedAccessor;
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return true;
			}
		}

		#endregion
	}
}
