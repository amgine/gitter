namespace gitter.Git
{
	using System;
	using System.IO;
	using System.Collections.Generic;

	using gitter.Framework;
	using gitter.Framework.Services;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	public sealed class Tree : GitObject
	{
		private readonly TreeDirectory _root;
		private readonly string _treeHash;

		internal Tree(Repository repository, string treeHash)
			: base(repository)
		{
			if(treeHash == null) throw new ArgumentNullException("treeHash");
			_treeHash = treeHash;
			var strRoot = repository.WorkingDirectory;
			if(strRoot.EndsWith("\\"))
			{
				strRoot = strRoot.Substring(0, strRoot.Length - 1);
			}
			int i = strRoot.LastIndexOf('\\');
			string name = (i != -1) ? strRoot.Substring(i + 1) : strRoot;
			_root = new TreeDirectory(Repository, string.Empty, null, name);
			Refresh();
		} 

		internal Tree(Repository repository)
			: this(repository, GitConstants.HEAD)
		{
		}

		public string TreeHash
		{
			get { return _treeHash; }
		}

		public TreeDirectory Root
		{
			get { return _root; }
		}

		public void Refresh()
		{
			if(Repository.IsEmpty) return;
			var tree = Repository.Accessor.QueryTreeContent(new QueryTreeContentParameters(_treeHash, true, false));
			_root.Files.Clear();
			_root.Directories.Clear();
			var trees = new Dictionary<string, TreeDirectory>();
			foreach(var item in tree)
			{
				int slashPos = item.Name.IndexOf('/');
				string name = (slashPos == -1)?(item.Name):GetName(item.Name);
				var parent = _root;
				while(slashPos != -1)
				{
					string parentPath = item.Name.Substring(0, slashPos);
					TreeDirectory p;
					if(!trees.TryGetValue(parentPath, out p))
					{
						p = new TreeDirectory(Repository, parentPath, parent, GetName(parentPath));
						parent.AddDirectory(p);
						trees.Add(parentPath, p);
					}
					parent = p;
					slashPos = item.Name.IndexOf('/', slashPos + 1);
				}
				switch(item.Type)
				{
					case TreeContentType.Tree:
						var wtf = new TreeDirectory(Repository, item.Name, parent, name);
						trees.Add(item.Name, wtf);
						parent.AddDirectory(wtf);
						break;
					case TreeContentType.Blob:
						parent.AddFile(new TreeFile(Repository, item.Name, parent, FileStatus.Cached, name, ((BlobData)item).Size));
						break;
				}
			}
		}

		private static string GetName(string path)
		{
			var index = path.LastIndexOf('/');
			return (index == -1)?path:path.Substring(index + 1);
		}

		public byte[] GetBlobContent(string blobPath)
		{
			return Repository.Accessor.QueryBlobBytes(
				new QueryBlobBytesParameters()
				{
					Treeish = _treeHash,
					ObjectName = blobPath,
				});
		}
	}
}
