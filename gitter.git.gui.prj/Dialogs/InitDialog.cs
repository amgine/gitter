#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
using System.Threading.Tasks;

using gitter.Framework;
using gitter.Framework.Mvc;
using gitter.Framework.Mvc.WinForms;

using gitter.Git.Gui.Controllers;
using gitter.Git.Gui.Interfaces;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
public partial class InitDialog : GitDialogBase, IExecutableDialog, IAsyncExecutableDialog, IInitView
{
	private readonly IInitController _controller;

	public InitDialog(IGitRepositoryProvider gitRepositoryProvider)
	{
		Verify.Argument.IsNotNull(gitRepositoryProvider);

		InitializeComponent();
		Localize();

		var inputs = new IUserInputSource[]
		{
			RepositoryPath    = new TextBoxInputSource(_txtPath),
			Bare              = new CheckBoxInputSource(_chkBare),
			UseCustomTemplate = new CheckBoxInputSource(_chkUseTemplate),
			Template          = new TextBoxInputSource(_txtTemplate),
		};
		ErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

		GitterApplication.FontManager.InputFont.Apply(_txtPath, _txtTemplate);

		_controller = new InitController(gitRepositoryProvider) { View = this };
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(DefaultWidth, 98));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrInit;

	public IUserInputSource<string> RepositoryPath { get; }

	public IUserInputSource<bool> Bare { get; }

	public IUserInputSource<bool> UseCustomTemplate { get; }

	public IUserInputSource<string> Template { get; }

	public IUserInputErrorNotifier ErrorNotifier { get; }

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		BeginInvoke(_txtPath.Focus);
	}

	private void Localize()
	{
		Text = Resources.StrInitRepository;

		_lblPath.Text        = Resources.StrPath.AddColon();
		_grpOptions.Text     = Resources.StrOptions;
		_chkUseTemplate.Text = Resources.StrTemplate.AddColon();
		_chkBare.Text        = Resources.StrBare;
	}

	private void _btnSelectDirectory_Click(object sender, EventArgs e)
	{
		var path = Utility.ShowPickFolderDialog(this);
		if(path is not null)
		{
			_txtPath.Text = path;
		}
	}

	private void _btnSelectTemplate_Click(object sender, EventArgs e)
	{
		var path = Utility.ShowPickFolderDialog(this);
		if(path is not null)
		{
			_txtTemplate.Text = path;
		}
	}

	private void _chkUseTemplate_CheckedChanged(object sender, EventArgs e)
	{
		bool check = _chkUseTemplate.Checked;
		_txtTemplate.Enabled = check;
		_btnSelectTemplate.Enabled = check;
	}

	public bool Execute() => _controller.TryInit();

	public Task<bool> ExecuteAsync() => _controller.TryInitAsync();
}
