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

namespace gitter.Git.Gui
{
	using System;

	using gitter.Framework.Controls;

	sealed class RepositoryExplorer
	{
		#region Data

		private readonly GuiProvider _guiProvider;
		private readonly RepositoryRootItem _rootItem;
		private Repository _repository;

		#endregion

		public RepositoryExplorer(GuiProvider guiProvider)
		{
			Verify.Argument.IsNotNull(guiProvider, nameof(guiProvider));

			_guiProvider = guiProvider;
			_rootItem = new RepositoryRootItem(_guiProvider.Environment)
				{
					Repository = guiProvider.Repository,
				};
			_repository = guiProvider.Repository;

			_rootItem.IsExpanded = true;
		}

		public CustomListBoxItem RootItem
		{
			get { return _rootItem; }
		}

		public Repository Repository
		{
			get { return _repository; }
			set
			{
				if(_repository != value)
				{
					if(_repository != null)
					{
						DetachFromRepository(_repository);
					}
					if(value != null)
					{
						AttachToRepository(value);
					}
				}
			}
		}

		private void AttachToRepository(Repository repository)
		{
			_repository = repository;
			_rootItem.Repository = repository;
		}

		private void DetachFromRepository(Repository repository)
		{
			_repository = null;
			_rootItem.Repository = null;
		}
	}
}
