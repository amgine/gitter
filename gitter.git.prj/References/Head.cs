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

#nullable enable

namespace gitter.Git;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using gitter.Git.AccessLayer;

using Resources = gitter.Git.Properties.Resources;

/// <summary>Repository's HEAD reference.</summary>
public sealed class Head : Reference
{
	#region Events

	/// <summary>Occurs when HEAD gets detached.</summary>
	public event EventHandler? Detached;

	/// <summary>Occurs when HEAD gets attached.</summary>
	public event EventHandler? Attached;

	/// <summary>Invokes <see cref="Detached"/> event.</summary>
	private void OnDetached()
		=> Detached?.Invoke(this, EventArgs.Empty);

	/// <summary>Invokes <see cref="Attached"/> event.</summary>
	private void OnAttached()
		=> Attached?.Invoke(this, EventArgs.Empty);

	#endregion

	#region Data

	private bool _wasDetached;

	#endregion

	#region .ctor

	/// <summary>Initializes a new instance of the <see cref="Head"/> class.</summary>
	/// <param name="repository">Host repository.</param>
	/// <exception cref="ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
	internal Head(Repository repository)
		: base(repository, GitConstants.HEAD, GetHeadPointer(repository))
	{
	}

	/// <summary>Initializes a new instance of the <see cref="Head"/> class.</summary>
	/// <param name="repository">Host repository.</param>
	/// <param name="pointer">Target of this reference.</param>
	/// <exception cref="ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
	internal Head(Repository repository, IRevisionPointer pointer)
		: base(repository, GitConstants.HEAD, pointer)
	{
	}

	#endregion

	/// <summary>Gets the current branch.</summary>
	/// <value>Current branch or <c>null</c> if HEAD is detached.</value>
	public Branch? CurrentBranch => Pointer as Branch;

	/// <summary>Returns object pointed by HEAD.</summary>
	/// <param name="repository">Repository to get HEAD from.</param>
	/// <returns>Object pointed by HEAD of the specified repository.</returns>
	internal static IRevisionPointer GetHeadPointer(Repository repository)
	{
		Assert.IsNotNull(repository);

		var head = repository.Accessor.QuerySymbolicReference.Invoke(
			new QuerySymbolicReferenceParameters(GitConstants.HEAD));

		switch(head.TargetType)
		{
			case ReferenceType.LocalBranch:
				Branch branch;
				lock(repository.Refs.Heads.SyncRoot)
				{
					branch = repository.Refs.Heads.TryGetItem(head.TargetObject);
					if(branch is null)
					{
						var info = repository.Accessor.QueryBranch.Invoke(
							new QueryBranchParameters(head.TargetObject, false));
						if(info is not null)
						{
							branch = repository.Refs.Heads.NotifyCreated(info);
						}
					}
				}
				return branch is not null ? branch : new NowherePointer(repository, head.TargetObject);
			case ReferenceType.Revision:
				lock(repository.Revisions.SyncRoot)
				{
					return repository.Revisions.GetOrCreateRevision(new Hash(head.TargetObject));
				}
			default:
				return new NowherePointer(repository, head.TargetObject);
		}
	}

	/// <summary>Returns object pointed by HEAD.</summary>
	/// <param name="repository">Repository to get HEAD from.</param>
	/// <returns>Object pointed by HEAD of the specified repository.</returns>
	internal static async Task<IRevisionPointer> GetHeadPointerAsync(Repository repository)
	{
		Assert.IsNotNull(repository);

		var head = await repository.Accessor.QuerySymbolicReference
			.InvokeAsync(new QuerySymbolicReferenceParameters(GitConstants.HEAD))
			.ConfigureAwait(continueOnCapturedContext: false);

		switch(head.TargetType)
		{
			case ReferenceType.LocalBranch:
				Branch branch;
				lock(repository.Refs.Heads.SyncRoot)
				{
					branch = repository.Refs.Heads.TryGetItem(head.TargetObject);
				}
				if(branch is null)
				{
					var info = await repository.Accessor.QueryBranch
						.InvokeAsync(new QueryBranchParameters(head.TargetObject, false))
						.ConfigureAwait(continueOnCapturedContext: false);
					if(info is not null)
					{
						lock(repository.Refs.Heads.SyncRoot)
						{
							branch = repository.Refs.Heads.NotifyCreated(info);
						}
					}
				}
				return branch is not null ? branch : new NowherePointer(repository, head.TargetObject);
			case ReferenceType.Revision:
				lock(repository.Revisions.SyncRoot)
				{
					return repository.Revisions.GetOrCreateRevision(new Hash(head.TargetObject));
				}
			default:
				return new NowherePointer(repository, head.TargetObject);
		}
	}

