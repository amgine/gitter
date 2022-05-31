﻿#region Copyright Notice
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
using System.Threading;
using System.Threading.Tasks;

using gitter.Framework;
using gitter.Git.AccessLayer;

using Resources = gitter.Git.Properties.Resources;

/// <summary>Git submodule.</summary>
public sealed class Submodule : GitNamedObjectWithLifetime
{
	#region Data

	private string _path;
	private string _url;

	#endregion

	#region Events

	/// <summary><see cref="M:Path"/> property value changed.</summary>
	public event EventHandler PathChanged;

	/// <summary><see cref="M:Url"/> property value changed.</summary>
	public event EventHandler UrlChanged;

	private void OnPathChanged()
		=> PathChanged?.Invoke(this, EventArgs.Empty);

	private void OnUrlChanged()
		=> UrlChanged?.Invoke(this, EventArgs.Empty);

	#endregion

	#region .ctor

	/// <summary>Create <see cref="Submodule"/>.</summary>
	/// <param name="repository">Related <see cref="Repository"/>.</param>
	/// <param name="name">Submodule name.</param>
	/// <param name="path">Submodule path.</param>
	/// <param name="url">Submodule URL.</param>
	internal Submodule(Repository repository, string name, string path, string url)
		: base(repository, name)
	{
		Verify.Argument.IsNeitherNullNorWhitespace(path);
		Verify.Argument.IsNotNull(path);

		_path = path;
		_url = url;
	}

	#endregion

	#region Properties

	/// <summary>Submodule full path.</summary>
	public string FullPath => System.IO.Path.Combine(Repository.WorkingDirectory, _path);

	/// <summary>Submodule path.</summary>
	public string Path
	{
		get => _path;
		private set
		{
			if(_path != value)
			{
				_path = value;
				OnPathChanged();
			}
		}
	}

	/// <summary>Submodule URL.</summary>
	public string Url
	{
		get => _url;
		private set
		{
			if(_url != value)
			{
				_url = value;
				OnUrlChanged();
			}
		}
	}

	#endregion

	#region Methods

	private UpdateSubmoduleParameters GetUpdateParameters()
		=> new()
		{
			Path = _path,
			Recursive = true,
			Init = true,
		};

	private SyncSubmoduleParameters GetSyncParameters(bool recursive)
		=> new()
		{
			Recursive  = recursive,
			Submodules = new[] { _path },
		};

	public void Update()
	{
		Verify.State.IsNotDeleted(this);

		var parameters = GetUpdateParameters();
		Repository.Accessor.UpdateSubmodule.Invoke(parameters);
	}

	public Task UpdateAsync(IProgress<OperationProgress> progress = default, CancellationToken cancellationToken = default)
	{
		Verify.State.IsNotDeleted(this);

		progress?.Report(new OperationProgress(Resources.StrsUpdatingSubmodule.AddEllipsis()));
		var parameters = GetUpdateParameters();
		return Repository.Accessor.UpdateSubmodule.InvokeAsync(parameters, progress, cancellationToken);
	}

	public void Sync(bool recursive = true)
	{
		Verify.State.IsNotDeleted(this);

		var parameters = GetSyncParameters(recursive);
		Repository.Accessor.SyncSubmodule.Invoke(parameters);
		Repository.Submodules.Refresh();
	}

	public async Task SyncAsync(bool recursive = true,
		IProgress<OperationProgress> progress = default, CancellationToken cancellationToken = default)
	{
		Verify.State.IsNotDeleted(this);

		progress?.Report(new OperationProgress(Resources.StrsSynchronizingSubmodule.AddEllipsis()));
		var parameters = GetSyncParameters(recursive);
		await Repository.Accessor.SyncSubmodule
			.InvokeAsync(parameters, progress, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		await Repository.Submodules
			.RefreshAsync()
			.ConfigureAwait(continueOnCapturedContext: false);
	}

	internal void UpdateInfo(string path, string url)
	{
		Assert.IsNotNull(path);
		Assert.IsNotNull(url);

		Path = path;
		Url = url;
	}

	#endregion
}
