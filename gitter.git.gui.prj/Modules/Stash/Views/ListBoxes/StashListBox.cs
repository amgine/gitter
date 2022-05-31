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

namespace gitter.Git.Gui.Controls;

using System;
using System.Windows.Forms;

using gitter.Framework.Controls;

using Resources = Properties.Resources;

/// <summary><see cref="CustomListBox"/> for displaying <see cref="Repository.Stash"/> contents.</summary>
sealed class StashListBox : CustomListBox
{
	private Repository _repository;

	/// <summary>Create <see cref="StashListBox"/>.</summary>
	public StashListBox()
	{
		Text = Resources.StrsNothingStashed;
	}

	public void LoadData(Repository repository)
	{
		if(_repository != null)
			DetachFromRepository();
		_repository = repository;
		if(_repository != null)
			AttachToRepository();
	}

	private void AttachToRepository()
	{
		BeginUpdate();
		Items.Clear();
		lock(_repository.Stash.SyncRoot)
		{
			foreach(var ss in _repository.Stash)
			{
				Items.Add(new StashedStateListItem(ss));
			}
		}
		EndUpdate();
		_repository.Stash.StashedStateCreated += OnStashCreated;
	}

	private void OnStashCreated(object sender, StashedStateEventArgs e)
	{
		Items.AddSafe(new StashedStateListItem(e.Object));
	}

	private void DetachFromRepository()
	{
		_repository.Stash.StashedStateCreated -= OnStashCreated;
		Items.Clear();
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			if(_repository != null)
			{
				DetachFromRepository();
				_repository = null;
			}
		}
		base.Dispose(disposing);
	}
}
