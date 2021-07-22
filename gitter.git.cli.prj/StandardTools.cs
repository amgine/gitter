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
	using gitter.Git.AccessLayer.CLI;

	public static class StandardTools
	{
		public static void StartGitGui(string workingDirectory)
			=> GitProcess.ExecNormal(new GitInput(workingDirectory, new Command("gui"))).Dispose();

		public static bool CanStartGitk => GitProcess.CanExecGitk;

		public static void StartGitk(string workingDirectory)
			=> GitProcess.ExecGitk(workingDirectory, "--all").Dispose();

		public static bool CanStartBash => GitProcess.CanExecSh;

		public static void StartBash(string workingDirectory)
			=> GitProcess.ExecSh(workingDirectory, "--login -i").Dispose();
	}
}
