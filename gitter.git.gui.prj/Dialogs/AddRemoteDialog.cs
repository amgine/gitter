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
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;
using gitter.Framework.Mvc;
using gitter.Framework.Mvc.WinForms;

using gitter.Git.Gui.Controllers;
using gitter.Git.Gui.Interfaces;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
public partial class AddRemoteDialog : GitDialogBase, IAsyncExecutableDialog, IAddRemoteView
{
	readonly struct DialogControls
	{
		public readonly LabelControl _lblUrl;
		public readonly LabelControl _lblName;
		public readonly TextBox _txtUrl;
		public readonly TextBox _txtName;
		public readonly ICheckBoxWidget _chkFetch;
		public readonly ICheckBoxWidget _chkMirror;
		public readonly GroupSeparator _grpOptions;
		public readonly IRadioButtonWidget _tagFetchAll;
		public readonly IRadioButtonWidget _tagFetchNone;
		public readonly IRadioButtonWidget _tagFetchDefault;
		public readonly GroupSeparator _grpTagImport;
		//public readonly GroupSeparator _grpBranches;
		//public readonly IRadioButtonWidget _trackSpecified;
		//public readonly IRadioButtonWidget _trackAllBranches;

		public DialogControls(IGitterStyle style)
		{
			style ??= GitterApplication.Style;

			var cbf = style.CheckBoxFactory;
			var rbf = style.RadioButtonFactory;
			_lblUrl           = new();
			_lblName          = new();
			_txtUrl           = new();
			_txtName          = new();
			_chkFetch         = cbf.Create();
			_chkMirror        = cbf.Create();
			_grpOptions       = new();
			_tagFetchAll      = rbf.Create();
			_tagFetchNone     = rbf.Create();
			_tagFetchDefault  = rbf.Create();
			_grpTagImport     = new();
			//_grpBranches      = new();
			//_trackSpecified   = rbf.Create();
			//_trackAllBranches = rbf.Create();

			_tagFetchDefault.IsChecked = true;

			GitterApplication.FontManager.InputFont.Apply(_txtName, _txtUrl);
		}

		public void Localize()
		{
			_lblName.Text          = Resources.StrName.AddColon();
			_lblUrl.Text           = Resources.StrUrl.AddColon();
			
			_grpOptions.Text       = Resources.StrOptions;
			_chkFetch.Text         = Resources.StrsFetchRemote;
			_chkMirror.Text        = Resources.StrMirror;

			//_grpBranches.Text      = Resources.StrTrackingBranches;
			//_trackAllBranches.Text = Resources.StrlTrackAllBranches;
			//_trackSpecified.Text   = Resources.StrlTrackSpecifiedBranches.AddColon();

			_grpTagImport.Text     = Resources.StrsTagFetchMode;
			_tagFetchDefault.Text  = Resources.StrDefault;
			_tagFetchAll.Text      = Resources.StrsFetchAll;
			_tagFetchNone.Text     = Resources.StrsFetchNone;
		}

		public void Layout(Control parent)
		{
			var nameDec = new TextBoxDecorator(_txtName);
			var urlDec  = new TextBoxDecorator(_txtUrl);
			var pnlTagImport = new Panel();

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
						LayoutConstants.GroupSeparatorRowHeight,
						LayoutConstants.CheckBoxRowHeight,
						LayoutConstants.CheckBoxRowHeight,
						LayoutConstants.GroupSeparatorRowHeight,
						LayoutConstants.RadioButtonRowHeight,
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_lblName,      marginOverride: LayoutConstants.NoMargin),           row: 0, column: 0),
						new GridContent(new ControlContent(nameDec,       marginOverride: LayoutConstants.TextBoxLabelMargin), row: 0, column: 1),
						new GridContent(new ControlContent(_lblUrl,       marginOverride: LayoutConstants.NoMargin),           row: 1, column: 0),
						new GridContent(new ControlContent(urlDec,        marginOverride: LayoutConstants.TextBoxLabelMargin), row: 1, column: 1),
						new GridContent(new ControlContent(_grpOptions,   marginOverride: LayoutConstants.NoMargin),           row: 2, columnSpan: 2),
						new GridContent(new WidgetContent (_chkFetch,     marginOverride: LayoutConstants.GroupPadding),       row: 3, columnSpan: 2),
						new GridContent(new WidgetContent (_chkMirror,    marginOverride: LayoutConstants.GroupPadding),       row: 4, columnSpan: 2),
						new GridContent(new ControlContent(_grpTagImport, marginOverride: LayoutConstants.NoMargin),           row: 5, columnSpan: 2),
						new GridContent(new ControlContent(pnlTagImport,  marginOverride: LayoutConstants.GroupPadding),       row: 6, columnSpan: 2),
					]),
			};

			_ = new ControlLayout(pnlTagImport)
			{
				Content = new Grid(
					columns:
					[
						SizeSpec.Absolute(110),
						SizeSpec.Absolute(110),
						SizeSpec.Absolute(110),
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new WidgetContent(_tagFetchDefault, marginOverride: LayoutConstants.NoMargin), column: 0),
						new GridContent(new WidgetContent(_tagFetchNone,    marginOverride: LayoutConstants.NoMargin), column: 1),
						new GridContent(new WidgetContent(_tagFetchAll,     marginOverride: LayoutConstants.NoMargin), column: 2),
					]),
			};

			var tabIndex = 0;
			_lblName.TabIndex = tabIndex++;
			nameDec.TabIndex = tabIndex++;
			_lblUrl.TabIndex = tabIndex++;
			urlDec.TabIndex = tabIndex++;
			_grpOptions.TabIndex = tabIndex++;
			_chkFetch.TabIndex = tabIndex++;
			_chkMirror.TabIndex = tabIndex++;
			_grpTagImport.TabIndex = tabIndex++;
			pnlTagImport.TabIndex = tabIndex++;
			_tagFetchDefault.TabIndex = tabIndex++;
			_tagFetchNone.TabIndex = tabIndex++;
			_tagFetchAll.TabIndex = tabIndex++;

			_lblName.Parent = parent;
			nameDec.Parent = parent;
			_lblUrl.Parent = parent;
			urlDec.Parent = parent;
			_grpOptions.Parent = parent;
			_chkFetch.Parent = parent;
			_chkMirror.Parent = parent;
			_grpTagImport.Parent = parent;
			pnlTagImport.Parent = parent;
			_tagFetchDefault.Parent = pnlTagImport;
			_tagFetchNone.Parent = pnlTagImport;
			_tagFetchAll.Parent = pnlTagImport;
		}
	}

	private readonly DialogControls _controls;
	private readonly IAddRemoteController _controller;

	public AddRemoteDialog(Repository repository)
	{
		Verify.Argument.IsNotNull(repository);

		Repository = repository;

		Name = nameof(AddRemoteDialog);
		Text = Resources.StrAddRemote;

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Size                = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.StyleOnNextStartup);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		var inputs = new IUserInputSource[]
		{
			RemoteName   = new TextBoxInputSource(_controls._txtName),
			Url          = new TextBoxInputSource(_controls._txtUrl),
			Fetch        = new CheckBoxWidgetInputSource(_controls._chkFetch),
			Mirror       = new CheckBoxWidgetInputSource(_controls._chkMirror),
			TagFetchMode = new RadioButtonWidgetGroupInputSource<TagFetchMode>(
				[
					Tuple.Create(_controls._tagFetchDefault, gitter.Git.TagFetchMode.Default),
					Tuple.Create(_controls._tagFetchNone,    gitter.Git.TagFetchMode.NoTags),
					Tuple.Create(_controls._tagFetchAll,     gitter.Git.TagFetchMode.AllTags),
				]),
		};
		ErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

		SetupReferenceNameInputBox(_controls._txtName, ReferenceType.Remote);

		if(Repository.Remotes.Count == 0)
		{
			_controls._txtName.Text = GitConstants.DefaultRemoteName;
		}

		_controller = new AddRemoteController(repository) { View = this };
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(DefaultWidth, 176));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrAdd;

	public Repository Repository { get; }

	public IUserInputSource<string?> RemoteName { get; }

	public IUserInputSource<string?> Url { get; }

	public IUserInputSource<bool> Fetch { get; }

	public IUserInputSource<bool> Mirror { get; }

	public IUserInputSource<TagFetchMode> TagFetchMode { get; }

	public IUserInputErrorNotifier ErrorNotifier { get; }

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		BeginInvoke(_controls._txtName.Focus);
	}

	/// <inheritdoc/>
	public Task<bool> ExecuteAsync() => _controller.TryAddRemoteAsync();
}
