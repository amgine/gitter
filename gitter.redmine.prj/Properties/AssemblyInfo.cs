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

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("gitter.redmine")]
[assembly: AssemblyDescription("Redmine support lib")]

[assembly: ComVisible(false)]
[assembly: Guid("0bb4a887-3ef6-4cc1-990a-26d6c2d5b6ef")]

#if NET6_0_OR_GREATER
[module: System.Runtime.Versioning.SupportedOSPlatform("windows")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("", "WFAC010")]
#endif
