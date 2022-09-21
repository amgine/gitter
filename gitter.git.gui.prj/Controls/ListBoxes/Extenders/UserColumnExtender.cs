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

namespace gitter.Git.Gui.Controls;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;

using Resources = gitter.Git.Gui.Properties.Resources;

/// <summary>Extender for <see cref="UserColumn"/>.</summary>
[ToolboxItem(false)]
partial class UserColumnExtender : ExtenderBase
{
	private readonly DpiBindings _dpiBindings;
	private readonly ControlLayout _layout;
	private ICheckBoxWidget _chkShowEmail;
	private ICheckBoxWidget _chkShowAvatar;
	private bool _disableEvents;

	/// <summary>Create <see cref="UserColumnExtender"/>.</summary>
	/// <param name="column">Related column.</param>
	public UserColumnExtender(UserColumn column)
		: base(column)
	{
		SuspendLayout();
		Name = nameof(UserColumnExtender);
		Size = new(138, 52);
		ResumeLayout();

		_dpiBindings = new(this);
		_layout      = new(this);

		CreateControls();
		SubscribeToColumnEvents();
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(138, 52));

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			if(_chkShowEmail is not null)
			{
				_chkShowEmail.Dispose();
				_chkShowEmail = null;
			}
			if(_chkShowAvatar is not null)
			{
				_chkShowAvatar.Dispose();
				_chkShowAvatar = null;
			}
			UnsubscribeFromColumnEvents();
		}
		base.Dispose(disposing);
	}

	private void CreateControls()
	{
		var conv = new DpiConverter(this);

		var height  = conv.ConvertY(27);
		var spacing = conv.ConvertY(-4);

		_dpiBindings.UnbindAll();

		_chkShowEmail?.Dispose();
		_chkShowEmail = Style.CreateCheckBox();
		_chkShowEmail.IsChecked = Column.ShowEmail;
		_chkShowEmail.IsCheckedChanged += OnShowEmailCheckedChanged;
		_chkShowEmail.Text = Resources.StrShowEmail;
		_chkShowEmail.Control.Parent = this;
		_dpiBindings.BindImage(_chkShowEmail, Icons.Mail);

		_chkShowAvatar?.Dispose();
		_chkShowAvatar = Style.CreateCheckBox();
		_chkShowAvatar.IsChecked = Column.ShowAvatar;
		_chkShowAvatar.IsCheckedChanged += OnShowAvatarCheckedChanged;
		_chkShowAvatar.Text = Resources.StrShowAvatar;
		_chkShowAvatar.Control.Parent = this;
		_dpiBindings.BindImage(_chkShowAvatar, CommonIcons.Gravatar);

		var noMargin = DpiBoundValue.Constant(Padding.Empty);
		_layout.Content = new Grid(
			padding: DpiBoundValue.Padding(new(6, 2, 6, 2)),
			rows: new[]
			{
				SizeSpec.Absolute(23),
				SizeSpec.Absolute(23),
			},
			content: new[]
			{
				new GridContent(new ControlContent(_chkShowEmail.Control,  marginOverride: noMargin), row: 0),
				new GridContent(new ControlContent(_chkShowAvatar.Control, marginOverride: noMargin), row: 1),
			});
	}

	/// <inheritdoc/>
	protected override void OnStyleChanged()
	{
		base.OnStyleChanged();
		CreateControls();
	}

	public new UserColumn Column => (UserColumn)base.Column;

	private void SubscribeToColumnEvents()
	{
		Column.ShowEmailChanged  += OnColumnShowEmailChanged;
		Column.ShowAvatarChanged += OnColumnShowAvatarChanged;
	}

	private void UnsubscribeFromColumnEvents()
	{
		Column.ShowEmailChanged  -= OnColumnShowEmailChanged;
		Column.ShowAvatarChanged -= OnColumnShowAvatarChanged;
	}

	private void OnColumnShowEmailChanged(object sender, EventArgs e)
	{
		ShowEmail = Column.ShowEmail;
	}

	private void OnColumnShowAvatarChanged(object sender, EventArgs e)
	{
		ShowAvatar = Column.ShowAvatar;
	}

	public bool ShowEmail
	{
		get => _chkShowEmail is not null ? _chkShowEmail.IsChecked : Column.ShowEmail;
		private set
		{
			if(_chkShowEmail is not null)
			{
				_disableEvents = true;
				_chkShowEmail.IsChecked = value;
				_disableEvents = false;
			}
		}
	}

	public bool ShowAvatar
	{
		get => _chkShowAvatar is not null ? _chkShowAvatar.IsChecked : Column.ShowAvatar;
		private set
		{
			if(_chkShowAvatar is not null)
			{
				_disableEvents = true;
				_chkShowAvatar.IsChecked = value;
				_disableEvents = false;
			}
		}
	}

	private void OnShowEmailCheckedChanged(object sender, EventArgs e)
	{
		if(!_disableEvents)
		{
			_disableEvents = true;
			Column.ShowEmail = _chkShowEmail.IsChecked;
			_disableEvents = false;
		}
	}

	private void OnShowAvatarCheckedChanged(object sender, EventArgs e)
	{
		if(!_disableEvents)
		{
			_disableEvents = true;
			Column.ShowAvatar = _chkShowAvatar.IsChecked;
			_disableEvents = false;
		}
	}
}
