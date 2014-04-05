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

	public class RevisionPicker : CustomPopupComboBox
	{
		private ReferencesListBox _lstReferences;
		private RevisionToolTip _revisionToolTip;

		public RevisionPicker()
		{
			_lstReferences = new ReferencesListBox()
			{
				HeaderStyle = HeaderStyle.Hidden,
				BorderStyle = BorderStyle.FixedSingle,
				ItemActivation = gitter.Framework.Controls.ItemActivation.SingleClick,
				Size = new Size(Width, 2 + 2 + 21 * 10),
				DisableContextMenus = true,
				Style = GitterApplication.DefaultStyle,
				Font = LicenseManager.UsageMode == LicenseUsageMode.Runtime ?
					GitterApplication.FontManager.UIFont.Font :
					SystemFonts.MessageBoxFont,
			};
			_lstReferences.ItemActivated += OnItemActivated;
			_revisionToolTip = new RevisionToolTip();

			DropDownControl = _lstReferences;
		}

		public ReferencesListBox References
		{
			get { return _lstReferences; }
		}

		private void OnItemActivated(object sender, ItemEventArgs e)
		{
			if(e.Item is BranchListItem)
			{
				var branch = ((BranchListItem)e.Item).DataContext;
				Text = branch.Name;
				HideDropDown();
			}
			if(e.Item is RemoteBranchListItem)
			{
				var branch = ((RemoteBranchListItem)e.Item).DataContext;
				Text = branch.Name;
				HideDropDown();
			}
			else if(e.Item is TagListItem)
			{
				var tag = ((TagListItem)e.Item).DataContext;
				Text = tag.Name;
				HideDropDown();
			}
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			var repository = References.Repository;
			if(repository != null)
			{
				try
				{
					var p = repository.GetRevisionPointer(Text.Trim());
					var revision = p.Dereference();
					if(revision != null && !revision.IsLoaded)
					{
						revision.Load();
					}
					_revisionToolTip.Revision = revision;
					if(revision != null)
					{
						_revisionToolTip.Show(this, new Point(0, Height + 1));
					}
				}
				catch(Exception exc)
				{
					if(exc.IsCritical())
					{
						throw;
					}
					_revisionToolTip.Revision = null;
				}
			}
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			_revisionToolTip.Hide(this);
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_lstReferences != null)
				{
					_lstReferences.LoadData(null);
					_lstReferences.ItemActivated -= OnItemActivated;
					_lstReferences.Dispose();
					_lstReferences = null;
				}
				if(_revisionToolTip != null)
				{
					_revisionToolTip.Dispose();
					_revisionToolTip = null;
				}
			}
			base.Dispose(disposing);
		}

		public override int DropDownHeight
		{
			get { return _lstReferences.Height; }
			set { _lstReferences.Height = value; }
		}
	}
}
