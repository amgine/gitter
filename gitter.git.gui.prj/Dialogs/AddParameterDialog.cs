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

namespace gitter.Git.Gui.Dialogs;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;
using gitter.Framework.Services;

using gitter.Git.AccessLayer;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
public partial class AddParameterDialog : GitDialogBase, IExecutableDialog
{
	readonly struct DialogControl
	{
		private readonly LabelControl _lblName;
		private readonly LabelControl _lblValue;
		public  readonly TextBox _txtName;
		public  readonly TextBox _txtValue;

		public DialogControl(IGitterStyle style)
		{
			style ??= GitterApplication.Style;

			_lblName  = new();
			_lblValue = new();
			_txtName  = new();
			_txtValue = new();

			GitterApplication.FontManager.InputFont.Apply(_txtName, _txtValue);
		}

		public void Localize()
		{
			_lblName.Text  = Resources.StrName.AddColon();
			_lblValue.Text = Resources.StrValue.AddColon();
		}

		public void Layout(Control parent)
		{
			var nameDec  = new TextBoxDecorator(_txtName);
			var valueDec = new TextBoxDecorator(_txtValue);

			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					columns:
					[
						SizeSpec.Absolute(94),
						SizeSpec.Everything(),
					],
					rows:
					[
						LayoutConstants.TextInputRowHeight,
						LayoutConstants.TextInputRowHeight,
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_lblName,  marginOverride: LayoutConstants.NoMargin),      column: 0, row: 0),
						new GridContent(new ControlContent(nameDec,   marginOverride: LayoutConstants.TextBoxMargin), column: 1, row: 0),
						new GridContent(new ControlContent(_lblValue, marginOverride: LayoutConstants.NoMargin),      column: 0, row: 1),
						new GridContent(new ControlContent(valueDec,  marginOverride: LayoutConstants.TextBoxMargin), column: 1, row: 1),
					]),
			};

			var tabIndex = 0;
			_lblName.TabIndex  = tabIndex++;
			nameDec.TabIndex   = tabIndex++;
			_lblValue.TabIndex = tabIndex++;
			valueDec.TabIndex  = tabIndex++;

			_lblName.Parent  = parent;
			nameDec.Parent   = parent;
			_lblValue.Parent = parent;
			valueDec.Parent  = parent;
		}
	}

	private readonly DialogControl _controls;
	private readonly IWorkingEnvironment _environment;
	private Repository? _repository;
	private ConfigFile _configFile;

	/// <summary>Create <see cref="AddParameterDialog"/>.</summary>
	/// <param name="environment">Application environment.</param>
	private AddParameterDialog(IWorkingEnvironment environment)
	{
		Verify.Argument.IsNotNull(environment);

		_environment = environment;

		Name = nameof(AddParameterDialog);
		Text = Resources.StrAddParameter;

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Size                = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.Style);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();
	}

	/// <summary>Create <see cref="AddParameterDialog"/>.</summary>
	/// <param name="environment">Application environment.</param>
	/// <param name="repository">Related <see cref="Repository"/>.</param>
	public AddParameterDialog(IWorkingEnvironment environment, Repository repository)
		: this(environment)
	{
		Verify.Argument.IsNotNull(repository);

		_repository = repository;
		_configFile = ConfigFile.Repository;
	}

	/// <summary>Create <see cref="AddParameterDialog"/>.</summary>
	/// <param name="environment">Application environment.</param>
	/// <param name="configFile">Config file to use.</param>
	public AddParameterDialog(IWorkingEnvironment environment, ConfigFile configFile)
		: this(environment)
	{
		Verify.Argument.IsTrue(configFile is ConfigFile.LocalSystem or ConfigFile.CurrentUser, nameof(configFile));

		_configFile = configFile;
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(DefaultWidth, 55));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrAdd;

	/// <summary>Parameter name.</summary>
	public string ParameterName
	{
		get => _controls._txtName.Text;
		set => _controls._txtName.Text = value;
	}

	/// <summary>Parameter value.</summary>
	public string ParameterValue
	{
		get => _controls._txtValue.Text;
		set => _controls._txtValue.Text = value;
	}

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		BeginInvoke(_controls._txtName.Focus);
	}

	/// <inheritdoc/>
	public bool Execute()
	{
		var name  = _controls._txtName.Text.Trim();
		var value = _controls._txtValue.Text.Trim();
		if(name.Length == 0)
		{
			NotificationService.NotifyInputError(
				_controls._txtName,
				Resources.ErrInvalidParameterName,
				Resources.ErrParameterNameCannotBeEmpty);
			return false;
		}
		try
		{
			using(this.ChangeCursor(Cursors.WaitCursor))
			{
				if(_configFile == ConfigFile.Repository)
				{
					var p = _repository!.Configuration.TryGetParameter(name);
					if(p is not null)
					{
						p.Value = value;
					}
					else
					{
						_ = _repository.Configuration.CreateParameter(name, value);
					}
				}
				else
				{
					var accessor = _environment.GetRepositoryProvider<IGitRepositoryProvider>()?.GitAccessor;
					if(accessor is not null)
					{
						accessor.SetConfigValue.Invoke(
							new SetConfigValueRequest(name, value)
							{
								ConfigFile = _configFile,
							});
					}
				}
			}
		}
		catch(GitException exc)
		{
			GitterApplication.MessageBoxService.Show(
				this,
				exc.Message,
				Resources.ErrFailedToSetParameter,
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
			return false;
		}
		return true;
	}
}
