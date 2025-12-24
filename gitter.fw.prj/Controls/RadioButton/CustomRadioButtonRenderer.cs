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

using System;
using System.Drawing;

public abstract class CustomRadioButtonRenderer
{
	private static CustomRadioButtonRenderer? _msvs2012Dark;

	public static CustomRadioButtonRenderer MSVS2012Dark
		=> _msvs2012Dark ??= new MSVS2012RadioButtonRenderer(MSVS2012RadioButtonRenderer.DarkColors);

	public static CustomRadioButtonRenderer Default => MSVS2012Dark;

	public virtual Rectangle Measure(CustomRadioButton radioButton) => radioButton.ClientRectangle;

	public abstract void Render(Graphics graphics, Dpi dpi, Rectangle clipRectangle, CustomRadioButton radioButton);
}
