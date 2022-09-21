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
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using gitter.Framework;

using gitter.Git.AccessLayer;

using Resources = gitter.Git.Properties.Resources;

/// <summary>Repository submodules collection.</summary>
public sealed class SubmodulesCollection : GitObjectsCollection<Submodule, SubmoduleEventArgs>
{
	internal SubmodulesCollection(Repository repository)
		: base(repository)
	{
	}

	/// <inheritdoc/>
	protected override SubmoduleEventArgs CreateEventArgs(Submodule item) => new(item);

	private UpdateSubmoduleParameters GetUpdateParameters()
		=> new()
		{
			Recursive = true,
			Init      = true,
		};

	private SyncSubmoduleParameters GetSyncParameters(bool recursive)
		=> new()
		{
			Recursive = recursive,
		};

	public void Update()
	{
		var parameters = GetUpdateParameters();
		Repository.Accessor.UpdateSubmodule.Invoke(parameters);
	}

	public Task UpdateAsync(IProgress<OperationProgress>? progress = default, CancellationToken cancellationToken = default)
	{
		progress?.Report(new OperationProgress(Resources.StrsUpdatingSubmodules.AddEllipsis()));
		var parameters = GetUpdateParameters();
		return Repository.Accessor.UpdateSubmodule.InvokeAsync(parameters, progress, cancellationToken);
	}

	public void Sync(bool recursive = true)
	{
		var parameters = GetSyncParameters(recursive);
		Repository.Accessor.SyncSubmodule.Invoke(parameters);
	}

