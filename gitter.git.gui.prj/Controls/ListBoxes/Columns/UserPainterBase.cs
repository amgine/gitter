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

using System.Diagnostics.CodeAnalysis;
using System.Drawing;

using gitter.Framework;
using gitter.Framework.Controls;

abstract class UserPainterBase<T> : ISubItemPainter
{
	private const string EmailFormat = "{0} <{1}>";

	protected abstract User? GetUser(T item);

	private static async void ReloadAvatar(IAvatar avatar, CustomListBoxItem item, CustomListBoxColumn column)
	{
		await avatar.UpdateAsync();

		if(item.ListBox is null) return;
		item.InvalidateSubItem(column.Id);
	}

	protected virtual bool TryGetTextBrush(SubItemPaintEventArgs paintEventArgs,
		[MaybeNullWhen(returnValue: false)] out Brush textBrush,
		[MaybeNullWhen(returnValue: false)] out bool disposeBrush)
	{
		textBrush    = default;
		disposeBrush = false;
		return false;
	}

	public bool TryMeasure(SubItemMeasureEventArgs measureEventArgs, out Size size)
	{
		Verify.Argument.IsNotNull(measureEventArgs);

		if( measureEventArgs.Column is not UserColumn column ||
			measureEventArgs.Item   is not T item)
		{
			size = Size.Empty;
			return false;
		}

		var user = GetUser(item);
		if(user is null)
		{
			size = Size.Empty;
			return false;
		}

		var showEmail  = column.ShowEmail;
		var showAvatar = column.ShowAvatar;
		var text = showEmail && !string.IsNullOrEmpty(user.Email)
			? string.Format(EmailFormat, user.Name, user.Email)
			: user.Name;

		if(string.IsNullOrEmpty(text))
		{
			size = Size.Empty;
			return true;
		}

		size = showAvatar
			? measureEventArgs.MeasureImageAndText(default(Image), text)
			: measureEventArgs.MeasureText(text);
		return true;
	}

	public bool TryPaint(SubItemPaintEventArgs paintEventArgs)
	{
		Verify.Argument.IsNotNull(paintEventArgs);

		if(paintEventArgs.Column is not UserColumn column) return false;
		if(paintEventArgs.Item   is not T          item)   return false;

		var user = GetUser(item);
		if(user is null) return false;

		var showEmail  = column.ShowEmail;
		var showAvatar = column.ShowAvatar;
		var text = showEmail && !string.IsNullOrEmpty(user.Email)
			? string.Format(EmailFormat, user.Name, user.Email)
			: user.Name;

		if(string.IsNullOrEmpty(text)) return true;

		if(!TryGetTextBrush(paintEventArgs, out var brush, out var disposeBrush))
		{
			brush = paintEventArgs.Brush;
		}
		try
		{
			if(showAvatar)
			{
				var avatar = user.Avatar;
				var image = avatar?.Image;
				if(image is null && avatar is not null)
				{
					ReloadAvatar(avatar, paintEventArgs.Item, paintEventArgs.Column);
				}
				paintEventArgs.PaintImageAndText(image, ImagePainter.Circle, text, brush);
			}
			else
			{
				paintEventArgs.PaintText(text, brush);
			}
		}
		finally
		{
			if(disposeBrush) brush.Dispose();
		}
		return true;
	}
}
