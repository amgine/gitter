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

		#region Data

		private readonly FileStatusIconEntry[] _iconEntries;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="FakeRevisionListItem"/>.</summary>
		/// <param name="repository">Related repository.</param>
		/// <param name="type">Item type.</param>
		public FakeRevisionListItem(Repository repository, FakeRevisionItemType type)
			: base(null)
		{
			Verify.Argument.IsNotNull(repository, nameof(repository));

			Repository = repository;
			Type = type;
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
						SubType = GetSubType(repository.Status);
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
					throw new ArgumentException("Unknown type value.", nameof(type));
			}
		}

		#endregion

		#region Properties

		public Repository Repository { get; }

		public FakeRevisionItemType Type { get; }

		public UnstagedRevisionItemSubtype SubType { get; private set; }

		public GraphAtom[] Graph { get; set; }

		private string SubjectText
			=> Type switch
			{
				FakeRevisionItemType.StagedChanges   => Resources.StrUncommittedLocalChanges,
				FakeRevisionItemType.UnstagedChanges => SubType switch
				{
					UnstagedRevisionItemSubtype.Conflicts      => Resources.StrlUnmergedFilesPresent,
					UnstagedRevisionItemSubtype.RemovedFiles   => Resources.StrlUnstagedRemovedFiles,
					UnstagedRevisionItemSubtype.UntrackedFiles => Resources.StrlUnstagedUntrackedFiles,
					UnstagedRevisionItemSubtype.Modifications  => Resources.StrUnstagedLocalChanges,
					_ => Resources.StrUnstagedLocalChanges,
				},
				_ => default,
			};

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
					SubType = GetSubType(status);
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
			Assert.IsNotNull(paintEventArgs);

			var rect = paintEventArgs.Bounds;
			var conv = paintEventArgs.DpiConverter;
			var alignToGraph = paintEventArgs.Column is SubjectColumn rsc
				? rsc.AlignToGraph
				: SubjectColumn.DefaultAlignToGraph;
			var graphColumn = ListBox.GetPrevVisibleColumn(paintEventArgs.ColumnIndex);
			if(graphColumn is not null && graphColumn.Id != (int)ColumnId.Graph)
			{
				graphColumn = null;
			}
			if(alignToGraph && graphColumn is not null)
			{
				int availWidth;
				if(Graph is not null)
				{
					availWidth = graphColumn.Width - paintEventArgs.ListBox.ItemHeight * Graph.Length;
					for(int i = Graph.Length - 1; i != -1; --i)
					{
						if(Graph[i].Elements != GraphElement.Space)
						{
							break;
						}
						availWidth += paintEventArgs.ListBox.ItemHeight;
					}
				}
				else
				{
					availWidth = graphColumn.Width;
				}
				if(availWidth != 0)
				{
					rect.X     -= availWidth;
					rect.Width += availWidth;
				}
			}
			paintEventArgs.PrepareContentRectangle(ref rect);
			if(rect.Width > 1)
			{
				var text = SubjectText;
				paintEventArgs.PrepareTextRectangle(ref rect);
				var useDefaultBrush = (paintEventArgs.State & ItemState.Selected) == ItemState.Selected && paintEventArgs.ListBox.Style.Type == GitterStyleType.DarkBackground;
				var textBrush = useDefaultBrush ? paintEventArgs.Brush : new SolidBrush(paintEventArgs.ListBox.Style.Colors.GrayText);
				if(!string.IsNullOrWhiteSpace(text))
				{
					var w = GitterApplication.TextRenderer.MeasureText(
						paintEventArgs.Graphics, text, paintEventArgs.Font, int.MaxValue).Width;
						GitterApplication.TextRenderer.DrawText(
							paintEventArgs.Graphics, text, paintEventArgs.Font, textBrush, rect);
					w += conv.ConvertX(3);
					rect.X     += w;
					rect.Width -= w;
				}
				var iconSize = conv.Convert(new Size(16, 16));
				for(int i = 0; i < _iconEntries.Length; ++i)
				{
					if(rect.Width <= 0) break;
					if(_iconEntries[i].Count != 0)
					{
						var image = _iconEntries[i].Image;
						var imageRect = new Rectangle(rect.X, rect.Y - 1 + (rect.Height - iconSize.Height) / 2, iconSize.Width, iconSize.Height);
						var dx = imageRect.Width + conv.ConvertX(2);
						rect.X     += dx;
						rect.Width -= dx;
						if(rect.Width <= 0) break;
						paintEventArgs.Graphics.DrawImage(image, imageRect);
						var countText = _iconEntries[i].Count.ToString(CultureInfo.CurrentCulture);
						var textW = GitterApplication.TextRenderer.MeasureText(
							paintEventArgs.Graphics, countText, paintEventArgs.Font, int.MaxValue).Width;
						GitterApplication.TextRenderer.DrawText(
							paintEventArgs.Graphics, countText, paintEventArgs.Font, textBrush, rect);
						textW += conv.ConvertX(2);
						rect.X     += textW;
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
				using var textBrush = new SolidBrush(style.Colors.GrayText);
				paintMethod(paintEventArgs, value, textBrush);
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
				using var textBrush = new SolidBrush(style.Colors.GrayText);
				paintEventArgs.PaintText(text, textBrush);
			}
		}

		#endregion

		#region Overrides

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			Repository.Status.Changed += OnStatusUpdated;
		}

		protected override void OnListBoxDetached()
		{
			base.OnListBoxDetached();
			Repository.Status.Changed -= OnStatusUpdated;
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.AuthorDate:
				case ColumnId.Date:
				case ColumnId.CommitDate:
					return measureEventArgs.MeasureText(Resources.StrUncommitted.SurroundWith('<', '>'));
				case ColumnId.Author:
				case ColumnId.Committer:
					{
						var username = Repository.Configuration.TryGetParameterValue(GitConstants.UserNameParameter);
						var usermail = Repository.Configuration.TryGetParameterValue(GitConstants.UserEmailParameter);
						return UserColumn.OnMeasureSubItem(measureEventArgs,
							username == null ? string.Empty : username, usermail == null ? string.Empty : usermail);
					}
				case ColumnId.Email:
				case ColumnId.CommitterEmail:
				case ColumnId.AuthorEmail:
					{
						var usermail = Repository.Configuration.TryGetParameter(GitConstants.UserEmailParameter);
						return EmailColumn.OnMeasureSubItem(measureEventArgs, usermail == null ? "" : usermail.Value);
					}
				default:
					return base.OnMeasureSubItem(measureEventArgs);
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Subject:
					DrawSubjectColumn(paintEventArgs);
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
				default:
					base.OnPaintSubItem(paintEventArgs);
					break;
			}
		}

		/// <summary>Gets the context menu.</summary>
		/// <param name="requestEventArgs">Request parameters.</param>
		/// <returns>Context menu for specified location.</returns>
		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			ContextMenuStrip menu = Type switch
			{
				FakeRevisionItemType.UnstagedChanges => new UnstagedChangesMenu(Repository),
				FakeRevisionItemType.StagedChanges   => new StagedChangesMenu(Repository),
				_ => default,
			};
			if(menu is not null)
			{
				Utility.MarkDropDownForAutoDispose(menu);
			}
			return menu;
		}

		#endregion
	}
}
