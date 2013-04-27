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
	/// <summary>What happened with single pushed reference.</summary>
	public enum PushResultType
	{
		/// <summary>Successfully pushed fast-forward.</summary>
		FastForwarded,
		/// <summary>Successful forced update.</summary>
		ForceUpdated,
		/// <summary>Successfully pushed new ref.</summary>
		CreatedReference,
		/// <summary>Successfully deleted ref.</summary>
		DeletedReference,
		/// <summary>Ref was rejected or failed to push.</summary>
		Rejected,
		/// <summary>Ref was up to date and did not need pushing.</summary>
		UpToDate,
	}
}
