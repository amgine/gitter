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

namespace gitter.Git.Gui;

using gitter.Framework;

static class Icons
{
	static IImageProvider Init(string name)
		=> new ScaledImageProvider(CachedResources.ScaledBitmaps, name);

	public static class Overlays
	{
		private static IImageProvider Init(string name)
			=> new ScaledImageProvider(CachedResources.ScaledBitmaps, "overlays." + name);

		public static readonly IImageProvider Delete       = Init(@"delete");
		public static readonly IImageProvider DeleteStaged = Init(@"delete.staged");
		public static readonly IImageProvider Add          = Init(@"add");
		public static readonly IImageProvider AddStaged    = Init(@"add.staged");
		public static readonly IImageProvider Edit         = Init(@"edit");
		public static readonly IImageProvider EditStaged   = Init(@"edit.staged");
		public static readonly IImageProvider Conflict     = Init(@"conflict");
		public static readonly IImageProvider Rename       = Init(@"rename");
		public static readonly IImageProvider Copy         = Init(@"copy");
		public static readonly IImageProvider Chmod        = Init(@"chmod");
	}

	public static readonly IImageProvider Git                = Init(@"git");
	public static readonly IImageProvider AlignToGraph       = Init(@"align.to.graph");
	public static readonly IImageProvider Archive            = Init(@"archive");
	public static readonly IImageProvider Refresh            = CommonIcons.Refresh;
	public static readonly IImageProvider History            = Init(@"history");
	public static readonly IImageProvider Folder             = CommonIcons.Folder;
	public static readonly IImageProvider FolderUp           = Init(@"folder.up");
	public static readonly IImageProvider FolderTree         = Init(@"folder.tree");
	public static readonly IImageProvider FolderHistory      = Init(@"folder.history");
	public static readonly IImageProvider Blame              = Init(@"blame");
	public static readonly IImageProvider Diff               = Init(@"diff");
	public static readonly IImageProvider Checkout           = Init(@"checkout");
	public static readonly IImageProvider Revert             = Init(@"revert");
	public static readonly IImageProvider RevertAbort        = Init(@"revert.abort");
	public static readonly IImageProvider RevertContinue     = Init(@"revert.continue");
	public static readonly IImageProvider RevertQuit         = Init(@"revert.quit");
	public static readonly IImageProvider Reset              = Init(@"reset");
	public static readonly IImageProvider Rebase             = Init(@"rebase");
	public static readonly IImageProvider RebaseAbort        = Init(@"rebase.abort");
	public static readonly IImageProvider RebaseContinue     = Init(@"rebase.continue");
	public static readonly IImageProvider RebaseSkip         = Init(@"rebase.skip");
	public static readonly IImageProvider Merge              = Init(@"merge");
	public static readonly IImageProvider CherryPick         = Init(@"cherry.pick");
	public static readonly IImageProvider CherryPickAbort    = Init(@"cherry.pick.abort");
	public static readonly IImageProvider CherryPickContinue = Init(@"cherry.pick.continue");
	public static readonly IImageProvider CherryPickQuit     = Init(@"cherry.pick.quit");
	public static readonly IImageProvider Delete             = CommonIcons.Delete;
	public static readonly IImageProvider Branches           = Init(@"branches");
	public static readonly IImageProvider Branch             = CommonIcons.Branch;
	public static readonly IImageProvider BranchAdd          = Init(@"branch.add");
	public static readonly IImageProvider BranchDelete       = Init(@"branch.delete");
	public static readonly IImageProvider BranchRename       = Init(@"branch.rename");
	public static readonly IImageProvider BranchReflog       = Init(@"branch.reflog");
	public static readonly IImageProvider RemoteBranches     = Init(@"rbranches");
	public static readonly IImageProvider RemoteBranch       = Init(@"rbranch");
	public static readonly IImageProvider RemoteBranchDelete = Init(@"rbranch.delete");
	public static readonly IImageProvider RemoteBranchReflog = Init(@"rbranch.reflog");
	public static readonly IImageProvider Tags               = Init(@"tags");
	public static readonly IImageProvider Tag                = CommonIcons.Tag;
	public static readonly IImageProvider TagAnnotated       = Init(@"tag.annotated");
	public static readonly IImageProvider TagAdd             = Init(@"tag.add");
	public static readonly IImageProvider TagDelete          = Init(@"tag.delete");
	public static readonly IImageProvider Clean              = Init(@"clean");
	public static readonly IImageProvider OrderDate          = Init(@"order.date");
	public static readonly IImageProvider OrderTopo          = Init(@"order.topo");
	public static readonly IImageProvider GarbageCollect     = Init(@"gc");
	public static readonly IImageProvider Remotes            = Init(@"remotes");
	public static readonly IImageProvider Remote             = Init(@"remote");
	public static readonly IImageProvider RemoteAdd          = Init(@"remote.add");
	public static readonly IImageProvider RemoteDelete       = Init(@"remote.delete");
	public static readonly IImageProvider RemoteRename       = Init(@"remote.rename");
	public static readonly IImageProvider RemoteProperties   = Init(@"remote.properties");
	public static readonly IImageProvider Patch              = Init(@"patch");
	public static readonly IImageProvider PatchApply         = Init(@"patch.apply");
	public static readonly IImageProvider PatchSave          = Init(@"patch.save");
	public static readonly IImageProvider Fetch              = Init(@"fetch");
	public static readonly IImageProvider Pull               = Init(@"pull");
	public static readonly IImageProvider Push               = Init(@"push");
	public static readonly IImageProvider PushWarning        = Init(@"push.warning");
	public static readonly IImageProvider Prune              = Init(@"prune");
	public static readonly IImageProvider ClipboardCopy      = CommonIcons.ClipboardCopy;
	public static readonly IImageProvider StatusClean        = Init(@"status.clean");
	public static readonly IImageProvider Search             = Init(@"search");
	public static readonly IImageProvider Stash              = Init(@"stash");
	public static readonly IImageProvider StashApply         = Init(@"stash.apply");
	public static readonly IImageProvider StashPop           = Init(@"stash.pop");
	public static readonly IImageProvider StashSave          = Init(@"stash.save");
	public static readonly IImageProvider StashDrop          = Init(@"stash.drop");
	public static readonly IImageProvider StashClear         = Init(@"stash.clear");
	public static readonly IImageProvider Stage              = Init(@"stage");
	public static readonly IImageProvider StageAll           = Init(@"stage.all");
	public static readonly IImageProvider Submodules         = Init(@"submodules");
	public static readonly IImageProvider Submodule          = Init(@"submodule");
	public static readonly IImageProvider SubmoduleSync      = Init(@"submodule.sync");
	public static readonly IImageProvider SubmoduleAdd       = Init(@"submodule.add");
	public static readonly IImageProvider Unstage            = Init(@"unstage");
	public static readonly IImageProvider UnstageAll         = Init(@"unstage.all");
	public static readonly IImageProvider Commit             = Init(@"commit");
	public static readonly IImageProvider MarkResolved       = Init(@"mark.resolved");
	public static readonly IImageProvider Terminal           = CommonIcons.Terminal;
	public static readonly IImageProvider Mail               = Init(@"mail");
	public static readonly IImageProvider MailSend           = Init(@"mail.send");
	public static readonly IImageProvider Users              = Init(@"users");
	public static readonly IImageProvider User               = CommonIcons.User;
	public static readonly IImageProvider UserUnknown        = Init(@"user.unknown");
	public static readonly IImageProvider Configuration      = Init(@"configuration");
	public static readonly IImageProvider Config             = Init(@"config");
	public static readonly IImageProvider ConfigAdd          = Init(@"config.add");
	public static readonly IImageProvider ConfigEdit         = Init(@"config.edit");
	public static readonly IImageProvider ConfigUnset        = Init(@"config.unset");
	public static readonly IImageProvider FileHistory        = Init(@"file.history");
	public static readonly IImageProvider Note               = Init(@"note");
	public static readonly IImageProvider NoteAdd            = Init(@"note.add");
	public static readonly IImageProvider Plus               = Init(@"plus");
	public static readonly IImageProvider Minus              = Init(@"minus");
	public static readonly IImageProvider Filter             = Init(@"filter");
	public static readonly IImageProvider Info               = Init(@"info");
	public static readonly IImageProvider Warning            = Init(@"warning");
}
