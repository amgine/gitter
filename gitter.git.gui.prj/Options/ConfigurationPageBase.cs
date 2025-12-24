#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
using System.Drawing;
using System.Linq;
using System.ComponentModel;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Options;

using gitter.Git.Gui.Dialogs;

using gitter.Git.Gui.Controls;
using gitter.Framework.Controls;
using gitter.Framework.Layout;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
abstract class ConfigurationPageBase : PropertyPage
{
	readonly struct DialogControls
	{
		public readonly ConfigListBox _lstConfig;
		public readonly IButtonWidget _btnAddParameter;

		public DialogControls(IGitterStyle? style)
		{
			style ??= GitterApplication.Style;

			_lstConfig = new() { Style = style, AllowColumnReorder = false };
			_btnAddParameter = style.ButtonFactory.Create();
		}

		public void Localize()
		{
			_btnAddParameter.Text = Resources.StrAddParameter;
		}

		public void Layout(Control parent)
		{
			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					rows:
					[
						SizeSpec.Everything(),
						SizeSpec.Absolute(4),
						LayoutConstants.ButtonRowHeight,
					],
					content:
					[
						new GridContent(new ControlContent(_lstConfig, marginOverride: LayoutConstants.NoMargin), row: 0),
						new GridContent(new Grid(
							columns:
							[
								SizeSpec.Everything(),
								SizeSpec.Absolute(100),
							],
							content:
							[
								new GridContent(new WidgetContent(_btnAddParameter, marginOverride: LayoutConstants.NoMargin), column: 1),
							]), row: 2),
					]),
			};

			var tabIndex = 0;
			_lstConfig.TabIndex = tabIndex++;
			_btnAddParameter.TabIndex = tabIndex++;

			_lstConfig.Parent = parent;
			_btnAddParameter.Parent = parent;
		}
	}

	private readonly IWorkingEnvironment _environment;
	private readonly ConfigFile _fileType;
	private readonly ConfigurationFile? _file;
	private readonly DialogControls _controls;

	protected ConfigurationPageBase(Guid guid, IWorkingEnvironment environment, ConfigFile file) : base(guid)
	{
		Verify.Argument.IsNotNull(environment);

		_environment = environment;
		_fileType    = file;

		SuspendLayout();
		_controls = new(GitterApplication.Style);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		var gitAccessor = environment
			.RepositoryProviders
			.OfType<IGitRepositoryProvider>()
			.First()
			.GitAccessor;

		if(gitAccessor is not null)
		{
			_file = file switch
			{
				ConfigFile.CurrentUser => ConfigurationFile.OpenCurrentUserFile (gitAccessor),
				ConfigFile.LocalSystem => ConfigurationFile.OpenSystemFile      (gitAccessor),
				_ => throw new ArgumentException($"Unsupported configuration file type: {file}", nameof(file))
			};
			_controls._btnAddParameter.Click += OnAddParameterClick;
			_controls._lstConfig.LoadData(_file);
		}
		else
		{
			_controls._btnAddParameter.Enabled = false;
		}
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(528, 374));

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	private void OnAddParameterClick(object? sender, EventArgs e)
	{
		if(_file is null) return;

		using var dialog = new AddParameterDialog(_environment, _fileType);
		if(dialog.Run(this) == DialogResult.OK)
		{
			_file.Refresh();
		}
	}
}
