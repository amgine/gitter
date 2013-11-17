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

namespace gitter.Git
{
	using System;
	using System.IO;
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;
	using gitter.Framework.Services;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	public sealed class Tree : GitObject
	{
		#region Data

		private readonly TreeDirectory _root;
		private readonly string _treeHash;

		#endregion

		public static Task<Tree> GetAsync(Repository repository, string treeHash, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			if(progress != null)
			{
				progress.Report(new OperationProgress(Resources.StrsFetchingTree.AddEllipsis()));
			}
			var parameters = new QueryTreeContentParameters(treeHash, true, false);
			return repository.Accessor.QueryTreeContent.InvokeAsync(parameters, progress, cancellationToken)
				.ContinueWith(
				t =>
				{
					var treeData = TaskUtility.UnwrapResult(t);
					var tree = new Tree(repository, treeHash, false);
					tree.SetContent(treeData);
					return tree;
				},
				cancellationToken,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		#region .ctor

		private Tree(Repository repository, string treeHash, bool load)
			: base(repository)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(treeHash, "treeHash");

			_treeHash = treeHash;
			var strRoot = repository.WorkingDirectory;
			if(strRoot.EndsWith("\\"))
			{
				strRoot = strRoot.Substring(0, strRoot.Length - 1);
			}
			int i = strRoot.LastIndexOf('\\');
			string name = (i != -1) ? strRoot.Substring(i + 1) : strRoot;
			_root = new TreeDirectory(Repository, string.Empty, null, name);
			if(load)
			{
				Refresh();
			}
		}

		internal Tree(Repository repository, string treeHash)
			: this(repository, treeHash, true)
		{
		} 

		internal Tree(Repository repository)
			: this(repository, GitConstants.HEAD)
		{
		}

		#endregion

		#region Properties

		public string TreeHash
		{
			get { return _treeHash; }
		}

		public TreeDirectory Root
		{
			get { return _root; }
		}

		#endregion

		#region Methods

		private void SetContent(IList<TreeContentData> tree)
		{
			Root.Files.Clear();
			Root.Directories.Clear();
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
						{
							var dir = new TreeDirectory(Repository, item.Name, parent, name);
							trees.Add(item.Name, dir);
							parent.AddDirectory(dir);
						}
						break;
					case TreeContentType.Blob:
						{
							var blob = new TreeFile(Repository, item.Name, parent, FileStatus.Cached, name, ((BlobData)item).Size);
							parent.AddFile(blob);
						}
						break;
					case TreeContentType.Commit:
						{
							var commit = new TreeCommit(Repository, item.Name, parent, FileStatus.Cached, name);
							parent.AddCommit(commit);
						}
						break;
				}
			}
		}

		public void Refresh()
		{
			if(Repository.IsEmpty) return;
			var tree = Repository.Accessor.QueryTreeContent.Invoke(
				new QueryTreeContentParameters(_treeHash, true, false));
			SetContent(tree);
		}

		private static string GetName(string path)
		{
			var index = path.LastIndexOf('/');
			return (index == -1)?path:path.Substring(index + 1);
		}

		public byte[] GetBlobContent(string blobPath)
		{
			return Repository.Accessor.QueryBlobBytes.Invoke(
				new QueryBlobBytesParameters()
				{
					Treeish    = TreeHash,
					ObjectName = blobPath,
				});
		}

		public Task<byte[]> GetBlobContentAsync(string blobPath, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			if(progress != null)
			{
				progress.Report(new OperationProgress(Resources.StrsFetchingBlob.AddEllipsis()));
			}
			return Repository.Accessor.QueryBlobBytes.InvokeAsync(
				new QueryBlobBytesParameters()
				{
					Treeish    = TreeHash,
					ObjectName = blobPath,
				},
				progress,
				cancellationToken);
		}

		#endregion
	}
}