	/// <summary>Filter <see cref="IRevisionPointer"/> to types supported by this <see cref="Reference"/>.</summary>
	/// <param name="pointer">Raw pointer.</param>
	/// <returns>Valid pointer.</returns>
	protected override IRevisionPointer PrepareInputPointer(IRevisionPointer pointer)
	{
		Verify.Argument.IsNotNull(pointer);

		return pointer.Type switch
		{
			ReferenceType.None or ReferenceType.LocalBranch => pointer,
			_ => pointer.Dereference(),
		};
	}

	/// <summary>Gets a value indicating whether HEAD is detached.</summary>
	/// <value><c>true</c> if HEAD is detached; otherwise, <c>false</c>.</value>
	public bool IsDetached => Pointer.Type != ReferenceType.LocalBranch;

	/// <summary>Gets a value indicating whether HEAD is pointing to non-existent object.</summary>
	/// <value><c>true</c> if HEAD is pointing to non-existent object; otherwise, <c>false</c>.</value>
	/// <remarks>It is typical to newly created repository with Head.IsEmpty == <c>true</c>.</remarks>
	public bool IsEmpty => Pointer.Type == ReferenceType.None;

	/// <summary>Called when this <see cref="Reference"/> is moved away from <paramref name="pointer"/>.</summary>
	/// <param name="pointer">Object, which this <see cref="Reference"/> was pointing to.</param>
	protected override void LeavePointer(IRevisionPointer pointer)
	{
		var branch = pointer as Branch;
		_wasDetached = branch is null;
		if(branch is not null)
		{
			branch.PositionChanged -= OnBranchPositionChanged;
		}
	}

	/// <summary>Called when this <see cref="Reference"/> is moved to <paramref name="pointer"/>.</summary>
	/// <param name="pointer">Object, which this <see cref="Reference"/> will be pointing to.</param>
	protected override void EnterPointer(IRevisionPointer pointer)
	{
		var branch = pointer as Branch;
		bool detached = branch is null;
		if(_wasDetached != detached)
		{
			if(detached)
			{
				OnDetached();
			}
			else
			{
				OnAttached();
			}
		}
		if(branch is not null)
		{
			branch.PositionChanged += OnBranchPositionChanged;
		}
	}

	/// <summary>Notifies about reflog modification.</summary>
	internal override void NotifyReflogRecordAdded()
	{
		base.NotifyReflogRecordAdded();
		(Pointer as Reference)?.NotifyReflogRecordAdded();
	}

	private void OnBranchPositionChanged(object? sender, RevisionChangedEventArgs e)
	{
		Assert.IsNotNull(e);

		LeaveRevision(e.OldValue);
		EnterRevision(e.NewValue);
		InvokePositionChanged(e.OldValue, e.NewValue);
	}

