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
using System.IO;
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
partial class CloneDialog : GitDialogBase, IExecutableDialog, ICloneView
{
	private sealed class PathInfoBarControl : Control
	{
		private static readonly string _header = Resources.StrsWillBeClonedInto.AddColon();
		private static readonly string _noPath = Resources.StrlNoPathSpecified;

		public PathInfoBarControl()
		{
			SetStyle(ControlStyles.Selectable | ControlStyles.ContainerControl, false);
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque, true);
		}

		protected override bool ScaleChildren => false;

		protected override void OnPaintBackground(PaintEventArgs pevent) { }

		protected override void OnTextChanged(EventArgs e)
		{
			Invalidate();
			base.OnTextChanged(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var bounds  = ClientRectangle;
			var conv = DpiConverter.FromDefaultTo(this);
			bounds.Inflate(-conv.ConvertX(4), -conv.ConvertY(4));
			var rcLine1 = bounds;
			rcLine1.Height /= 2;
			var rcLine2 = rcLine1;
			rcLine2.Y += rcLine2.Height;
			rcLine2.Height = bounds.Height - rcLine1.Height;
			var path = Text;
			const TextFormatFlags flags
				= TextFormatFlags.Left
				| TextFormatFlags.VerticalCenter
				| TextFormatFlags.NoPadding
				| TextFormatFlags.PreserveGraphicsClipping;
			if(string.IsNullOrWhiteSpace(Text))
			{
				var (borderColor, backColor) = GitterApplication.Style.Type switch
				{
					GitterStyleType.DarkBackground => (Color.FromArgb( 29,  29,  29), Color.FromArgb( 68,  39,  38)),
					_                              => (Color.FromArgb(200, 200, 200), Color.FromArgb(230, 200, 200)),
				};
				using(var hdc = e.Graphics.AsGdi())
				{
					hdc.DrawBorder(ClientRectangle, borderColor, backColor, conv.ConvertX(1));
				}
				TextRenderer.DrawText(e.Graphics, _header, Font, rcLine1, ForeColor, flags);
				TextRenderer.DrawText(e.Graphics, _noPath, Font, rcLine2, GitterApplication.Style.Colors.GrayText, flags);
			}
			else
			{
				var (borderColor, backColor) = GitterApplication.Style.Type switch
				{
					GitterStyleType.DarkBackground => (Color.FromArgb( 29,  29,  29), Color.FromArgb( 57,  61,  27)),
					_                              => (Color.FromArgb(200, 200, 200), Color.FromArgb(210, 230, 190)),
				};
				using(var hdc = e.Graphics.AsGdi())
				{
					hdc.DrawBorder(ClientRectangle, borderColor, backColor, conv.ConvertX(1));
				}
				TextRenderer.DrawText(e.Graphics, _header, Font, rcLine1, ForeColor, flags);
				TextRenderer.DrawText(e.Graphics, path, Font, rcLine2, ForeColor, flags);
			}
		}
	}

	private sealed class RepositoryPathInput : IUserInputSource<string>, IWin32ControlInputSource
	{
		private readonly TextBoxBase _txtPath;
		private readonly TextBoxBase _txtUrl;
		private readonly ICheckBoxWidget _chkAppendUrlToPath;

		public event EventHandler? ValueChanged;

		private void OnValueChanged()
			=> ValueChanged?.Invoke(this, EventArgs.Empty);

		public RepositoryPathInput(TextBoxBase txtPath, TextBoxBase txtUrl, ICheckBoxWidget chkAppendUrlToPath)
		{
			Verify.Argument.IsNotNull(txtPath);
			Verify.Argument.IsNotNull(txtUrl);
			Verify.Argument.IsNotNull(chkAppendUrlToPath);

			_txtPath = txtPath;
			_txtUrl = txtUrl;
			_chkAppendUrlToPath = chkAppendUrlToPath;

			_chkAppendUrlToPath.IsCheckedChanged += OnAppendUrlToPathCheckedChanged;
			_txtPath.TextChanged += OnPathTextChanged;
			_txtUrl.TextChanged  += OnUrlTextChanged;
		}

