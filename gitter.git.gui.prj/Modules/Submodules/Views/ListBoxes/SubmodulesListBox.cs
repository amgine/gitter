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

using gitter.Framework;
using gitter.Framework.Controls;

public sealed class SubmodulesListBox : CustomListBox
{
	private Repository? _repository;
	private SubmoduleListBinding? _binding;

	/// <summary>Create <see cref="SubmodulesListBox"/>.</summary>
	public SubmodulesListBox()
	{
		Items.Comparison = SubmoduleListItem.CompareByName;
	}

	private void DetachFromRepository()
	{
		DisposableUtility.Dispose(ref _binding);
	}

	private void AttachToRepository(Repository repository)
	{
		_binding = new SubmoduleListBinding(Items, repository);
	}

	public void Load(Repository? repository)
	{
		if(_repository == repository) return;

		if(_repository is not null)
		{
			DetachFromRepository();
		}
		_repository = repository;
		if(_repository is not null)
		{
			AttachToRepository(_repository);
		}
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			if(_repository is not null)
			{
				DetachFromRepository();
				_repository = null;
			}
		}
		base.Dispose(disposing);
	}
}
