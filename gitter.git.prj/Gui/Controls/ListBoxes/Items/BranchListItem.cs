namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>A <see cref="CustomListBoxItem"/> representing <see cref="Branch"/> object.</summary>
	public sealed class BranchListItem : ReferenceListItemBase<Branch>
	{
		private static readonly Bitmap ImgBranch = CachedResources.Bitmaps["ImgBranch"];

		#region .ctor

		/// <summary>Create <see cref="BranchListItem"/>.</summary>
		/// <param name="branch">Related <see cref="Branch"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="branch"/> == <c>null</c>.</exception>
		public BranchListItem(Branch branch)
			: base(branch)
		{
			if(branch == null) throw new ArgumentNullException("branch");
		}

		#endregion

		protected override Image Image
		{
			get { return ImgBranch; }
		}

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			Data.Renamed += OnRenamed;
		}

		protected override void OnListBoxDetached()
		{
			base.OnListBoxDetached();
			Data.Renamed -= OnRenamed;
		}

		private void OnRenamed(object sender, EventArgs e)
		{
			if(EnsureSortOrderSafe())
			{
				InvalidateSubItemSafe((int)ColumnId.Name);
			}
		}

		/// <summary>Gets the context menu.</summary>
		/// <param name="requestEventArgs">Request parameters.</param>
		/// <returns>Context menu for specified location.</returns>
		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var mnu = new BranchMenu(Data);
			Utility.MarkDropDownForAutoDispose(mnu);
			return mnu;
		}
	}
}
