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

namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;

	using Resources = gitter.Framework.Properties.Resources;

	public sealed class ViewButton
	{
		private static readonly Bitmap ImgMenu = Resources.ImgViewHostMenu;
		private static readonly Bitmap ImgNormalize = Resources.ImgNormalize;
		private static readonly Bitmap ImgMaximize = Resources.ImgMaximize;
		private static readonly Bitmap ImgPin = Resources.ImgViewHostPin;
		private static readonly Bitmap ImgClose = Resources.ImgViewHostClose;
		private static readonly Bitmap ImgScrollLeft = Resources.ImgTabScrollLeft;
		private static readonly Bitmap ImgScrollRight = Resources.ImgTabScrollRight;
		private static readonly Bitmap ImgTabMenu = Resources.ImgTabMenu;
		private static readonly Bitmap ImgTabMenuExt = Resources.ImgTabMenuExtends;

		internal ViewButton(int offset, ViewButtonType type)
		{
			Offset = offset;
			Type = type;
			switch(Type)
			{
				case ViewButtonType.Menu:
					Image = ImgMenu;
					break;
				case ViewButtonType.Pin:
					Image = ImgPin;
					break;
				case ViewButtonType.Unpin:
					Image = ImgPin;
					break;
				case ViewButtonType.Normalize:
					Image = ImgNormalize;
					break;
				case ViewButtonType.Maximize:
					Image = ImgMaximize;
					break;
				case ViewButtonType.Close:
					Image = ImgClose;
					break;
				case ViewButtonType.ScrollTabsLeft:
					Image = ImgScrollLeft;
					break;
				case ViewButtonType.ScrollTabsRight:
					Image = ImgScrollRight;
					break;
				case ViewButtonType.TabsMenu:
					Image = ImgTabMenu;
					break;
				case ViewButtonType.TabsScrollMenu:
					Image = ImgTabMenuExt;
					break;
				default:
					Image = null;
					break;
			}
		}

		public int Offset { get; }

		public ViewButtonType Type { get; }

		public Image Image { get; }

		internal void OnPaint(Graphics graphics, Rectangle bounds, bool focus, bool hover, bool pressed)
		{
			ViewManager.Renderer.RenderViewButton(this, graphics, bounds, focus, hover, pressed);
		}

		public override string ToString() => Type.ToString();
	}
}
