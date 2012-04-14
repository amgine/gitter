namespace gitter.Redmine
{
	using System;
	using System.Collections.Generic;

	public sealed class UserMembership
	{
		private readonly User _user;
		private readonly Project _project;
		private readonly IEnumerable<UserRole> _userRoles;

		internal UserMembership(User user, Project project, IEnumerable<UserRole> userRoles)
		{
			_user = user;
			_project = project;
			_userRoles = userRoles;
		}

		public User User
		{
			get { return _user; }
		}

		public Project Project
		{
			get { return _project; }
		}

		public IEnumerable<UserRole> UserRoles
		{
			get { return _userRoles; }
		}
	}
}
