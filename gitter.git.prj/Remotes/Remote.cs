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

namespace gitter.Git;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using gitter.Framework;

using gitter.Git.AccessLayer;

using Resources = gitter.Git.Properties.Resources;

/// <summary>git remote repository object.</summary>
public sealed class Remote : GitNamedObjectWithLifetime
{
	#region Events

	/// <summary>Remote is renamed.</summary>
	public event EventHandler<NameChangeEventArgs>? Renamed;

	/// <summary>Invoke <see cref="Renamed"/>.</summary>
	private void InvokeRenamed(string oldName, string newName)
		=> Renamed?.Invoke(this, new NameChangeEventArgs(oldName, newName));

	#endregion

	#region Data

	private string? _fetchUrl;
	private string? _pushUrl;

	#endregion

	#region .ctor

	/// <summary>Create <see cref="Remote"/> object.</summary>
	/// <param name="repository">Host repository.</param>
	/// <param name="name">Remote name.</param>
	/// <param name="fetchUrl">Fetch URL.</param>
	/// <param name="pushUrl">Push URL.</param>
	internal Remote(Repository repository, string name, string? fetchUrl, string? pushUrl)
		: base(repository, name)
	{
		_fetchUrl = fetchUrl;
		_pushUrl  = pushUrl;
	}

	/// <summary>Create <see cref="Remote"/> object.</summary>
	/// <param name="repository">Host repository.</param>
	/// <param name="name">Remote name.</param>
	internal Remote(Repository repository, string name)
		: this(repository, name, null, null)
	{
	}

	#endregion

	#region Properties

	/// <summary>Url used by fetch/pull commands.</summary>
	public string FetchUrl
	{
		get => _fetchUrl ?? "";
		set
		{
			Verify.Argument.IsNotNull(value);
			Verify.State.IsNotDeleted(this);

			if(_fetchUrl != value)
			{
				var pname = "remote." + Name + ".url";
				Repository.Configuration.SetValue(pname, value);
				pname = "remote." + Name + ".pushurl";
				if(_fetchUrl == _pushUrl && !Repository.Configuration.Exists(pname))
				{
					_pushUrl = value;
				}
				_fetchUrl = value;
			}
		}
	}

	internal void SetFetchUrl(string fetchUrl)
	{
		_fetchUrl = fetchUrl;
	}

	/// <summary>Url used by push command.</summary>
	public string PushUrl
	{
		get => _pushUrl ?? "";
		set
		{
			Verify.Argument.IsNotNull(value);
			Verify.State.IsNotDeleted(this);

			if(_pushUrl != value)
			{
				var pname = "remote." + Name + ".pushurl";
				Repository.Configuration.SetValue(pname, value);
				_pushUrl = value;
			}
		}
	}

	internal void SetPushUrl(string pushUrl)
	{
		_pushUrl = pushUrl;
	}

	private string GetConfigurationProperty(string propertySuffix)
	{
		var p = Repository.Configuration.TryGetParameter("remote." + Name + propertySuffix);
		return p is not null ? p.Value : string.Empty;
	}

	private void SetConfigurationProperty(string propertySuffix, string value)
	{
		Verify.Argument.IsNotNull(value);
		Verify.State.IsNotDeleted(this);

		var pname = "remote." + Name + propertySuffix;
		var p = Repository.Configuration.TryGetParameter(pname);
		if(p == null)
		{
			if(value == "") return;
			Repository.Configuration.SetValue(pname, value);
		}
		else
		{
			p.Value = value;
		}
	}

	/// <summary>Proxy.</summary>
	public string Proxy
	{
		get => GetConfigurationProperty(".proxy");
		set => SetConfigurationProperty(".proxy", value);
	}

	/// <summary>Version control system.</summary>
	public string VCS
	{
		get => GetConfigurationProperty(".vcs");
		set => SetConfigurationProperty(".vcs", value);
	}

	public string ReceivePack
	{
		get => GetConfigurationProperty(".receivepack");
		set => SetConfigurationProperty(".receivepack", value);
	}

	public string UploadPack
	{
		get => GetConfigurationProperty(".uploadpack");
		set => SetConfigurationProperty(".uploadpack", value);
	}

	public string FetchRefspec
	{
		get => GetConfigurationProperty(".fetch");
		set => SetConfigurationProperty(".fetch", value);
	}

	public string PushRefspec
	{
		get => GetConfigurationProperty(".push");
		set => SetConfigurationProperty(".push", value);
	}

	public bool Mirror
	{
		get => GetConfigurationProperty(".mirror") == "true";
		set => SetConfigurationProperty(".mirror", value ? "true" : "false");
	}

	public bool SkipFetchAll
	{
		get => GetConfigurationProperty(".skipfetchall") == "true";
		set => SetConfigurationProperty(".skipfetchall", value ? "true" : "false");
	}

	public TagFetchMode TagFetchMode
	{
		get => GetConfigurationProperty(".tagopt") switch
		{
			"--tags"    => TagFetchMode.AllTags,
			"--no-tags" => TagFetchMode.NoTags,
			_           => TagFetchMode.Default,
		};
		set
		{
			var strvalue = value switch
			{
				TagFetchMode.AllTags => "--tags",
				TagFetchMode.NoTags  => "--no-tags",
				_ => "",
			};
			SetConfigurationProperty(".tagopt", strvalue);
		}
	}

	#endregion

	#region Methods

