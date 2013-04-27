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
	using System.Collections.Generic;
	using System.Globalization;

	using gitter.Framework.CLI;

	using gitter.Git.AccessLayer.CLI;

	internal sealed partial class RepositoryCLI : ITreeAccessor
	{
		/// <summary>Checkout files from tree object to working directory.</summary>
		/// <param name="parameters"><see cref="CheckoutFilesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void CheckoutFiles(CheckoutFilesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>((parameters.Paths != null ? parameters.Paths.Count + 1 : 0) + 2);
			switch(parameters.Mode)
			{
				case CheckoutFileMode.Ours:
					args.Add(CheckoutCommand.Ours());
					break;
				case CheckoutFileMode.Theirs:
					args.Add(CheckoutCommand.Theirs());
					break;
				case CheckoutFileMode.Merge:
					args.Add(CheckoutCommand.Merge());
					break;
				case CheckoutFileMode.IgnoreUnmergedEntries:
					args.Add(CheckoutCommand.Force());
					break;
			}
			if(!string.IsNullOrEmpty(parameters.Revision))
			{
				args.Add(new CommandArgument(parameters.Revision));
			}
			if(parameters.Paths != null && parameters.Paths.Count != 0)
			{
				args.Add(CheckoutCommand.NoMoreOptions());
				foreach(var path in parameters.Paths)
				{
					args.Add(new PathCommandArgument(path));
				}
			}

			var cmd = new CheckoutCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
		}

		/// <summary>Queries the BLOB bytes.</summary>
		/// <param name="parameters">The parameters.</param>
		/// <returns>Requested blob content.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public byte[] QueryBlobBytes(QueryBlobBytesParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var cmd = new CatFileCommand(
				new CommandArgument(GitConstants.BlobObjectType),
				new CommandArgument(parameters.Treeish + ":" + parameters.ObjectName));

			var stdOutReceiver = new AsyncBytesReader();
			var stdErrReceiver = new AsyncTextReader();
			var executor = GitProcess.CreateExecutor(
				stdOutReceiver, stdErrReceiver);
			var code = executor.Execute(new GitInput(_repository.WorkingDirectory, cmd));
			if(code != 0)
			{
				var output = new GitOutput(string.Empty, stdErrReceiver.GetText(), code);
				output.ThrowOnBadReturnCode();
			}

			return stdOutReceiver.GetBytes();
		}

		/// <summary>Get objects contained in a tree.</summary>
		/// <param name="parameters"><see cref="QueryTreeContentParameters"/>.</param>
		/// <returns><see cref="TreeData"/> representing requested tree.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<TreeContentData> QueryTreeContent(QueryTreeContentParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>();

			if(parameters.OnlyTrees)
			{
				args.Add(LsTreeCommand.Directories());
			}
			if(parameters.Recurse)
			{
				args.Add(LsTreeCommand.Recurse());
			}
			args.Add(LsTreeCommand.Tree());
			args.Add(LsTreeCommand.FullName());
			args.Add(LsTreeCommand.Long());
			args.Add(LsTreeCommand.NullTerminate());
			args.Add(new CommandArgument(parameters.TreeId));
			if(parameters.Paths != null)
			{
				foreach(var path in parameters.Paths)
				{
					args.Add(new PathCommandArgument(path));
				}
			}

			var cmd = new LsTreeCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();

			var content = output.Output;
			int pos = 0;
			int l = content.Length;
			var res = new List<TreeContentData>();
			// <mode> SP <type> SP <object> SP <object size> TAB <file>
			while(pos < l)
			{
				var end = content.IndexOf('\0', pos);
				if(end == -1) end = l;

				int delimeter = content.IndexOf(' ', pos);
				int mode = int.Parse(content.Substring(pos, delimeter - pos), CultureInfo.InvariantCulture);
				pos = delimeter + 1;
				while(content[pos] == ' ') ++pos;

				bool isTree		= CheckValue(content, pos, GitConstants.TreeObjectType);
				bool isBlob		= !isTree && CheckValue(content, pos, GitConstants.BlobObjectType);
				bool isCommit	= !isTree && !isBlob && CheckValue(content, pos, GitConstants.CommitObjectType);
				bool isTag		= !isTree && !isBlob && !isCommit && CheckValue(content, pos, GitConstants.TagObjectType);

				pos += 5;
				delimeter = content.IndexOf(' ', pos);
				var hash = content.Substring(pos, delimeter - pos);
				pos += 41;
				while(content[pos] == ' ') ++pos;
				delimeter = content.IndexOf('\t', pos);
				long size = 0;
				if(isBlob)
				{
					size = long.Parse(content.Substring(pos, delimeter - pos), CultureInfo.InvariantCulture);
				}
				pos = delimeter + 1;
				var name = content.Substring(pos, end - pos);
				if(isBlob)
				{
					res.Add(new BlobData(hash, mode, name, size));
				}
				else if(isTree)
				{
					res.Add(new TreeData(hash, mode, name));
				}
				else if(isCommit)
				{
					res.Add(new TreeCommitData(hash, mode, name));
				}
				pos = end + 1;
			}

			return res;
		}
	}
}
