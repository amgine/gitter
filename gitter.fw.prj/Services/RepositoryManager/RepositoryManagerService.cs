#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework.Services
{
	using System;
	using System.Globalization;
	using System.IO;
	using gitter.Framework.Configuration;

	public sealed class RepositoryManagerService
	{
		private readonly RepositoryCollection _recent;
		private readonly RepositoryGroup _local;
		private int _maxRecentREpositories;

		public RepositoryManagerService(int maxRecentRepositories = 10)
		{
			_maxRecentREpositories = maxRecentRepositories;
			_recent = new RepositoryCollection();
			_local  = new RepositoryGroup("ROOT");
		}

		public RepositoryCollection RecentRepositories
		{
			get { return _recent; }
		}

		public RepositoryGroup LocalRepositories
		{
			get { return _local; }
		}

		public void RegisterRecentRepository(string repositoryPath)
		{
			if(string.IsNullOrWhiteSpace(repositoryPath))
			{
				return;
			}
			if(repositoryPath.EndsWith(Path.DirectorySeparatorChar) || repositoryPath.EndsWith(Path.AltDirectorySeparatorChar))
			{
				repositoryPath = repositoryPath.Substring(0, repositoryPath.Length - 1);
			}
			var repository = new RepositoryLink(repositoryPath, @"");
			RegisterRecentRepository(repository);
		}

		public void RegisterRecentRepository(RepositoryLink repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			int id = -1;
			for(int i = 0; i < _recent.Count; ++i)
			{
				if(string.Compare(_recent[i].Path, repository.Path, true) == 0)
				{
					id = i;
					break;
				}
			}
			if(id == 0)
			{
				return;
			}
			if(id != -1)
			{
				_recent.RemoveAt(id);
			}
			_recent.Insert(0, repository);
			var diff = _recent.Count - _maxRecentREpositories;
			if(diff > 0)
			{
				_recent.RemoveRange(_recent.Count - diff, diff);
			}
		}

		private static RepositoryGroup TryLoadGroupFrom(Section section)
		{
			Assert.IsNotNull(section);

			var name = section.GetValue<string>("Name", string.Empty);
			if(string.IsNullOrWhiteSpace(name))
			{
				return null;
			}
			var group = new RepositoryGroup(name.Trim());
			LoadGroupContent(group, section);
			return group;
		}

		private static void LoadGroupContent(RepositoryGroup group, Section section)
		{
			Assert.IsNotNull(group);
			Assert.IsNotNull(section);

			group.Groups.Clear();
			group.Respositories.Clear();
			var groups = section.TryGetSection("Groups");
			if(groups != null)
			{
				foreach(var subGroupSection in groups.Sections)
				{
					var subGroup = TryLoadGroupFrom(subGroupSection);
					if(subGroup != null)
					{
						group.Groups.Add(subGroup);
					}
				}
			}
			var repositories = section.TryGetSection("Repositories");
			if(repositories != null)
			{
				foreach(var repositorySection in repositories.Sections)
				{
					var repository = TryLoadRepositoryFrom(repositorySection);
					if(repository != null)
					{
						group.Respositories.Add(repository);
					}
				}
			}
		}

		private static void SaveGroupTo(RepositoryGroup group, Section section)
		{
			Assert.IsNotNull(group);
			Assert.IsNotNull(section);

			section.SetValue<string>("Name", group.Name);
			SaveGroupContentTo(group, section);
		}

		private static void SaveGroupContentTo(RepositoryGroup group, Section section)
		{
			Assert.IsNotNull(group);
			Assert.IsNotNull(section);

			int id;
			var groupsSection = section.GetCreateEmptySection("Groups");
			id = 0;
			foreach(var subGroup in group.Groups)
			{
				var subGroupSection = groupsSection.CreateSection(string.Format(CultureInfo.InvariantCulture, "Group{0}", id++));
				SaveGroupTo(subGroup, subGroupSection);
			}
			var repositoriesSection = section.GetCreateEmptySection("Repositories");
			id = 0;
			foreach(var repository in group.Respositories)
			{
				var repositorySection = repositoriesSection.CreateSection(string.Format(CultureInfo.InvariantCulture, "Repository{0}", id++));
				SaveRepositoryTo(repository, repositorySection);
			}
		}

		private static void SaveRepositoryTo(RepositoryLink repository, Section section)
		{
			Assert.IsNotNull(repository);
			Assert.IsNotNull(section);

			repository.SaveTo(section);
		}

		private static RepositoryLink TryLoadRepositoryFrom(Section section)
		{
			Assert.IsNotNull(section);

			return new RepositoryLink(section);
		}

		public void LoadFrom(Section section)
		{
			Verify.Argument.IsNotNull(section, "section");

			var localRepositories = section.TryGetSection("LocalRepositories");
			if(localRepositories != null)
			{
				LoadGroupContent(_local, localRepositories);
			}
			var recentRepositories = section.TryGetSection("RecentRepositories");
			if(recentRepositories != null)
			{
				_recent.Clear();
				foreach(var repositorySection in recentRepositories.Sections)
				{
					var repository = TryLoadRepositoryFrom(repositorySection);
					if(repository != null)
					{
						_recent.Add(repository);
					}
				}
			}
		}

		public void SaveTo(Section section)
		{
			Verify.Argument.IsNotNull(section, "section");

			var localRepositories = section.GetCreateEmptySection("LocalRepositories");
			if(_local.Groups.Count != 0 && _local.Respositories.Count != 0)
			{
				SaveGroupTo(_local, localRepositories);
			}
			var recentRepositories = section.GetCreateEmptySection("RecentRepositories");
			if(_recent.Count != 0)
			{
				int id = 0;
				foreach(var repository in _recent)
				{
					var repositorySection = recentRepositories.CreateSection(string.Format(CultureInfo.InvariantCulture, "Repository{0}", id++));
					SaveRepositoryTo(repository, repositorySection);
				}
			}
		}
	}
}
