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

	public sealed class RebaseParameters
	{
		public RebaseParameters(RebaseControl control)
		{
			Control = control;
		}

		public RebaseParameters(string onto, string branch, string target)
		{
			NewBase = onto;
			Branch = branch;
			Upstream = target;
		}

		public RebaseParameters(string branch, string target)
		{
			Branch = branch;
			Upstream = target;
		}

		public RebaseParameters(string target)
		{
			Upstream = target;
		}

		public RebaseControl? Control { get; set; }

		public string NewBase { get; set; }

		public string Branch { get; set; }

		public string Upstream { get; set; }
	}
}