	public RemoteReferencesCollection GetReferences()
	{
		Verify.State.IsNotDeleted(this);

		return new RemoteReferencesCollection(this);
	}

	public void Delete()
	{
		Verify.State.IsNotDeleted(this);

		Repository.Remotes.RemoveRemote(this);
	}

	public Task DeleteAsync()
	{
		Verify.State.IsNotDeleted(this);

		return Repository.Remotes.RemoveRemoteAsync(this);
	}

	/// <summary>Download new objects from remote repository.</summary>
	public void Fetch()
	{
		Verify.State.IsNotDeleted(this);

		RemotesUtility.FetchOrPull(Repository, this, false);
	}

	/// <summary>Download new objects from remote repository.</summary>
	public Task FetchAsync(IProgress<OperationProgress>? progress = default, CancellationToken cancellationToken = default)
	{
		Verify.State.IsNotDeleted(this);

		return RemotesUtility.FetchOrPullAsync(Repository, this, false, progress, cancellationToken);
	}

	/// <summary>Download new objects from remote repository and merge tracking branches.</summary>
	public void Pull()
	{
		Verify.State.IsNotDeleted(this);

		RemotesUtility.FetchOrPull(Repository, this, true);
	}

	/// <summary>Download new objects from remote repository and merge tracking branches.</summary>
	public Task PullAsync(IProgress<OperationProgress>? progress = default, CancellationToken cancellationToken = default)
	{
		Verify.State.IsNotDeleted(this);

		return RemotesUtility.FetchOrPullAsync(Repository, this, true, progress, cancellationToken);
	}

	/// <summary>Send local objects to remote repository.</summary>
	public void Push(Many<Branch> branches, bool forceOverwrite, bool thinPack, bool sendTags)
	{
		Verify.State.IsNotDeleted(this);
		Verify.Argument.IsValidRevisionPointerSequence(branches, Repository, nameof(branches));
		Verify.Argument.IsFalse(branches.IsEmpty, nameof(branches),
			Resources.ExcCollectionMustContainAtLeastOneObject.UseAsFormat("branch"));

		var names = branches.ConvertAll(static b => b.Name);
		Many<ReferencePushResult> res;
		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.BranchChanged))
		{
			res = Repository.Accessor.Push.Invoke(
				new PushRequest(Name, sendTags?PushMode.Tags:PushMode.Default, names)
				{
					Force = forceOverwrite,
					ThinPack = thinPack,
				});
		}

		var changed = res.Any(static r
			=> r.Type != PushResultType.UpToDate
			&& r.Type != PushResultType.Rejected);
		if(changed)
		{
			Repository.Refs.Remotes.Refresh();
		}
	}

	public Task PushAsync(Many<Branch> branches, bool forceOverwrite, bool thinPack, bool sendTags,
		IProgress<OperationProgress>? progress = default, CancellationToken cancellationToken = default)
	{
		Verify.State.IsNotDeleted(this);

		return RemotesUtility.PushAsync(Repository, this, branches, forceOverwrite, thinPack, sendTags, progress, cancellationToken);
	}

	private PruneRemoteRequest GetPruneRequest()
	{
		return new PruneRemoteRequest(Name);
	}

	/// <summary>Deletes all stale tracking branches.</summary>
	public void Prune()
	{
		Verify.State.IsNotDeleted(this);

		var before = RefsState.Capture(Repository, ReferenceType.RemoteBranch);
		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.BranchChanged))
		{
			var request = GetPruneRequest();
			Repository.Accessor.PruneRemote.Invoke(request);
		}
		Repository.Refs.Remotes.Refresh();
		var after = RefsState.Capture(Repository, ReferenceType.RemoteBranch);
		var changes = RefsDiff.Calculate(before, after);
		Repository.Remotes.OnPruneCompleted(this, changes);
	}

	/// <summary>Deletes all stale tracking branches.</summary>
	public async Task PruneAsync(IProgress<OperationProgress>? progress = null, CancellationToken cancellationToken = default)
	{
		Verify.State.IsNotDeleted(this);

		progress?.Report(new OperationProgress(Resources.StrsSearchingStaleBranches.AddEllipsis()));
		var before = RefsState.Capture(Repository, ReferenceType.RemoteBranch);
		using(var block = Repository.Monitor.BlockNotifications(RepositoryNotifications.BranchChanged))
		{
			var request = GetPruneRequest();
			await Repository
				.Accessor
				.PruneRemote
				.InvokeAsync(request, progress, cancellationToken)
				.ConfigureAwait(continueOnCapturedContext: false);
		}
		Repository.Refs.Remotes.Refresh();
		var after = RefsState.Capture(Repository, ReferenceType.RemoteBranch);
		var changes = RefsDiff.Calculate(before, after);
		Repository.Remotes.OnPruneCompleted(this, changes);
	}

	#endregion

	#region Overrides

	/// <summary>Rename remote.</summary>
	/// <param name="newName">New name.</param>
	protected override void RenameCore(string newName)
	{
		Verify.State.IsNotDeleted(this);

		Repository.Remotes.RenameRemote(this, newName);
	}

	/// <summary>Called after remote is renamed.</summary>
	/// <param name="oldName">Old name.</param>
	protected override void AfterRename(string oldName)
	{
		Assert.IsNeitherNullNorWhitespace(oldName);

		InvokeRenamed(oldName, Name);
		Repository.Remotes.NotifyRenamed(this, oldName);
	}

	#endregion
}
