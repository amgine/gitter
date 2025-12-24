#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2020  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

[assembly: AssemblyCompany("amgine")]
[assembly: AssemblyProduct("gitter")]
[assembly: AssemblyCopyright("Copyright © amgine 2024")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: NeutralResourcesLanguage("en-us")]

[assembly: AssemblyFileVersion("1.1.5.0")]
[assembly: AssemblyVersion("1.1.5.0")]

#if NET6_0_OR_GREATER
[module: System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif

[assembly: InternalsVisibleTo("gitter.tests")]