		private void OnAppendUrlToPathCheckedChanged(object? sender, EventArgs e)
		{
			OnValueChanged();
		}

		private void OnPathTextChanged(object? sender, EventArgs e)
		{
			OnValueChanged();
		}

		private void OnUrlTextChanged(object? sender, EventArgs e)
		{
			if(_chkAppendUrlToPath.IsChecked)
			{
				OnValueChanged();
			}
		}

		private static string AppendUrlToPath(string path, string url)
		{
			if(!string.IsNullOrWhiteSpace(url))
			{
				try
				{
					path = Path.Combine(path, GitUtils.GetHumanishName(url));
				}
				catch(Exception exc) when(!exc.IsCritical)
				{
				}
			}
			return path;
		}

		public string Value
		{
			get
			{
				return _chkAppendUrlToPath.IsChecked
					? AppendUrlToPath(_txtPath.Text.Trim(), _txtUrl.Text.Trim())
					: _txtPath.Text;
			}
			set
			{
				_chkAppendUrlToPath.IsChecked = false;
				_txtPath.Text = value;
			}
		}

		public bool IsReadOnly
		{
			get => _txtUrl.ReadOnly;
			set
			{
				_txtUrl.ReadOnly = value;
				_chkAppendUrlToPath.Enabled = value;
			}
		}

		public Control Control => _txtPath;
	}

