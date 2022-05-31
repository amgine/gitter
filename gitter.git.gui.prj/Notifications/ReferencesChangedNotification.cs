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

namespace gitter.Git.Gui;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

[DesignerCategory("")]
sealed class ReferencesChangedNotification : NotificationContent
{
	private readonly ReferenceChange[] _changes;

	private const int HorizontalMargin = 2;
	private const int VerticalMargin = 2;
	private const int ItemHeight = 18;
	private const int MaxItems = 10;

	public ReferencesChangedNotification(ReferenceChange[] changes)
	{
		_changes = changes;
		Height = Measure(changes, Dpi.FromControl(this));
	}

	private static int Measure(ReferenceChange[] changes, Dpi dpi)
	{
		var conv = DpiConverter.FromDefaultTo(dpi);
		if(changes is not { Length: > 0 })
		{
			return conv.ConvertY(VerticalMargin) * 2 + conv.ConvertY(ItemHeight);
		}
		else
		{
			int count = changes.Length;
			if(count > MaxItems)
			{
				count = MaxItems;
			}
			return conv.ConvertY(VerticalMargin) * 2 + count * conv.ConvertY(ItemHeight);
		}
	}

	private static Image GetIcon(ReferenceType referenceType, int size)
	{
		var icon = referenceType switch
		{
			ReferenceType.RemoteBranch => Icons.RemoteBranch,
			ReferenceType.LocalBranch  => Icons.Branch,
			ReferenceType.Tag          => Icons.Tag,
			_ => null,
		};
		return icon?.GetImage(size);
	}

	private static string GetTextPrefix(ReferenceChangeType change)
		=> change switch
		{
			ReferenceChangeType.Added   => Resources.StrAdded,
			ReferenceChangeType.Moved   => Resources.StrUpdated,
			ReferenceChangeType.Removed => Resources.StrRemoved,
			_ => string.Empty,
		};

	/// <inheritdoc/>
	protected override void OnPaint(PaintEventArgs e)
	{
		Assert.IsNotNull(e);

		e.Graphics.GdiFill(BackColor, e.ClipRectangle);
		var conv = new DpiConverter(this);
		int x = conv.ConvertX(HorizontalMargin);
		int y = conv.ConvertY(VerticalMargin);
		using var brush = new SolidBrush(ForeColor);
		if(_changes is not { Length: > 0 })
		{
			GitterApplication.TextRenderer.DrawText(
				e.Graphics, Resources.StrsEverythingIsUpToDate, Font, brush, new Point(x, y + 2));
		}
		else
		{
			var itemHeight = conv.ConvertY(ItemHeight);
			var v = conv.ConvertX(54);
			var spacing = conv.ConvertX(4);
			for(int i = 0; i < _changes.Length; ++i)
			{
				if(i == MaxItems - 1 && _changes.Length > MaxItems)
				{
					GitterApplication.TextRenderer.DrawText(
						e.Graphics, Resources.StrfNMoreChangesAreNotShown.UseAsFormat(_changes.Length - MaxItems + 1),
						Font, brush, new Point(x, y + 2));
					break;
				}
				var prefix = GetTextPrefix(_changes[i].ChangeType);
				if(!string.IsNullOrWhiteSpace(prefix))
				{
					GitterApplication.TextRenderer.DrawText(
						e.Graphics, prefix, Font, brush, new Point(x, y + 2));
				}
				var icon = GetIcon(_changes[i].ReferenceType, conv.ConvertX(16));
				if(icon is not null)
				{
					e.Graphics.DrawImage(icon, new Rectangle(x + v, y + (itemHeight - icon.Height) / 2, icon.Width, icon.Height));
				}
				GitterApplication.TextRenderer.DrawText(
					e.Graphics, _changes[i].Name, Font, brush, new Point(x + v + (icon is not null ? icon.Width : 0) + spacing, y + 2));
				y += itemHeight;
			}
		}
	}
}
