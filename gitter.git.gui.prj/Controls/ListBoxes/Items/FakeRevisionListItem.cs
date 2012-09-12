namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	public enum FakeRevisionItemType
	{
		UnstagedChanges,
		StagedChanges,
	}

	public enum UnstagedRevisionItemSubtype
	{
		None,
		Conflicts,
		Modifications,
		RemovedFiles,
		UntrackedFiles,
	}

	/// <summary>Item used to represent uncommitted or unstaged changes to the working tree.</summary>
	public sealed class FakeRevisionListItem : CustomListBoxItem<Revision>, IRevisionGraphListItem
	{
		private struct FileStatusIconEntry
		{
			public Bitmap Image;
			public int Count;
		}

		private readonly Repository _repository;
		private readonly FakeRevisionItemType _type;
		private readonly FileStatusIconEntry[] _iconEntries;
		private UnstagedRevisionItemSubtype _subType;
		private GraphAtom[] _graph;

		const string NoHash = "----------------------------------------";

		/// <summary>Create <see cref="FakeRevisionListItem"/>.</summary>
		/// <param name="repository">Related repository.</param>
		/// <param name="type">Item type.</param>
		public FakeRevisionListItem(Repository repository, FakeRevisionItemType type)
			: base(null)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;
			_type = type;
			switch(type)
			{
				case FakeRevisionItemType.StagedChanges:
					lock(repository.Status.SyncRoot)
					{
						_iconEntries = new FileStatusIconEntry[]
						{
							new FileStatusIconEntry { Image = FileStatusIcons.ImgStagedAdded,
								Count = repository.Status.StagedAddedCount },
							new FileStatusIconEntry { Image = FileStatusIcons.ImgStagedRemoved,
								Count = repository.Status.StagedRemovedCount },
							new FileStatusIconEntry { Image = FileStatusIcons.ImgStagedModified,
								Count = repository.Status.StagedModifiedCount },
						};
					}
					break;
				case FakeRevisionItemType.UnstagedChanges:
					lock(repository.Status.SyncRoot)
					{
						_subType = GetSubType(repository.Status);
						_iconEntries = new FileStatusIconEntry[]
						{
							new FileStatusIconEntry { Image = FileStatusIcons.ImgUnmerged,
								Count = repository.Status.UnmergedCount },
							new FileStatusIconEntry { Image = FileStatusIcons.ImgUnstagedUntracked,
								Count = repository.Status.UnstagedUntrackedCount },
							new FileStatusIconEntry { Image = FileStatusIcons.ImgUnstagedRemoved,
								Count = repository.Status.UnstagedRemovedCount },
							new FileStatusIconEntry { Image = FileStatusIcons.ImgUnstagedModified,
								Count = repository.Status.UnstagedModifiedCount },
						};
					}
					break;
				default:
					throw new ArgumentException("type");
			}
		}

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			_repository.Status.Changed += OnStatusUpdated;
		}

		protected override void OnListBoxDetached()
		{
			base.OnListBoxDetached();
			_repository.Status.Changed -= OnStatusUpdated;
		}

		private static UnstagedRevisionItemSubtype GetSubType(Status status)
		{
			if(status.UnmergedCount != 0)
				return UnstagedRevisionItemSubtype.Conflicts;
			if(status.UnstagedModifiedCount != 0)
				return UnstagedRevisionItemSubtype.Modifications;
			if(status.UnstagedRemovedCount != 0)
				return UnstagedRevisionItemSubtype.RemovedFiles;
			if(status.UnstagedUntrackedCount != 0)
				return UnstagedRevisionItemSubtype.UntrackedFiles;
			return UnstagedRevisionItemSubtype.None;
		}

		private void OnStatusUpdated(object sender, EventArgs e)
		{
			var status = (Status)sender;
			switch(_type)
			{
				case FakeRevisionItemType.StagedChanges:
					_iconEntries[0].Count = status.StagedAddedCount;
					_iconEntries[1].Count = status.StagedRemovedCount;
					_iconEntries[2].Count = status.StagedModifiedCount;
					break;
				case FakeRevisionItemType.UnstagedChanges:
					_subType = GetSubType(status);
					_iconEntries[0].Count = status.UnmergedCount;
					_iconEntries[1].Count = status.UnstagedUntrackedCount;
					_iconEntries[2].Count = status.UnstagedRemovedCount;
					_iconEntries[3].Count = status.UnstagedModifiedCount;
					break;
			}
			InvalidateSafe();
		}

		private void DrawSubjectColumn(SubItemPaintEventArgs paintEventArgs)
		{
			bool alignToGraph;
			var rsc = paintEventArgs.Column as SubjectColumn;
			var rect = paintEventArgs.Bounds;
			if(rsc != null)
			{
				alignToGraph = rsc.AlignToGraph;
			}
			else
			{
				alignToGraph = SubjectColumn.DefaultAlignToGraph;
			}
			var graphColumn = ListBox.GetPrevVisibleColumn(paintEventArgs.ColumnIndex);
			if(graphColumn != null && graphColumn.Id != (int)ColumnId.Graph)
				graphColumn = null;
			if(alignToGraph && graphColumn != null)
			{
				int availWidth;
				if(_graph != null)
				{
					availWidth = graphColumn.Width - 21 * _graph.Length;
					for(int i = _graph.Length - 1; i != -1; --i)
					{
						if(_graph[i].Elements != GraphElement.Space)
							break;
						availWidth += 21;
					}
				}
				else
				{
					availWidth = graphColumn.Width;
				}
				if(availWidth != 0)
				{
					rect.X -= availWidth;
					rect.Width += availWidth;
				}
			}
			SubItemPaintEventArgs.PrepareContentRectangle(ref rect);
			if(rect.Width > 1)
			{
				string text;
				switch(_type)
				{
					case FakeRevisionItemType.StagedChanges:
						text = Resources.StrUncommittedLocalChanges;
						break;
					case FakeRevisionItemType.UnstagedChanges:
						switch(_subType)
						{
							case UnstagedRevisionItemSubtype.Conflicts:
								text = Resources.StrlUnmergedFilesPresent;
								break;
							case UnstagedRevisionItemSubtype.RemovedFiles:
								text = Resources.StrlUnstagedRemovedFiles;
								break;
							case UnstagedRevisionItemSubtype.UntrackedFiles:
								text = Resources.StrlUnstagedUntrackedFiles;
								break;
							case UnstagedRevisionItemSubtype.Modifications:
								text = Resources.StrUnstagedLocalChanges;
								break;
							default:
								text = Resources.StrUnstagedLocalChanges;
								break;
						}
						break;
					default:
						text = null;
						break;
				}
				paintEventArgs.PrepareTextRectangle(ref rect);
				if(text != null)
				{
					var w = GitterApplication.TextRenderer.MeasureText(
						paintEventArgs.Graphics, text, paintEventArgs.Font, int.MaxValue).Width;
					GitterApplication.TextRenderer.DrawText(
						paintEventArgs.Graphics, text, paintEventArgs.Font, SystemBrushes.GrayText, rect);
					w += 3;
					rect.X += w;
					rect.Width -= w;
				}
				for(int i = 0; i < _iconEntries.Length; ++i)
				{
					if(rect.Width <= 0) break;
					if(_iconEntries[i].Count != 0)
					{
						var image = _iconEntries[i].Image;
						var imageRect = new Rectangle(rect.X, rect.Y - 1 + (rect.Height - image.Height) / 2, image.Width, image.Height);
						rect.X += imageRect.Width + 2;
						rect.Width -= imageRect.Width + 2;
						if(rect.Width <= 0) break;
						paintEventArgs.Graphics.DrawImage(image, imageRect);
						var countText = _iconEntries[i].Count.ToString(System.Globalization.CultureInfo.CurrentCulture);
						var textW = GitterApplication.TextRenderer.MeasureText(
							paintEventArgs.Graphics, countText, paintEventArgs.Font, int.MaxValue).Width;
						GitterApplication.TextRenderer.DrawText(
							paintEventArgs.Graphics, countText, paintEventArgs.Font, SystemBrushes.GrayText, rect);
						textW += 2;
						rect.X += textW;
						rect.Width -= textW;
					}
				}
			}
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Hash:
					return HashColumn.OnMeasureSubItem(measureEventArgs, NoHash);
				case ColumnId.TreeHash:
					return TreeHashColumn.OnMeasureSubItem(measureEventArgs, NoHash);
				case ColumnId.AuthorDate:
				case ColumnId.Date:
				case ColumnId.CommitDate:
					return measureEventArgs.MeasureText(Resources.StrUncommitted.SurroundWith('<', '>'));
				case ColumnId.Author:
				case ColumnId.Committer:
					{
						var username = _repository.Configuration.TryGetParameterValue(GitConstants.UserNameParameter);
						var usermail = _repository.Configuration.TryGetParameterValue(GitConstants.UserEmailParameter);
						return UserColumn.OnMeasureSubItem(measureEventArgs,
							username == null ? "" : username, usermail == null ? "" : usermail);
					}
				case ColumnId.Email:
				case ColumnId.CommitterEmail:
				case ColumnId.AuthorEmail:
					{
						var usermail = _repository.Configuration.TryGetParameter(GitConstants.UserEmailParameter);
						return EmailColumn.OnMeasureSubItem(measureEventArgs, usermail == null ? "" : usermail.Value);
					}
				case ColumnId.Graph:
					return GraphColumn.OnMeasureSubItem(measureEventArgs, _graph);
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Subject:
					DrawSubjectColumn(paintEventArgs);
					break;
				case ColumnId.Graph:
					GraphColumn.OnPaintSubItem(paintEventArgs, _graph, _type==FakeRevisionItemType.StagedChanges?RevisionGraphItemType.Uncommitted:RevisionGraphItemType.Unstaged);
					break;
				case ColumnId.Author:
				case ColumnId.Committer:
					var user = _repository.UserIdentity;
					if(user != null)
					{
						UserColumn.OnPaintSubItem(paintEventArgs, user, SystemBrushes.GrayText);
					}
					break;
				case ColumnId.Email:
				case ColumnId.AuthorEmail:
				case ColumnId.CommitterEmail:
					var usermail = _repository.Configuration.TryGetParameterValue(GitConstants.UserEmailParameter);
					if(usermail != null)
					{
						paintEventArgs.PaintText(usermail, SystemBrushes.GrayText);
					}
					break;
				case ColumnId.Date:
				case ColumnId.CommitDate:
				case ColumnId.AuthorDate:
					paintEventArgs.PaintText(Resources.StrUncommitted.SurroundWith('<', '>'), SystemBrushes.GrayText);
					break;
				case ColumnId.Hash:
				case ColumnId.TreeHash:
					HashColumn.OnPaintSubItem(paintEventArgs, NoHash, SystemBrushes.GrayText);
					break;
			}
		}

		public Repository Repository
		{
			get { return _repository; }
		}

		public FakeRevisionItemType Type
		{
			get { return _type; }
		}

		public UnstagedRevisionItemSubtype SubType
		{
			get { return _subType; }
		}

		public GraphAtom[] Graph
		{
			get { return _graph; }
			set { _graph = value; }
		}

		/// <summary>Gets the context menu.</summary>
		/// <param name="requestEventArgs">Request parameters.</param>
		/// <returns>Context menu for specified location.</returns>
		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			ContextMenuStrip menu = null;
			switch(_type)
			{
				case FakeRevisionItemType.UnstagedChanges:
					menu = new UnstagedChangesMenu(_repository);
					break;
				case FakeRevisionItemType.StagedChanges:
					menu = new StagedChangesMenu(_repository);
					break;
			}
			if(menu != null)
			{
				Utility.MarkDropDownForAutoDispose(menu);
			}
			return menu;
		}
	}
}
