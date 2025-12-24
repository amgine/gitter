#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git.AccessLayer.CLI;

using System;
using System.IO;

static class GitFinder
{
	const string GitExe = "git.exe";
	const string GitCmd = "git.cmd";

	public static string? DetectGitExePath()
		=> TryFindGitInPATH(GitExe)
		?? TryFindGitInPATH(GitCmd)
		?? TryFindGitInDefaultInstallationPath();

	private static string? GetFullPath(string filename)
	{
		var environmentVariable = Environment.GetEnvironmentVariable("PATH");
		if(string.IsNullOrWhiteSpace(environmentVariable)) return default;

#if NETCOREAPP
		var separators = Path.PathSeparator;
#else
		var separators = new char[] { Path.PathSeparator };
#endif
		foreach(string pathEntry in environmentVariable.Split(
			separators, StringSplitOptions.RemoveEmptyEntries))
		{
			try
			{
				string path = Path.Combine(pathEntry, filename);
				if(File.Exists(path))
				{
					return path;
				}
			}
			catch(Exception exc) when(!exc.IsCritical)
			{
			}
		}
		return default;
	}

	private static string? TryFindGitInPATH(string fileName)
	{
		var gitExeFullPath = GetFullPath(fileName);
		if(string.IsNullOrWhiteSpace(gitExeFullPath))
		{
			return default;
		}

		string gitWrapperExecutable = string.Format(@"{0}cmd{0}{1}", Path.DirectorySeparatorChar, fileName);
		if(gitExeFullPath!.EndsWith(gitWrapperExecutable, StringComparison.OrdinalIgnoreCase))
		{
			var realGitExe = Path.Combine(
				gitExeFullPath.Substring(0, gitExeFullPath.Length - gitWrapperExecutable.Length),
				@"bin", GitExe);
			if(File.Exists(realGitExe))
			{
				return realGitExe;
			}
		}
		return gitExeFullPath;
	}

	private static string? TryFindGitInDefaultInstallationPath()
	{
#if NETCOREAPP
		if(!OperatingSystem.IsWindows()) return default;
#endif
		var programFilesPath = Environment.GetFolderPath(
			Environment.Is64BitOperatingSystem
				? Environment.SpecialFolder.ProgramFilesX86
				: Environment.SpecialFolder.ProgramFiles);
		var defaultInstallationPath = Path.Combine(
			programFilesPath, @"Git", @"bin", GitExe);
		return File.Exists(defaultInstallationPath)
			? defaultInstallationPath
			: default;
	}
}
