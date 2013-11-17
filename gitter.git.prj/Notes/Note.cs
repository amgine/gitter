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

namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using gitter.Git.AccessLayer;

	/// <summary>Additional commit description.</summary>
	public sealed class Note : GitNamedObjectWithLifetime
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
			_message = Repository.Accessor.QueryObjects.Invoke(
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
