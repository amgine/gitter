namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;

	using gitter.Framework;
	using gitter.Framework.Controls;

	public class ComboFileStatusItem : CustomListBoxItem
	{
		private TreeFile _staged;
		private TreeFile _unstaged;

		private bool _cachedInfo;
		private Icon _icon;
		private Bitmap _bmpIcon;

		public ComboFileStatusItem(TreeFile staged, TreeFile unstaged)
		{
			_staged = staged;
			_unstaged = unstaged;
			UpdateCheckedState();
		}

		public TreeFile Staged
		{
			get { return _staged; }
			internal set
			{
				if(_staged != value)
				{
					if(_staged != null)
						_staged.Deleted -= OnStagedDeleted;
					_staged = value;
					if(value != null)
					{
						if(ListBox != null)
							_staged.Deleted += OnStagedDeleted;
					}
				}
			}
		}

		public TreeFile Unstaged
		{
			get { return _unstaged; }
			set
			{
				if(_unstaged != value)
				{
					if(_unstaged != null)
						_unstaged.Deleted -= OnUnstagedDeleted;
					_unstaged = value;
					if(value != null)
					{
						if(ListBox != null)
							_unstaged.Deleted += OnUnstagedDeleted;
					}
				}
			}
		}

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			if(_staged != null)
			{
				_staged.Deleted += OnStagedDeleted;
			}
			if(_unstaged != null)
			{
				_unstaged.Deleted += OnUnstagedDeleted;
			}
		}

		protected override void OnListBoxDetached()
		{
			if(_staged != null)
			{
				_staged.Deleted -= OnStagedDeleted;
			}
			if(_unstaged != null)
			{
				_unstaged.Deleted -= OnUnstagedDeleted;
			}
			base.OnListBoxDetached();
		}

		private void OnStagedDeleted(object sender, EventArgs e)
		{
			_staged = null;
			if(_unstaged == null)
			{
				RemoveSafe();
			}
			else
			{
				UpdateCheckedState();
			}
		}

		private void OnUnstagedDeleted(object sender, EventArgs e)
		{
			_unstaged = null;
			if(_staged == null)
			{
				RemoveSafe();
			}
			else
			{
				UpdateCheckedState();
			}
		}

		internal void UpdateCheckedState()
		{
			CheckedStateChanged -= OnCSChanged;
			if(_staged == null)
			{
				CheckedState = CheckedState.Unchecked;
			}
			else
			{
				if(_unstaged == null)
				{
					CheckedState = CheckedState.Checked;
				}
				else
				{
					CheckedState = CheckedState.Intermediate;
				}
			}
			CheckedStateChanged += OnCSChanged;
		}

		private void OnCSChanged(object sender, EventArgs e)
		{
			switch(CheckedState)
			{
				case CheckedState.Checked:
					if(_unstaged != null)
					{
						_unstaged.Stage();
					}
					break;
				case CheckedState.Unchecked:
					if(_staged != null)
					{
						_staged.Unstage();
					}
					break;
			}
		}

		private Icon GetIcon()
		{
			return null;
		}

		private Bitmap GetBitmapIcon()
		{
			if(_staged != null)
				return Utility.QueryIcon(_staged.FullPath);
			if(_unstaged != null)
				return Utility.QueryIcon(_unstaged.FullPath);
			return null;
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Name:
					string data;
					if(_unstaged != null)
					{
						if((Parent != null) && (Parent is TreeDirectoryListItem))
						{
							data = _unstaged.Name;
						}
						else
						{
							data = _unstaged.RelativePath;
						}
					}
					else if(_staged != null)
					{
						if((Parent != null) && (Parent is TreeDirectoryListItem))
						{
							data = _staged.Name;
						}
						else
						{
							data = _staged.RelativePath;
						}
					}
					else
					{
						data = string.Empty;
					}
					return measureEventArgs.MeasureImageAndText(null, data);
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			if(!_cachedInfo)
			{
				_icon = GetIcon();
				if(_icon == null)
				{
					_bmpIcon = GetBitmapIcon();
				}
				_cachedInfo = true;
			}
			Bitmap overlay;
			string data;
			if(_unstaged != null)
			{
				switch(_unstaged.Status)
				{
					case FileStatus.Modified:
						overlay = CachedResources.Bitmaps["ImgOverlayEdit"];
						break;
					case FileStatus.Added:
						overlay = CachedResources.Bitmaps["ImgOverlayAdd"];
						break;
					case FileStatus.Removed:
						overlay = CachedResources.Bitmaps["ImgOverlayDel"];
						break;
					case FileStatus.Unmerged:
						overlay = CachedResources.Bitmaps["ImgOverlayConflict"];
						break;
					default:
						overlay = null;
						break;
				}
				if((Parent != null) && (Parent is TreeDirectoryListItem))
				{
					data = _unstaged.Name;
				}
				else
				{
					data = _unstaged.RelativePath;
				}
			}
			else if(_staged != null)
			{
				switch(_staged.Status)
				{
					case FileStatus.Modified:
						overlay = CachedResources.Bitmaps["ImgOverlayEditStaged"];
						break;
					case FileStatus.Added:
						overlay = CachedResources.Bitmaps["ImgOverlayAddStaged"];
						break;
					case FileStatus.Removed:
						overlay = CachedResources.Bitmaps["ImgOverlayDelStaged"];
						break;
					case FileStatus.Unmerged:
						overlay = CachedResources.Bitmaps["ImgOverlayConflict"];
						break;
					default:
						overlay = null;
						break;
				}
				if((Parent != null) && (Parent is TreeDirectoryListItem))
				{
					data = _staged.Name;
				}
				else
				{
					data = _staged.RelativePath;
				}
			}
			else
			{
				data = string.Empty;
				overlay = null;
			}
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Name:
					paintEventArgs.PaintImageOverlayAndText(_bmpIcon, overlay, data);
					break;
			}
		}
	}
}
