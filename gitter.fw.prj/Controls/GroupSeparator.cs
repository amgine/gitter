#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

[DefaultProperty(nameof(Text))]
[DesignerCategory("")]
public partial class GroupSeparator : Control
{
	private readonly Label _lblText;
	private readonly Panel _line;

	/// <summary>Create <see cref="GroupSeparator"/>.</summary>
	public GroupSeparator()
	{
		SuspendLayout();
		_line = new()
		{
			BackColor = SystemColors.ControlDark,
			Name      = nameof(_line),
			Size      = new(340, 1),
			Parent    = this,
		};
		_lblText = new()
		{
			Name      = nameof(_lblText),
			AutoSize  = false,
			Margin    = Padding.Empty,
			Padding   = Padding.Empty,
			Dock      = DockStyle.Fill,
			TextAlign = ContentAlignment.MiddleLeft,
			Parent    = this,
		};
		Name = nameof(GroupSeparator);
		Size = new(407, 19);
		ResumeLayout(false);
		PerformLayout();

		SetStyle(ControlStyles.Selectable, false);
	}

	/// <inheritdoc/>
	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[DefaultValue("")]
	[Localizable(true)]
	public override string Text
	{
		get => base.Text;
		set
		{
			base.Text = value;
			_lblText.Text = value;
			UpdateLineBounds();
		}
	}

	private void UpdateLineBounds()
	{
		var dpi  = Dpi.FromControl(this);
		var conv = DpiConverter.FromDefaultTo(dpi);

		var x = _lblText.PreferredWidth + conv.ConvertX(4);
		var w = Width - x;
		var h = conv.ConvertY(1);
		_line.SetBounds(x, (Height - h) / 2 + dpi.Y * 2 / 96 - 1, w, h);
	}

	/// <inheritdoc/>
	protected override void OnDpiChangedAfterParent(EventArgs e)
	{
		base.OnDpiChangedAfterParent(e);
		UpdateLineBounds();
	}

	/// <inheritdoc/>
	protected override void OnSizeChanged(EventArgs e)
	{
		base.OnSizeChanged(e);
		UpdateLineBounds();
	}
}
