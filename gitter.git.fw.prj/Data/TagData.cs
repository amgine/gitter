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

namespace gitter.Git.AccessLayer
{
	using System;

	using gitter.Framework;

	/// <summary>Tag description.</summary>
	public sealed class TagData : INamedObject
	{
		#region .ctor

		public TagData(string name, Hash sha1, TagType tagType)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(name, nameof(name));

			Name    = name;
			SHA1    = sha1;
			TagType = tagType;
		}

		#endregion

		#region Properties

		public string Name { get; }

		public Hash SHA1 { get; }

		public TagType TagType { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString() => Name;

		#endregion
	}
}
