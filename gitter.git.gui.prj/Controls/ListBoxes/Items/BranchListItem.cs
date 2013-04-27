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
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	/// <summary>A <see cref="CustomListBoxItem"/> representing <see cref="Branch"/> object.</summary>
	public class BranchListItem : ReferenceListItemBase<Branch>
	{
		#region Static

		private static readonly Bitmap ImgBranch = CachedResources.Bitmaps["ImgBranch"];

		#endregion

		#region .ctor

		/// <summary>Create <see cref="BranchListItem"/>.</summary>
		/// <param name="branch">Related <see cref="Branch"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="branch"/> == <c>null</c>.</exception>
		public BranchListItem(Branch branch)
			: base(branch)
		{
			Verify.Argument.IsNotNull(branch, "branch");
		}

		#endregion

		#region Event Handlers

		private void OnRenamed(object sender, EventArgs e)
		{
			if(EnsureSortOrderSafe())
			{
				InvalidateSubItemSafe((int)ColumnId.Name);
			}
		}

		#endregion

		#region Overrides

		protected override Image Image
		{
			get { return ImgBranch; }
		}

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			DataContext.Renamed += OnRenamed;
		}

		protected override void OnListBoxDetached()
		{
			base.OnListBoxDetached();
			DataContext.Renamed -= OnRenamed;
		}

		/// <summary>Gets the context menu.</summary>
		/// <param name="requestEventArgs">Request parameters.</param>
		/// <returns>Context menu for specified location.</returns>
		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var mnu = new BranchMenu(DataContext);
			Utility.MarkDropDownForAutoDispose(mnu);
			return mnu;
		}

		#endregion
	}
}
