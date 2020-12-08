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

namespace gitter.Git.Gui.Controls
{
	using System.Drawing;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	public sealed class UsersListBox : CustomListBox
	{
		#region Data

		private readonly CustomListBoxColumn _colName;
		private readonly CustomListBoxColumn _colEmail;
		private readonly CustomListBoxColumn _colCommits;

		private Repository _repository;
		private UserListBinding _binding;

		#endregion

		#region .ctor

		public UsersListBox()
		{
			Columns.AddRange(
				new[]
				{
					_colName    = new NameColumn(Resources.StrName),
					_colEmail   = new EmailColumn(),
					_colCommits = new CustomListBoxColumn((int)ColumnId.Commits, Resources.StrCommits) { Width = 80 },
				});
		}

		#endregion

		public void Load(Repository repository)
		{
			if(_repository != repository)
			{
				if(_repository != null)
				{
					DetachFromRepositoy();
				}
				_repository = repository;
				if(_repository != null)
				{
					AttachToRepository();
				}
			}
		}

		private void AttachToRepository()
		{
			BeginUpdate();
			_binding = new UserListBinding(Items, _repository);
			EndUpdate();
		}

		private void DetachFromRepositoy()
		{
			BeginUpdate();
			_binding.Dispose();
			_binding = null;
			EndUpdate();
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_repository != null)
				{
					_binding.Dispose();
					_binding = null;
					_repository = null;
				}
			}
			base.Dispose(disposing);
		}
	}
}
