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

namespace gitter.Git;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;
using gitter.Framework.Options;

using gitter.Git.AccessLayer;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
partial class GitOptionsPage : PropertyPage, IExecutableDialog
{
	public static readonly new Guid Guid = new("22102F21-350D-426A-AE8A-685928EAABE5");

	private DialogBase? _gitAccessorOptions;

	private readonly Dictionary<Type, Tuple<IGitAccessor, DialogBase>> _cachedControls = [];

	readonly struct DialogControls
	{
		private readonly LabelControl _lblAccessmethod;
		public  readonly AccessorProviderPicker _cmbAccessorProvider;
		public  readonly Panel _hostPanel;

		public DialogControls()
		{
			_lblAccessmethod = new();
			_cmbAccessorProvider = new();
			_hostPanel = new();
		}

		public void Localize()
		{
			_lblAccessmethod.Text = Resources.StrAccessMethod.AddColon();
		}

		public void Layout(Control parent)
		{
			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					columns:
					[
						SizeSpec.Absolute(117),
						SizeSpec.Everything(),
					],
					rows:
					[
						LayoutConstants.TextInputRowHeight,
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_lblAccessmethod,     marginOverride: LayoutConstants.TextBoxLabelMargin), row: 0),
						new GridContent(new ControlContent(_cmbAccessorProvider, marginOverride: LayoutConstants.TextBoxMargin), row: 0, column: 1),
						new GridContent(new ControlContent(_hostPanel,           marginOverride: LayoutConstants.NoMargin), row: 1, columnSpan: 2),
					]),
			};

			var tabIndex = 0;

			_lblAccessmethod.TabIndex = tabIndex++;
			_cmbAccessorProvider.TabIndex = tabIndex++;
			_hostPanel.TabIndex = tabIndex++;

			_lblAccessmethod.Parent = parent;
			_cmbAccessorProvider.Parent = parent;
			_hostPanel.Parent = parent;
		}
	}

	private readonly DialogControls _controls;
	private readonly IGitRepositoryProvider _repositoryProvider;
	private IGitAccessorProvider? _selectedAccessorProvder;
	private IGitAccessor? _selectedAccessor;

	public GitOptionsPage(
		IGitRepositoryProvider         repositoryProvider,
		IAccessLayerOptionsPageFactory accessLayerOptionsPageFactory)
		: base(Guid)
	{
		Verify.Argument.IsNotNull(repositoryProvider);
		Verify.Argument.IsNotNull(accessLayerOptionsPageFactory);

		Name = nameof(GitOptionsPage);
		Text = Resources.StrGit;

		SuspendLayout();
		_controls = new();
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		_repositoryProvider           = repositoryProvider;
		AccessLayerOptionsPageFactory = accessLayerOptionsPageFactory;

		ShowGitAccessorProviders();
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(479, 218));

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	private IAccessLayerOptionsPageFactory AccessLayerOptionsPageFactory { get; }

	private void ShowGitAccessorProviders()
	{
		_controls._cmbAccessorProvider.SelectedValueChanged -= OnGitAccessorChanged;
		_controls._cmbAccessorProvider.LoadData(_repositoryProvider);
		_controls._cmbAccessorProvider.SelectedValueChanged += OnGitAccessorChanged;
		ShowGitAccessorSetupControl(
			_repositoryProvider.ActiveGitAccessorProvider,
			_repositoryProvider.GitAccessor);
	}

	private void ShowGitAccessorSetupControl(IGitAccessorProvider? accessorProvider, IGitAccessor? accessor)
	{
		if(accessorProvider is null) return;

		var type = accessorProvider.GetType();
		if(_cachedControls.TryGetValue(type, out var cachedControl))
		{
			ShowGitAccessorSetupControl(cachedControl.Item2);
			_selectedAccessorProvder = accessorProvider;
			_selectedAccessor = cachedControl.Item1;
		}
		else
		{
			accessor ??= accessorProvider.CreateAccessor();

			var setupControl = AccessLayerOptionsPageFactory.Create(type, accessor);
			if(setupControl is not null)
			{
				ShowGitAccessorSetupControl(setupControl);
				_cachedControls.Add(type, Tuple.Create(accessor, setupControl));
			}
			_selectedAccessorProvder = accessorProvider;
			_selectedAccessor = accessor;
		}
	}

	private void ShowGitAccessorSetupControl(DialogBase setupControl)
	{
		if(setupControl == _gitAccessorOptions) return;

		if(setupControl is not null)
		{
			setupControl.Dock   = DockStyle.Fill;
			setupControl.Parent = _controls._hostPanel;
		}
		if(_gitAccessorOptions is not null)
		{
			_gitAccessorOptions.Parent = null;
		}
		_gitAccessorOptions = setupControl;
	}

	private void OnGitAccessorChanged(object? sender, EventArgs e)
	{
		var selectedAccessor = _controls._cmbAccessorProvider.SelectedValue;
		ShowGitAccessorSetupControl(selectedAccessor, null);
	}

	protected override void OnShown()
	{
		base.OnShown();

		_gitAccessorOptions?.InvokeOnShown();
	}

	public bool Execute()
	{
		if(_gitAccessorOptions is not IExecutableDialog executableDialog) return true;

		if(executableDialog.Execute())
		{
			_repositoryProvider.GitAccessor = _selectedAccessor;
			return true;
		}
		return false;
	}
}
