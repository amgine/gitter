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
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;
using gitter.Framework.Services;

using Resources = gitter.Git.Gui.Properties.Resources;

/// <summary>Dialog for displaying and/or editing <see cref="Remote"/> object properties.</summary>
public partial class RemotePropertiesDialog : GitDialogBase, IExecutableDialog
{
	readonly struct DialogControls
	{
		public readonly LabelControl _lblFetchURL;
		public readonly LabelControl _lblPushURL;
		public readonly TextBox _txtFetchURL;
		public readonly TextBox _txtPushURL;
		public readonly TextBox _txtProxy;
		public readonly LabelControl _lblProxy;
		public readonly GroupSeparator _grpUpdatedReferences;
		public readonly GroupSeparator _grpOptions;
		public readonly ICheckBoxWidget _chkMirror;
		public readonly ICheckBoxWidget _chkSkipFetchAll;
		public readonly LabelControl _lblVCS;
		public readonly TextBox _txtVCS;
		public readonly LabelControl _lblReceivePack;
		public readonly LabelControl _lblUploadPack;
		public readonly TextBox _txtReceivePack;
		public readonly TextBox _txtUploadPack;
		public readonly LabelControl _lblFetchTags;
		public readonly IRadioButtonWidget _radNormal;
		public readonly IRadioButtonWidget _radFetchAll;
		public readonly IRadioButtonWidget _radFetchNone;
		public readonly CustomListBox _lstUpdatedReferences;
		public readonly IButtonWidget _btnAddRefspec;
		public readonly LabelControl _lblRefspec;
		public readonly TextBox _txtRefspec;
		public readonly IRadioButtonWidget _radFetch;
		public readonly IRadioButtonWidget _radPush;

		public DialogControls(IGitterStyle style)
		{
			style ??= GitterApplication.Style;

			var cbf = style.CheckBoxFactory;
			var rbf = style.RadioButtonFactory;

			_radFetchNone         = rbf.Create();
			_radFetchAll          = rbf.Create();
			_radNormal            = rbf.Create();
			_lblFetchTags         = new();
			_lblUploadPack        = new();
			_lblReceivePack       = new();
			_lblVCS               = new();
			_chkSkipFetchAll      = cbf.Create();
			_chkMirror            = cbf.Create();
			_grpOptions           = new();
			_grpUpdatedReferences = new();
			_txtUploadPack        = new();
			_txtReceivePack       = new();
			_txtVCS               = new();
			_txtProxy             = new();
			_txtPushURL           = new();
			_txtFetchURL          = new();
			_lblProxy             = new();
			_lblPushURL           = new();
			_lblFetchURL          = new();
			_lstUpdatedReferences = new() { Style = style, Multiselect = true, AllowColumnReorder = false };
			_btnAddRefspec        = style.ButtonFactory.Create();
			_lblRefspec           = new();
			_txtRefspec           = new();
			_radFetch             = rbf.Create();
			_radPush              = rbf.Create();

			_radFetch.IsChecked = true;

			_lstUpdatedReferences.Columns.AddRange(
			[
				new CustomListBoxColumn(0, "") { SizeMode = ColumnSizeMode.Fixed, Width = 20 },
				new CustomListBoxColumn(1, "") { SizeMode = ColumnSizeMode.Fixed, Width = 20 },
				new CustomListBoxColumn(2, Resources.StrFrom) { SizeMode = ColumnSizeMode.Fill },
				new CustomListBoxColumn(3, Resources.StrTo) { SizeMode = ColumnSizeMode.Fill },
			]);

			GitterApplication.FontManager.InputFont.Apply(
				_txtFetchURL, _txtPushURL, _txtProxy, _txtVCS, _txtRefspec, _txtReceivePack, _txtUploadPack);
		}

		public void Localize()
		{
			_lblFetchURL.Text = Resources.StrFetchUrl.AddColon();
			_lblPushURL.Text = Resources.StrPushUrl.AddColon();
			_lblProxy.Text = Resources.StrProxy.AddColon();
			_lblVCS.Text = Resources.StrVCS.AddColon();

			_grpUpdatedReferences.Text = Resources.StrsUpdatedReferences;
			_lblRefspec.Text = Resources.StrRefspec.AddColon();
			_btnAddRefspec.Text = Resources.StrAdd;

			_grpOptions.Text = Resources.StrsDefaultBehavior;

			_lblReceivePack.Text = Resources.StrsReceivePack.AddColon();
			_lblUploadPack.Text = Resources.StrsUploadPack.AddColon();

			_chkMirror.Text = Resources.StrMirror;
			_chkSkipFetchAll.Text = Resources.StrSkipFetchAll;
			_lblFetchTags.Text = Resources.StrsFetchTags.AddColon();

			_radNormal.Text = Resources.StrDefault;
			_radFetchAll.Text = Resources.StrAll;
			_radFetchNone.Text = Resources.StrNone;

			_radFetch.Text = Resources.StrFetch;
			_radPush.Text = Resources.StrPush;
		}

		public void Layout(Control parent)
		{
			var fetchDec   = new TextBoxDecorator(_txtFetchURL);
			var pushDec    = new TextBoxDecorator(_txtPushURL);
			var proxyDec   = new TextBoxDecorator(_txtProxy);
			var vcsDec     = new TextBoxDecorator(_txtVCS);
			var receiveDec = new TextBoxDecorator(_txtReceivePack);
			var uploadDec  = new TextBoxDecorator(_txtUploadPack);
			var refspecDec = new TextBoxDecorator(_txtRefspec);

			var pnlFetchTags      = new Panel();
			var pnlAddRefspecType = new Panel();

			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					columns:
					[
						SizeSpec.Absolute(106),
						SizeSpec.Everything(),
					],
					rows:
					[
						/*  0 */ LayoutConstants.TextInputRowHeight,
						/*  1 */ LayoutConstants.TextInputRowHeight,
						/*  2 */ LayoutConstants.TextInputRowHeight,
						/*  3 */ LayoutConstants.TextInputRowHeight,
						/*  4 */ LayoutConstants.GroupSeparatorRowHeight,
						/*  5 */ LayoutConstants.TextInputRowHeight,
						/*  6 */ LayoutConstants.TextInputRowHeight,
						/*  7 */ LayoutConstants.RadioButtonRowHeight,
						/*  8 */ LayoutConstants.CheckBoxRowHeight,
						/*  9 */ LayoutConstants.CheckBoxRowHeight,
						/* 10 */ LayoutConstants.GroupSeparatorRowHeight,
						/* 11 */ SizeSpec.Everything(),
						/* 12 */ LayoutConstants.TextInputRowHeight,
						/* 13 */ LayoutConstants.RadioButtonRowHeight,
					],
					content:
					[
						new GridContent(new ControlContent(_lblFetchURL,          marginOverride: LayoutConstants.TextBoxLabelMargin), row: 0, column: 0),
						new GridContent(new ControlContent(fetchDec,              marginOverride: LayoutConstants.TextBoxMargin), row: 0, column: 1),
						new GridContent(new ControlContent(_lblPushURL,           marginOverride: LayoutConstants.TextBoxLabelMargin), row: 1, column: 0),
						new GridContent(new ControlContent(pushDec,               marginOverride: LayoutConstants.TextBoxMargin), row: 1, column: 1),
						new GridContent(new ControlContent(_lblProxy,             marginOverride: LayoutConstants.TextBoxLabelMargin), row: 2, column: 0),
						new GridContent(new ControlContent(proxyDec,              marginOverride: LayoutConstants.TextBoxMargin), row: 2, column: 1),
						new GridContent(new ControlContent(_lblVCS,               marginOverride: LayoutConstants.TextBoxLabelMargin), row: 3, column: 0),
						new GridContent(new ControlContent(vcsDec,                marginOverride: LayoutConstants.TextBoxMargin), row: 3, column: 1),
						new GridContent(new ControlContent(_grpOptions,           marginOverride: LayoutConstants.NoMargin), row: 4, columnSpan: 2),
						new GridContent(new ControlContent(_lblReceivePack,       marginOverride: LayoutConstants.TextBoxLabelMargin), row: 5, column: 0),
						new GridContent(new ControlContent(receiveDec,            marginOverride: LayoutConstants.TextBoxMargin), row: 5, column: 1),
						new GridContent(new ControlContent(_lblUploadPack,        marginOverride: LayoutConstants.TextBoxLabelMargin), row: 6, column: 0),
						new GridContent(new ControlContent(uploadDec,             marginOverride: LayoutConstants.TextBoxMargin), row: 6, column: 1),
						new GridContent(new ControlContent(_lblFetchTags,         marginOverride: LayoutConstants.NoMargin), row: 7, column: 0),
						new GridContent(new ControlContent(pnlFetchTags,          marginOverride: LayoutConstants.NoMargin), row: 7, column: 1),
						new GridContent(new WidgetContent (_chkMirror,            marginOverride: LayoutConstants.NoMargin), row:  8),
						new GridContent(new WidgetContent (_chkSkipFetchAll,      marginOverride: LayoutConstants.NoMargin), row:  9),
						new GridContent(new ControlContent(_grpUpdatedReferences, marginOverride: LayoutConstants.NoMargin), row: 10, columnSpan: 2),
						new GridContent(new ControlContent(_lstUpdatedReferences, marginOverride: LayoutConstants.NoMargin), row: 11, columnSpan: 2),
						new GridContent(new ControlContent(_lblRefspec,           marginOverride: LayoutConstants.TextBoxLabelMargin), row: 12, column: 0),
						new GridContent(new Grid(
							columns:
							[
								SizeSpec.Everything(),
								SizeSpec.Absolute(4),
								SizeSpec.Absolute(75),
							],
							content:
							[
								new GridContent(new ControlContent(refspecDec,     marginOverride: LayoutConstants.TextBoxMargin), column: 0),
								new GridContent(new WidgetContent (_btnAddRefspec, marginOverride: LayoutConstants.TextBoxMargin), column: 2),
							]), row: 12, column: 1),
						new GridContent(new ControlContent(pnlAddRefspecType,     marginOverride: LayoutConstants.NoMargin), row: 13, column: 1),
					]),
			};

