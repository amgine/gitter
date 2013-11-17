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

namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;
	using gitter.Framework.CLI;

	using gitter.Git.AccessLayer.CLI;

	using Resources = gitter.Git.AccessLayer.CLI.Properties.Resources;

	/// <summary>Accesses repository through git command line interface.</summary>
	internal sealed partial class RepositoryCLI : IRepositoryAccessor
	{
		#region Data

		private readonly GitCLI _gitCLI;
		private readonly IGitRepository _repository;
		private readonly ICommandExecutor _executor;

		#endregion

		#region .ctor

		public RepositoryCLI(GitCLI gitCLI, IGitRepository repository)
		{
			Verify.Argument.IsNotNull(gitCLI, "gitCLI");
			Verify.Argument.IsNotNull(repository, "repository");

			_gitCLI = gitCLI;
			_repository = repository;
			_executor = new RepositoryCommandExecutor(
				gitCLI, repository.WorkingDirectory);
		}

		#endregion

		#region Properties

		/// <summary>Returns git accessor.</summary>
		/// <value>git accessor.</value>
		public IGitAccessor GitAccessor
		{
			get { return _gitCLI; }
		}

		private CommandBuilder CommandBuilder
		{
			get { return _gitCLI.CommandBuilder; }
		}

		private ICommandExecutor CommandExecutor
		{
			get { return _executor; }
		}

		private OutputParser OutputParser
		{
			get { return _gitCLI.OutputParser; }
		}

		#endregion

		#region Helpers

		private void DefaultExecute(Command command)
		{
			Assert.IsNotNull(command);

			var output = CommandExecutor.ExecuteCommand(command);
			output.ThrowOnBadReturnCode();
		}

		private void DefaultExecute(Command command, Action<GitOutput> resultHandler)
		{
			Assert.IsNotNull(command);
			Assert.IsNotNull(resultHandler);

			var output = CommandExecutor.ExecuteCommand(command);
			resultHandler(output);
		}

		private T DefaultExecute<T>(Command command, Func<GitOutput, T> resultParser)
		{
			Assert.IsNotNull(command);
			Assert.IsNotNull(resultParser);

			var output = CommandExecutor.ExecuteCommand(command);
			return resultParser(output);
		}

		private Task DefaultExecuteAsync(Command command, CancellationToken cancellationToken)
		{
			Assert.IsNotNull(command);

			return CommandExecutor
				.ExecuteCommandAsync(command, cancellationToken)
				.ContinueWith(
				t =>
				{
					var output = TaskUtility.UnwrapResult(t);
					output.ThrowOnBadReturnCode();
				},
				cancellationToken,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		private Task DefaultExecuteAsync(Command command, Action<GitOutput> resultHandler, CancellationToken cancellationToken)
		{
			Assert.IsNotNull(command);
			Assert.IsNotNull(resultHandler);

			return CommandExecutor
				.ExecuteCommandAsync(command, cancellationToken)
				.ContinueWith(
				t =>
				{
					var output = TaskUtility.UnwrapResult(t);
					resultHandler(output);
				},
				cancellationToken,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		private Task<T> DefaultExecuteAsync<T>(Command command, Func<GitOutput, T> resultParser, CancellationToken cancellationToken)
		{
			Assert.IsNotNull(command);
			Assert.IsNotNull(resultParser);

			return CommandExecutor
				.ExecuteCommandAsync(command, cancellationToken)
				.ContinueWith(
				t =>
				{
					var output = TaskUtility.UnwrapResult(t);
					var result = resultParser(output);
					return result;
				},
				cancellationToken,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		private Task FetchOrPullAsync(Command command, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Assert.IsNotNull(command);

			if(progress != null)
			{
				progress.Report(new OperationProgress(Resources.StrsConnectingToRemoteHost.AddEllipsis()));
			}
			List<string> errorMessages = null;
			var stdOutReceiver = new NullReader();
			var stdErrReceiver = new NotifyingAsyncTextReader();
			stdErrReceiver.TextLineReceived += (s, e) =>
			{
				if(!string.IsNullOrWhiteSpace(e.Text))
				{
					var parser = new GitParser(e.Text);
					var operationProgress = parser.ParseProgress();
					if(progress != null)
					{
						progress.Report(operationProgress);
					}
					if(operationProgress.IsIndeterminate)
					{
						if(errorMessages == null)
						{
							errorMessages = new List<string>();
						}
						errorMessages.Add(operationProgress.ActionName);
					}
					else
					{
						if(errorMessages != null)
						{
							errorMessages.Clear();
						}
					}
				}
			};
			return CommandExecutor
				.ExecuteCommandAsync(command, stdOutReceiver, stdErrReceiver, cancellationToken)
				.ContinueWith(task =>
				{
					int exitCode = TaskUtility.UnwrapResult(task);
					if(exitCode != 0)
					{
						string errorMessage;
						if(errorMessages != null && errorMessages.Count != 0)
						{
							errorMessage = string.Join(Environment.NewLine, errorMessages);
						}
						else
						{
							errorMessage = string.Format(CultureInfo.InvariantCulture, "git process exited with code {0}", exitCode);
						}
						throw new GitException(errorMessage);
					}
				},
				cancellationToken,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		#endregion

		const string refPrefix = "ref: ";

		#region AddRemote

		/// <summary>Add remote repository.</summary>
		/// <param name="parameters"><see cref="AddRemoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void AddRemote(AddRemoteParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(CommandBuilder.GetAddRemoteCommand(parameters));
		}

		/// <summary>Add remote repository.</summary>
		/// <param name="parameters"><see cref="AddRemoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task AddRemoteAsync(AddRemoteParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetAddRemoteCommand(parameters),
				cancellationToken);
		}

		#endregion

		#region AddSubmodule

		/// <summary>Adds new submodule.</summary>
		/// <param name="parameters"><see cref="AddSubmoduleParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void AddSubmodule(AddSubmoduleParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(CommandBuilder.GetAddSubmoduleCommand(parameters));
		}

		/// <summary>Adds new submodule.</summary>
		/// <param name="parameters"><see cref="AddSubmoduleParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task AddSubmoduleAsync(AddSubmoduleParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetAddSubmoduleCommand(parameters),
				cancellationToken);
		}

		#endregion

		#region AddFiles

		/// <summary>Add file to index.</summary>
		/// <param name="parameters"><see cref="AddFilesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void AddFiles(AddFilesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(CommandBuilder.GetAddFilesCommand(parameters));
		}

		/// <summary>Add file to index.</summary>
		/// <param name="parameters"><see cref="AddFilesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task AddFilesAsync(AddFilesParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetAddFilesCommand(parameters),
				cancellationToken);
		}

		#endregion

		#region AppendNote

		/// <summary>Append new note to object.</summary>
		/// <param name="parameters"><see cref="AppendNoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void AppendNote(AppendNoteParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(CommandBuilder.GetAppendNoteCommand(parameters));
		}

		/// <summary>Append new note to object.</summary>
		/// <param name="parameters"><see cref="AppendNoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task AppendNoteAsync(AppendNoteParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetAppendNoteCommand(parameters),
				cancellationToken);
		}

		#endregion

		#region ApplyPatch

		/// <summary>Apply patches to working directory and/or index.</summary>
		/// <param name="parameters"><see cref="ApplyPatchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void ApplyPatch(ApplyPatchParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(CommandBuilder.GetApplyPatchCommand(parameters));
		}

		/// <summary>Apply patches to working directory and/or index.</summary>
		/// <param name="parameters"><see cref="ApplyPatchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task ApplyPatchAsync(ApplyPatchParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetApplyPatchCommand(parameters),
				cancellationToken);
		}

		#endregion

		#region Archive

		/// <summary>Create an archive of files from a named tree.</summary>
		/// <param name="parameters"><see cref="ArchiveParameters"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void Archive(ArchiveParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");
			Verify.Argument.IsNeitherNullNorEmpty(parameters.Tree, "parameters.Tree");

			DefaultExecute(CommandBuilder.GetArchiveCommand(parameters));
		}

		/// <summary>Create an archive of files from a named tree.</summary>
		/// <param name="parameters"><see cref="ArchiveParameters"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task ArchiveAsync(ArchiveParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");
			Verify.Argument.IsNeitherNullNorEmpty(parameters.Tree, "parameters.Tree");

			return DefaultExecuteAsync(
				CommandBuilder.GetArchiveCommand(parameters),
				cancellationToken);
		}

		#endregion

		#region CleanFiles

		/// <summary>Remove untracked files from the working tree.</summary>
		/// <param name="parameters"><see cref="CleanFilesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void CleanFiles(CleanFilesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(CommandBuilder.GetCleanFilesCommand(parameters));
		}

		/// <summary>Remove untracked files from the working tree.</summary>
		/// <param name="parameters"><see cref="CleanFilesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task CleanFilesAsync(CleanFilesParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetCleanFilesCommand(parameters),
				cancellationToken);
		}

		#endregion

		#region Checkout

		/// <summary>Checkout branch/revision.</summary>
		/// <param name="parameters"><see cref="CheckoutParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void Checkout(CheckoutParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(
				CommandBuilder.GetCheckoutCommand(parameters),
				output => OutputParser.HandleCheckoutResult(parameters, output));
		}

		/// <summary>Checkout branch/revision.</summary>
		/// <param name="parameters"><see cref="CheckoutParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task CheckoutAsync(CheckoutParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetCheckoutCommand(parameters),
				output => OutputParser.HandleCheckoutResult(parameters, output),
				cancellationToken);
		}

		#endregion

		#region CheckoutFiles

		/// <summary>Checkout files from tree object to working directory.</summary>
		/// <param name="parameters"><see cref="CheckoutFilesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void CheckoutFiles(CheckoutFilesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(CommandBuilder.GetCheckoutFilesCommand(parameters));
		}

		/// <summary>Checkout files from tree object to working directory.</summary>
		/// <param name="parameters"><see cref="CheckoutFilesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task CheckoutFilesAsync(CheckoutFilesParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetCheckoutFilesCommand(parameters),
				cancellationToken);
		}

		#endregion

		#region CherryPick

		/// <summary>Performs a cherry-pick operation.</summary>
		/// <param name="parameters"><see cref="CherryPickParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		/// <exception cref="T:gitter.Git.HaveConflictsException">Conflicted files are present, cherry-pick is not possible.</exception>
		/// <exception cref="T:gitter.Git.CommitIsMergeException">Specified revision is a merge, cherry-pick cannot be performed on merges.</exception>
		/// <exception cref="T:gitter.Git.HaveLocalChangesException">Dirty working directory, cannot cherry-pick.</exception>
		/// <exception cref="T:gitter.Git.CherryPickIsEmptyException">Resulting cherry-pick is empty.</exception>
		/// <exception cref="T:gitter.Git.AutomaticCherryPickFailedException">Cherry-pick was not finished because of conflicts.</exception>
		public void CherryPick(CherryPickParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(
				CommandBuilder.GetCherryPickCommand(parameters),
				output => OutputParser.HandleCherryPickResult(output));
		}

		/// <summary>Performs a cherry-pick operation.</summary>
		/// <param name="parameters"><see cref="CherryPickParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		/// <exception cref="T:gitter.Git.HaveConflictsException">Conflicted files are present, cherry-pick is not possible.</exception>
		/// <exception cref="T:gitter.Git.CommitIsMergeException">Specified revision is a merge, cherry-pick cannot be performed on merges.</exception>
		/// <exception cref="T:gitter.Git.HaveLocalChangesException">Dirty working directory, cannot cherry-pick.</exception>
		/// <exception cref="T:gitter.Git.CherryPickIsEmptyException">Resulting cherry-pick is empty.</exception>
		/// <exception cref="T:gitter.Git.AutomaticCherryPickFailedException">Cherry-pick was not finished because of conflicts.</exception>
		public Task CherryPickAsync(CherryPickParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetCherryPickCommand(parameters),
				output => OutputParser.HandleCherryPickResult(output),
				cancellationToken);
		}

		/// <summary>Performs a cherry-pick operation.</summary>
		/// <param name="control">Sequencer command to execute.</param>
		public void CherryPick(CherryPickControl control)
		{
			DefaultExecute(CommandBuilder.GetCherryPickCommand(control));
		}

		/// <summary>Performs a cherry-pick operation.</summary>
		/// <param name="control">Sequencer command to execute.</param>
		public Task CherryPickAsync(CherryPickControl control,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			return DefaultExecuteAsync(
				CommandBuilder.GetCherryPickCommand(control),
				cancellationToken);
		}

		#endregion

		#region Commit

		/// <summary>Commit changes.</summary>
		/// <param name="parameters"><see cref="CommitParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void Commit(CommitParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(CommandBuilder.GetCommitCommand(parameters));
		}

		/// <summary>Commit changes.</summary>
		/// <param name="parameters"><see cref="CommitParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task CommitAsync(CommitParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetCommitCommand(parameters),
				cancellationToken);
		}

		#endregion

		#region CountObjects

		public ObjectCountData CountObjects()
		{
			return DefaultExecute(
				CommandBuilder.GetCountObjectsCommand(),
				output => OutputParser.ParseObjectCountData(output));
		}

		public Task<ObjectCountData> CountObjectsAsync(
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			return DefaultExecuteAsync(
				CommandBuilder.GetCountObjectsCommand(),
				output => OutputParser.ParseObjectCountData(output),
				cancellationToken);
		}

		#endregion

		#region CreateBranch

		/// <summary>Create local branch.</summary>
		/// <param name="parameters"><see cref="CreateBranchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void CreateBranch(CreateBranchParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(
				CommandBuilder.GetCreateBranchCommand(parameters),
				output => OutputParser.HandleCreateBranchResult(parameters, output));
		}

		/// <summary>Create local branch.</summary>
		/// <param name="parameters"><see cref="CreateBranchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task CreateBranchAsync(CreateBranchParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetCreateBranchCommand(parameters),
				output => OutputParser.HandleCreateBranchResult(parameters, output),
				cancellationToken);
		}

		#endregion

		#region CreateTag

		/// <summary>Create new tag object.</summary>
		/// <param name="parameters"><see cref="CreateTagParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void CreateTag(CreateTagParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(
				CommandBuilder.GetCreateTagCommand(parameters),
				output => OutputParser.HandleCreateTagResult(parameters, output));
		}

		/// <summary>Create new tag object.</summary>
		/// <param name="parameters"><see cref="CreateTagParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task CreateTagAsync(CreateTagParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetCreateTagCommand(parameters),
				output => OutputParser.HandleCreateTagResult(parameters, output),
				cancellationToken);
		}

		#endregion

		#region DeleteBranch

		/// <summary>Remove branch <paramref name="branchName"/>.</summary>
		/// <param name="parameters"><see cref="DeleteBranchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void DeleteBranch(DeleteBranchParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(
				CommandBuilder.GetDeleteBranchCommand(parameters),
				output => OutputParser.HandleDeleteBranchResult(parameters, output));
			;
		}

		/// <summary>Remove branch <paramref name="branchName"/>.</summary>
		/// <param name="parameters"><see cref="DeleteBranchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task DeleteBranchAsync(DeleteBranchParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetDeleteBranchCommand(parameters),
				output => OutputParser.HandleDeleteBranchResult(parameters, output),
				cancellationToken);
		}

		#endregion

		#region DeleteTag

		/// <summary>Delete tag.</summary>
		/// <param name="parameters"><see cref="DeleteTagParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void DeleteTag(DeleteTagParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(
				CommandBuilder.GetDeleteTagCommand(parameters),
				output => OutputParser.HandleDeleteTagResult(parameters, output));
		}

		/// <summary>Delete tag.</summary>
		/// <param name="parameters"><see cref="DeleteTagParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task DeleteTagAsync(DeleteTagParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetDeleteTagCommand(parameters),
				output => OutputParser.HandleDeleteTagResult(parameters, output),
				cancellationToken);
		}

		#endregion

		#region Dereference

		/// <summary>Dereference valid ref.</summary>
		/// <param name="parameters"><see cref="DereferenceParameters"/>.</param>
		/// <returns>Corresponding <see cref="RevisionData"/>.</returns>
		public RevisionData Dereference(DereferenceParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecute(
				CommandBuilder.GetDereferenceCommand(parameters),
				output => OutputParser.ParseDereferenceOutput(parameters, output));
		}

		/// <summary>Dereference valid ref.</summary>
		/// <param name="parameters"><see cref="DereferenceParameters"/>.</param>
		/// <returns>Corresponding <see cref="RevisionData"/>.</returns>
		public Task<RevisionData> DereferenceAsync(DereferenceParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetDereferenceCommand(parameters),
				output => OutputParser.ParseDereferenceOutput(parameters, output),
				cancellationToken);
		}

		#endregion

		#region Describe

		public string Describe(DescribeParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecute(
				CommandBuilder.GetDescribeCommand(parameters),
				output => OutputParser.ParseDescribeResult(parameters, output));
		}

		public Task<string> DescribeAsync(DescribeParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetDescribeCommand(parameters),
				output => OutputParser.ParseDescribeResult(parameters, output),
				cancellationToken);
		}

		#endregion

		#region Fetch

		/// <summary>Download objects and refs from another repository.</summary>
		/// <param name="parameters"><see cref="FetchParameters"/>.</param>
		/// <returns>true if any objects were downloaded.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void Fetch(FetchParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(CommandBuilder.GetFetchCommand(parameters, false));
		}

		/// <summary>Download objects and refs from another repository.</summary>
		/// <param name="parameters"><see cref="FetchParameters"/>.</param>
		/// <returns>true if any objects were downloaded.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task FetchAsync(FetchParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = CommandBuilder.GetFetchCommand(parameters, true);
			return FetchOrPullAsync(command, progress, cancellationToken);
		}

		#endregion

		#region FormatMergeMessage

		private const string changeLogFormat = "  * %s\r";

		public string FormatMergeMessage(FormatMergeMessageParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			if(parameters.Revisions.Count == 1)
			{
				var rev = parameters.Revisions[0];
				var commits = GetMergedCommits(rev, parameters.HeadReference, changeLogFormat);
				var msg = string.Format("Merge branch '{0}' into {1}\r\n\r\n", rev, parameters.HeadReference) + "Changes:\r\n" + commits;
				return msg;
			}
			else
			{
				var sb = new StringBuilder();
				sb.Append("Merge branches ");
				for(int i = 0; i < parameters.Revisions.Count; ++i)
				{
					sb.Append('\'');
					sb.Append(parameters.Revisions[i]);
					sb.Append('\'');
					if(i != parameters.Revisions.Count - 1)
					{
						sb.Append(", ");
					}
				}
				sb.Append(" into ");
				sb.Append(parameters.HeadReference);
				sb.Append("\r\n");
				for(int i = 0; i < parameters.Revisions.Count; ++i)
				{
					sb.Append("\r\nChanges from ");
					sb.Append(parameters.Revisions[i]);
					sb.Append(":\r\n");
					sb.Append(GetMergedCommits(parameters.Revisions[i], parameters.HeadReference, changeLogFormat));
				}
				return sb.ToString();
			}
		}

		private string GetMergedCommits(string branch, string head, string lineFormat)
		{
			var cmd = new LogCommand(
				LogCommand.TFormat(lineFormat),
				new CommandArgument(head + ".." + branch));
			var output = _executor.ExecuteCommand(cmd);
			output.ThrowOnBadReturnCode();
			return output.Output;
		}

		#endregion

		#region GarbageCollect

		/// <summary>Cleanup unnecessary files and optimize the local repository.</summary>
		/// <param name="parameters"><see cref="GarbageCollectParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void GarbageCollect(GarbageCollectParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(CommandBuilder.GetGarbageCollectCommand(parameters));
		}

		/// <summary>Cleanup unnecessary files and optimize the local repository.</summary>
		/// <param name="parameters"><see cref="GarbageCollectParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task GarbageCollectAsync(GarbageCollectParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetGarbageCollectCommand(parameters),
				cancellationToken);
		}

		#endregion

		#region GetHead

		/// <summary>Get HEAD of repository.</summary>
		/// <param name="refType">Type of the HEAD.</param>
		/// <returns>HEAD reference.</returns>
		public string GetHead(out ReferenceType refType)
		{
			var headFile = _repository.GetGitFileName(GitConstants.HEAD);
			if(File.Exists(headFile))
			{
				string head;
				using(var fs = new FileStream(headFile, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					if(fs.Length == 0)
					{
						refType = ReferenceType.None;
						return null;
					}
					else
					{
						using(var sr = new StreamReader(fs))
						{
							head = sr.ReadLine();
							sr.Close();
						}
					}
					fs.Close();
				}
				if(head.StartsWith(refPrefix + GitConstants.LocalBranchPrefix) && head.Length >= 17)
				{
					refType = ReferenceType.LocalBranch;
					return head.Substring(16);
				}
				else
				{
					if(GitUtils.IsValidSHA1(head))
					{
						refType = ReferenceType.Revision;
						return head;
					}
				}
			}
			refType = ReferenceType.None;
			return null;
		}

		#endregion

		#region Merge

		/// <summary>Merge development histories together.</summary>
		/// <param name="parameters"><see cref="MergeParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		/// <exception cref="T:gitter.Git.AutomaticMergeFailedException">Merge resulted in conflicts.</exception>
		public void Merge(MergeParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(
				CommandBuilder.GetMergeCommand(parameters),
				output => OutputParser.HandleMergeResult(output));
		}

		/// <summary>Merge development histories together.</summary>
		/// <param name="parameters"><see cref="MergeParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		/// <exception cref="T:gitter.Git.AutomaticMergeFailedException">Merge resulted in conflicts.</exception>
		public Task MergeAsync(MergeParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetMergeCommand(parameters),
				output => OutputParser.HandleMergeResult(output),
				cancellationToken);
		}

		#endregion

		#region PruneNotes

		/// <summary>Remove all notes for non-existing/unreachable objects.</summary>
		public void PruneNotes()
		{
			DefaultExecute(CommandBuilder.GetPruneNotesCommand());
		}

		/// <summary>Remove all notes for non-existing/unreachable objects.</summary>
		public Task PruneNotesAsync(
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			return DefaultExecuteAsync(
				CommandBuilder.GetPruneNotesCommand(),
				cancellationToken);
		}

		#endregion

		#region PruneRemote

		/// <summary>Remove stale remote tracking branches.</summary>
		/// <param name="parameters"><see cref="RemoveRemoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void PruneRemote(PruneRemoteParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(CommandBuilder.GetPruneRemoteCommand(parameters));
		}

		/// <summary>Remove stale remote tracking branches.</summary>
		/// <param name="parameters"><see cref="RemoveRemoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task PruneRemoteAsync(PruneRemoteParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetPruneRemoteCommand(parameters),
				cancellationToken);
		}

		#endregion

		#region Pull

		/// <summary>Download objects and refs from another repository and merge with local branches configured for this.</summary>
		/// <param name="parameters"><see cref="PullParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void Pull(PullParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(CommandBuilder.GetPullCommand(parameters, false));
		}

		/// <summary>Download objects and refs from another repository and merge with local branches configured for this.</summary>
		/// <param name="parameters"><see cref="PullParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task PullAsync(PullParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = CommandBuilder.GetPullCommand(parameters, true);
			return FetchOrPullAsync(command, progress, cancellationToken);
		}

		#endregion

		#region Push

		/// <summary>Update remote refs along with associated objects.</summary>
		/// <param name="parameters"><see cref="PushParameters"/>.</param>
		/// <returns>List of pushed references.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<ReferencePushResult> Push(PushParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = CommandBuilder.GetPushCommand(parameters, false);
			var output  = CommandExecutor.ExecuteCommand(command);
			output.ThrowOnBadReturnCode();
			return OutputParser.ParsePushResults(output.Output);
		}

		/// <summary>Update remote refs along with associated objects.</summary>
		/// <param name="parameters"><see cref="PushParameters"/>.</param>
		/// <returns>List of pushed references.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task<IList<ReferencePushResult>> PushAsync(PushParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = CommandBuilder.GetPushCommand(parameters, true);

			if(progress != null)
			{
				progress.Report(new OperationProgress(Resources.StrsConnectingToRemoteHost.AddEllipsis()));
			}
			List<string> errorMessages = null;
			var stdOutReceiver = new AsyncTextReader();
			var stdErrReceiver = new NotifyingAsyncTextReader();
			stdErrReceiver.TextLineReceived += (s, e) =>
			{
				if(!string.IsNullOrWhiteSpace(e.Text))
				{
					var parser = new GitParser(e.Text);
					var operationProgress = parser.ParseProgress();
					if(progress != null)
					{
						progress.Report(operationProgress);
					}
					if(operationProgress.IsIndeterminate)
					{
						if(errorMessages == null)
						{
							errorMessages = new List<string>();
						}
						errorMessages.Add(operationProgress.ActionName);
					}
					else
					{
						if(errorMessages != null)
						{
							errorMessages.Clear();
						}
					}
				}
			};
			return CommandExecutor
				.ExecuteCommandAsync(command, stdOutReceiver, stdErrReceiver, cancellationToken)
				.ContinueWith(task =>
				{
					int exitCode = TaskUtility.UnwrapResult(task);
					if(exitCode != 0)
					{
						string errorMessage;
						if(errorMessages != null && errorMessages.Count != 0)
						{
							errorMessage = string.Join(Environment.NewLine, errorMessages);
						}
						else
						{
							errorMessage = string.Format(CultureInfo.InvariantCulture, "git process exited with code {0}", exitCode);
						}
						throw new GitException(errorMessage);
					}
					else
					{
						return OutputParser.ParsePushResults(stdOutReceiver.GetText());
					}
				},
				cancellationToken,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		#endregion

		#region QueryBlame

		/// <summary>Get <see cref="BlameFile"/>, annotating each line of file with commit information.</summary>
		/// <param name="parameters"><see cref="QueryBlameParameters"/>.</param>
		/// <returns><see cref="BlameFile"/>, annotating each line of file with commit information.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public BlameFile QueryBlame(QueryBlameParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecute(
				CommandBuilder.GetQueryBlameCommand(parameters),
				output => OutputParser.ParseBlame(parameters, output));
		}

		/// <summary>Get <see cref="BlameFile"/>, annotating each line of file with commit information.</summary>
		/// <param name="parameters"><see cref="QueryBlameParameters"/>.</param>
		/// <returns><see cref="BlameFile"/>, annotating each line of file with commit information.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task<BlameFile> QueryBlameAsync(QueryBlameParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetQueryBlameCommand(parameters),
				output => OutputParser.ParseBlame(parameters, output),
				cancellationToken);
		}

		#endregion

		#region QueryBranch

		/// <summary>Check if branch exists and get its position.</summary>
		/// <param name="parameters"><see cref="QueryBranchParameters"/>.</param>
		/// <returns><see cref="BranchData"/> or null, if requested branch doesn't exist.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public BranchData QueryBranch(QueryBranchParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecute(
				CommandBuilder.GetQueryBranchCommand(parameters),
				output => OutputParser.ParseSingleBranch(parameters, output));
		}

		/// <summary>Check if branch exists and get its position.</summary>
		/// <param name="parameters"><see cref="QueryBranchParameters"/>.</param>
		/// <returns><see cref="BranchData"/> or null, if requested branch doesn't exist.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task<BranchData> QueryBranchAsync(QueryBranchParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetQueryBranchCommand(parameters),
				output => OutputParser.ParseSingleBranch(parameters, output),
				cancellationToken);
		}

		#endregion

		#region QueryBranches

		/// <summary>Query branch list.</summary>
		/// <param name="parameters"><see cref="QueryBranchesParameters"/>.</param>
		/// <returns>List of requested branches.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public BranchesData QueryBranches(QueryBranchesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecute(
				CommandBuilder.GetQueryBranchesCommand(parameters),
				output => OutputParser.ParseBranches(parameters, output));
		}

		/// <summary>Query branch list.</summary>
		/// <param name="parameters"><see cref="QueryBranchesParameters"/>.</param>
		/// <returns>List of requested branches.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task<BranchesData> QueryBranchesAsync(QueryBranchesParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetQueryBranchesCommand(parameters),
				output => OutputParser.ParseBranches(parameters, output),
				cancellationToken);
		}

		#endregion

		#region QueryRevisions

		/// <summary>Get revision list.</summary>
		/// <param name="parameters"><see cref="QueryRevisionsParameters"/>.</param>
		/// <returns>List of revisions.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<RevisionData> QueryRevisions(QueryRevisionsParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecute(
				CommandBuilder.GetQueryRevisionsCommand(parameters),
				output => OutputParser.ParseRevisions(output));
		}

		/// <summary>Get revision list.</summary>
		/// <param name="parameters"><see cref="QueryRevisionsParameters"/>.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>List of revisions.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task<IList<RevisionData>> QueryRevisionsAsync(QueryRevisionsParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetQueryRevisionsCommand(parameters),
				output => OutputParser.ParseRevisions(output),
				cancellationToken);
		}

		#endregion

		#region QueryRevision

		/// <summary>Get revision information.</summary>
		/// <param name="parameters"><see cref="QueryRevisionParameters"/>.</param>
		/// <returns>Revision data.</returns>
		public RevisionData QueryRevision(QueryRevisionParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");
			Verify.Argument.IsTrue(GitUtils.IsValidSHA1(parameters.SHA1),
				"parameters.SHA1", "Provided expression is not a valid SHA-1.");

			return DefaultExecute(
				CommandBuilder.GetQueryRevisionCommand(parameters),
				output => OutputParser.ParseSingleRevision(parameters, output));
		}

		/// <summary>Get revision information.</summary>
		/// <param name="parameters"><see cref="QueryRevisionParameters"/>.</param>
		/// <returns>Revision data.</returns>
		public Task<RevisionData> QueryRevisionAsync(QueryRevisionParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");
			Verify.Argument.IsTrue(GitUtils.IsValidSHA1(parameters.SHA1),
				"parameters.SHA1", "Provided expression is not a valid SHA-1.");

			return DefaultExecuteAsync(
				CommandBuilder.GetQueryRevisionCommand(parameters),
				output => OutputParser.ParseSingleRevision(parameters, output),
				cancellationToken);
		}

		#endregion

		#region QueryRevisionGraph

		/// <summary>Get revision graph.</summary>
		/// <param name="parameters"><see cref="QueryRevisionsParameters"/>.</param>
		/// <returns>Revision graph.</returns>
		public IList<RevisionGraphData> QueryRevisionGraph(QueryRevisionsParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecute(
				CommandBuilder.GetQueryRevisionGraphCommand(parameters),
				output => OutputParser.ParseRevisionGraph(output));
		}

		/// <summary>Get revision graph.</summary>
		/// <param name="parameters"><see cref="QueryRevisionsParameters"/>.</param>
		/// <returns>Revision graph.</returns>
		public Task<IList<RevisionGraphData>> QueryRevisionGraphAsync(QueryRevisionsParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetQueryRevisionGraphCommand(parameters),
				output => OutputParser.ParseRevisionGraph(output),
				cancellationToken);
		}

		#endregion

		#region QueryDiff

		/// <summary>Get <see cref="Diff"/>, representing difference between specified objects.</summary>
		/// <param name="parameters"><see cref="QueryDiffParameters"/>.</param>
		/// <returns><see cref="Diff"/>, representing difference between requested objects.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Diff QueryDiff(QueryDiffParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecute(
				CommandBuilder.GetDiffCommand(parameters),
				output => OutputParser.ParseDiff(parameters, output));
		}

		/// <summary>Get <see cref="Diff"/>, representing difference between specified objects.</summary>
		/// <param name="parameters"><see cref="QueryDiffParameters"/>.</param>
		/// <returns><see cref="Diff"/>, representing difference between requested objects.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task<Diff> QueryDiffAsync(QueryDiffParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetDiffCommand(parameters),
				output => OutputParser.ParseDiff(parameters, output),
				cancellationToken);
		}

		#endregion

		#region QueryNotes

		/// <summary>Get list of all note objects.</summary>
		/// <param name="parameters"><see cref="QueryNotesParameters"/>.</param>
		/// <returns>List of all note objects.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<NoteData> QueryNotes(QueryNotesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			if(!GitFeatures.AdvancedNotesCommands.IsAvailableFor(_gitCLI))
			{
				return new NoteData[0];
			}

			return DefaultExecute(
				CommandBuilder.GetQueryNotesCommand(parameters),
				output => OutputParser.ParseNotes(output));
		}

		#endregion

		#region QueryFilesToAdd

		/// <summary>Get the list of files that can be added.</summary>
		/// <param name="parameters"><see cref="AddFilesParameters"/>.</param>
		/// <returns>List of files which will be added by call to <see cref="AddFiles"/>(<paramref name="parameters"/>).</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<TreeFileData> QueryFilesToAdd(AddFilesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecute(
				CommandBuilder.GetQueryFilesToAddCommand(parameters),
				output => OutputParser.ParseFilesToAdd(output));
		}

		/// <summary>Get the list of files that can be added.</summary>
		/// <param name="parameters"><see cref="AddFilesParameters"/>.</param>
		/// <returns>List of files which will be added by call to <see cref="AddFiles"/>(<paramref name="parameters"/>).</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task<IList<TreeFileData>> QueryFilesToAddAsync(AddFilesParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetQueryFilesToAddCommand(parameters),
				output => OutputParser.ParseFilesToAdd(output),
				cancellationToken);
		}

		#endregion

		#region QueryFilesToRemove

		/// <summary>
		/// Get list of files which will be removed by a <see cref="IIndexAccessor.RemoveFiles"/>(<paramref name="parameters"/>) call.
		/// </summary>
		/// <param name="parameters"><see cref="RemoveFilesParameters"/>.</param>
		/// <returns>List of files which will be removed by a <see cref="IIndexAccessor.RemoveFiles"/>(<paramref name="parameters"/>) call.</returns>
		public IList<string> QueryFilesToRemove(RemoveFilesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecute(
				CommandBuilder.GetQueryFilesToRemoveCommand(parameters),
				output => OutputParser.ParseFilesToRemove(output));
		}

		/// <summary>
		/// Get list of files which will be removed by a <see cref="IIndexAccessor.RemoveFiles"/>(<paramref name="parameters"/>) call.
		/// </summary>
		/// <param name="parameters"><see cref="RemoveFilesParameters"/>.</param>
		/// <returns>List of files which will be removed by a <see cref="IIndexAccessor.RemoveFiles"/>(<paramref name="parameters"/>) call.</returns>
		public Task<IList<string>> QueryFilesToRemoveAsync(RemoveFilesParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetQueryFilesToRemoveCommand(parameters),
				output => OutputParser.ParseFilesToRemove(output),
				cancellationToken);
		}

		#endregion

		#region QueryFilesToClean

		/// <summary>
		/// Get list of files and directories which will be removed
		/// by <see cref="IIndexAccessor.CleanFiles"/>(<paramref name="parameters"/>) call.
		/// </summary>
		/// <param name="parameters"><see cref="CleanFilesParameters"/>.</param>
		/// <returns>List of files and directories which will be removed by <see cref="IIndexAccessor.CleanFiles"/>(<paramref name="parameters"/>) call.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<string> QueryFilesToClean(CleanFilesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecute(
				CommandBuilder.GetQueryFilesToCleanCommand(parameters),
				output => OutputParser.ParseFilesToClean(output));
		}

		/// <summary>
		/// Get list of files and directories which will be removed
		/// by <see cref="IIndexAccessor.CleanFiles"/>(<paramref name="parameters"/>) call.
		/// </summary>
		/// <param name="parameters"><see cref="CleanFilesParameters"/>.</param>
		/// <returns>List of files and directories which will be removed by <see cref="IIndexAccessor.CleanFiles"/>(<paramref name="parameters"/>) call.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task<IList<string>> QueryFilesToCleanAsync(CleanFilesParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetQueryFilesToCleanCommand(parameters),
				output => OutputParser.ParseFilesToClean(output),
				cancellationToken);
		}

		#endregion

		#region QuerySymbolicReference

		/// <summary>Get symbolic reference target.</summary>
		/// <param name="parameters"><see cref="QuerySymbolicReferenceParameters"/>.</param>
		/// <returns>Symbolic reference data.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public SymbolicReferenceData QuerySymbolicReference(QuerySymbolicReferenceParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var fileName = _repository.GetGitFileName(parameters.Name);
			if(File.Exists(fileName))
			{
				string pointer;
				using(var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					if(fs.Length == 0)
					{
						return new SymbolicReferenceData(null, ReferenceType.None);
					}
					else
					{
						using(var sr = new StreamReader(fs))
						{
							pointer = sr.ReadLine();
							sr.Close();
						}
					}
					fs.Close();
				}
				if(pointer.Length >= 17 && pointer.StartsWith(refPrefix + GitConstants.LocalBranchPrefix))
				{
					return new SymbolicReferenceData(pointer.Substring(16), ReferenceType.LocalBranch);
				}
				else
				{
					if(GitUtils.IsValidSHA1(pointer))
					{
						return new SymbolicReferenceData(pointer, ReferenceType.Revision);
					}
				}
			}
			return new SymbolicReferenceData(null, ReferenceType.None);
		}

		#endregion

		#region QueryObjects

		/// <summary>Get contents of requested objects.</summary>
		/// <param name="parameters"><see cref="parameters"/>.</param>
		/// <returns>Objects contents.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public string QueryObjects(QueryObjectsParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecute(
				CommandBuilder.GetQueryObjectsCommand(parameters),
				o => o.Output);
		}

		/// <summary>Get contents of requested objects.</summary>
		/// <param name="parameters"><see cref="parameters"/>.</param>
		/// <returns>Objects contents.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task<string> QueryObjectsAsync(QueryObjectsParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetQueryObjectsCommand(parameters),
				o => o.Output, cancellationToken);
		}

		#endregion

		#region QueryRemote

		/// <summary>Get information about remote.</summary>
		/// <param name="parameters"><see cref="QueryRemoteParameters"/>.</param>
		/// <returns>Requested remote.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public RemoteData QueryRemote(QueryRemoteParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecute(
				CommandBuilder.GetQueryRemoteCommand(parameters),
				output => OutputParser.ParseSingleRemote(parameters, output));
		}

		/// <summary>Get information about remote.</summary>
		/// <param name="parameters"><see cref="QueryRemoteParameters"/>.</param>
		/// <returns>Requested remote.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task<RemoteData> QueryRemoteAsync(QueryRemoteParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetQueryRemoteCommand(parameters),
				output => OutputParser.ParseSingleRemote(parameters, output),
				cancellationToken);
		}

		#endregion

		#region QueryRemotes

		/// <summary>Query list of remotes.</summary>
		/// <param name="parameters"><see cref="QueryRemotesParameters"/>.</param>
		/// <returns>List of remotes.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<RemoteData> QueryRemotes(QueryRemotesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecute(
				CommandBuilder.GetQueryRemotesCommand(parameters),
				output => OutputParser.ParseRemotesOutput(output));
		}

		/// <summary>Query list of remotes.</summary>
		/// <param name="parameters"><see cref="QueryRemotesParameters"/>.</param>
		/// <returns>List of remotes.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task<IList<RemoteData>> QueryRemotesAsync(QueryRemotesParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetQueryRemotesCommand(parameters),
				output => OutputParser.ParseRemotesOutput(output),
				cancellationToken);
		}

		#endregion

		#region QueryPrunedBranches

		/// <summary>Get list of stale remote tracking branches that are subject to pruninig.</summary>
		/// <param name="parameters"><see cref="PruneRemoteParameters"/>.</param>
		/// <returns>List of stale remote tracking branches that are subject to pruninig.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<string> QueryPrunedBranches(PruneRemoteParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecute(
				CommandBuilder.GetQueryPrunedBranchesCommand(parameters),
				output => OutputParser.ParsePrunedBranches(output));
		}

		/// <summary>Get list of stale remote tracking branches that are subject to pruninig.</summary>
		/// <param name="parameters"><see cref="PruneRemoteParameters"/>.</param>
		/// <returns>List of stale remote tracking branches that are subject to pruninig.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task<IList<string>> QueryPrunedBranchesAsync(PruneRemoteParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetQueryPrunedBranchesCommand(parameters),
				output => OutputParser.ParsePrunedBranches(output),
				cancellationToken);
		}

		#endregion

		#region QueryRemoteReferences

		/// <summary>Get list of references on remote repository.</summary>
		/// <param name="parameters"><see cref="QueryRemoteReferencesParameters"/>.</param>
		/// <returns>List of remote references.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<RemoteReferenceData> QueryRemoteReferences(QueryRemoteReferencesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecute(
				CommandBuilder.GetQueryRemoteReferencesCommand(parameters),
				output => OutputParser.ParseRemoteReferences(output));
		}

		/// <summary>Get list of references on remote repository.</summary>
		/// <param name="parameters"><see cref="QueryRemoteReferencesParameters"/>.</param>
		/// <returns>List of remote references.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task<IList<RemoteReferenceData>> QueryRemoteReferencesAsync(QueryRemoteReferencesParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetQueryRemoteReferencesCommand(parameters),
				output => OutputParser.ParseRemoteReferences(output),
				cancellationToken);
		}

		#endregion

		#region QueryReferences

		/// <summary>Get list of references.</summary>
		/// <param name="parameters"><see cref="QueryReferencesParameters"/>.</param>
		/// <returns>Lists of references.</returns>
		public ReferencesData QueryReferences(QueryReferencesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecute(
				CommandBuilder.GetQueryReferencesCommand(parameters),
				output => OutputParser.ParseReferences(parameters, output));
		}

		/// <summary>Get list of references.</summary>
		/// <param name="parameters"><see cref="QueryReferencesParameters"/>.</param>
		/// <returns>Lists of references.</returns>
		public Task<ReferencesData> QueryReferencesAsync(QueryReferencesParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetQueryReferencesCommand(parameters),
				output => OutputParser.ParseReferences(parameters, output),
				cancellationToken);
		}

		#endregion

		#region QueryReflog

		/// <summary>Get reflog.</summary>
		/// <param name="parameters"><see cref="QueryReflogParameters"/>.</param>
		/// <returns>List of reflog records.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<ReflogRecordData> QueryReflog(QueryReflogParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = CommandBuilder.GetQueryReflogCommand(parameters);
			var output = CommandExecutor.ExecuteCommand(command);
			output.ThrowOnBadReturnCode();

			var cache = new Dictionary<string, RevisionData>();
			var list = new List<ReflogRecordData>();
			if(output.Output.Length < 40) return new ReflogRecordData[0];
			var parser = new GitParser(output.Output);
			int index = 0;
			while(!parser.IsAtEndOfString)
			{
				var selector = parser.ReadLine();
				if(selector.Length == 0) break;
				var message = parser.ReadLine();
				var sha1 = parser.ReadString(40, 1);
				RevisionData rev;
				if(!cache.TryGetValue(sha1, out rev))
				{
					rev = new RevisionData(sha1);
					cache.Add(sha1, rev);
				}
				parser.ParseRevisionData(rev, cache);
				list.Add(new ReflogRecordData(index++, message, rev));
			}

			// get real commit parents
			var args = new List<CommandArgument>();
			args.Add(LogCommand.WalkReflogs());
			if(parameters.MaxCount != 0)
			{
				args.Add(LogCommand.MaxCount(parameters.MaxCount));
			}
			args.Add(LogCommand.NullTerminate());
			args.Add(LogCommand.FormatRaw());
			if(parameters.Reference != null)
			{
				args.Add(new CommandArgument(parameters.Reference));
			}
			command = new LogCommand(args);
			output = CommandExecutor.ExecuteCommand(command);
			output.ThrowOnBadReturnCode();
			parser = new GitParser(output.Output);
			parser.ParseCommitParentsFromRaw(list.Select(rrd => rrd.Revision), cache);

			return list;
		}

		#endregion

		#region QueryStashPatch

		/// <summary>Get patch representing stashed changes.</summary>
		/// <param name="parameters"><see cref="QueryRevisionDiffParameters"/>.</param>
		/// <returns>Patch, representing specified stashed changes.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public byte[] QueryStashPatch(QueryRevisionDiffParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			parameters.EnableTextConvFilters = false;
			var command = CommandBuilder.GetQueryStashDiffCommand(parameters);
			var stdInReceiver = new AsyncBytesReader();
			var stdErrReceiver = new AsyncTextReader();
			var exitCode = CommandExecutor.ExecuteCommand(command, stdInReceiver, stdErrReceiver);
			if(exitCode != 0)
			{
				throw new GitException(stdErrReceiver.GetText());
			}
			return stdInReceiver.GetBytes();
		}

		/// <summary>Get patch representing stashed changes.</summary>
		/// <param name="parameters"><see cref="QueryRevisionDiffParameters"/>.</param>
		/// <returns>Patch, representing specified stashed changes.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task<byte[]> QueryStashPatchAsync(QueryRevisionDiffParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			parameters.EnableTextConvFilters = false;
			var command = CommandBuilder.GetQueryStashDiffCommand(parameters);
			var stdInReceiver = new AsyncBytesReader();
			var stdErrReceiver = new AsyncTextReader();
			return CommandExecutor
				.ExecuteCommandAsync(command, stdInReceiver, stdErrReceiver, cancellationToken)
				.ContinueWith(
				t =>
				{
					var exitCode = TaskUtility.UnwrapResult(t);
					if(exitCode != 0)
					{
						throw new GitException(stdErrReceiver.GetText());
					}
					return stdInReceiver.GetBytes();
				},
				cancellationToken,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		#endregion

		#region QueryStashTop

		/// <summary>Query most recent stashed state.</summary>
		/// <param name="parameters"><see cref="QueryStashTopParameters"/>.</param>
		/// <returns>Most recent stashed state or null if stash is empty.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public RevisionData QueryStashTop(QueryStashTopParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			if(parameters.LoadCommitInfo)
			{
				var command = CommandBuilder.GetDereferenceByNameCommand(GitConstants.StashFullName);
				return DefaultExecute(command, o => new GitParser(o.Output).ParseRevision());
			}
			else
			{
				var command = new ShowRefCommand(
					ShowRefCommand.Verify(),
					new CommandArgument(GitConstants.StashFullName));

				var output = CommandExecutor.ExecuteCommand(command);
				if(output.ExitCode != 0 || output.Output.Length < 40)
				{
					return null;
				}

				var hash = output.Output.Substring(0, 40);
				return new RevisionData(hash);
			}
		}

		#endregion

		#region QueryStash

		/// <summary>Query all stashed states.</summary>
		/// <param name="parameters"><see cref="QueryStashParameters"/>.</param>
		/// <returns>List of all stashed states.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<StashedStateData> QueryStash(QueryStashParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = CommandBuilder.GetQueryStashCommand(parameters);
			var output = CommandExecutor.ExecuteCommand(command);
			output.ThrowOnBadReturnCode();

			var cache = new Dictionary<string, RevisionData>();
			int index = 0;
			var parser = new GitParser(output.Output);
			var res = new List<StashedStateData>();
			while(!parser.IsAtEndOfString)
			{
				var sha1 = parser.ReadString(40, 1);
				var rev = new RevisionData(sha1);
				parser.ParseRevisionData(rev, cache);
				var state = new StashedStateData(index, rev);
				res.Add(state);
				++index;
			}

			// get real commit parents
			command = new LogCommand(
				LogCommand.WalkReflogs(),
				LogCommand.NullTerminate(),
				LogCommand.FormatRaw(),
				new CommandArgument(GitConstants.StashFullName));
			output = CommandExecutor.ExecuteCommand(command);
			output.ThrowOnBadReturnCode();
			parser = new GitParser(output.Output);
			parser.ParseCommitParentsFromRaw(res.Select(ssd => ssd.Revision), cache);

			return res;
		}

		#endregion

		#region QueryStashDiff

		/// <summary>Get patch representing stashed changes.</summary>
		/// <param name="parameters"><see cref="QueryRevisionDiffParameters"/>.</param>
		/// <returns><see cref="Diff"/>, representing specified stashed changes.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Diff QueryStashDiff(QueryRevisionDiffParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecute(
				CommandBuilder.GetQueryStashDiffCommand(parameters),
				output => OutputParser.ParseRevisionDiff(output));
		}

		/// <summary>Get patch representing stashed changes.</summary>
		/// <param name="parameters"><see cref="QueryRevisionDiffParameters"/>.</param>
		/// <returns><see cref="Diff"/>, representing specified stashed changes.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task<Diff> QueryStashDiffAsync(QueryRevisionDiffParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetQueryStashDiffCommand(parameters),
				output => OutputParser.ParseRevisionDiff(output),
				cancellationToken);
		}

		#endregion

		#region QueryStatus

		/// <summary>Get working directory status information.</summary>
		/// <param name="parameters"><see cref="QueryStatusParameters"/>.</param>
		/// <returns>Working directory status information.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public StatusData QueryStatus(QueryStatusParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecute(
				CommandBuilder.GetQueryStatusCommand(parameters),
				output => OutputParser.ParseStatus(output));
		}

		/// <summary>Get working directory status information.</summary>
		/// <param name="parameters"><see cref="QueryStatusParameters"/>.</param>
		/// <returns>Working directory status information.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task<StatusData> QueryStatusAsync(QueryStatusParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetQueryStatusCommand(parameters),
				output => OutputParser.ParseStatus(output),
				cancellationToken);
		}

		#endregion

		#region QueryRevisionDiff

		/// <summary>Get <see cref="Diff"/>, representing changes made by specified commit.</summary>
		/// <param name="parameters"><see cref="QueryRevisionDiffParameters"/>.</param>
		/// <returns><see cref="Diff"/>, representing changes made by specified commit.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Diff QueryRevisionDiff(QueryRevisionDiffParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecute(
				CommandBuilder.GetQueryRevisionDiffCommand(parameters),
				output => OutputParser.ParseRevisionDiff(output));
		}

		/// <summary>Get <see cref="Diff"/>, representing changes made by specified commit.</summary>
		/// <param name="parameters"><see cref="QueryRevisionDiffParameters"/>.</param>
		/// <returns><see cref="Diff"/>, representing changes made by specified commit.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task<Diff> QueryRevisionDiffAsync(QueryRevisionDiffParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetQueryRevisionDiffCommand(parameters),
				output => OutputParser.ParseRevisionDiff(output),
				cancellationToken);
		}

		#endregion

		#region QueryBlobBytes

		/// <summary>Queries the BLOB bytes.</summary>
		/// <param name="parameters">The parameters.</param>
		/// <returns>Requested blob content.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public byte[] QueryBlobBytes(QueryBlobBytesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = CommandBuilder.GetQueryBlobBytesCommand(parameters);
			var stdOutReceiver = new AsyncBytesReader();
			var stdErrReceiver = new AsyncTextReader();
			var code = CommandExecutor.ExecuteCommand(command, stdOutReceiver, stdErrReceiver);
			if(code != 0)
			{
				var output = new GitOutput(string.Empty, stdErrReceiver.GetText(), code);
				output.ThrowOnBadReturnCode();
			}
			return stdOutReceiver.GetBytes();
		}

		/// <summary>Queries the BLOB bytes.</summary>
		/// <param name="parameters">The parameters.</param>
		/// <returns>Requested blob content.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task<byte[]> QueryBlobBytesAsync(QueryBlobBytesParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = CommandBuilder.GetQueryBlobBytesCommand(parameters);
			var stdOutReceiver = new AsyncBytesReader();
			var stdErrReceiver = new AsyncTextReader();
			return CommandExecutor
				.ExecuteCommandAsync(command, stdOutReceiver, stdErrReceiver, cancellationToken)
				.ContinueWith(
				t =>
				{
					var code = TaskUtility.UnwrapResult(t);
					if(code != 0)
					{
						var output = new GitOutput(string.Empty, stdErrReceiver.GetText(), code);
						output.ThrowOnBadReturnCode();
					}
					return stdOutReceiver.GetBytes();
				},
				cancellationToken,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		#endregion

		#region QueryTreeContent

		/// <summary>Get objects contained in a tree.</summary>
		/// <param name="parameters"><see cref="QueryTreeContentParameters"/>.</param>
		/// <returns><see cref="TreeData"/> representing requested tree.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<TreeContentData> QueryTreeContent(QueryTreeContentParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecute(
				CommandBuilder.GetQueryTreeContentCommand(parameters),
				output => OutputParser.ParseTreeContent(output));
		}

		/// <summary>Get objects contained in a tree.</summary>
		/// <param name="parameters"><see cref="QueryTreeContentParameters"/>.</param>
		/// <returns><see cref="TreeData"/> representing requested tree.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task<IList<TreeContentData>> QueryTreeContentAsync(QueryTreeContentParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetQueryTreeContentCommand(parameters),
				output => OutputParser.ParseTreeContent(output),
				cancellationToken);
		}

		#endregion

		#region QueryRevisionPatch

		/// <summary>Get patch representing changes made by specified commit.</summary>
		/// <param name="parameters"><see cref="QueryRevisionDiffParameters"/>.</param>
		/// <returns>Patch, representing changes made by specified commit.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public byte[] QueryRevisionPatch(QueryRevisionDiffParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			parameters.EnableTextConvFilters = false;
			var command = CommandBuilder.GetQueryRevisionDiffCommand(parameters);
			var stdInReceiver = new AsyncBytesReader();
			var stdErrReceiver = new AsyncTextReader();
			var exitCode = CommandExecutor.ExecuteCommand(command, stdInReceiver, stdErrReceiver);
			if(exitCode != 0)
			{
				throw new GitException(stdErrReceiver.GetText());
			}
			return stdInReceiver.GetBytes();
		}

		/// <summary>Get patch representing changes made by specified commit.</summary>
		/// <param name="parameters"><see cref="QueryRevisionDiffParameters"/>.</param>
		/// <returns>Patch, representing changes made by specified commit.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task<byte[]> QueryRevisionPatchAsync(QueryRevisionDiffParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			parameters.EnableTextConvFilters = false;
			var command = CommandBuilder.GetQueryRevisionDiffCommand(parameters);
			var stdInReceiver = new AsyncBytesReader();
			var stdErrReceiver = new AsyncTextReader();
			return CommandExecutor
				.ExecuteCommandAsync(command, stdInReceiver, stdErrReceiver, cancellationToken)
				.ContinueWith(
				t =>
				{
					var exitCode = TaskUtility.UnwrapResult(t);
					if(exitCode != 0)
					{
						throw new GitException(stdErrReceiver.GetText());
					}
					return stdInReceiver.GetBytes();
				},
				cancellationToken,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		#endregion

		#region QueryUsers

		/// <summary>Get user list.</summary>
		/// <param name="parameters"><see cref="QueryUsersParameters"/>.</param>
		/// <returns>List of committers and authors.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<UserData> QueryUsers(QueryUsersParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecute(
				CommandBuilder.GetQueryUsersCommand(parameters),
				output => OutputParser.ParseUsers(output));
		}

		/// <summary>Get user list.</summary>
		/// <param name="parameters"><see cref="QueryUsersParameters"/>.</param>
		/// <returns>List of committers and authors.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task<IList<UserData>> QueryUsersAsync(QueryUsersParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetQueryUsersCommand(parameters),
				output => OutputParser.ParseUsers(output),
				cancellationToken);
		}

		#endregion

		#region QueryTag

		/// <summary>Check if tag exists and get its position.</summary>
		/// <param name="parameters"><see cref="QueryTagParameters"/>.</param>
		/// <returns><see cref="TagData"/> or null, if requested tag doesn't exist.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public TagData QueryTag(QueryTagParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecute(
				CommandBuilder.GetQueryTagCommand(parameters),
				output => OutputParser.ParseTag(parameters, output));
		}

		/// <summary>Check if tag exists and get its position.</summary>
		/// <param name="parameters"><see cref="QueryTagParameters"/>.</param>
		/// <returns><see cref="TagData"/> or null, if requested tag doesn't exist.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task<TagData> QueryTagAsync(QueryTagParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetQueryTagCommand(parameters),
				output => OutputParser.ParseTag(parameters, output),
				cancellationToken);
		}

		#endregion

		#region QueryTags

		/// <summary>Query tag list.</summary>
		/// <param name="parameters"><see cref="QueryTagsParameters"/>.</param>
		/// <returns>List of requested tags.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<TagData> QueryTags(QueryTagsParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecute(
				CommandBuilder.GetQueryTagsCommand(parameters),
				output => OutputParser.ParseTags(output));
		}

		/// <summary>Query tag list.</summary>
		/// <param name="parameters"><see cref="QueryTagsParameters"/>.</param>
		/// <returns>List of requested tags.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task<IList<TagData>> QueryTagsAsync(QueryTagsParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetQueryTagsCommand(parameters),
				output => OutputParser.ParseTags(output),
				cancellationToken);
		}

		#endregion

		#region QueryTagMessage

		/// <summary>Query tag message.</summary>
		/// <param name="tag">Tag name or object hash.</param>
		/// <returns>Tag message.</returns>
		public string QueryTagMessage(string tag)
		{
			var command = CommandBuilder.GetQueryTagMessageCommand(tag);
			var output = _executor.ExecuteCommand(command);
			return ParseTagMessage(command, output);
		}

		/// <summary>Query tag message.</summary>
		/// <param name="tag">Tag name or object hash.</param>
		/// <returns>Tag message.</returns>
		public Task<string> QueryTagMessage(string tag,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			var command = CommandBuilder.GetQueryTagMessageCommand(tag);
			return DefaultExecuteAsync(
				command, output => ParseTagMessage(command, output), cancellationToken);
		}

		private string ParseTagMessage(Command command, GitOutput output)
		{
			Assert.IsNotNull(output);

			output.ThrowOnBadReturnCode();
			var parser = new GitParser(output.Output);
			while(!parser.IsAtEndOfLine)
			{
				parser.SkipLine();
			}
			parser.SkipLine();
			if(parser.RemainingSymbols > 1)
			{
				var message = parser.ReadStringUpTo(parser.Length - 1);
				const char c = '�';
				if(message.ContainsAnyOf(c))
				{
					output = CommandExecutor.ExecuteCommand(command, Encoding.Default);
					output.ThrowOnBadReturnCode();
					parser = new GitParser(output.Output);
					while(!parser.IsAtEndOfLine)
					{
						parser.SkipLine();
					}
					parser.SkipLine();
					if(parser.RemainingSymbols > 1)
					{
						message = parser.ReadStringUpTo(parser.Length - 1);
					}
					else
					{
						message = string.Empty;
					}
				}
				return message;
			}
			else
			{
				return string.Empty;
			}
		}

		#endregion

		#region RemoveFiles

		/// <summary>Remove file from index and/or working directory.</summary>
		/// <param name="parameters"><see cref="RemoveFilesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void RemoveFiles(RemoveFilesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(CommandBuilder.GetRemoveFilesCommand(parameters));
		}

		/// <summary>Remove file from index and/or working directory.</summary>
		/// <param name="parameters"><see cref="RemoveFilesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task RemoveFilesAsync(RemoveFilesParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetRemoveFilesCommand(parameters),
				cancellationToken);
		}

		#endregion

		#region RemoveRemoteReferences

		/// <summary>Remove reference on remote repository.</summary>
		/// <param name="parameters"><see cref="RemoveRemoteReferencesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void RemoveRemoteReferences(RemoveRemoteReferencesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(CommandBuilder.GetRemoveRemoteReferencesCommand(parameters));
		}

		/// <summary>Remove reference on remote repository.</summary>
		/// <param name="parameters"><see cref="RemoveRemoteReferencesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task RemoveRemoteReferencesAsync(RemoveRemoteReferencesParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetRemoveRemoteReferencesCommand(parameters),
				cancellationToken);
		}

		#endregion

		#region Rebase

		/// <summary>Forward-port local commits to the updated upstream head.</summary>
		/// <param name="parameters"><see cref="RebaseParameters"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void Rebase(RebaseParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(CommandBuilder.GetRebaseCommand(parameters));
		}

		/// <summary>Forward-port local commits to the updated upstream head.</summary>
		/// <param name="parameters"><see cref="RebaseParameters"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task RebaseAsync(RebaseParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetRebaseCommand(parameters),
				cancellationToken);
		}

		/// <summary>Control rebase process.</summary>
		/// <param name="control">Rebase control option.</param>
		public void Rebase(RebaseControl control)
		{
			DefaultExecute(CommandBuilder.GetRebaseCommand(control));
		}

		/// <summary>Control rebase process.</summary>
		/// <param name="control">Rebase control option.</param>
		public Task RebaseAsync(RebaseControl control,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			return DefaultExecuteAsync(
				CommandBuilder.GetRebaseCommand(control),
				cancellationToken);
		}

		#endregion

		#region RemoveRemote

		/// <summary>Remove remote repository.</summary>
		/// <param name="parameters"><see cref="RemoveRemoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void RemoveRemote(RemoveRemoteParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(CommandBuilder.GetRemoveRemoteCommand(parameters));
		}

		/// <summary>Remove remote repository.</summary>
		/// <param name="parameters"><see cref="RemoveRemoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task RemoveRemoteAsync(RemoveRemoteParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetRemoveRemoteCommand(parameters),
				cancellationToken);
		}

		#endregion

		#region RenameBranch

		/// <summary>Rename branch.</summary>
		/// <param name="parameters"><see cref="RenameBranchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void RenameBranch(RenameBranchParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(
				CommandBuilder.GetRenameBranchCommand(parameters),
				output => OutputParser.HandleRenameBranchResult(parameters, output));
		}

		/// <summary>Rename branch.</summary>
		/// <param name="parameters"><see cref="RenameBranchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task RenameBranchAsync(RenameBranchParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetRenameBranchCommand(parameters),
				output => OutputParser.HandleRenameBranchResult(parameters, output),
				cancellationToken);
		}

		#endregion

		#region RenameRemote

		/// <summary>Rename remote repository.</summary>
		/// <param name="parameters"><see cref="RenameRemoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void RenameRemote(RenameRemoteParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(CommandBuilder.GetRenameRemoteCommand(parameters));
		}

		/// <summary>Rename remote repository.</summary>
		/// <param name="parameters"><see cref="RenameRemoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task RenameRemoteAsync(RenameRemoteParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetRenameRemoteCommand(parameters),
				cancellationToken);
		}

		#endregion

		#region Reset

		/// <summary>Reset HEAD.</summary>
		/// <param name="parameters"><see cref="ResetParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void Reset(ResetParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(CommandBuilder.GetResetCommand(parameters));
		}

		/// <summary>Reset HEAD.</summary>
		/// <param name="parameters"><see cref="ResetParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task ResetAsync(ResetParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetResetCommand(parameters),
				cancellationToken);
		}

		#endregion

		#region ResetFiles

		/// <summary>Resets files.</summary>
		/// <param name="parameters"><see cref="ResetFilesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void ResetFiles(ResetFilesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(CommandBuilder.GetResetFilesCommand(parameters));
		}

		/// <summary>Resets files.</summary>
		/// <param name="parameters"><see cref="ResetFilesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task ResetFilesAsync(ResetFilesParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetResetFilesCommand(parameters),
				cancellationToken);
		}

		#endregion

		#region ResetBranch

		/// <summary>Reset local branch.</summary>
		/// <param name="parameters"><see cref="ResetBranchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void ResetBranch(ResetBranchParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(CommandBuilder.GetResetBranchCommand(parameters));
		}

		/// <summary>Reset local branch.</summary>
		/// <param name="parameters"><see cref="ResetBranchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task ResetBranchAsync(ResetBranchParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetResetBranchCommand(parameters),
				cancellationToken);
		}

		#endregion

		#region Revert

		/// <summary>Performs a revert operation.</summary>
		/// <param name="parameters"><see cref="RevertParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		/// <exception cref="T:gitter.Git.HaveConflictsException">Conflicted files are present, revert is not possible.</exception>
		/// <exception cref="T:gitter.Git.CommitIsMergeException">Specified revision is a merge, revert cannot be performed on merges.</exception>
		/// <exception cref="T:gitter.Git.HaveLocalChangesException">Dirty working directory, unable to revert.</exception>
		public void Revert(RevertParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(
				CommandBuilder.GetRevertCommand(parameters),
				output => OutputParser.HandleRevertResult(output));
		}

		/// <summary>Performs a revert operation.</summary>
		/// <param name="parameters"><see cref="RevertParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		/// <exception cref="T:gitter.Git.HaveConflictsException">Conflicted files are present, revert is not possible.</exception>
		/// <exception cref="T:gitter.Git.CommitIsMergeException">Specified revision is a merge, revert cannot be performed on merges.</exception>
		/// <exception cref="T:gitter.Git.HaveLocalChangesException">Dirty working directory, unable to revert.</exception>
		public Task RevertAsync(RevertParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetRevertCommand(parameters),
				output => OutputParser.HandleRevertResult(output),
				cancellationToken);
		}

		/// <summary>Executes revert sequencer subcommand.</summary>
		/// <param name="control">Operation to execute.</param>
		public void Revert(RevertControl control)
		{
			DefaultExecute(
				CommandBuilder.GetRevertCommand(control),
				output => OutputParser.HandleRevertResult(output));
		}

		/// <summary>Executes revert sequencer subcommand.</summary>
		/// <param name="control">Operation to execute.</param>
		public Task RevertAsync(RevertControl control,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			return DefaultExecuteAsync(
				CommandBuilder.GetRevertCommand(control),
				output => OutputParser.HandleRevertResult(output),
				cancellationToken);
		}

		#endregion

		#region RunMergeTool

		/// <summary>Run merge tool to resolve conflicts.</summary>
		/// <param name="parameters"><see cref="RunMergeToolParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void RunMergeTool(RunMergeToolParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = CommandBuilder.GetRunMergeToolCommand(parameters);
			DefaultExecute(command);
		}

		/// <summary>Run merge tool to resolve conflicts.</summary>
		/// <param name="parameters"><see cref="RunMergeToolParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task RunMergeToolAsync(RunMergeToolParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetRunMergeToolCommand(parameters),
				cancellationToken);
		}

		#endregion

		#region StashApply

		/// <summary>Apply stashed changes and do not remove stashed state.</summary>
		/// <param name="parameters"><see cref="StashApplyParameters/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void StashApply(StashApplyParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(
				CommandBuilder.GetStashApplyCommand(parameters),
				output => OutputParser.HandleStashApplyResult(output));
		}

		/// <summary>Apply stashed changes and do not remove stashed state.</summary>
		/// <param name="parameters"><see cref="StashApplyParameters/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task StashApplyAsync(StashApplyParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetStashApplyCommand(parameters),
				output => OutputParser.HandleStashApplyResult(output),
				cancellationToken);
		}

		#endregion

		#region StashDrop

		/// <summary>Remove stashed state.</summary>
		/// <param name="parameters"><see cref="StashDropParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void StashDrop(StashDropParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(CommandBuilder.GetStashDropCommand(parameters));
		}

		/// <summary>Remove stashed state.</summary>
		/// <param name="parameters"><see cref="StashDropParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task StashDropAsync(StashDropParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetStashDropCommand(parameters),
				cancellationToken);
		}

		#endregion

		#region StashClear

		/// <summary>Clear stash.</summary>
		/// <param name="parameters"><see cref="StashClearParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void StashClear(StashClearParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(CommandBuilder.GetStashClearCommand(parameters));
		}

		/// <summary>Clear stash.</summary>
		/// <param name="parameters"><see cref="StashClearParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task StashClearAsync(StashClearParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetStashClearCommand(parameters),
				cancellationToken);
		}

		#endregion

		#region StashPop

		/// <summary>Apply stashed changes and remove stashed state.</summary>
		/// <param name="parameters"><see cref="StashPopParameters"/></param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void StashPop(StashPopParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(
				CommandBuilder.GetStashPopCommand(parameters),
				output => OutputParser.HandleStashPopResult(output));
		}

		/// <summary>Apply stashed changes and remove stashed state.</summary>
		/// <param name="parameters"><see cref="StashPopParameters"/></param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task StashPopAsync(StashPopParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetStashPopCommand(parameters),
				output => OutputParser.HandleStashPopResult(output),
				cancellationToken);
		}

		#endregion

		#region StashSave

		/// <summary>Stash changes in working directory.</summary>
		/// <param name="parameters"><see cref="StashSaveParameters"/>.</param>
		/// <returns>true if something was stashed, false otherwise.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public bool StashSave(StashSaveParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecute(
				CommandBuilder.GetStashSaveCommand(parameters),
				output => OutputParser.ParseStashSaveResult(output));
		}

		/// <summary>Stash changes in working directory.</summary>
		/// <param name="parameters"><see cref="StashSaveParameters"/>.</param>
		/// <returns>true if something was stashed, false otherwise.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task<bool> StashSaveAsync(StashSaveParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetStashSaveCommand(parameters),
				output => OutputParser.ParseStashSaveResult(output),
				cancellationToken);
		}

		#endregion

		#region StashToBranch

		/// <summary>Create new branch, checkout that branch and pop stashed state.</summary>
		/// <param name="parameters"><see cref="StashToBranchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void StashToBranch(StashToBranchParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(CommandBuilder.GetStashToBranchCommand(parameters));
		}

		/// <summary>Create new branch, checkout that branch and pop stashed state.</summary>
		/// <param name="parameters"><see cref="StashToBranchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task StashToBranchAsync(StashToBranchParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetStashToBranchCommand(parameters),
				cancellationToken);
		}

		#endregion

		#region UpdateSubmodule

		/// <summary>Updates submodule.</summary>
		/// <param name="parameters"><see cref="SubmoduleUpdateParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void UpdateSubmodule(SubmoduleUpdateParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(CommandBuilder.GetUpdateSubmoduleCommand(parameters));
		}

		/// <summary>Updates submodule.</summary>
		/// <param name="parameters"><see cref="SubmoduleUpdateParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task UpdateSubmoduleAsync(SubmoduleUpdateParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetUpdateSubmoduleCommand(parameters),
				cancellationToken);
		}

		#endregion

		#region VerifyTags

		/// <summary>Verify tags GPG signatures.</summary>
		/// <param name="parameters"><see cref="VerifyTagsParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void VerifyTags(VerifyTagsParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			DefaultExecute(CommandBuilder.GetVerifyTagsCommand(parameters));
		}

		/// <summary>Verify tags GPG signatures.</summary>
		/// <param name="parameters"><see cref="VerifyTagsParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public Task VerifyTagsAsync(VerifyTagsParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			return DefaultExecuteAsync(
				CommandBuilder.GetVerifyTagsCommand(parameters),
				cancellationToken);
		}

		#endregion
	}
}
