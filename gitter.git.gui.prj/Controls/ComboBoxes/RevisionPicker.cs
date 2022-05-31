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

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;

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

	public ReferencesListBox References => _lstReferences;

	private void OnItemActivated(object sender, ItemEventArgs e)
	{
		switch(e.Item)
		{
			case BranchListItem branch:
				Text = branch.DataContext.Name;
				HideDropDown();
				break;
			case RemoteBranchListItem remoteBranch:
				Text = remoteBranch.DataContext.Name;
				HideDropDown();
				break;
			case TagListItem tag:
				Text = tag.DataContext.Name;
				HideDropDown();
				break;
		}
	}

	/// <inheritdoc/>
	protected override void OnMouseEnter(EventArgs e)
	{
		base.OnMouseEnter(e);
		var repository = References.Repository;
		if(repository is not null)
		{
			try
			{
				var p = repository.GetRevisionPointer(Text.Trim());
				var revision = p.Dereference();
				if(revision is { IsLoaded: false })
				{
					revision.Load();
				}
				_revisionToolTip.Revision = revision;
				if(revision is not null)
				{
					_revisionToolTip.Show(this, new Point(0, Height + 1));
				}
			}
			catch(Exception exc) when(!exc.IsCritical())
			{
				_revisionToolTip.Revision = null;
			}
		}
	}

	/// <inheritdoc/>
	protected override void OnMouseLeave(EventArgs e)
	{
		base.OnMouseLeave(e);
		_revisionToolTip.Hide(this);
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			if(_lstReferences is not null)
			{
				_lstReferences.LoadData(null);
				_lstReferences.ItemActivated -= OnItemActivated;
				_lstReferences.Dispose();
				_lstReferences = null;
			}
			if(_revisionToolTip is not null)
			{
				_revisionToolTip.Dispose();
				_revisionToolTip = null;
			}
		}
		base.Dispose(disposing);
	}

	/// <inheritdoc/>
	public override int DropDownHeight
	{
		get => _lstReferences.Height;
		set => _lstReferences.Height = value;
	}
}