			_ = new ControlLayout(pnlFetchTags)
			{
				Content = new Grid(
					columns:
					[
						SizeSpec.Absolute(80),
						SizeSpec.Absolute(80),
						SizeSpec.Absolute(80),
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new WidgetContent(_radNormal,    marginOverride: LayoutConstants.NoMargin), column: 0),
						new GridContent(new WidgetContent(_radFetchAll,  marginOverride: LayoutConstants.NoMargin), column: 1),
						new GridContent(new WidgetContent(_radFetchNone, marginOverride: LayoutConstants.NoMargin), column: 2),
					]),
			};

			_ = new ControlLayout(pnlAddRefspecType)
			{
				Content = new Grid(
					columns:
					[
						SizeSpec.Absolute(80),
						SizeSpec.Absolute(80),
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new WidgetContent(_radFetch, marginOverride: LayoutConstants.NoMargin), column: 0),
						new GridContent(new WidgetContent(_radPush,  marginOverride: LayoutConstants.NoMargin), column: 1),
					]),
			};

			var tabIndex = 0;
			_lblFetchURL.TabIndex = tabIndex++;
			fetchDec.TabIndex = tabIndex++;
			_lblPushURL.TabIndex = tabIndex++;
			pushDec.TabIndex = tabIndex++;
			_lblProxy.TabIndex = tabIndex++;
			proxyDec.TabIndex = tabIndex++;
			_lblVCS.TabIndex = tabIndex++;
			vcsDec.TabIndex = tabIndex++;
			_grpOptions.TabIndex = tabIndex++;
			_lblReceivePack.TabIndex = tabIndex++;
			receiveDec.TabIndex = tabIndex++;
			_lblUploadPack.TabIndex = tabIndex++;
			uploadDec.TabIndex = tabIndex++;
			_lblFetchTags.TabIndex = tabIndex++;
			pnlFetchTags.TabIndex += tabIndex++;
			_radNormal.TabIndex += tabIndex++;
			_radFetchAll.TabIndex += tabIndex++;
			_radFetchNone.TabIndex += tabIndex++;
			_chkMirror.TabIndex = tabIndex++;
			_chkSkipFetchAll.TabIndex = tabIndex++;
			_grpUpdatedReferences.TabIndex = tabIndex++;
			_lstUpdatedReferences.TabIndex = tabIndex++;
			_lblRefspec.TabIndex = tabIndex++;
			refspecDec.TabIndex = tabIndex++;
			_btnAddRefspec.TabIndex = tabIndex++;
			pnlAddRefspecType.TabIndex = tabIndex++;
			_radFetch.TabIndex = tabIndex++;
			_radPush.TabIndex = tabIndex++;

			_lblFetchURL.Parent = parent;
			fetchDec.Parent = parent;
			_lblPushURL.Parent = parent;
			pushDec.Parent = parent;
			_lblProxy.Parent = parent;
			proxyDec.Parent = parent;
			_lblVCS.Parent = parent;
			vcsDec.Parent = parent;
			_grpOptions.Parent = parent;
			_lblReceivePack.Parent = parent;
			receiveDec.Parent = parent;
			_lblUploadPack.Parent = parent;
			uploadDec.Parent = parent;
			_lblFetchTags.Parent = parent;
			_radNormal.Parent = pnlFetchTags;
			_radFetchAll.Parent = pnlFetchTags;
			_radFetchNone.Parent = pnlFetchTags;
			pnlFetchTags.Parent = parent;
			_chkMirror.Parent = parent;
			_chkSkipFetchAll.Parent = parent;
			_grpUpdatedReferences.Parent = parent;
			_lstUpdatedReferences.Parent = parent;
			_lblRefspec.Parent = parent;
			refspecDec.Parent = parent;
			_btnAddRefspec.Parent = parent;
			_radFetch.Parent = pnlAddRefspecType;
			_radPush.Parent = pnlAddRefspecType;
			pnlAddRefspecType.Parent = parent;
		}
	}

	private sealed class RefspecItem : CustomListBoxItem<string>
	{
		private readonly bool _forced;
		private readonly string _from;
		private readonly string _to;
		private readonly bool _fetch;

		public RefspecItem(string refspec, bool fetch)
			: base(refspec)
		{
			_fetch = fetch;
			int pos = refspec.IndexOf(':');
			int start;
			if(refspec[0] == '+')
			{
				start = 1;
				_forced = true;
			}
			else
			{
				start = 0;
			}
			if(pos == -1)
			{
				_from = refspec.Substring(start, refspec.Length - start);
				_to = "";
			}
			else
			{
				_from = refspec.Substring(start, pos - start);
				_to = refspec.Substring(pos + 1);
			}
		}

		public bool Fetch => _fetch;

		public bool Forced => _forced;

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			return measureEventArgs.SubItemId switch
			{
				0 => measureEventArgs.MeasureImage(Icons.Pull.GetImage(measureEventArgs.Dpi.X * 16 / 96)),
				1 => measureEventArgs.MeasureImage(Icons.Plus.GetImage(measureEventArgs.Dpi.X * 16 / 96)),
				2 => measureEventArgs.MeasureText(_from),
				3 => measureEventArgs.MeasureText(_to),
				_ => Size.Empty,
			};
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch(paintEventArgs.SubItemId)
			{
				case 0:
					{
						var image = _fetch ? Icons.Pull : Icons.Push;
						paintEventArgs.PaintImage(image.GetImage(paintEventArgs.Dpi.X * 16 / 96));
					}
					break;
				case 1:
					if(_forced)
					{
						paintEventArgs.PaintImage(Icons.Plus.GetImage(paintEventArgs.Dpi.X * 16 / 96));
					}
					break;
				case 2:
					paintEventArgs.PaintText(_from);
					break;
				case 3:
					paintEventArgs.PaintText(_to);
					break;
			}
		}
	}

	private readonly DialogControls _controls;

	/// <summary>Create <see cref="RemotePropertiesDialog"/>.</summary>
	/// <param name="remote">Related remote.</param>
	public RemotePropertiesDialog(Remote remote)
	{
		Verify.Argument.IsNotNull(remote);

		Remote = remote;
		Name   = nameof(RemotePropertiesDialog);
		Text   = string.Format("{0}: {1}", Resources.StrProperties, remote.Name);

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Size                = ScalableSize.GetValue(Dpi.Default);
		_controls = new DialogControls(GitterApplication.Style);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		_controls._txtFetchURL.Text = remote.FetchUrl;
		_controls._txtPushURL.Text = remote.PushUrl;
		_controls._txtProxy.Text = remote.Proxy;
		_controls._txtVCS.Text = remote.VCS;
		_controls._txtReceivePack.Text = remote.ReceivePack;
		_controls._txtUploadPack.Text = remote.ReceivePack;
		_controls._chkMirror.IsChecked = remote.Mirror;
		_controls._chkSkipFetchAll.IsChecked = remote.SkipFetchAll;
		TagFetchMode = remote.TagFetchMode;

		var fetchRefspecs = remote.FetchRefspec.Split([' '], StringSplitOptions.RemoveEmptyEntries);
		for(int i = 0; i < fetchRefspecs.Length; ++i)
		{
			_controls._lstUpdatedReferences.Items.Add(new RefspecItem(fetchRefspecs[i], true));
		}
		var pushRefspecs = remote.PushRefspec.Split([' '], StringSplitOptions.RemoveEmptyEntries);
		for(int i = 0; i < pushRefspecs.Length; ++i)
		{
			_controls._lstUpdatedReferences.Items.Add(new RefspecItem(pushRefspecs[i], false));
		}

		ToolTipService.Register(_controls._txtFetchURL, Resources.TipRemoteFetchURL);
		ToolTipService.Register(_controls._txtPushURL, Resources.TipRemotePushURL);
		ToolTipService.Register(_controls._txtProxy, Resources.TipRemoteProxy);
		ToolTipService.Register(_controls._txtVCS, Resources.TipRemoteVCS);
		ToolTipService.Register(_controls._txtUploadPack, Resources.TipRemoteUploadPack);
		ToolTipService.Register(_controls._txtReceivePack, Resources.TipRemoteReceivePack);
		ToolTipService.Register(_controls._chkMirror.Control, Resources.TipRemoteMirror);
		ToolTipService.Register(_controls._chkSkipFetchAll.Control, Resources.TipRemoteSkipFetchAll);

		_controls._lstUpdatedReferences.KeyDown += OnUpdatedReferencesKeyDown;
		_controls._btnAddRefspec.Click          += OnAddRefspecClick;
	}

	private void OnUpdatedReferencesKeyDown(object? sender, KeyEventArgs e)
	{
		if(e.KeyCode == Keys.Delete)
		{
			while(_controls._lstUpdatedReferences.SelectedItems.Count != 0)
			{
				_controls._lstUpdatedReferences.SelectedItems[0].Remove();
			}
		}
	}

	private void GetRefspecs(out string fetch, out string push)
	{
		var sbfetch = new StringBuilder();
		var sbpush  = new StringBuilder();
		foreach(RefspecItem refspec in _controls._lstUpdatedReferences.Items)
		{
			var sb = (refspec.Fetch) ? sbfetch : sbpush;
			if(sb.Length != 0) sb.Append(' ');
			sb.Append(refspec.DataContext);
		}
		fetch = sbfetch.ToString();
		push  = sbpush.ToString();
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(400, 452));

	protected override bool ScaleChildren => false;

	public Remote Remote { get; }

	public string FetchURL
	{
		get => _controls._txtFetchURL.Text;
		set => _controls._txtFetchURL.Text = value;
	}

	public string PushURL
	{
		get => _controls._txtPushURL.Text;
		set => _controls._txtPushURL.Text = value;
	}

	public string Proxy
	{
		get => _controls._txtProxy.Text;
		set => _controls._txtProxy.Text = value;
	}

	public string VCS
	{
		get => _controls._txtVCS.Text;
		set => _controls._txtVCS.Text = value;
	}

	public bool Mirror
	{
		get => _controls._chkMirror.IsChecked;
		set => _controls._chkMirror.IsChecked = value;
	}

	public bool SkipFetchAll
	{
		get => _controls._chkSkipFetchAll.IsChecked;
		set => _controls._chkSkipFetchAll.IsChecked = value;
	}

	public TagFetchMode TagFetchMode
	{
		get
		{
			if(_controls._radFetchAll.IsChecked)  return TagFetchMode.AllTags;
			if(_controls._radFetchNone.IsChecked) return TagFetchMode.NoTags;
			return TagFetchMode.Default;
		}
		set
		{
			var radio = value switch
			{
				TagFetchMode.AllTags => _controls._radFetchAll,
				TagFetchMode.NoTags  => _controls._radFetchNone,
				TagFetchMode.Default => _controls._radNormal,
				_ => default,
			};
			if(radio is not null) radio.IsChecked = true;
		}
	}

	private void OnAddRefspecClick(object? sender, EventArgs e)
	{
		var refspec = _controls._txtRefspec.Text.Trim();
		if(string.IsNullOrWhiteSpace(refspec))
		{
			NotificationService.NotifyInputError(
				_controls._txtRefspec,
				Resources.ErrInvalidRefspec,
				Resources.ErrRefspecCannotBeEmpty);
			return;
		}
		if(refspec.IndexOf(' ') != -1)
		{
			NotificationService.NotifyInputError(
				_controls._txtRefspec,
				Resources.ErrInvalidRefspec,
				Resources.ErrRefspecCannotContainSpaces);
			return;
		}
		_controls._lstUpdatedReferences.Items.Add(new RefspecItem(refspec, _controls._radFetch.IsChecked));
		_controls._txtRefspec.Clear();
	}

	#region IExecutableDialog Members

	public bool Execute()
	{
		string fetchUrl = _controls._txtFetchURL.Text.Trim();
		string pushUrl  = _controls._txtPushURL.Text.Trim();
		string proxy    = _controls._txtProxy.Text.Trim();
		string vcs      = _controls._txtVCS.Text.Trim();
		GetRefspecs(out var fetch, out var push);
		var receivePack  = _controls._txtReceivePack.Text.Trim();
		var uploadPack   = _controls._txtUploadPack.Text.Trim();
		var mirror       = _controls._chkMirror.IsChecked;
		var skipFetchAll = _controls._chkSkipFetchAll.IsChecked;
		var tagFetchMode = TagFetchMode;
		try
		{
			using(this.ChangeCursor(Cursors.WaitCursor))
			{
				lock(Remote.Repository.Configuration.SyncRoot)
				{
					Remote.FetchUrl		= fetchUrl;
					Remote.PushUrl		= pushUrl;
					Remote.Proxy		= proxy;
					Remote.VCS			= vcs;
					Remote.FetchRefspec	= fetch;
					Remote.PushRefspec	= push;
					Remote.ReceivePack	= receivePack;
					Remote.UploadPack	= uploadPack;
					Remote.Mirror		= mirror;
					Remote.SkipFetchAll	= skipFetchAll;
					Remote.TagFetchMode	= tagFetchMode;
				}
			}
		}
		catch(GitException exc)
		{
			GitterApplication.MessageBoxService.Show(
				this,
				exc.Message,
				Resources.ErrFailedToSetRemoteProperties,
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
		}
		return true;
	}

	#endregion
}
