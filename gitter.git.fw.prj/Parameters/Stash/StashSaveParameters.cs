﻿#region Copyright Notice
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

	/// <summary>Parameters for <see cref="IRepositoryAccessor.StashSave"/> operation.</summary>
	public sealed class StashSaveParameters
	{
		/// <summary>Create <see cref="StashSaveParameters"/>.</summary>
		public StashSaveParameters()
		{
		}

		/// <summary>Create <see cref="StashSaveParameters"/>.</summary>
		/// <param name="message">Custom stash message.</param>
		/// <param name="keepIndex">Do not stash staged changes.</param>
		/// <param name="includeUntracked">Include untracked files in stash.</param>
		public StashSaveParameters(string message, bool keepIndex, bool includeUntracked)
		{
			Message          = message;
			KeepIndex        = keepIndex;
			IncludeUntracked = includeUntracked;
		}

		/// <summary>Custom stash message.</summary>
		public string Message { get; set; }

		/// <summary>Do not stash staged changes.</summary>
		public bool KeepIndex { get; set; }

		/// <summary>Include untracked files in stash.</summary>
		public bool IncludeUntracked { get; set; }
	}
}
