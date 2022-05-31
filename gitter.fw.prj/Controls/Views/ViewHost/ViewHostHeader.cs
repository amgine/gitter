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

namespace gitter.Framework.Controls;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

[ToolboxItem(false)]
[DesignerCategory("")]
public sealed class ViewHostHeader : Control
{
	#region Events

	public event EventHandler<ViewButtonClickEventArgs> HeaderButtonClick
	{
		add    => Buttons.ButtonClick += value;
		remove => Buttons.ButtonClick -= value;
	}

	#endregion

	#region Data

	private bool _buttonsHovered;

	#endregion

	/// <summary>Create <see cref="ViewHostHeader"/>.</summary>
	internal ViewHostHeader(ViewHost viewHost)
	{
		Verify.Argument.IsNotNull(viewHost);

		ViewHost = viewHost;
		SetStyle(
			ControlStyles.ContainerControl |
			ControlStyles.Selectable |
			ControlStyles.SupportsTransparentBackColor,
			false);
		SetStyle(
			ControlStyles.UserPaint |
			ControlStyles.ResizeRedraw |
			ControlStyles.AllPaintingInWmPaint |
			ControlStyles.OptimizedDoubleBuffer,
			true);

		Buttons = new ViewButtons(this);
	}

	public ViewHost ViewHost { get; }

	public ViewButtons Buttons { get; }

	/// <inheritdoc/>
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override string Text
	{
		get => ViewHost.Text;
		set => throw new InvalidOperationException();
	}

	public void SetAvailableButtons(params ViewButtonType[] buttons)
	{
		Buttons.SetAvailableButtons(buttons);
	}

	/// <inheritdoc/>
	protected override void OnPaint(PaintEventArgs e)
	{
		Assert.IsNotNull(e);

		ViewManager.Renderer.RenderViewHostHeader(this, e);
		if(Buttons.Count != 0)
		{
			Buttons.OnPaint(e.Graphics, GetButtonsRect(), ViewHost.IsActive);
		}
	}

	/// <inheritdoc/>
	protected override void OnPaintBackground(PaintEventArgs pevent)
	{
	}

	/// <inheritdoc/>
	protected override void OnResize(EventArgs e)
	{
		Buttons.Height = Height;
		base.OnResize(e);
	}

	private Rectangle GetButtonsRect()
	{
		var rc = ClientRectangle;
		var w = Buttons.Width;
		return new Rectangle(rc.Width - w - 2, 0, w, rc.Height);
	}

	/// <inheritdoc/>
	protected override void OnMouseMove(MouseEventArgs e)
	{
		Assert.IsNotNull(e);

		if(Buttons.Count != 0)
		{
			var rc = GetButtonsRect();
			if(rc.Contains(e.X, e.Y))
			{
				var x = e.X - rc.X;
				var y = e.Y - rc.Y;
				Buttons.OnMouseMove(x, y, e.Button);
				_buttonsHovered = true;
			}
			else
			{
				if(_buttonsHovered)
				{
					Buttons.OnMouseLeave();
					_buttonsHovered = false;
				}
				if(Buttons.PressedButton is null)
				{
					base.OnMouseMove(e);
				}
			}
		}
		else
		{
			base.OnMouseMove(e);
		}
	}

	/// <inheritdoc/>
	protected override void OnMouseLeave(EventArgs e)
	{
		if(_buttonsHovered)
		{
			Buttons.OnMouseLeave();
			_buttonsHovered = false;
		}
		base.OnMouseLeave(e);
	}

	/// <inheritdoc/>
	protected override void OnMouseDown(MouseEventArgs e)
	{
		ViewHost.Activate();
		if(Buttons.Count != 0)
		{
			var rc = GetButtonsRect();
			if(rc.Contains(e.X, e.Y))
			{
				var x = e.X - rc.X;
				var y = e.Y - rc.Y;
				Buttons.OnMouseDown(x, y, e.Button);
			}
		}
		base.OnMouseDown(e);
	}

	/// <inheritdoc/>
	protected override void OnMouseUp(MouseEventArgs e)
	{
		if(Buttons.Count != 0)
		{
			var rc = GetButtonsRect();
			if(Buttons.PressedButton != null || rc.Contains(e.X, e.Y))
			{
				var x = e.X - rc.X;
				var y = e.Y - rc.Y;
				Buttons.OnMouseUp(x, y, e.Button);
			}
		}
		base.OnMouseUp(e);
	}
}
