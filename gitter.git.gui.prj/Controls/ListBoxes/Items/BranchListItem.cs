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
