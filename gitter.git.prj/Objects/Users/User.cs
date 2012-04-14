namespace gitter.Git
{
	using System;
	using System.Drawing;

	using gitter.Framework;
	using gitter.Framework.Services;

	/// <summary>Committer/Author.</summary>
	public sealed class User : GitLifeTimeNamedObject
	{
		#region Data

		private readonly string _name;
		private readonly string _email;
		private int _commits;
		private IAvatar _avatar;

		#endregion

		#region Events

		/// <summary><see cref="Commits"/> has changed.</summary>
		public event EventHandler CommitsChanged;

		#endregion

		/// <summary>Create <see cref="User"/>.</summary>
		/// <param name="repository">Related repository.</param>
		/// <param name="name">User name.</param>
		/// <param name="email">User email.</param>
		/// <param name="commits">Commit count.</param>
		internal User(Repository repository, string name, string email, int commits)
			: base(repository, name + "\n" + email)
		{
			_name = name;
			_email = email;
			_commits = commits;
			_avatar = new Gravatar(email);
		}

		/// <summary>User name.</summary>
		public new string Name
		{
			get { return _name; }
		}

		/// <summary>User email.</summary>
		public string Email
		{
			get { return _email; }
		}

		/// <summary>User commit count.</summary>
		public int Commits
		{
			get { return _commits; }
			internal set
			{
				if(_commits != value)
				{
					_commits = value;
					CommitsChanged.Raise(this);
				}
			}
		}

		/// <summary>Avatar.</summary>
		public IAvatar Avatar
		{
			get { return _avatar; }
		}
	}
}
