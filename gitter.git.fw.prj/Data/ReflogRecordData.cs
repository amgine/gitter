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

	public sealed class ReflogRecordData
	{
		#region Data

		private readonly int _index;
		private readonly string _message;
		private readonly RevisionData _revision;

		#endregion

		#region .ctor

		public ReflogRecordData(int index, string message, RevisionData revision)
		{
			_index = index;
			_message = message;
			_revision = revision;
		}

		#endregion

		#region Properties

		public string Name
		{
			get { return GitConstants.StashFullName + "@{" + _index + "}"; }
		}

		public string Message
		{
			get { return _message; }
		}

		public int Index
		{
			get { return _index; }
		}

		public RevisionData Revision
		{
			get { return _revision; }
		}

		#endregion
	}
}
