namespace gitter.Git.AccessLayer
{
	using System;

	using gitter.Framework;

	public sealed class UserData : INamedObject
	{
		#region Data

		private readonly string _name;
		private string _email;
		private int _commits;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="UserData"/>.</summary>
		/// <param name="name">User name.</param>
		/// <param name="email">User email.</param>
		/// <param name="commits">Commit count.</param>
		public UserData(string name, string email, int commits)
		{
			_name = name;
			_email = email;
			_commits = commits;
		}

		#endregion

		#region Properties

		/// <summary>User id.</summary>
		public string Name
		{
			get { return _name + "\n" + _email; }
		}

		/// <summary>User name.</summary>
		public string UserName
		{
			get { return _name; }
		}

		/// <summary>User email.</summary>
		public string Email
		{
			get { return _email; }
			set { _email = value; }
		}

		/// <summary>Commit count.</summary>
		public int Commits
		{
			get { return _commits; }
			set { _commits = value; }
		}

		#endregion

		public override string ToString()
		{
			return string.Format("{0} <{1}>", _name, _email);
		}
	}
}
