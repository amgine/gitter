namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Drawing;

	using gitter.Framework.Controls;

	public class HookListItem : CustomListBoxItem<Hook>
	{
		public HookListItem(Hook hook)
			: base(hook)
		{
		}

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			DataContext.Deleted += OnDeleted;
			DataContext.Revived += OnRevived;
		}

		protected override void OnListBoxDetached()
		{
			DataContext.Deleted -= OnDeleted;
			DataContext.Revived -= OnRevived;
			base.OnListBoxDetached();
		}

		private void OnDeleted(object sender, EventArgs e)
		{
			InvalidateSafe();
		}

		private void OnRevived(object sender, EventArgs e)
		{
			InvalidateSafe();
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			throw new NotImplementedException();
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			throw new NotImplementedException();
		}
	}
}
