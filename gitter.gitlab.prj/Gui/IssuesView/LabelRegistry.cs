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

namespace gitter.GitLab.Gui;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using gitter.GitLab.Api;

sealed class LabelRegistry : IDisposable
{
	public static LabelRegistry? Current { get; set; }

	private readonly GitLabServiceContext _ctx;
	private Dictionary<string, Label> _byName = new(StringComparer.Ordinal);
	private IReadOnlyList<Label>      _all    = Array.Empty<Label>();
	private CancellationTokenSource?  _inflight;
	private readonly object _gate = new();

	public LabelRegistry(GitLabServiceContext ctx)
	{
		_ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
	}

	public event EventHandler? Updated;

	public IReadOnlyList<Label> All
	{
		get { lock(_gate) return _all; }
	}

	public Label? Find(string name)
	{
		if(string.IsNullOrEmpty(name)) return null;
		lock(_gate) return _byName.TryGetValue(name, out var label) ? label : null;
	}

	public async Task RefreshAsync(CancellationToken externalToken = default)
	{
		CancellationTokenSource cts;
		lock(_gate)
		{
			if(_inflight is not null) return; // coalesce
			_inflight = cts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
		}
		try
		{
			var labels = await _ctx.GetLabelsAsync(withCounts: false, cts.Token).ConfigureAwait(false);
			var map    = new Dictionary<string, Label>(labels.Count, StringComparer.Ordinal);
			foreach(var l in labels) map[l.Name] = l;
			lock(_gate)
			{
				_all    = labels;
				_byName = map;
			}
			Updated?.Invoke(this, EventArgs.Empty);
		}
		catch(OperationCanceledException) { /* expected on dispose */ }
		finally
		{
			lock(_gate)
			{
				cts.Dispose();
				if(ReferenceEquals(_inflight, cts)) _inflight = null;
			}
		}
	}

	public void Dispose()
	{
		lock(_gate)
		{
			_inflight?.Cancel();
			_inflight = null;
		}
		if(ReferenceEquals(Current, this)) Current = null;
	}
}
