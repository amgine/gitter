#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

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

		private GitOptionsPage()
			: base(Guid)
		{
			InitializeComponent();

			Text = Resources.StrGit;
			_grpRepositoryAccessor.Text = Resources.StrsRepositoryAccessMethod;
			_lblAccessmethod.Text = Resources.StrAccessMethod.AddColon();
		}

		public GitOptionsPage(IWorkingEnvironment environment)
			: this()
		{
			Verify.Argument.IsNotNull(environment, "environment");

			_repositoryProvider = environment.GetRepositoryProvider<RepositoryProvider>();
			ShowGitAccessorProviders();
		}

		public GitOptionsPage(IGitRepositoryProvider repositoryProvider)
			: this()
		{
			Verify.Argument.IsNotNull(repositoryProvider, "repositoryProvider");

			_repositoryProvider = repositoryProvider;
			ShowGitAccessorProviders();
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
