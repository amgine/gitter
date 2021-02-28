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

namespace gitter.Git
{
	using System;
	using System.IO;
	using System.Text;
	using System.Threading.Tasks;
	using System.Reflection;

	using gitter.Framework;
	using gitter.Framework.Services;

	using gitter.Git.AccessLayer.CLI;

	public sealed class GitRepositoryUpdateChannel : IUpdateChannel
	{
		private readonly string _url;
		private readonly string _branch;

		sealed class UpdateVersion : IUpdateVersion
		{
			public UpdateVersion(string url, Version version)
			{
				Url     = url;
				Version = version;
			}

			public string Url { get; }

			public Version Version { get; }

			private string FormatUpdaterCommand()
			{
				var sb = new StringBuilder();
				// update driver
				sb.Append(@"/driver:git");
				sb.Append(' ');
				// git exe path
				sb.Append('"');
				sb.Append(@"/git:");
				sb.Append(GitProcess.GitExePath);
				sb.Append('"');
				sb.Append(' ');
				// skip version check
				sb.Append(@"/skipversioncheck");
				sb.Append(' ');
				// remote url
				sb.Append(@"/url:");
				sb.Append(Url);
				sb.Append(' ');
				// install directory
				sb.Append('"');
				sb.Append(@"/target:");
				sb.Append(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
				sb.Append('"');

				return sb.ToString();
			}

			public void Update()
			{
				var command = FormatUpdaterCommand();
				HelperExecutables.LaunchUpdater(command);
			}
		}

		public GitRepositoryUpdateChannel(string url, string branch)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(url, nameof(url));
			Verify.Argument.IsNeitherNullNorWhitespace(branch, nameof(branch));

			_url    = url;
			_branch = branch;
		}

		/// <summary>Check latest gitter version on this channel.</summary>
		/// <returns>Latest gitter version.</returns>
		public async Task<IUpdateVersion> GetLatestVersionAsync()
		{
			Version result = null;
			var cmd = new LsRemoteCommand(
				LsRemoteCommand.Heads(),
				LsRemoteCommand.Tags(),
				new CommandParameter(_url));

			GitOutput output;
			try
			{
				output = await GitProcess
					.ExecuteAsync(new GitInput(cmd))
					.ConfigureAwait(continueOnCapturedContext: false);
			}
			catch
			{
				return null;
			}
			if(output.ExitCode != 0)
			{
				return null;
			}
			var parser = new GitParser(output.Output);
			string branchSHA1 = null;
			while(!parser.IsAtEndOfString)
			{
				var sha1	= parser.ReadString(40, 1);
				var refname	= parser.ReadLine();
				if(branchSHA1 == null)
				{
					if(refname == GitConstants.LocalBranchPrefix + _branch)
					{
						branchSHA1 = sha1;
					}
				}
				else
				{
					if(sha1 == branchSHA1 &&
						refname.Length > GitConstants.TagPrefix.Length + 1 &&
						refname.StartsWith(GitConstants.TagPrefix) &&
						refname[GitConstants.TagPrefix.Length] == 'v')
					{
						var s = GitConstants.TagPrefix.Length + 1;
						var e = refname.Length - 1;
						while(s < refname.Length && !char.IsDigit(refname[s])) ++s;
						while(e > 0 && !char.IsDigit(refname[e])) --e;
						if(e > s && Version.TryParse(refname.Substring(s, e - s + 1), out result))
						{
							break;
						}
						else
						{
							result = null;
						}
					}
				}
			}
			return result != null
				? new UpdateVersion(_url, result)
				: default;
		}
	}
}
