﻿namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using gitter.Git.AccessLayer;

	/// <summary>Additional commit description.</summary>
	public sealed class Note : GitLifeTimeNamedObject
	{
		#region Data

		private string _object;
		private string _message;

		#endregion

		/// <summary>Create <see cref="Note"/>.</summary>
		/// <param name="repository">Related <see cref="Repository"/>.</param>
		/// <param name="name">Note object name.</param>
		/// <param name="object">Noted object name.</param>
		/// <param name="message">Note message.</param>
		internal Note(Repository repository, string name, string @object, string message)
			: base(repository, name)
		{
			_object = @object;
			_message = message;
		}

		/// <summary>Create <see cref="Note"/>.</summary>
		/// <param name="repository">Related <see cref="Repository"/>.</param>
		/// <param name="name">Note object name.</param>
		/// <param name="object">Noted object name.</param>
		internal Note(Repository repository, string name, string @object)
			: this(repository, name, @object, null)
		{
		}

		private void LoadContent()
		{
			_message = Repository.Accessor.QueryObjects(
				new QueryObjectsParameters(Name));
		}

		/// <summary>Object id, to which this <see cref="Note"/> is attached.</summary>
		public string Object
		{
			get { return _object; }
			internal set { _object = value; }
		}

		/// <summary>Note contents.</summary>
		public string Message
		{
			get
			{
				if(_message == null)
				{
					LoadContent();
				}
				return _message;
			}
			internal set { _message = value; }
		}
	}
}