	public async Task SyncAsync(bool recursive = true,
		IProgress<OperationProgress>? progress = default,
		CancellationToken cancellationToken = default)
	{
		progress?.Report(new OperationProgress(Resources.StrsSynchronizingSubmodules.AddEllipsis()));
		var parameters = GetSyncParameters(recursive);
		await Repository.Accessor.SyncSubmodule
			.InvokeAsync(parameters, progress, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		Refresh();
	}

	public bool ExistsPath(string path)
	{
		Verify.Argument.IsNotNull(path);

		lock(SyncRoot)
		{
			foreach(var s in ObjectStorage.Values)
			{
				if(s.Path == path) return true;
			}
		}
		return false;
	}

	public bool ExistsUrl(string url)
	{
		Verify.Argument.IsNotNull(url);

		lock(SyncRoot)
		{
			foreach(var s in ObjectStorage.Values)
			{
				if(s.Url == url) return true;
			}
		}
		return false;
	}

	public bool TryGetSubmoduleByPath(string path, out Submodule? submodule)
	{
		Verify.Argument.IsNotNull(path);

		lock(SyncRoot)
		{
			foreach(var s in ObjectStorage.Values)
			{
				if(s.Path == path)
				{
					submodule = s;
					return true;
				}
			}
		}
		submodule = null;
		return false;
	}

	public Submodule? TryGetSubmoduleByPath(string path)
	{
		Verify.Argument.IsNotNull(path);

		lock(SyncRoot)
		{
			foreach(var s in ObjectStorage.Values)
			{
				if(s.Path == path)
				{
					return s;
				}
			}
		}
		return null;
	}

	public bool TryGetSubmoduleByUrl(string url, out Submodule? submodule)
	{
		Verify.Argument.IsNotNull(url);

		lock(SyncRoot)
		{
			foreach(var s in ObjectStorage.Values)
			{
				if(s.Url == url)
				{
					submodule = s;
					return true;
				}
			}
		}
		submodule = null;
		return false;
	}

	public Submodule? TryGetSubmoduleByUrl(string url)
	{
		Verify.Argument.IsNotNull(url);

		lock(SyncRoot)
		{
			foreach(var s in ObjectStorage.Values)
			{
				if(s.Url == url) return s;
			}
		}
		return null;
	}

	private static AddSubmoduleParameters GetAddSubmoduleParameters(string path, string url, string? branch = default)
		=> new AddSubmoduleParameters
		{
			Branch     = branch,
			Path       = "./" + path,
			Repository = url,
		};

	public Submodule Add(string path, string url, string? branch = default)
	{
		Verify.Argument.IsNotNull(path);
		Verify.Argument.IsNotNull(url);

		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.WorktreeUpdated,
			RepositoryNotifications.SubmodulesChanged))
		{
			try
			{
				var parameters = GetAddSubmoduleParameters(path, url, branch);
				Repository.Accessor.AddSubmodule.Invoke(parameters);
			}
			finally
			{
				Repository.Status.Refresh();
			}
			lock(SyncRoot)
			{
				var submodule = new Submodule(Repository, path, path, url);
				AddObject(submodule);
				return submodule;
			}
		}
	}

	public async Task<Submodule> AddAsync(string path, string url, string? branch = default)
	{
		Verify.Argument.IsNotNull(path);
		Verify.Argument.IsNotNull(url);

		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.WorktreeUpdated,
			RepositoryNotifications.SubmodulesChanged))
		{
			try
			{
				var parameters = GetAddSubmoduleParameters(path, url, branch);
				await Repository.Accessor.AddSubmodule
					.InvokeAsync(parameters)
					.ConfigureAwait(continueOnCapturedContext: false);
			}
			finally
			{
				await Repository.Status
					.RefreshAsync()
					.ConfigureAwait(continueOnCapturedContext: false);
			}
			lock(SyncRoot)
			{
				var submodule = new Submodule(Repository, path, path, url);
				AddObject(submodule);
				return submodule;
			}
		}
	}

	private static Dictionary<string, SubmoduleData> ParseConfig(ConfigurationFile cfgFile)
	{
		var submodules = new Dictionary<string, SubmoduleData>();
		foreach(var param in cfgFile)
		{
			if(param.Name.StartsWith("submodule."))
			{
				int p = param.Name.LastIndexOf('.', param.Name.Length - 1, param.Name.Length - "submodule.".Length);
				if(p != -1 && p != param.Name.Length - 1)
				{
					var p3 = param.Name.Substring(p + 1);
					bool valueIsPath = false;
					bool valueIsUrl = false;
					switch(p3)
					{
						case "path":
							valueIsPath = true;
							break;
						case "url":
							valueIsUrl = true;
							break;
						default:
							continue;
					}
					var name = param.Name.Substring("submodule.".Length, p - "submodule.".Length);
					if(!submodules.TryGetValue(name, out var info))
					{
						info = new SubmoduleData(name);
						submodules.Add(name, info);
					}
					if(valueIsPath) info.Path = param.Value;
					if(valueIsUrl)  info.Url  = param.Value;
				}
			}
		}
		return submodules;
	}

	public void Refresh()
	{
		var submodules = default(Dictionary<string, SubmoduleData>);
		var cfg = Path.Combine(Repository.WorkingDirectory, GitConstants.SubmodulesConfigFile);
		bool skipUpdate = false;
		if(File.Exists(cfg))
		{
			var cfgFile = default(ConfigurationFile);
			try
			{
				cfgFile = new ConfigurationFile(Repository, GitConstants.SubmodulesConfigFile, load: true);
			}
			catch(Exception exc) when(!exc.IsCritical())
			{
				skipUpdate = true;
			}
			if(cfgFile != null)
			{
				submodules = ParseConfig(cfgFile);
			}
		}

		if(!skipUpdate)
		{
			submodules ??= new();
			lock(SyncRoot)
			{
				CacheUpdater.UpdateObjectDictionary<Submodule, SubmoduleData>(
					ObjectStorage,
					null,
					null,
					submodules,
					submoduleData => ObjectFactories.CreateSubmodue(Repository, submoduleData),
					ObjectFactories.UpdateSubmodule,
					InvokeObjectAdded,
					InvokeObjectRemoved,
					true);
			}
		}
	}

	public async Task RefreshAsync()
	{
		var submodules = default(Dictionary<string, SubmoduleData>);
		var cfg = Path.Combine(Repository.WorkingDirectory, GitConstants.SubmodulesConfigFile);
		bool skipUpdate = false;
		if(File.Exists(cfg))
		{
			var cfgFile = default(ConfigurationFile);
			try
			{
				cfgFile = new ConfigurationFile(Repository, GitConstants.SubmodulesConfigFile, load: false);
				await cfgFile
					.RefreshAsync()
					.ConfigureAwait(continueOnCapturedContext: false);
			}
			catch(Exception exc) when(!exc.IsCritical())
			{
				skipUpdate = true;
			}
			if(cfgFile != null)
			{
				submodules = ParseConfig(cfgFile);
			}
		}

		if(!skipUpdate)
		{
			submodules ??= new();
			lock(SyncRoot)
			{
				CacheUpdater.UpdateObjectDictionary<Submodule, SubmoduleData>(
					ObjectStorage,
					null,
					null,
					submodules,
					submoduleData => ObjectFactories.CreateSubmodue(Repository, submoduleData),
					ObjectFactories.UpdateSubmodule,
					InvokeObjectAdded,
					InvokeObjectRemoved,
					true);
			}
		}
	}
}
