namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using gitter.Git.AccessLayer.CLI;

	internal sealed partial class RepositoryCLI : ITreeAccessor
	{
		/// <summary>Checkout files from tree object to working directory.</summary>
		/// <param name="parameters"><see cref="CheckoutFilesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void CheckoutFiles(CheckoutFilesParameters parameters)
		{
			if(parameters == null) throw new ArgumentNullException("parameters");

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
			if(parameters == null) throw new ArgumentNullException("parameters");

			var cmd = new CatFileCommand(
				new CommandArgument(GitConstants.BlobObjectType),
				new CommandArgument(parameters.Treeish + ":" + parameters.ObjectName));

			var output = _executor.ExecCommand(cmd, Encoding.Default);
			output.ThrowOnBadReturnCode();

			return Encoding.Default.GetBytes(output.Output);
		}

		/// <summary>Get objects contained in a tree.</summary>
		/// <param name="parameters"><see cref="QueryTreeContentParameters"/>.</param>
		/// <returns><see cref="TreeData"/> representing requested tree.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<TreeContentData> QueryTreeContent(QueryTreeContentParameters parameters)
		{
			if(parameters == null) throw new ArgumentNullException("parameters");

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
			var output = _executor.ExecCommand(cmd, Encoding.Default);
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
				int mode = int.Parse(content.Substring(pos, delimeter - pos), System.Globalization.CultureInfo.InvariantCulture);
				pos = delimeter + 1;
				while(content[pos] == ' ') ++pos;

				bool isTree = CheckValue(content, pos, GitConstants.TreeObjectType);
				bool isBlob = !isTree & CheckValue(content, pos, GitConstants.BlobObjectType);
				bool isCommit = !isTree & !isBlob & CheckValue(content, pos, GitConstants.CommitObjectType);
				bool isTag = !isTree & !isBlob & !isCommit & CheckValue(content, pos, GitConstants.TagObjectType);

				pos += 5;
				delimeter = content.IndexOf(' ', pos);
				var hash = content.Substring(pos, delimeter - pos);
				pos += 41;
				while(content[pos] == ' ') ++pos;
				delimeter = content.IndexOf('\t', pos);
				long size = 0;
				if(isBlob)
					size = long.Parse(content.Substring(pos, delimeter - pos), System.Globalization.CultureInfo.InvariantCulture);
				pos = delimeter + 1;
				var name = content.Substring(pos, end - pos);
				if(isBlob)
				{
					res.Add(new BlobData(hash, mode, name, size));
				}
				else if(isTree/* || isCommit || isTag*/)
				{
					res.Add(new TreeData(hash, mode, name));
				}
				pos = end + 1;
			}

			return res;
		}
	}
}
