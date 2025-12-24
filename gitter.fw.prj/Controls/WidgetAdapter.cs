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

namespace gitter.Framework.Controls;

using System.Drawing;
using System.Windows.Forms;

public abstract class WidgetAdapter<T>(T control) : IWidget
	where T : Control
{
	protected readonly T _control = control;

	public Control Control => _control;

	public Control? Parent
	{
		get => _control.Parent;
		set => _control.Parent = value;
	}

	public bool Enabled
	{
		get => _control.Enabled;
		set => _control.Enabled = value;
	}

	public Rectangle Bounds
	{
		get => _control.Bounds;
		set => _control.Bounds = value;
	}

	public Padding Margin
	{
		get => _control.Margin;
		set => _control.Margin = value;
	}

	public Padding Padding
	{
		get => _control.Padding;
		set => _control.Padding = value;
	}

	public DockStyle Dock
	{
		get => _control.Dock;
		set => _control.Dock = value;
	}

	public AnchorStyles Anchor
	{
		get => _control.Anchor;
		set => _control.Anchor = value;
	}

	public int TabIndex
	{
		get => _control.TabIndex;
		set => _control.TabIndex = value;
	}

	public virtual string Text
	{
		get => _control.Text;
		set => _control.Text = value;
	}

	public virtual object? Tag
	{
		get => _control.Tag;
		set => _control.Tag = value;
	}

	public bool IsDisposed => _control.IsDisposed;

	public virtual void Dispose()
	{
		_control.Dispose();
	}
}
