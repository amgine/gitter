namespace gitter
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Properties.Resources;

	internal sealed class LocalRepositoriesListBox : CustomListBox
	{
		private readonly DragHelper _dragHelper;

		public LocalRepositoriesListBox()
		{
			Columns.Add(new CustomListBoxColumn(1, Resources.StrName, true)
			{
				SizeMode = ColumnSizeMode.Fill
			});

			HeaderStyle = HeaderStyle.Hidden;
			ItemHeight = 36;
			AllowDrop = true;
			_dragHelper = new DragHelper();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if(e.Button == MouseButtons.Left && SelectedItems.Count != 0)
			{
				_dragHelper.Start(e.X, e.Y);
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			if(_dragHelper.IsTracking)
			{
				_dragHelper.Stop();
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if(_dragHelper.IsTracking && _dragHelper.Update(e.X, e.Y))
			{
				if(SelectedItems.Count != 1) return;
				var item = SelectedItems[0];
				using(var dragImage = RepositoryDragImage.Create(
					((RepositoryListItem)item).DataContext.Path))
				{
					dragImage.Show();
					DoDragDrop(item, DragDropEffects.Move);
				}
				_dragHelper.Stop();
			}
		}
	}
}
