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

namespace gitter.Git.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;

	using gitter.Git.AccessLayer;
	
	using Resources = gitter.Git.Gui.Properties.Resources;

	public static class GuiCommands
	{
		private static string ExtractErrorMessage(Exception exc)
		{
			exc = TaskUtility.UnwrapException(exc);
			return exc.Message;
		}

		private static GuiCommandStatus Fetch(IWin32Window parent, Repository repository, Remote remote)
		{
			Func<IProgress<OperationProgress>, CancellationToken, Task> func;
			if(remote == null)
			{
				func = repository.Remotes.FetchAsync;
			}
			else
			{
				func = remote.FetchAsync;
			}
			try
			{
				ProgressForm.MonitorTaskAsModalWindow(parent, Resources.StrFetch, func);
				return GuiCommandStatus.Completed;
			}
			catch(OperationCanceledException)
			{
				return GuiCommandStatus.Canceled;
			}
			catch(GitException exc)
			{
				string messageTitle = remote != null ?
					string.Format(CultureInfo.InvariantCulture, Resources.ErrFailedToFetchFrom, remote.Name) :
					Resources.ErrFailedToFetch;
				GitterApplication.MessageBoxService.Show(
					parent,
					ExtractErrorMessage(exc),
					messageTitle,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return GuiCommandStatus.Faulted;
			}
		}

		public static GuiCommandStatus Fetch(IWin32Window parent, Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			return Fetch(parent, repository, null);
		}

		public static GuiCommandStatus Fetch(IWin32Window parent, Remote remote)
		{
			Verify.Argument.IsNotNull(remote, "remote");

			return Fetch(parent, remote.Repository, remote);
		}

		private static GuiCommandStatus Pull(IWin32Window parent, Repository repository, Remote remote)
		{
			Func<IProgress<OperationProgress>, CancellationToken, Task> func;
			if(remote == null)
			{
				func = repository.Remotes.PullAsync;
			}
			else
			{
				func = remote.PullAsync;
			}
			try
			{
				ProgressForm.MonitorTaskAsModalWindow(parent, Resources.StrPull, func);
				return GuiCommandStatus.Completed;
			}
			catch(OperationCanceledException)
			{
				return GuiCommandStatus.Canceled;
			}
			catch(GitException exc)
			{
				string messageTitle = remote != null ?
					string.Format(CultureInfo.InvariantCulture, Resources.ErrFailedToPullFrom, remote.Name) :
					Resources.ErrFailedToPull;
				GitterApplication.MessageBoxService.Show(
					parent,
					ExtractErrorMessage(exc),
					messageTitle,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return GuiCommandStatus.Faulted;
			}
		}

		public static GuiCommandStatus Pull(IWin32Window parent, Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			return Pull(parent, repository, null);
		}

		public static GuiCommandStatus Pull(IWin32Window parent, Remote remote)
		{
			Verify.Argument.IsNotNull(remote, "remote");

			return Pull(parent, remote.Repository, remote);
		}

		private static GuiCommandStatus Push(IWin32Window parent, Func<IProgress<OperationProgress>, CancellationToken, Task> func, string remoteRepository)
		{
			try
			{
				ProgressForm.MonitorTaskAsModalWindow(parent, Resources.StrPush, func);
				return GuiCommandStatus.Completed;
			}
			catch(OperationCanceledException)
			{
				return GuiCommandStatus.Canceled;
			}
			catch(GitException exc)
			{
				var messageTitle = string.Format(CultureInfo.InvariantCulture, Resources.ErrPushFailed, remoteRepository);
				GitterApplication.MessageBoxService.Show(
					parent,
					ExtractErrorMessage(exc),
					messageTitle,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return GuiCommandStatus.Faulted;
			}
		}

		public static GuiCommandStatus Push(IWin32Window parent, Remote remote, ICollection<Branch> branches, bool forceOverwrite, bool thinPack, bool sendTags)
		{
			Func<IProgress<OperationProgress>, CancellationToken, Task> func =
				(p, c) => remote.PushAsync(branches, forceOverwrite, thinPack, sendTags, p, c);

			return Push(parent, func, remote.Name);
		}

		public static GuiCommandStatus Push(IWin32Window parent, Repository repository, string url, ICollection<Branch> branches, bool forceOverwrite, bool thinPack, bool sendTags)
		{
			Func<IProgress<OperationProgress>, CancellationToken, Task> func =
				(p, c) => repository.Remotes.PushToAsync(url, branches, forceOverwrite, thinPack, sendTags, p, c);

			return Push(parent, func, url);
		}

		public static GuiCommandStatus Prune(IWin32Window parent, Remote remote)
		{
			Verify.Argument.IsNotNull(remote, "remote");

			try
			{
				ProgressForm.MonitorTaskAsModalWindow(
					parent,
					Resources.StrPrune + ": " + remote.Name,
					remote.PruneAsync);
				return GuiCommandStatus.Completed;
			}
			catch(OperationCanceledException)
			{
				return GuiCommandStatus.Canceled;
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					string.Format(Resources.ErrFailedToPrune, remote.Name),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return GuiCommandStatus.Faulted;
			}
		}

		public static GuiCommandStatus Clone(IWin32Window parent, IGitAccessor gitAccessor, string url, string path, string template, string remoteName, bool shallow, int depth, bool bare, bool mirror, bool recursive, bool noCheckout)
		{
			try
			{
				ProgressForm.MonitorTaskAsModalWindow(parent, Resources.StrClone, (p, c) =>
					Repository.CloneAsync(gitAccessor, url, path, template, remoteName, shallow, depth, bare, mirror, recursive, noCheckout, p, c));
				return GuiCommandStatus.Completed;
			}
			catch(OperationCanceledException)
			{
				return GuiCommandStatus.Canceled;
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToClone.UseAsFormat(url),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return GuiCommandStatus.Faulted;
			}
		}

		public static GuiCommandStatus Archive(IWin32Window parent, IRevisionPointer revision, string path, string format)
		{
			string outputPath = null;
			using(var dlg = new SaveFileDialog()
				{
					FileName = revision.Pointer,
					Filter = "zip files|.zip|" +
							 "tar.gz files|.tar.gz|" +
							 "tar files|.tar|" +
							 "tgz files|.tgz",
					DefaultExt = ".zip",
					OverwritePrompt = true,
					Title = Resources.StrArchive,
				})
			{
				if(dlg.ShowDialog(parent) == DialogResult.OK)
				{
					outputPath = dlg.FileName;
				}
			}
			if(string.IsNullOrWhiteSpace(outputPath))
			{
				return GuiCommandStatus.Canceled;
			}
			try
			{
				ProgressForm.MonitorTaskAsModalWindow(parent, Resources.StrArchive,
					(p) => revision.ArchiveAsync(outputPath, null, null, p));
				return GuiCommandStatus.Completed;
			}
			catch(OperationCanceledException)
			{
				return GuiCommandStatus.Canceled;
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToArchive,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return GuiCommandStatus.Faulted;
			}
		}

		public static GuiCommandStatus FormatPatch(IWin32Window parent, IRevisionPointer revision)
		{
			Verify.Argument.IsNotNull(revision, "revision");

			const string patchExt = ".patch";
			string outputPath = null;
			using(var dlg = new SaveFileDialog()
				{
					FileName        = revision.Pointer + patchExt,
					Filter          = Resources.StrPatches + "|" + patchExt,
					DefaultExt      = patchExt,
					OverwritePrompt = true,
					Title           = Resources.StrSavePatch,
				})
			{
				if(dlg.ShowDialog(parent) == DialogResult.OK)
				{
					outputPath = dlg.FileName;
				}
				else
				{
					return GuiCommandStatus.Canceled;
				}
			}
			if(string.IsNullOrWhiteSpace(outputPath))
			{
				return GuiCommandStatus.Canceled;
			}
			byte[] patch;
			try
			{
				patch = ProgressForm.MonitorTaskAsModalWindow(parent,
					Resources.StrSavePatch, revision.FormatPatchAsync);
			}
			catch(OperationCanceledException)
			{
				return GuiCommandStatus.Canceled;
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToFormatPatch,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return GuiCommandStatus.Faulted;
			}
			if(patch != null)
			{
				try
				{
					File.WriteAllBytes(outputPath, patch);
					return GuiCommandStatus.Completed;
				}
				catch(Exception exc)
				{
					if(exc.IsCritical())
					{
						throw;
					}
					GitterApplication.MessageBoxService.Show(
						parent,
						exc.Message,
						Resources.ErrFailedToSavePatch,
						MessageBoxButton.Close,
						MessageBoxIcon.Error);
					return GuiCommandStatus.Faulted;
				}
			}
			return GuiCommandStatus.Faulted;
		}

		public static GuiCommandStatus RebaseHeadTo(IWin32Window parent, IRevisionPointer revision)
		{
			Verify.Argument.IsNotNull(revision, "revision");

			try
			{
				ProgressForm.MonitorTaskAsModalWindow(parent, Resources.StrRebase,
					p => revision.RebaseHeadHereAsync(p));
				return GuiCommandStatus.Completed;
			}
			catch(OperationCanceledException)
			{
				return GuiCommandStatus.Canceled;
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToRebase,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return GuiCommandStatus.Faulted;
			}
		}

		public static GuiCommandStatus Rebase(IWin32Window parent, Repository repository, RebaseControl control)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			try
			{
				ProgressForm.MonitorTaskAsModalWindow(parent, Resources.StrRebase,
					p => repository.RebaseAsync(control, p));
				return GuiCommandStatus.Completed;
			}
			catch(OperationCanceledException)
			{
				return GuiCommandStatus.Canceled;
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToRebase,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return GuiCommandStatus.Faulted;
			}
		}

		public static GuiCommandStatus SaveStash(IWin32Window parent, StashedStatesCollection stash, bool keepIndex, bool includeUntracked, string message)
		{
			try
			{
				ProgressForm.MonitorTaskAsModalWindow(parent, Resources.StrStashSave,
					p => stash.SaveAsync(keepIndex, includeUntracked, message, p));
				return GuiCommandStatus.Completed;
			}
			catch(OperationCanceledException)
			{
				return GuiCommandStatus.Canceled;
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToStash,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return GuiCommandStatus.Faulted;
			}
		}

		public static GuiCommandStatus PopStashedState(IWin32Window parent, StashedState stashedState, bool restoreIndex)
		{
			Verify.Argument.IsNotNull(stashedState, "stashedState");

			try
			{
				ProgressForm.MonitorTaskAsModalWindow(parent, Resources.StrStashPop,
					p => stashedState.PopAsync(restoreIndex, p));
				return GuiCommandStatus.Completed;
			}
			catch(OperationCanceledException)
			{
				return GuiCommandStatus.Canceled;
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					string.Format(Resources.ErrFailedToStashPopState, ((IRevisionPointer)stashedState).Pointer),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return GuiCommandStatus.Faulted;
			}
		}

		public static GuiCommandStatus PopStashedState(IWin32Window parent, StashedStatesCollection stash, bool restoreIndex)
		{
			Verify.Argument.IsNotNull(stash, "stash");

			try
			{
				ProgressForm.MonitorTaskAsModalWindow(parent, Resources.StrStashPop,
					p => stash.PopAsync(restoreIndex, p));
				return GuiCommandStatus.Completed;
			}
			catch(OperationCanceledException)
			{
				return GuiCommandStatus.Canceled;
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToStashPop,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return GuiCommandStatus.Faulted;
			}
		}

		public static GuiCommandStatus ApplyStashedState(IWin32Window parent, StashedState stashedState, bool restoreIndex)
		{
			Verify.Argument.IsNotNull(stashedState, "stashedState");

			try
			{
				ProgressForm.MonitorTaskAsModalWindow(parent, Resources.StrStashApply,
					p => stashedState.ApplyAsync(restoreIndex, p));
				return GuiCommandStatus.Completed;
			}
			catch(OperationCanceledException)
			{
				return GuiCommandStatus.Canceled;
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					string.Format(Resources.ErrFailedToStashApplyState, ((IRevisionPointer)stashedState).Pointer),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return GuiCommandStatus.Faulted;
			}
		}

		public static GuiCommandStatus ApplyStashedState(IWin32Window parent, StashedStatesCollection stash, bool restoreIndex)
		{
			Verify.Argument.IsNotNull(stash, "stash");

			try
			{
				ProgressForm.MonitorTaskAsModalWindow(parent, Resources.StrStashApply,
					p => stash.ApplyAsync(restoreIndex, p));
				return GuiCommandStatus.Completed;
			}
			catch(OperationCanceledException)
			{
				return GuiCommandStatus.Canceled;
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToStashApply,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return GuiCommandStatus.Faulted;
			}
		}

		public static GuiCommandStatus DropStashedState(IWin32Window parent, StashedState stashedState)
		{
			Verify.Argument.IsNotNull(stashedState, "stashedState");

			try
			{
				ProgressForm.MonitorTaskAsModalWindow(parent, Resources.StrStashDrop, stashedState.DropAsync);
				return GuiCommandStatus.Completed;
			}
			catch(OperationCanceledException)
			{
				return GuiCommandStatus.Canceled;
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					string.Format(Resources.ErrFailedToStashDropState, ((IRevisionPointer)stashedState).Pointer),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return GuiCommandStatus.Faulted;
			}
		}

		public static GuiCommandStatus DropStashedState(IWin32Window parent, StashedStatesCollection stash)
		{
			Verify.Argument.IsNotNull(stash, "stash");

			try
			{
				ProgressForm.MonitorTaskAsModalWindow(parent, Resources.StrStashDrop, stash.DropAsync);
				return GuiCommandStatus.Completed;
			}
			catch(OperationCanceledException)
			{
				return GuiCommandStatus.Canceled;
			}
			catch(InvalidOperationException)
			{
				return GuiCommandStatus.Faulted;
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToStashDrop,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return GuiCommandStatus.Faulted;
			}
		}

		public static GuiCommandStatus ClearStash(IWin32Window parent, StashedStatesCollection stash)
		{
			Verify.Argument.IsNotNull(stash, "stash");

			try
			{
				ProgressForm.MonitorTaskAsModalWindow(parent, Resources.StrStashClear, stash.ClearAsync);
				return GuiCommandStatus.Completed;
			}
			catch(OperationCanceledException)
			{
				return GuiCommandStatus.Canceled;
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToStashClear,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return GuiCommandStatus.Faulted;
			}
		}

		public static GuiCommandStatus UpdateSubmodule(IWin32Window parent, Submodule submodule)
		{
			Verify.Argument.IsNotNull(submodule, "submodule");

			if(parent == null)
			{
				parent = GitterApplication.MainForm;
			}
			try
			{
				ProgressForm.MonitorTaskAsModalWindow(parent, Resources.StrUpdate + ": " + submodule.Name,
					submodule.UpdateAsync);
				return GuiCommandStatus.Completed;
			}
			catch(OperationCanceledException)
			{
				return GuiCommandStatus.Canceled;
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					string.Format(Resources.ErrFailedToUpdateSubmodule, submodule.Name),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return GuiCommandStatus.Faulted;
			}
		}

		public static GuiCommandStatus UpdateSubmodules(IWin32Window parent, SubmodulesCollection submodules)
		{
			Verify.Argument.IsNotNull(submodules, "submodules");

			if(parent == null)
			{
				parent = GitterApplication.MainForm;
			}
			try
			{
				ProgressForm.MonitorTaskAsModalWindow(parent, Resources.StrUpdate, submodules.UpdateAsync);
				return GuiCommandStatus.Completed;
			}
			catch(OperationCanceledException)
			{
				return GuiCommandStatus.Canceled;
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToUpdateSubmodule,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return GuiCommandStatus.Faulted;
			}
		}

		public static GuiCommandStatus GarbageCollect(IWin32Window parent, Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			try
			{
				ProgressForm.MonitorTaskAsModalWindow(parent, Resources.StrHousekeeping, repository.GarbageCollectAsync);
				return GuiCommandStatus.Completed;
			}
			catch(OperationCanceledException)
			{
				return GuiCommandStatus.Canceled;
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToCompressRepository,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return GuiCommandStatus.Faulted;
			}
		}
	}
}
