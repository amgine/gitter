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
	using System.Collections.Generic;

	/// <summary>Results of pushing local reference to remote repository.</summary>
	public sealed class ReferencePushResult
	{
		private readonly PushResultType _type;
		private readonly string _localRefName;
		private readonly string _remoteRefName;
		private readonly string _summary;

		public ReferencePushResult(PushResultType type, string localRefName, string remoteRefName, string summary)
		{
			_type = type;
			_localRefName = localRefName;
			_remoteRefName = remoteRefName;
			_summary = summary;
		}

		public PushResultType Type
		{
			get { return _type; }
		}

		public string LocalRefName
		{
			get { return _localRefName; }
		}

		public string RemoteRefName
		{
			get { return _remoteRefName; }
		}

		public string Summary
		{
			get { return _summary; }
		}

		private static char TypeToChar(PushResultType type)
		{
			switch(type)
			{
				case PushResultType.ForceUpdated:
					return '+';
				case PushResultType.FastForwarded:
					return ' ';
				case PushResultType.Rejected:
					return '!';
				case PushResultType.UpToDate:
					return '=';
				case PushResultType.DeletedReference:
					return '-';
				case PushResultType.CreatedReference:
					return '*';
				default:
					throw new ArgumentException("type");
			}
		}

		public override string ToString()
		{
			return string.Format("{0} {1} -> {2} {3}", TypeToChar(_type), _localRefName, _remoteRefName, _summary);
		}
	}
}
