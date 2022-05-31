#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

using gitter.Framework.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

/// <summary><see cref="CustomListBox"/> for displaying <see cref="Repository.Remotes"/> content.</summary>
public sealed class RemotesListBox : CustomListBox
{
	private Repository _repository;
	private RemoteListBinding _binding;

	/// <summary>Initializes a new instance of the <see cref="RemotesListBox"/> class.</summary>
	public RemotesListBox()
	{
		Text = Resources.StrsNoRemotes;
	}

	private void AttachToRepository()
	{
		BeginUpdate();
		_binding = new RemoteListBinding(Items, _repository);
		EndUpdate();
	}

	private void DetachFromRepository()
	{
		if(_binding is not null)
		{
			_binding.Dispose();
			_binding = null;
		}
	}

	public void LoadData(Repository repository)
	{
		if(_repository is not null)
		{
			DetachFromRepository();
		}
		_repository = repository;
		if(_repository is not null)
		{
			AttachToRepository();
		}
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
