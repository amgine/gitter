#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;
	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	static class RepositoryLoader
	{
		const int MaxProgress = 8;

		private static readonly ILoadStep[] _loadSteps = new ILoadStep[]
		{
			new ConfigurationLoadStep(),
			new ReferencesLoadStep(),
			new StashLoadStep(),
			new NotesLoadStep(),
			new HeadLoadStep(),
			new RemotesLoadStep(),
			new SubmoduilesLoadStep(),
			new ContributorsLoadStep(),
			new StatusLoadStep(),
			new FinalizeLoadStep(),
		};

		public static void Load(Repository repository,
			IProgress<OperationProgress> progress = default, CancellationToken cancellationToken = default)
		{
			Verify.Argument.IsNotNull(repository, nameof(repository));

			var context = new RepositoryLoadContext(repository, progress, cancellationToken);
			foreach(var step in _loadSteps)
			{
				step.Execute(context);
			}
		}

		public static async Task LoadAsync(Repository repository,
			IProgress<OperationProgress> progress = default, CancellationToken cancellationToken = default)
		{
			Verify.Argument.IsNotNull(repository, nameof(repository));

			var context = new RepositoryLoadContext(repository, progress, cancellationToken);
			foreach(var step in _loadSteps)
			{
				await step
					.ExecuteAsync(context)
					.ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		interface ILoadStep
		{
			void Execute(RepositoryLoadContext context);

			Task ExecuteAsync(RepositoryLoadContext context);
		}

		class LoadStep : ILoadStep
		{
			public LoadStep(int index, string action)
			{
				Index  = index;
				Action = action;
			}

			private void ReportLoadProgress(IProgress<OperationProgress> progress)
			{
				if(progress != null)
				{
					var status = new OperationProgress
					{
						ActionName      = Action,
						MaxProgress     = MaxProgress,
						CurrentProgress = Index,
					};
					progress.Report(status);
				}
			}

			public int Index { get; }

			public string Action { get; }

			protected virtual bool SkipExecute(RepositoryLoadContext repository) => false;

			protected virtual void ExecuteCore(RepositoryLoadContext context)
			{
			}

			protected virtual Task ExecuteAsyncCore(RepositoryLoadContext context)
			{
				return Task.CompletedTask;
			}

			public void Execute(RepositoryLoadContext context)
			{
				context.CancellationToken.ThrowIfCancellationRequested();

				if(SkipExecute(context)) return;

				ReportLoadProgress(context.Progress);
				ExecuteCore(context);
			}

			public Task ExecuteAsync(RepositoryLoadContext context)
			{
				context.CancellationToken.ThrowIfCancellationRequested();

				if(SkipExecute(context)) return Task.CompletedTask;

				ReportLoadProgress(context.Progress);
				return ExecuteAsyncCore(context);
			}
		}

		sealed class ConfigurationLoadStep : LoadStep
		{
			public ConfigurationLoadStep()
				: base(0, Resources.StrLoadingConfiguration.AddEllipsis())
			{
			}

			protected override void ExecuteCore(RepositoryLoadContext context)
			{
				Assert.IsNotNull(context);

				context.Repository.Configuration.Refresh();
			}

			protected override Task ExecuteAsyncCore(RepositoryLoadContext context)
			{
				Assert.IsNotNull(context);

				return context.Repository.Configuration.RefreshAsync();
			}
		}

		sealed class ReferencesLoadStep : LoadStep
		{
			public ReferencesLoadStep()
				: base(1, Resources.StrLoadingReferences.AddEllipsis())
			{
			}

			private static QueryReferencesParameters GetQueryReferencesParameters()
				=> new QueryReferencesParameters(ReferenceType.Branch | ReferenceType.Tag | ReferenceType.Stash);

			protected override void ExecuteCore(RepositoryLoadContext context)
			{
				Assert.IsNotNull(context);

				var refs = context.Repository.Accessor.QueryReferences
					.Invoke(GetQueryReferencesParameters());
				context.ReferencesData = refs;
				context.Repository.Refs.Load(refs);
			}

			protected override async Task ExecuteAsyncCore(RepositoryLoadContext context)
			{
				Assert.IsNotNull(context);

				var refs = await context.Repository.Accessor.QueryReferences
					.InvokeAsync(GetQueryReferencesParameters(), cancellationToken: context.CancellationToken)
					.ConfigureAwait(continueOnCapturedContext: false);
				context.ReferencesData = refs;
				context.Repository.Refs.Load(refs);
			}
		}

		sealed class StashLoadStep : LoadStep
		{
			public StashLoadStep()
				: base(2, Resources.StrLoadingStash.AddEllipsis())
			{
			}

			protected override bool SkipExecute(RepositoryLoadContext context)
			{
				Assert.IsNotNull(context);

				return context.ReferencesData != null
					&& context.ReferencesData.Stash == null;
			}

			protected override void ExecuteCore(RepositoryLoadContext context)
			{
				Assert.IsNotNull(context);

				context.Repository.Stash.Refresh();
			}

			protected override Task ExecuteAsyncCore(RepositoryLoadContext context)
			{
				Assert.IsNotNull(context);

				return context.Repository.Stash.RefreshAsync();
			}
		}

		sealed class NotesLoadStep : LoadStep
		{
			public NotesLoadStep()
				: base(2, Resources.StrLoadingNotes.AddEllipsis())
			{
			}

			protected override void ExecuteCore(RepositoryLoadContext context)
			{
				Assert.IsNotNull(context);

				context.Repository.Notes.Refresh();
			}

			protected override Task ExecuteAsyncCore(RepositoryLoadContext context)
			{
				Assert.IsNotNull(context);

				return context.Repository.Notes.RefreshAsync();
			}
		}

		sealed class HeadLoadStep : LoadStep
		{
			public HeadLoadStep()
				: base(3, Resources.StrLoadingHEAD.AddEllipsis())
			{
			}

			protected override void ExecuteCore(RepositoryLoadContext context)
			{
				Assert.IsNotNull(context);

				context.Repository.Head.Refresh();
			}

			protected override Task ExecuteAsyncCore(RepositoryLoadContext context)
			{
				Assert.IsNotNull(context);

				return context.Repository.Head.RefreshAsync();
			}
		}

		sealed class RemotesLoadStep : LoadStep
		{
			public RemotesLoadStep()
				: base(4, Resources.StrLoadingRemotes.AddEllipsis())
			{
			}

			protected override void ExecuteCore(RepositoryLoadContext context)
			{
				Assert.IsNotNull(context);

				context.Repository.Remotes.Refresh();
			}

			protected override Task ExecuteAsyncCore(RepositoryLoadContext context)
			{
				Assert.IsNotNull(context);

				return context.Repository.Remotes.RefreshAsync();
			}
		}

		sealed class SubmoduilesLoadStep : LoadStep
		{
			public SubmoduilesLoadStep()
				: base(5, Resources.StrLoadingSubmodules.AddEllipsis())
			{
			}

			protected override void ExecuteCore(RepositoryLoadContext context)
			{
				Assert.IsNotNull(context);

				context.Repository.Submodules.Refresh();
			}

			protected override Task ExecuteAsyncCore(RepositoryLoadContext context)
			{
				Assert.IsNotNull(context);

				return context.Repository.Submodules.RefreshAsync();
			}
		}

		sealed class ContributorsLoadStep : LoadStep
		{
			public ContributorsLoadStep()
				: base(6, Resources.StrLoadingContributors.AddEllipsis())
			{
			}

			protected override bool SkipExecute(RepositoryLoadContext context) => context.Repository.Head.IsEmpty;

			protected override void ExecuteCore(RepositoryLoadContext context)
			{
				Assert.IsNotNull(context);

				context.Repository.Users.Refresh();
			}

			protected override Task ExecuteAsyncCore(RepositoryLoadContext context)
			{
				Assert.IsNotNull(context);

				return context.Repository.Users.RefreshAsync();
			}
		}

		sealed class StatusLoadStep : LoadStep
		{
			public StatusLoadStep()
				: base(7, Resources.StrLoadingStatus.AddEllipsis())
			{
			}

			protected override void ExecuteCore(RepositoryLoadContext context)
			{
				Assert.IsNotNull(context);

				context.Repository.Status.Refresh();
			}

			protected override Task ExecuteAsyncCore(RepositoryLoadContext context)
			{
				Assert.IsNotNull(context);

				return context.Repository.Status.RefreshAsync();
			}
		}

		sealed class FinalizeLoadStep : ILoadStep
		{
			public FinalizeLoadStep()
			{
			}

			public void Execute(RepositoryLoadContext context)
			{
				Assert.IsNotNull(context);

				var repository = context.Repository;

				context.ThrowIfCancellationRequested();
				repository.UpdateState();

				context.ThrowIfCancellationRequested();
				repository.UpdateUserIdentity(false);

				context.ThrowIfCancellationRequested();
				repository.Monitor.IsEnabled = true;

				context.ThrowIfCancellationRequested();
				context.Progress?.Report(new OperationProgress
				{
					ActionName      = Resources.StrCompleted.AddPeriod(),
					MaxProgress     = MaxProgress,
					CurrentProgress = MaxProgress,
				});
			}

			public Task ExecuteAsync(RepositoryLoadContext context)
			{
				Assert.IsNotNull(context);

				Execute(context);
				return Task.CompletedTask;
			}
		}

		sealed class RepositoryLoadContext
		{
			public RepositoryLoadContext(Repository repository,
				IProgress<OperationProgress> progress = default, CancellationToken cancellationToken = default)
			{
				Repository        = repository;
				Progress          = progress;
				CancellationToken = cancellationToken;
			}

			public Repository Repository { get; }

			public IProgress<OperationProgress> Progress { get; }

			public CancellationToken CancellationToken { get; }

			public ReferencesData ReferencesData { get; set; }

			public void ThrowIfCancellationRequested() => CancellationToken.ThrowIfCancellationRequested();
		}
	}
}
