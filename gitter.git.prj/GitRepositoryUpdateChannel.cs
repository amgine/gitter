using System;
using System.IO;
using System.Text;
using System.Reflection;

using gitter.Framework;
using gitter.Framework.Services;

using gitter.Git.AccessLayer.CLI;

namespace gitter.Git
{
	public sealed class GitRepositoryUpdateChannel : IUpdateChannel
	{
		private readonly string _url;

		public GitRepositoryUpdateChannel(string url)
		{
			_url = url;
		}

		/// <summary>Check latest gitter version on this chanel.</summary>
		/// <returns>Latest gitter version.</returns>
		public Version CheckVersion()
		{
			Version result = null;
			var cmd = new LsRemoteCommand(
				LsRemoteCommand.Heads(),
				LsRemoteCommand.Tags(),
				new CommandArgument(_url));

			GitOutput output;
			try
			{
				output = GitProcess.Exec(new GitInput(cmd));
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
			string masterSHA1 = null;
			while(!parser.IsAtEndOfString)
			{
				var sha1	= parser.ReadString(40, 1);
				var refname	= parser.ReadLine();
				if(masterSHA1 == null)
				{
					if(refname == GitConstants.LocalBranchPrefix + "master")
					{
						masterSHA1 = sha1;
					}
				}
				else
				{
					if(sha1 == masterSHA1 &&
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
			return result;
		}

		/// <summary>Update gitter using this channel.</summary>
		public void Update()
		{
			var sb = new StringBuilder();
			// update driver
			sb.Append(@"/driver:git");
			sb.Append(' ');
			// git exe path
			if(!RepositoryProvider.AutodetectGitExePath)
			{
				sb.Append('"');
				sb.Append(@"/git:");
				sb.Append(RepositoryProvider.ManualGitExePath);
				sb.Append('"');
				sb.Append(' ');
			}
			// skip version check
			sb.Append(@"/skipversioncheck");
			sb.Append(' ');
			// remote url
			sb.Append(@"/url:");
			sb.Append(_url);
			sb.Append(' ');
			// install directory
			sb.Append('"');
			sb.Append(@"/target:");
			sb.Append(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
			sb.Append('"');

			Utility.LaunchUpdater(sb.ToString());
		}
	}
}
