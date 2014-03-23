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

namespace gitter.Git.Gui
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

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
			Height = Measure(changes);
		}

		private static int Measure(ReferenceChange[] changes)
		{
			if(changes == null || changes.Length == 0)
			{
				return VerticalMargin * 2 + ItemHeight;
			}
			else
			{
				int count = changes.Length;
				if(count > MaxItems)
				{
					count = MaxItems;
				}
				return VerticalMargin * 2 + count * ItemHeight;
			}
		}

		private static Bitmap GetIcon(ReferenceType referenceType)
		{
			switch(referenceType)
			{
				case ReferenceType.RemoteBranch:
					return CachedResources.Bitmaps["ImgBranchRemote"];
				case ReferenceType.LocalBranch:
					return CachedResources.Bitmaps["ImgBranch"];
				case ReferenceType.Tag:
					return CachedResources.Bitmaps["ImgTag"];
				default:
					return null;
			}
		}

		private static string GetTextPrefix(ReferenceChangeType change)
		{
			switch(change)
			{
				case ReferenceChangeType.Added:
					return Resources.StrAdded;
				case ReferenceChangeType.Moved:
					return Resources.StrUpdated;
				case ReferenceChangeType.Removed:
					return Resources.StrRemoved;
				default:
					return string.Empty;
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			using(var brush = new SolidBrush(BackColor))
			{
				e.Graphics.FillRectangle(brush, e.ClipRectangle);
			}
			int x = HorizontalMargin;
			int y = VerticalMargin;
			if(_changes == null || _changes.Length == 0)
			{
				using(var brush = new SolidBrush(ForeColor))
				{
					GitterApplication.TextRenderer.DrawText(
						e.Graphics, Resources.StrsEverythingIsUpToDate, Font, brush, new Point(x, y + 2));
				}
			}
			else
			{
				using(var brush = new SolidBrush(ForeColor))
				{
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
						var icon = GetIcon(_changes[i].ReferenceType);
						if(icon != null)
						{
							e.Graphics.DrawImage(icon, new Rectangle(x + 54, y + (ItemHeight - icon.Height) / 2, icon.Width, icon.Height));
						}
						GitterApplication.TextRenderer.DrawText(
							e.Graphics, _changes[i].Name, Font, brush, new Point(x + 54 + (icon != null ? icon.Width : 0) + 4, y + 2));
						y += ItemHeight;
					}
				}
			}
		}
	}
}