	/// <summary>Reset HEAD to <paramref name="pointer"/>.</summary>
	/// <param name="mode">Reset mode</param>
	/// <param name="pointer">HEAD's new position.</param>
	/// <exception cref="T:System.ArgumentNullException"><paramref name="pointer"/> == <c>null</c>.</exception>
	/// <exception cref="T:System.ArgumentException">
	/// <paramref name="pointer"/> is not handled by this <see cref="Repository"/> or it is deleted.
	/// </exception>
	/// <exception cref="T:gitter.Git.GitException">
	/// Failed to dereference <paramref name="pointer"/> or git reset failed.
	/// </exception>
	public void Reset(IRevisionPointer pointer, ResetMode mode = ResetMode.Mixed)
	{
		Verify.Argument.IsValidRevisionPointer(pointer, Repository);

		var pos = Pointer.Dereference();
		var rev = pointer.Dereference();

		var currentBranch = Pointer as Branch;

		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.BranchChanged,
			RepositoryNotifications.Checkout,
			RepositoryNotifications.IndexUpdated,
			RepositoryNotifications.WorktreeUpdated,
			RepositoryNotifications.SubmodulesChanged))
		{
			Repository.Accessor.Reset
				.Invoke(new ResetParameters(rev.Hash.ToString(), mode));
		}

		if(currentBranch is not null)
		{
			currentBranch.Pointer = rev;
		}
		else
		{
			Pointer = rev;
		}

		if(pos != rev)
		{
			NotifyReflogRecordAdded();
		}

		Repository.Status.Refresh();
		Repository.Submodules.Refresh();
		Repository.OnStateChanged();
	}

	/// <summary>Reset HEAD to <paramref name="pointer"/>.</summary>
	/// <param name="mode">Reset mode</param>
	/// <param name="pointer">HEAD's new position.</param>
	/// <exception cref="T:System.ArgumentNullException"><paramref name="pointer"/> == <c>null</c>.</exception>
	/// <exception cref="T:System.ArgumentException">
	/// <paramref name="pointer"/> is not handled by this <see cref="Repository"/> or it is deleted.
	/// </exception>
	/// <exception cref="T:gitter.Git.GitException">
	/// Failed to dereference <paramref name="pointer"/> or git reset failed.
	/// </exception>
	public async Task ResetAsync(IRevisionPointer pointer, ResetMode mode = ResetMode.Mixed)
	{
		Verify.Argument.IsValidRevisionPointer(pointer, Repository);

		var pos = await Pointer
			.DereferenceAsync()
			.ConfigureAwait(continueOnCapturedContext: false);

		var rev = await pointer
			.DereferenceAsync()
			.ConfigureAwait(continueOnCapturedContext: false);

		var currentBranch = Pointer as Branch;

		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.BranchChanged,
			RepositoryNotifications.Checkout,
			RepositoryNotifications.IndexUpdated,
			RepositoryNotifications.WorktreeUpdated,
			RepositoryNotifications.SubmodulesChanged))
		{
			await Repository.Accessor.Reset
				.InvokeAsync(new ResetParameters(rev.Hash.ToString(), mode))
				.ConfigureAwait(continueOnCapturedContext: false);
		}

		if(currentBranch is not null)
		{
			currentBranch.Pointer = rev;
		}
		else
		{
			Pointer = rev;
		}

		if(pos != rev)
		{
			NotifyReflogRecordAdded();
		}

		await Repository.Status
			.RefreshAsync()
			.ConfigureAwait(continueOnCapturedContext: false);
		await Repository.Submodules
			.RefreshAsync()
			.ConfigureAwait(continueOnCapturedContext: false);
		Repository.OnStateChanged();
	}

	/// <summary>Updates cached information.</summary>
	public void Refresh() => Pointer = GetHeadPointer(Repository);

	/// <summary>Updates cached information.</summary>
	public async Task RefreshAsync() => Pointer = await GetHeadPointerAsync(Repository)
		.ConfigureAwait(continueOnCapturedContext: false);

	#region merge

	public string FormatMergeMessage(IRevisionPointer revision)
	{
		Verify.Argument.IsValidRevisionPointer(revision, Repository);
		Verify.State.IsFalse(IsEmpty,
			Resources.ExcCantDoOnEmptyRepository.UseAsFormat("format merge message"));

		return Repository.Accessor.FormatMergeMessage.Invoke(
			new FormatMergeMessageParameters(revision.Pointer, Pointer.Pointer));
	}

	public string FormatMergeMessage(ICollection<IRevisionPointer> revisions)
	{
		Verify.Argument.IsValidRevisionPointerSequence(revisions, Repository);
		Verify.Argument.IsTrue(revisions.Count != 0, nameof(revisions),
			Resources.ExcCollectionMustContainAtLeastOneObject.UseAsFormat("revision"));
		Verify.State.IsFalse(IsEmpty,
			Resources.ExcCantDoOnEmptyRepository.UseAsFormat("format merge message"));

		var names = new List<string>(revisions.Count);
		foreach(var branch in revisions)
		{
			names.Add(branch.Pointer);
		}
		return Repository.Accessor.FormatMergeMessage.Invoke(
			new FormatMergeMessageParameters(names, Pointer.Pointer));
	}

	private string SaveMessageForMerge(string message)
	{
		var fileName = Path.Combine(
			Repository.GitDirectory,
			GitConstants.CommitMessageFileName);
		File.WriteAllText(fileName, message);
		return fileName;
	}

	private static void DeleteMessageAfterMerge(string fileName)
	{
		try
		{
			File.Delete(fileName);
		}
		catch(Exception exc) when(!exc.IsCritical())
		{
		}
	}

	private Revision MergeCore(IReadOnlyList<string> revisions,
		bool    noCommit,
		bool    noFastForward,
		bool    squash,
		string? message)
	{
		var currentBranch = CurrentBranch;

		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.Checkout,
			RepositoryNotifications.IndexUpdated,
			RepositoryNotifications.WorktreeUpdated,
			RepositoryNotifications.BranchChanged))
		{
			var fileName = message is not null
				? SaveMessageForMerge(message)
				: default;

			MergeParameters parameters;

			parameters = new MergeParameters()
			{
				Revisions       = revisions,
				NoCommit        = noCommit,
				NoFastForward   = noFastForward,
				Squash          = squash,
				MessageFileName = fileName,
			};

			try
			{
				Repository.Accessor.Merge.Invoke(parameters);
			}
			catch(AutomaticMergeFailedException)
			{
				Repository.OnStateChanged();
				Repository.Status.Refresh();
				throw;
			}

			if(fileName is not null)
			{
				DeleteMessageAfterMerge(fileName);
			}
		}

		if(currentBranch is not null)
		{
			currentBranch.Refresh();
		}
		else
		{
			Refresh();
		}

		return Revision;
	}

	public Revision Merge(IRevisionPointer branch,
		bool    noCommit      = false,
		bool    noFastForward = false,
		bool    squash        = false,
		string? message       = default)
	{
		Verify.Argument.IsValidRevisionPointer(branch, Repository);
		Verify.State.IsFalse(IsEmpty,
			Resources.ExcCantDoOnEmptyRepository.UseAsFormat("merge"));

		var oldRev = branch.Dereference();
		
		var headRev = MergeCore(new[] { branch.FullName }, noCommit, noFastForward, squash, message); ;

		if(noCommit)
		{
			Repository.OnStateChanged();
			Repository.Status.Refresh();
		}
		else
		{
			if(noFastForward || headRev != oldRev) //not fast-forwarded
			{
				Repository.OnCommitCreated(headRev);
			}
		}
		NotifyReflogRecordAdded();
		return headRev;
	}

	public Revision Merge(ICollection<IRevisionPointer> branches,
		bool    noCommit      = false,
		bool    noFastForward = false,
		bool    squash        = false,
		string? message       = default)
	{
		Verify.Argument.IsValidRevisionPointerSequence(branches, Repository);
		Verify.Argument.IsTrue(branches.Count > 0, nameof(branches),
			Resources.ExcCollectionMustContainAtLeastOneObject.UseAsFormat("branch"));
		Verify.State.IsFalse(IsEmpty,
			Resources.ExcCantDoOnEmptyRepository.UseAsFormat("merge"));

		if(branches.Count == 1)
		{
			foreach(var branch in branches)
			{
				return Merge(branch, noCommit, noFastForward, squash, message);
			}
		}
		var oldRevs = new List<Revision>(branches.Count);
		var branchNames = new List<string>(branches.Count);
		foreach(var branch in branches)
		{
			oldRevs.Add(branch.Dereference());
			branchNames.Add(branch.FullName);
		}

		var headRev = MergeCore(branchNames, noCommit, noFastForward, squash, message);

		if(noCommit)
		{
			Repository.OnStateChanged();
			Repository.Status.Refresh();
		}
		else
		{
			if(noFastForward || !oldRevs.Contains(headRev)) //not fast-forwarded
			{
				Repository.OnCommitCreated(headRev);
			}
		}
		NotifyReflogRecordAdded();
		return headRev;
	}

	#endregion
}
