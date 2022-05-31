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

namespace gitter.Git.AccessLayer;

using System;
using System.Collections.Generic;

public sealed class FormatMergeMessageParameters
{
	public FormatMergeMessageParameters()
	{
	}

	public FormatMergeMessageParameters(string revision, string headRefName)
	{
		Revisions = new string[] { revision };
		HeadReference = headRefName;
	}

	public FormatMergeMessageParameters(IReadOnlyList<string> revisions, string headRefName)
	{
		Revisions     = revisions;
		HeadReference = headRefName;
	}

	public IReadOnlyList<string> Revisions { get; set; }

	public string HeadReference { get; set; }
}
