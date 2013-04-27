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
	using System.Globalization;
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
	public class FakeRevisionListItem : CustomListBoxItem<Revision>, IRevisionGraphListItem
	{
		#region Helpers

		private struct FileStatusIconEntry
		{
			public Bitmap Image;
			public int Count;
		}

		#endregion

		#region Constants

		private const string NoHash = "---------------------------------------";

		#endregion

		#region Data

		private readonly Repository _repository;
		private readonly FakeRevisionItemType _type;
		private readonly FileStatusIconEntry[] _iconEntries;
		private UnstagedRevisionItemSubtype _subType;
		private GraphAtom[] _graph;

		#endregion

		#region .ctor

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
					throw new ArgumentException("Unknown type value.", "type");
			}
		}

		#endregion

		#region Properties

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

		private string SubjectText
		{
			get
			{
				switch(Type)
				{
					case FakeRevisionItemType.StagedChanges:
						return Resources.StrUncommittedLocalChanges;
					case FakeRevisionItemType.UnstagedChanges:
						switch(SubType)
						{
							case UnstagedRevisionItemSubtype.Conflicts:
								return Resources.StrlUnmergedFilesPresent;
							case UnstagedRevisionItemSubtype.RemovedFiles:
								return Resources.StrlUnstagedRemovedFiles;
							case UnstagedRevisionItemSubtype.UntrackedFiles:
								return Resources.StrlUnstagedUntrackedFiles;
							case UnstagedRevisionItemSubtype.Modifications:
								return Resources.StrUnstagedLocalChanges;
							default:
								return Resources.StrUnstagedLocalChanges;
						}
					default:
						return null;
				}
			}
		}

		#endregion

		#region Methods

		private static UnstagedRevisionItemSubtype GetSubType(Status status)
		{
			if(status.UnmergedCount != 0)
			{
				return UnstagedRevisionItemSubtype.Conflicts;
			}
			if(status.UnstagedModifiedCount != 0)
			{
				return UnstagedRevisionItemSubtype.Modifications;
			}
			if(status.UnstagedRemovedCount != 0)
			{
				return UnstagedRevisionItemSubtype.RemovedFiles;
			}
			if(status.UnstagedUntrackedCount != 0)
			{
				return UnstagedRevisionItemSubtype.UntrackedFiles;
			}
			return UnstagedRevisionItemSubtype.None;
		}

		private void OnStatusUpdated(object sender, EventArgs e)
		{
			var status = (Status)sender;
			switch(Type)
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
			{
				graphColumn = null;
			}
			if(alignToGraph && graphColumn != null)
			{
				int availWidth;
				if(_graph != null)
				{
					availWidth = graphColumn.Width - 21 * _graph.Length;
					for(int i = _graph.Length - 1; i != -1; --i)
					{
						if(_graph[i].Elements != GraphElement.Space)
						{
							break;
						}
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
				string text = SubjectText;
				paintEventArgs.PrepareTextRectangle(ref rect);
				bool useDefaultBrush = (paintEventArgs.State & ItemState.Selected) == ItemState.Selected && paintEventArgs.ListBox.Style.Type == GitterStyleType.DarkBackground;
				var textBrush = useDefaultBrush ? paintEventArgs.Brush : new SolidBrush(paintEventArgs.ListBox.Style.Colors.GrayText);
				if(!string.IsNullOrWhiteSpace(text))
				{
					var w = GitterApplication.TextRenderer.MeasureText(
						paintEventArgs.Graphics, text, paintEventArgs.Font, int.MaxValue).Width;
						GitterApplication.TextRenderer.DrawText(
							paintEventArgs.Graphics, text, paintEventArgs.Font, textBrush, rect);
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
						var countText = _iconEntries[i].Count.ToString(CultureInfo.CurrentCulture);
						var textW = GitterApplication.TextRenderer.MeasureText(
							paintEventArgs.Graphics, countText, paintEventArgs.Font, int.MaxValue).Width;
						GitterApplication.TextRenderer.DrawText(
							paintEventArgs.Graphics, countText, paintEventArgs.Font, textBrush, rect);
						textW += 2;
						rect.X += textW;
						rect.Width -= textW;
					}
				}
				if(!useDefaultBrush)
				{
					textBrush.Dispose();
				}
			}
		}

		private static void PaintGrayText<T>(SubItemPaintEventArgs paintEventArgs, T value, Action<SubItemPaintEventArgs, T, Brush> paintMethod)
		{
			Assert.IsNotNull(paintEventArgs);
			Assert.IsNotNull(paintMethod);

			var style = paintEventArgs.ListBox.Style;
			if((paintEventArgs.State & ItemState.Selected) == ItemState.Selected && style.Type == GitterStyleType.DarkBackground)
			{
				paintMethod(paintEventArgs, value, paintEventArgs.Brush);
			}
			else
			{
				using(var textBrush = new SolidBrush(style.Colors.GrayText))
				{
					paintMethod(paintEventArgs, value, textBrush);
				}
			}
		}

		private static void PaintGrayText(SubItemPaintEventArgs paintEventArgs, string text)
		{
			Assert.IsNotNull(paintEventArgs);

			var style = paintEventArgs.ListBox.Style;
			if((paintEventArgs.State & ItemState.Selected) == ItemState.Selected && style.Type == GitterStyleType.DarkBackground)
			{
				paintEventArgs.PaintText(text, paintEventArgs.Brush);
			}
			else
			{
				using(var textBrush = new SolidBrush(style.Colors.GrayText))
				{
					paintEventArgs.PaintText(text, textBrush);
				}
			}
		}

		#endregion

		#region Overrides

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
							username == null ? string.Empty : username, usermail == null ? string.Empty : usermail);
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
					{
						var type = Type == FakeRevisionItemType.StagedChanges ?
							RevisionGraphItemType.Uncommitted : RevisionGraphItemType.Unstaged;
						GraphColumn.OnPaintSubItem(paintEventArgs, Graph, type);
					}
					break;
				case ColumnId.User:
				case ColumnId.Author:
				case ColumnId.Committer:
					var user = Repository.UserIdentity;
					if(user != null)
					{
						PaintGrayText(paintEventArgs, user, UserColumn.OnPaintSubItem);
					}
					break;
				case ColumnId.Email:
				case ColumnId.AuthorEmail:
				case ColumnId.CommitterEmail:
					var usermail = Repository.Configuration.TryGetParameterValue(GitConstants.UserEmailParameter);
					if(!string.IsNullOrWhiteSpace(usermail))
					{
						PaintGrayText(paintEventArgs, usermail, EmailColumn.OnPaintSubItem);
					}
					break;
				case ColumnId.Date:
				case ColumnId.CommitDate:
				case ColumnId.AuthorDate:
					PaintGrayText(paintEventArgs, Resources.StrUncommitted.SurroundWith('<', '>'));
					break;
				case ColumnId.Hash:
				case ColumnId.TreeHash:
					PaintGrayText(paintEventArgs, NoHash, HashColumn.OnPaintSubItem);
					break;
			}
		}

		/// <summary>Gets the context menu.</summary>
		/// <param name="requestEventArgs">Request parameters.</param>
		/// <returns>Context menu for specified location.</returns>
		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			ContextMenuStrip menu = null;
			switch(Type)
			{
				case FakeRevisionItemType.UnstagedChanges:
					menu = new UnstagedChangesMenu(Repository);
					break;
				case FakeRevisionItemType.StagedChanges:
					menu = new StagedChangesMenu(Repository);
					break;
			}
			if(menu != null)
			{
				Utility.MarkDropDownForAutoDispose(menu);
			}
			return menu;
		}

		#endregion
	}
}
