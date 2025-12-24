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
using System.Drawing;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Configuration;

using Resources = gitter.Git.Gui.Properties.Resources;

/// <summary>"User" column.</summary>
public class UserColumn : CustomListBoxColumn
{
	public const bool DefaultShowEmail  = false;
	public const bool DefaultShowAvatar = false;

	private const string EmailFormat = "{0} <{1}>";

	#region Data

	private bool _showEmail  = DefaultShowEmail;
	private bool _showAvatar = DefaultShowAvatar;
	private UserColumnExtender? _extender;

	#endregion

	#region Events

	public event EventHandler? ShowEmailChanged;

	public event EventHandler? ShowAvatarChanged;

	#endregion

	#region .ctor

	protected UserColumn(ISubItemPainter painter, int id, string name, bool visible)
		: base(id, name, visible)
	{
		Verify.Argument.IsNotNull(painter);

		Width   = 80;
		Painter = painter;
	}

	public UserColumn(ISubItemPainter painter, string name, bool visible)
		: this(painter, (int)ColumnId.User, name, visible)
	{
	}

	public UserColumn(ISubItemPainter painter, bool visible)
		: this(painter, (int)ColumnId.User, Resources.StrUser, visible)
	{
	}

	public UserColumn(ISubItemPainter painter)
		: this(painter, (int)ColumnId.User, Resources.StrUser, true)
	{
	}

	#endregion

	private ISubItemPainter Painter { get; }

	/// <inheritdoc/>
	protected override void OnListBoxAttached(CustomListBox listBox)
	{
		base.OnListBoxAttached(listBox);
		_extender = new UserColumnExtender(this);
		Extender = new Popup(_extender);
	}

	/// <inheritdoc/>
	protected override void OnListBoxDetached(CustomListBox listBox)
	{
		DisposeExtender();
		DisposableUtility.Dispose(ref _extender);
		base.OnListBoxDetached(listBox);
	}

	public bool ShowEmail
	{
		get => _showEmail;
		set
		{
			if(_showEmail == value) return;

			_showEmail = value;
			AutoSize(80);
			ShowEmailChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public bool ShowAvatar
	{
		get => _showAvatar;
		set
		{
			if(_showAvatar == value) return;

			_showAvatar = value;
			InvalidateContent();
			ShowAvatarChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	/// <inheritdoc/>
	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		Assert.IsNotNull(measureEventArgs);

		if(Painter.TryMeasure(measureEventArgs, out var size))
		{
			return size;
		}
		return Size.Empty;
	}

	/// <inheritdoc/>
	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(paintEventArgs);

		Painter.TryPaint(paintEventArgs);
	}

	/// <inheritdoc/>
	protected override void SaveMoreTo(Section section)
	{
		Assert.IsNotNull(section);

		base.SaveMoreTo(section);
		section.SetValue("ShowEmail",  ShowEmail);
		section.SetValue("ShowAvatar", ShowAvatar);
	}

	/// <inheritdoc/>
	protected override void LoadMoreFrom(Section section)
	{
		Assert.IsNotNull(section);

		base.LoadMoreFrom(section);
		ShowEmail  = section.GetValue("ShowEmail",  ShowEmail);
		ShowAvatar = section.GetValue("ShowAvatar", ShowAvatar);
	}

	/// <inheritdoc/>
	public override string IdentificationString => "User";
}
