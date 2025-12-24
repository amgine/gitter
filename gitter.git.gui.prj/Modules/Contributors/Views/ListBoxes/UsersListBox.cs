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

using Resources = Properties.Resources;

sealed class UsersListBox : CustomListBox
{
	private Repository? _repository;
	private UserListBinding? _binding;

	public UsersListBox()
	{
		Columns.AddRange(
			[
				new NameColumn(),
				new EmailColumn(new UserEmailPainter()),
				new CustomListBoxColumn((int)ColumnId.Commits, Resources.StrCommits) { Width = 80 },
			]);
	}

	public void Load(Repository? repository)
	{
		if(_repository == repository) return;

		if(_repository is not null)
		{
			DetachFromRepositoy();
		}
		_repository = repository;
		if(_repository is not null)
		{
			AttachToRepository(_repository);
		}
	}

	private void AttachToRepository(Repository repository)
	{
		BeginUpdate();
		_binding = new UserListBinding(Items, repository);
		EndUpdate();
	}

	private void DetachFromRepositoy()
	{
		BeginUpdate();
		DisposableUtility.Dispose(ref _binding);
		EndUpdate();
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			if(_repository is not null)
			{
				DisposableUtility.Dispose(ref _binding);
				_repository = null;
			}
		}
		base.Dispose(disposing);
	}
}
