#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2026  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

using gitter.Framework;

using Label = gitter.GitLab.Api.Label;

sealed class LabelsFilterModel
{
	private readonly HashSet<string> _selected = new(StringComparer.Ordinal);
	private IReadOnlyList<Label> _labels = Preallocated<Label>.EmptyArray;

	public IReadOnlyList<Label> Labels
	{
		get => _labels;
		set => _labels = value ?? Preallocated<Label>.EmptyArray;
	}

	public IReadOnlyCollection<string> Selected => _selected;

	public int SelectedCount => _selected.Count;

	public bool IsSelected(string name) => _selected.Contains(name);

	public bool Select(string name, bool selected)
	{
		if(string.IsNullOrEmpty(name)) return false;

		return selected ? _selected.Add(name) : _selected.Remove(name);
	}

	public void SetSelected(IEnumerable<string>? names)
	{
		_selected.Clear();
		if(names is null) return;

		foreach(var name in names)
		{
			if(!string.IsNullOrEmpty(name)) _selected.Add(name);
		}
	}

	public bool Clear()
	{
		if(_selected.Count == 0) return false;

		_selected.Clear();
		return true;
	}

	public List<Label> GetVisibleLabels(string? filter)
	{
		var result = new List<Label>(_labels.Count);
		foreach(var label in _labels)
		{
			if(Matches(label, filter)) result.Add(label);
		}
		return result;
	}

	public static bool Matches(Label? label, string? filter)
	{
		if(label is null) return false;
		if(string.IsNullOrWhiteSpace(filter)) return true;

		var needle = filter!.Trim();

		return Contains(label.Name, needle) || Contains(label.Description, needle);
	}

	private static bool Contains(string? haystack, string needle)
	{
		if(haystack is not { Length: not 0 }) return false;

#if NET6_0_OR_GREATER
		return haystack.Contains(needle, StringComparison.OrdinalIgnoreCase);
#else
		return haystack.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;
#endif
	}
}
