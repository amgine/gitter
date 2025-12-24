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

namespace gitter.Git.Gui.Controls;

using System;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

/// <summary>Item used to represent uncommitted or unstaged changes to the working tree.</summary>
public class FakeRevisionListItem : CustomListBoxItem<Revision?>, IRevisionGraphListItem
{
	#region Helpers

	public struct FileStatusIconEntry
	{
		public IImageProvider Image;
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
		Verify.Argument.IsNotNull(repository);

		Repository = repository;
		Type = type;
		switch(type)
		{
			case FakeRevisionItemType.StagedChanges:
				lock(repository.Status.SyncRoot)
				{
					_iconEntries =
					[
						new FileStatusIconEntry { Image = FileStatusIcons.ImgStagedAdded,
							Count = repository.Status.StagedAddedCount },
						new FileStatusIconEntry { Image = FileStatusIcons.ImgStagedRemoved,
							Count = repository.Status.StagedRemovedCount },
						new FileStatusIconEntry { Image = FileStatusIcons.ImgStagedModified,
							Count = repository.Status.StagedModifiedCount },
					];
				}
				break;
			case FakeRevisionItemType.UnstagedChanges:
				lock(repository.Status.SyncRoot)
				{
					SubType = GetSubType(repository.Status);
					_iconEntries =
					[
						new FileStatusIconEntry { Image = FileStatusIcons.ImgUnmerged,
							Count = repository.Status.UnmergedCount },
						new FileStatusIconEntry { Image = FileStatusIcons.ImgUnstagedUntracked,
							Count = repository.Status.UnstagedUntrackedCount },
						new FileStatusIconEntry { Image = FileStatusIcons.ImgUnstagedRemoved,
							Count = repository.Status.UnstagedRemovedCount },
						new FileStatusIconEntry { Image = FileStatusIcons.ImgUnstagedModified,
							Count = repository.Status.UnstagedModifiedCount },
					];
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

	public GraphCell[]? Graph { get; set; }

	public FileStatusIconEntry[] Icons => _iconEntries;

	public string? SubjectText
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
		Assert.IsNotNull(status);

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

	private void OnStatusUpdated(object? sender, EventArgs e)
	{
		var status = (Status)sender!;
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

	/// <inheritdoc/>
	protected override void OnListBoxAttached(CustomListBox listBox)
	{
		base.OnListBoxAttached(listBox);
		Repository.Status.Changed += OnStatusUpdated;
	}

	/// <inheritdoc/>
	protected override void OnListBoxDetached(CustomListBox listBox)
	{
		base.OnListBoxDetached(listBox);
		Repository.Status.Changed -= OnStatusUpdated;
	}

	/// <inheritdoc/>
	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		Assert.IsNotNull(measureEventArgs);

		switch((ColumnId)measureEventArgs.SubItemId)
		{
			case ColumnId.AuthorDate:
			case ColumnId.Date:
			case ColumnId.CommitDate:
				return measureEventArgs.MeasureText(Resources.StrUncommitted.SurroundWith('<', '>'));
			default:
				return base.OnMeasureSubItem(measureEventArgs);
		}
	}

	/// <inheritdoc/>
	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(paintEventArgs);

		switch((ColumnId)paintEventArgs.SubItemId)
		{
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

	/// <inheritdoc/>
	public override ContextMenuStrip? GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
	{
		Verify.Argument.IsNotNull(requestEventArgs);

		ContextMenuStrip? menu = Type switch
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
