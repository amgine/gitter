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

namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Framework.Options;

	using Resources = gitter.Git.Gui.Properties.Resources;

	public class RemotePicker : CustomPopupComboBox
	{
		private static readonly Bitmap ImgRemote = CachedResources.Bitmaps["ImgRemote"];
		
		private RemoteListBox _lstRemotes;
		private Remote _selectedRemote;

		/// <summary>Initializes a new instance of the <see cref="RemotePicker"/> class.</summary>
		public RemotePicker()
		{
			_lstRemotes = new RemoteListBox(new NameColumn())
			{
				HeaderStyle = HeaderStyle.Hidden,
				BorderStyle = BorderStyle.FixedSingle,
				ItemActivation = gitter.Framework.Controls.ItemActivation.SingleClick,
				Size = new Size(Width, 2 + 2 + 21 * 5),
				DisableContextMenus = true,
				Style = GitterApplication.DefaultStyle,
				Font = LicenseManager.UsageMode == LicenseUsageMode.Runtime?
					GitterApplication.FontManager.UIFont.Font:
					SystemFonts.MessageBoxFont,
			};
			_lstRemotes.ItemActivated += OnItemActivated;

			DropDownStyle = ComboBoxStyle.DropDownList;
			DrawMode = DrawMode.OwnerDrawFixed;

			DropDownControl = _lstRemotes;
		}

		public Remote SelectedRemote
		{
			get { return _selectedRemote; }
			set
			{
				if(_selectedRemote != value)
				{
					_selectedRemote = value;
					if(_selectedRemote == null)
					{
						Text = Resources.StrlNone.SurroundWith('<', '>');
						ForeColor = SystemColors.GrayText;
					}
					else
					{
						Text = _selectedRemote.Name;
						ForeColor = SystemColors.WindowText;
					}
					OnSelectedItemChanged(EventArgs.Empty);
					OnSelectedIndexChanged(EventArgs.Empty);
				}
			}
		}

		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			var graphics = e.Graphics;
			var bounds = e.Bounds;
			var iconBounds = e.Bounds;
			iconBounds.Width = ImgRemote.Width;
			var d = (iconBounds.Height - ImgRemote.Height);
			iconBounds.Y += d / 2;
			iconBounds.Height = ImgRemote.Height;
			bounds.X += iconBounds.Width + 3;
			bounds.Width -= iconBounds.Width + 3;

			e.DrawBackground();
			graphics.TextRenderingHint = Utility.TextRenderingHint;
			graphics.TextContrast = Utility.TextContrast;
			if(_selectedRemote == null)
			{
				GitterApplication.TextRenderer.DrawText(
					graphics, Resources.StrlNone.SurroundWith('<', '>'), Font,
					((e.State & DrawItemState.Selected) == DrawItemState.Selected) ?
					SystemBrushes.HighlightText : SystemBrushes.GrayText, bounds);
			}
			else
			{
				graphics.DrawImage(ImgRemote, iconBounds, new Rectangle(0, 0, ImgRemote.Width, ImgRemote.Height), GraphicsUnit.Pixel);
				GitterApplication.TextRenderer.DrawText(
					graphics, _selectedRemote.Name, Font,
					((e.State & DrawItemState.Selected) == DrawItemState.Selected) ?
					SystemBrushes.HighlightText : SystemBrushes.WindowText, bounds);
			}
		}

		public RemoteListBox Remotes
		{
			get { return _lstRemotes; }
		}

		public void LoadData(Repository repository)
		{
			_lstRemotes.LoadData(repository);
		}

		private void OnItemActivated(object sender, ItemEventArgs e)
		{
			var remoteItem = e.Item as RemoteListItem;
			if(remoteItem != null)
			{
				SelectedRemote = remoteItem.DataContext;
				HideDropDown();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_lstRemotes != null)
				{
					_lstRemotes.LoadData(null);
					_lstRemotes.ItemActivated -= OnItemActivated;
					_lstRemotes.Dispose();
					_lstRemotes = null;
				}
				_selectedRemote = null;
			}
			base.Dispose(disposing);
		}
	}
}
