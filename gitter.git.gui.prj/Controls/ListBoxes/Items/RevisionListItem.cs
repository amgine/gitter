namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	/// <summary><see cref="CustomListBoxItem"/>, representing <see cref="Revision"/>.</summary>
	public class RevisionListItem : RevisionPointerListItemBase<Revision>, IRevisionGraphListItem
	{
		private readonly List<Tuple<Rectangle, IRevisionPointer>> _drawnPointers;
		private GraphAtom[] _graph;

		private const int PointerTagHitOffset = 1;

		/// <summary>Create <see cref="RevisionListItem"/>.</summary>
		/// <param name="revision">Associated revision.</param>
		public RevisionListItem(Revision revision)
			: base(revision)
		{
			_drawnPointers = new List<Tuple<Rectangle, IRevisionPointer>>();
		}

		protected override void OnListBoxAttached()
		{
			DataContext.References.Changed += OnReferenceListChanged;
			base.OnListBoxAttached();
		}

		protected override void OnListBoxDetached()
		{
			DataContext.References.Changed -= OnReferenceListChanged;
			_drawnPointers.Clear();
			base.OnListBoxDetached();
		}

		private void OnReferenceListChanged(object sender, EventArgs e)
		{
			InvalidateSafe();
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			if(requestEventArgs.Column != null)
			{
				switch((ColumnId)requestEventArgs.SubItemId)
				{
					case ColumnId.Graph:
						var ncid = ListBox.GetNextVisibleColumnIndex(requestEventArgs.ColumnIndex);
						if(ncid != -1)
						{
							var rsc = ListBox.Columns[ncid] as SubjectColumn;
							if(rsc != null && rsc.AlignToGraph)
							{
								return GetContextMenu(new ItemContextMenuRequestEventArgs(
									requestEventArgs.Item, rsc, ncid,
									requestEventArgs.X, requestEventArgs.Y));
							}
						}
						break;
					case ColumnId.Name:
					case ColumnId.Subject:
						foreach(var ptr in _drawnPointers)
						{
							if(requestEventArgs.X >= ptr.Item1.X && requestEventArgs.X < ptr.Item1.Right)
							{
								var tag = ptr.Item2 as Tag;
								if(tag != null)
								{
									var tagMenu = new TagMenu(tag);
									Utility.MarkDropDownForAutoDispose(tagMenu);
									return tagMenu;
								}
								var branch = ptr.Item2 as BranchBase;
								if(branch != null)
								{
									var branchMenu = new BranchMenu(branch);
									Utility.MarkDropDownForAutoDispose(branchMenu);
									return branchMenu;
								}
								var stash = ptr.Item2 as StashedState;
								if(stash != null)
								{
									var stashMenu = new StashedStateMenu(stash);
									Utility.MarkDropDownForAutoDispose(stashMenu);
									return stashMenu;
								}
								break;
							}
						}
						break;
				}
			}
			var mnu = new RevisionMenu(DataContext);
			Utility.MarkDropDownForAutoDispose(mnu);
			return mnu;
		}

		private Bitmap PrepareDragImage(Branch branch)
		{
			int w = 0;
			using(var bmp = new Bitmap(1, 1))
			{
				using(var gx = Graphics.FromImage(bmp))
				{
					w = GlobalBehavior.GraphStyle.MeasureBranch(
						gx, ListBox.Font, GitterApplication.TextRenderer.LeftAlign, branch);
				}
			}
			if(w != 0)
			{
				var bmp = new Bitmap(w, 21, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
				using(var gx = Graphics.FromImage(bmp))
				{
					gx.Clear(Color.Transparent);
					gx.TextRenderingHint = Utility.TextRenderingHint;
					gx.TextContrast = Utility.TextContrast;
					GlobalBehavior.GraphStyle.DrawBranch(
						gx,
						ListBox.Font,
						GitterApplication.TextRenderer.LeftAlign,
						0, 0, w, 21,
						true, branch);
				}
				return bmp;
			}
			else
			{
				return null;
			}
		}

		private void DrawBranchDragImage(Branch branch, Graphics gx)
		{
			int w = GlobalBehavior.GraphStyle.MeasureBranch(
				gx, ListBox.Font, GitterApplication.TextRenderer.LeftAlign, branch);
			if(w != 0)
			{
				gx.TextRenderingHint = Utility.TextRenderingHint;
				gx.TextContrast = Utility.TextContrast;
				GlobalBehavior.GraphStyle.DrawBranch(
					gx,
					ListBox.Font,
					GitterApplication.TextRenderer.LeftAlign,
					0, 0, w, 21,
					true, branch);
			}
		}

		public override void OnMouseDown(MouseButtons button, int x, int y)
		{
			if(button == MouseButtons.Left)
			{
				for(int i = 0; i < _drawnPointers.Count; ++i)
				{
					var rc = _drawnPointers[i].Item1;
					if(rc.X <= x && rc.Right > x)
					{
						var branch = _drawnPointers[i].Item2 as Branch;
						if(branch != null && !branch.IsRemote)
						{
							int dx = _drawnPointers[i].Item1.X - x - 1;
							int w = 0;
							using(var bmp = new Bitmap(1, 1))
							using(var gx = Graphics.FromImage(bmp))
							{
								w = GlobalBehavior.GraphStyle.MeasureBranch(
									gx, ListBox.Font, GitterApplication.TextRenderer.LeftAlign, branch);
							}
							using(var dragImage = new DragImage(
								new Size(w, 21), -dx, y,
								eargs => DrawBranchDragImage(branch, eargs.Graphics)))
							{
								dragImage.Show();
								ListBox.DoDragDrop(branch, DragDropEffects.None | DragDropEffects.Scroll | DragDropEffects.Move);
							}
							return;
						}
						else
						{
							var tag = _drawnPointers[i].Item2 as Tag;
							if(tag != null && tag.TagType == TagType.Annotated)
							{
								//var message = tag.Message;
								//if(!string.IsNullOrEmpty(message))
								//{
								//    var tt = new ToolTip()
								//    {
								//        AutomaticDelay = 0,
								//        AutoPopDelay = 0,
								//        IsBalloon = false,
								//        UseAnimation = false,
								//        UseFading = false,
								//        InitialDelay = 0,
								//        StripAmpersands = false,
								//    };
								//    tt.Show(message, ListBox);
								//}
							}
						}
					}
				}
			}
			base.OnMouseDown(button, x, y);
		}

		protected override int OnHitTest(int x, int y)
		{
			for(int i = 0; i < _drawnPointers.Count; ++i)
			{
				var rc = _drawnPointers[i].Item1;
				if(rc.X <= x && rc.Right > x)
				{
					return PointerTagHitOffset + i;
				}
			}
			return base.OnHitTest(x, y);
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Hash:
					return HashColumn.OnMeasureSubItem(measureEventArgs, DataContext.Hash);
				case ColumnId.TreeHash:
					return TreeHashColumn.OnMeasureSubItem(measureEventArgs, DataContext.TreeHash);
				case ColumnId.Graph:
					return GraphColumn.OnMeasureSubItem(measureEventArgs, _graph);
				case ColumnId.Name:
				case ColumnId.Subject:
					return SubjectColumn.OnMeasureSubItem(measureEventArgs, DataContext, _graph);
				case ColumnId.Date:
				case ColumnId.CommitDate:
					return CommitDateColumn.OnMeasureSubItem(measureEventArgs, DataContext.CommitDate);
				case ColumnId.Committer:
					return CommitterColumn.OnMeasureSubItem(measureEventArgs, DataContext.Committer);
				case ColumnId.CommitterEmail:
					return CommitterEmailColumn.OnMeasureSubItem(measureEventArgs, DataContext.Committer.Email);
				case ColumnId.AuthorDate:
					return AuthorDateColumn.OnMeasureSubItem(measureEventArgs, DataContext.AuthorDate);
				case ColumnId.User:
				case ColumnId.Author:
					return AuthorColumn.OnMeasureSubItem(measureEventArgs, DataContext.Author);
				case ColumnId.AuthorEmail:
					return AuthorEmailColumn.OnMeasureSubItem(measureEventArgs, DataContext.Author.Email);
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Hash:
					HashColumn.OnPaintSubItem(paintEventArgs, DataContext.Hash);
					break;
				case ColumnId.TreeHash:
					TreeHashColumn.OnPaintSubItem(paintEventArgs, DataContext.TreeHash);
					break;
				case ColumnId.Graph:
					GraphColumn.OnPaintSubItem(paintEventArgs, _graph, DataContext.IsCurrent?RevisionGraphItemType.Current:RevisionGraphItemType.Generic);
					break;
				case ColumnId.Name:
				case ColumnId.Subject:
					SubjectColumn.OnPaintSubItem(paintEventArgs, DataContext, _graph, _drawnPointers, paintEventArgs.HoveredPart - PointerTagHitOffset);
					break;
				case ColumnId.Date:
				case ColumnId.CommitDate:
					CommitDateColumn.OnPaintSubItem(paintEventArgs, DataContext.CommitDate);
					break;
				case ColumnId.Committer:
					CommitterColumn.OnPaintSubItem(paintEventArgs, DataContext.Committer);
					break;
				case ColumnId.CommitterEmail:
					CommitterEmailColumn.OnPaintSubItem(paintEventArgs, DataContext.Committer.Email);
					break;
				case ColumnId.AuthorDate:
					AuthorDateColumn.OnPaintSubItem(paintEventArgs, DataContext.AuthorDate);
					break;
				case ColumnId.User:
				case ColumnId.Author:
					AuthorColumn.OnPaintSubItem(paintEventArgs, DataContext.Author);
					break;
				case ColumnId.AuthorEmail:
					AuthorEmailColumn.OnPaintSubItem(paintEventArgs, DataContext.Author.Email);
					break;
			}
		}

		public GraphAtom[] Graph
		{
			get { return _graph; }
			set { _graph = value; }
		}
	}
}