	readonly struct DialogControls
	{
		private readonly LabelControl _lblPath;
		public  readonly TextBox _txtPath;
		private readonly LabelControl _lblUrl;
		public  readonly TextBox _txtUrl;
		public  readonly Panel _pnlOptions;
		public  readonly GroupSeparator _grpOptions;
		public  readonly ICheckBoxWidget _chkBare;
		public  readonly ICheckBoxWidget _chkUseTemplate;
		public  readonly TextBox _txtTemplate;
		public  readonly FolderPickerTextBoxDecorator _decTemplate;
		public  readonly ICheckBoxWidget _chkMirror;
		public  readonly TextBox _txtRemoteName;
		public  readonly LabelControl _lblRemoteName;
		public  readonly ICheckBoxWidget _chkShallowClone;
		public  readonly LabelControl _lblDepth;
		public  readonly TextBoxDecoratorWithUpDown _numDepth;
		public  readonly ICheckBoxWidget _chkRecursive;
		public  readonly ICheckBoxWidget _chkNoCheckout;
		public  readonly ICheckBoxWidget _chkAppendRepositoryNameFromUrl;
		public  readonly PathInfoBarControl _targetPath;

		public DialogControls(IGitterStyle? style = default)
		{
			style ??= GitterApplication.Style;

			var cbf = style.CheckBoxFactory;
			_targetPath = new()
			{
				ForeColor = style.Colors.WindowText,
			};
			_chkAppendRepositoryNameFromUrl = cbf.Create();
			_pnlOptions = new();
			_lblDepth = new() { Enabled = false };
			_chkNoCheckout = cbf.Create();
			_chkRecursive = cbf.Create();
			_numDepth = new(new()
			{
				TextAlign = HorizontalAlignment.Right,
				MaxLength = 4,
			})
			{
				Enabled = false,
				Minimum = 1,
				Maximum = 9999
			};
			_chkShallowClone = cbf.Create();
			_txtRemoteName = new();
			_lblRemoteName = new();
			_chkMirror = cbf.Create();
			_grpOptions = new();
			_chkBare = cbf.Create();
			_chkUseTemplate = cbf.Create();
			_txtTemplate = new();
			_decTemplate = new(_txtTemplate) { Enabled = false };
			_lblUrl = new();
			_txtUrl = new();
			_lblPath = new();
			_txtPath = new();

			_chkMirror.Enabled = false;
			_numDepth.Enabled = false;

			_chkAppendRepositoryNameFromUrl.IsChecked = true;
			_chkRecursive.IsChecked = true;
		}

		public void Localize()
		{
			_lblPath.Text = Resources.StrPath.AddColon();
			_lblUrl.Text = Resources.StrUrl.AddColon();

			_grpOptions.Text = Resources.StrOptions;

			_chkAppendRepositoryNameFromUrl.Text = Resources.StrsAppendRepositoryNameFromURL;
			_chkUseTemplate.Text = Resources.StrTemplate.AddColon();
			_lblRemoteName.Text = Resources.StrsRemoteName.AddColon();
			_txtRemoteName.Text = GitConstants.DefaultRemoteName;
			_chkBare.Text = Resources.StrBare;
			_chkMirror.Text = Resources.StrMirror;
			_chkShallowClone.Text = Resources.StrsShallowClone;
			_chkRecursive.Text = Resources.StrRecursive;
			_lblDepth.Text = Resources.StrDepth.AddColon();
			_chkNoCheckout.Text = Resources.StrsNoCheckout;
		}

		public void Layout(Control parent)
		{
			var decUrl      = new TextBoxDecorator(_txtUrl);
			var decPath     = new FolderPickerTextBoxDecorator(_txtPath);
			var decRemote   = new TextBoxDecorator(_txtRemoteName);

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
						LayoutConstants.CheckBoxRowHeight,
						SizeSpec.Absolute(40),
						LayoutConstants.GroupSeparatorRowHeight,
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_lblUrl,  marginOverride: LayoutConstants.NoMargin), column: 0, row: 0),
						new GridContent(new ControlContent(decUrl,   marginOverride: LayoutConstants.TextBoxMargin), column: 1, row: 0),
						new GridContent(new ControlContent(_lblPath, marginOverride: LayoutConstants.NoMargin), column: 0, row: 1),
						new GridContent(new ControlContent(decPath,  marginOverride: LayoutConstants.TextBoxMargin), column: 1, row: 1),
						new GridContent(new WidgetContent(_chkAppendRepositoryNameFromUrl, marginOverride: LayoutConstants.NoMargin), column: 1, row: 2),
						new GridContent(new ControlContent(_targetPath, marginOverride: LayoutConstants.NoMargin), columnSpan: 2, row: 3),
						new GridContent(new ControlContent(_grpOptions, marginOverride: LayoutConstants.NoMargin), columnSpan: 2, row: 4),
						new GridContent(new Grid(
							padding: DpiBoundValue.Padding(new(12, 0, 0, 0)),
							columns:
							[
								SizeSpec.Absolute(106),
								SizeSpec.Everything(),
							],
							rows:
							[
								LayoutConstants.TextInputRowHeight,
								LayoutConstants.TextInputRowHeight,
								LayoutConstants.TextInputRowHeight,
								LayoutConstants.TextInputRowHeight,
								LayoutConstants.TextInputRowHeight,
								LayoutConstants.TextInputRowHeight,
								SizeSpec.Everything(),
							],
							content:
							[
								new GridContent(new ControlContent(_lblRemoteName,   marginOverride: LayoutConstants.NoMargin), column: 0, row: 0),
								new GridContent(new ControlContent(decRemote,        marginOverride: LayoutConstants.TextBoxMargin), column: 1, row: 0),
								new GridContent(new WidgetContent (_chkUseTemplate,  marginOverride: LayoutConstants.NoMargin), column: 0, row: 1),
								new GridContent(new ControlContent(_decTemplate,     marginOverride: LayoutConstants.TextBoxMargin), column: 1, row: 1),
								new GridContent(new WidgetContent (_chkBare,         marginOverride: LayoutConstants.NoMargin), column: 0, row: 2),
								new GridContent(new WidgetContent (_chkMirror,       marginOverride: LayoutConstants.NoMargin), column: 1, row: 2),
								new GridContent(new WidgetContent (_chkShallowClone, marginOverride: LayoutConstants.NoMargin), column: 0, row: 3),
								new GridContent(new Grid(
									columns:
									[
										SizeSpec.Absolute(64),
										SizeSpec.Absolute(76),
										SizeSpec.Everything(),
									],
									content:
									[
										new GridContent(new ControlContent(_lblDepth, marginOverride: LayoutConstants.NoMargin), column: 0),
										new GridContent(new ControlContent(_numDepth, marginOverride: LayoutConstants.TextBoxMargin), column: 1),
									]), column: 1, row: 3),
								new GridContent(new WidgetContent(_chkRecursive,  marginOverride: LayoutConstants.NoMargin), column: 0, row: 4),
								new GridContent(new WidgetContent(_chkNoCheckout, marginOverride: LayoutConstants.NoMargin), column: 0, row: 5),
							]), columnSpan: 2, row: 5),
					]),
			};

			var tabIndex = 0;

			_lblUrl.TabIndex = tabIndex++;
			decUrl.TabIndex = tabIndex++;
			_lblPath.TabIndex = tabIndex++;
			decPath.TabIndex = tabIndex++;
			_chkAppendRepositoryNameFromUrl.TabIndex = tabIndex++;
			_targetPath.TabIndex = tabIndex++;
			_grpOptions.TabIndex = tabIndex++;
			_lblRemoteName.TabIndex = tabIndex++;
			decRemote.TabIndex = tabIndex++;
			_chkUseTemplate.TabIndex = tabIndex++;
			_decTemplate.TabIndex = tabIndex++;
			_chkBare.TabIndex = tabIndex++;
			_chkMirror.TabIndex = tabIndex++;
			_chkShallowClone.TabIndex = tabIndex++;
			_lblDepth.TabIndex = tabIndex++;
			_numDepth.TabIndex = tabIndex++;
			_chkRecursive.TabIndex = tabIndex++;
			_chkNoCheckout.TabIndex = tabIndex++;

			_lblUrl.Parent = parent;
			decUrl.Parent = parent;
			_lblPath.Parent = parent;
			decPath.Parent = parent;
			_chkAppendRepositoryNameFromUrl.Parent = parent;
			_targetPath.Parent = parent;
			_grpOptions.Parent = parent;
			_lblRemoteName.Parent = parent;
			decRemote.Parent = parent;
			_chkUseTemplate.Parent = parent;
			_decTemplate.Parent = parent;
			_chkBare.Parent = parent;
			_chkMirror.Parent = parent;
			_chkShallowClone.Parent = parent;
			_lblDepth.Parent = parent;
			_numDepth.Parent = parent;
			_chkRecursive.Parent = parent;
			_chkNoCheckout.Parent = parent;
		}
	}

	private readonly DialogControls _controls;
	private readonly IUserInputSource<string?> _urlInput;
	private readonly RepositoryPathInput _repositoryPathInput;
	private readonly IUserInputSource<string?> _remoteNameInput;
	private readonly IUserInputSource<bool> _shallowCloneInput;
	private readonly IUserInputSource<int> _depthInput;
	private readonly IUserInputSource<bool> _useTemplateInput;
	private readonly IUserInputSource<string?> _templatePathInput;
	private readonly IUserInputSource<bool> _bareInput;
	private readonly IUserInputSource<bool> _mirrorInput;
	private readonly IUserInputSource<bool> _noCheckoutInput;
	private readonly IUserInputSource<bool> _recursiveInput;
	private readonly IUserInputErrorNotifier _errorNotifier;
	private readonly ICloneController _controller;

	public CloneDialog(IGitRepositoryProvider gitRepositoryProvider)
	{
		Verify.Argument.IsNotNull(gitRepositoryProvider);

		Name = nameof(CloneDialog);

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode = AutoScaleMode.Dpi;
		Size = ScalableSize.GetValue(Dpi.Default);
		ResumeLayout(performLayout: false);
		PerformLayout();

		_controls = new(GitterApplication.Style);
		_controls.Layout(this);
		Localize();

		var inputs = new IUserInputSource[]
		{
			_urlInput            = new TextBoxInputSource(_controls._txtUrl),
			_repositoryPathInput = new RepositoryPathInput(_controls._txtPath, _controls._txtUrl, _controls._chkAppendRepositoryNameFromUrl),
			_remoteNameInput     = new TextBoxInputSource(_controls._txtRemoteName),
			_shallowCloneInput   = new CheckBoxWidgetInputSource(_controls._chkShallowClone),
			_depthInput          = new UpDownInputSource(_controls._numDepth),
			_useTemplateInput    = new CheckBoxWidgetInputSource(_controls._chkUseTemplate),
			_templatePathInput   = new TextBoxInputSource(_controls._txtTemplate),
			_bareInput           = new CheckBoxWidgetInputSource(_controls._chkBare),
			_mirrorInput         = new CheckBoxWidgetInputSource(_controls._chkMirror),
			_noCheckoutInput     = new CheckBoxWidgetInputSource(_controls._chkNoCheckout),
			_recursiveInput      = new CheckBoxWidgetInputSource(_controls._chkRecursive),
		};
		_errorNotifier = new UserInputErrorNotifier(NotificationService, inputs);
		_repositoryPathInput.ValueChanged += (_, _) => UpdateTargetPathText();

		UpdateTargetPathText();

		_controls._chkShallowClone.IsCheckedChanged += _chkShallowClone_CheckedChanged;
		_controls._chkBare.IsCheckedChanged         += _chkBare_CheckedChanged;
		_controls._chkUseTemplate.IsCheckedChanged  += _chkUseTemplate_CheckedChanged;

		GitterApplication.FontManager.InputFont.Apply(_controls._txtUrl, _controls._txtPath, _controls._txtRemoteName);

		_controller = new CloneController(gitRepositoryProvider) { View = this };
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(400, 312));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrClone;

	public IUserInputSource<string?> Url => _urlInput;

	public IUserInputSource<string> RepositoryPath => _repositoryPathInput;

	public IUserInputSource<bool> Bare => _bareInput;

	public IUserInputSource<bool> Mirror => _mirrorInput;

	public IUserInputSource<bool> UseTemplate => _useTemplateInput;

	public IUserInputSource<string?> RemoteName => _remoteNameInput;

	public IUserInputSource<string?> TemplatePath => _templatePathInput;

	public IUserInputSource<bool> ShallowClone => _shallowCloneInput;

	public IUserInputSource<int> Depth => _depthInput;

	public IUserInputSource<bool> Recursive => _recursiveInput;

	public IUserInputSource<bool> NoCheckout => _noCheckoutInput;

	public IUserInputErrorNotifier ErrorNotifier => _errorNotifier;

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		BeginInvoke(_controls._txtUrl.Focus);
	}

	private void Localize()
	{
		Text = Resources.StrCloneRepository;
		_controls.Localize();
	}

	private void _btnSelectDirectory_Click(object? sender, EventArgs e)
	{
		var path = Utility.ShowPickFolderDialog(this);
		if(path is not null)
		{
			_controls._txtPath.Text = path;
		}
	}

	private void _btnSelectTemplate_Click(object? sender, EventArgs e)
	{
		var path = Utility.ShowPickFolderDialog(this);
		if(path is not null)
		{
			_controls._txtTemplate.Text = path;
		}
	}

	private void UpdateTargetPathText()
	{
		var path = RepositoryPath.Value;
		if(string.IsNullOrWhiteSpace(path))
		{
			_controls._targetPath.Text = string.Empty;
		}
		else
		{
			_controls._targetPath.Text = path;
		}
	}

	private void _chkUseTemplate_CheckedChanged(object? sender, EventArgs e)
	{
		bool enabled = _controls._chkUseTemplate.IsChecked;
		_controls._decTemplate.Enabled = enabled;
	}

	private void _chkShallowClone_CheckedChanged(object? sender, EventArgs e)
	{
		bool enabled = _controls._chkShallowClone.IsChecked;
		_controls._lblDepth.Enabled = enabled;
		_controls._numDepth.Enabled = enabled;
	}

	private void _chkBare_CheckedChanged(object? sender, EventArgs e)
	{
		_controls._chkMirror.Enabled = _controls._chkBare.IsChecked;
	}

	public bool Execute() => _controller.TryClone();
}
