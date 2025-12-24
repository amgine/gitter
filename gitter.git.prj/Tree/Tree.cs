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

namespace gitter.Git;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using gitter.Framework;

using gitter.Git.AccessLayer;

using Resources = gitter.Git.Properties.Resources;

public sealed class Tree : GitObject
{
	private readonly TreeDirectory _root;
	private readonly string _treeHash;

	public static async Task<Tree> GetAsync(Repository repository, string treeHash,
		IProgress<OperationProgress>? progress = default, CancellationToken cancellationToken = default)
	{
		Verify.Argument.IsNotNull(repository);

		progress?.Report(new OperationProgress(Resources.StrsFetchingTree.AddEllipsis()));
		var request = new QueryTreeContentRequest(treeHash, recurse: true, onlyTrees: false);
		var treeData = await repository.Accessor.QueryTreeContent
			.InvokeAsync(request, progress, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		var tree = new Tree(repository, treeHash, load: false);
		tree.SetContent(treeData);
		return tree;
	}

	private Tree(Repository repository, string treeHash, bool load)
		: base(repository)
	{
		Verify.Argument.IsNeitherNullNorWhitespace(treeHash);

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

	public string TreeHash => _treeHash;

	public TreeDirectory Root => _root;

	private TreeDirectory GetDirectory(string path, Dictionary<string, TreeDirectory> cache)
	{
		var sindex = path.LastIndexOf('/');
		TreeDirectory? dir, parent;
		string name;
		if(sindex < 0)
		{
			name = path;
			parent = Root;
		}
		else
		{
			name = path.Substring(sindex + 1);
			#if NET9_0_OR_GREATER
			var lookup = cache.GetAlternateLookup<ReadOnlySpan<char>>();
			if(!lookup.TryGetValue(path.AsSpan(0, sindex), out parent))
			{
				var parentPath = path.Substring(0, sindex);
				parent = GetDirectory(parentPath, cache);
			}
			#else
			var parentPath = path.Substring(0, sindex);
			if(!cache.TryGetValue(parentPath, out parent))
			{
				parent = GetDirectory(parentPath, cache);
			}
			#endif
		}
		dir = new(Repository, path, parent, name);
		cache.Add(dir.Name, dir);
		parent.AddDirectory(dir);
		return dir;
	}

	private void SetContent(IList<TreeContentData>? tree)
	{
		Root.Clear();
		if(tree is not { Count: not 0 }) return;

		var cache = new Dictionary<string, TreeDirectory>();
		#if NET9_0_OR_GREATER
		var lookup = cache.GetAlternateLookup<ReadOnlySpan<char>>();
		#endif
		foreach(var item in tree)
		{
			var sindex = item.Name.LastIndexOf('/');
			TreeDirectory? parent;
			string name;
			if(sindex < 0)
			{
				name = item.Name;
				parent = _root;
			}
			else
			{
				name = item.Name.Substring(sindex + 1);
				#if NET9_0_OR_GREATER
				if(!lookup.TryGetValue(item.Name.AsSpan(0, sindex), out parent))
				{
					var path = item.Name.Substring(0, sindex);
					parent = GetDirectory(path, cache);
				}
				#else
				var path = item.Name.Substring(0, sindex);
				if(!cache.TryGetValue(path, out parent))
				{
					parent = GetDirectory(path, cache);
				}
				#endif
			}
			switch(item.Type)
			{
				case TreeContentType.Tree:
					var dir = new TreeDirectory(Repository, item.Name, parent, name);
					cache.Add(item.Name, dir);
					parent.AddDirectory(dir);
					break;
				case TreeContentType.Blob:
					var blob = new TreeFile(Repository, item.Name, parent, FileStatus.Cached, name, ((BlobData)item).Size);
					parent.AddFile(blob);
					break;
				case TreeContentType.Commit:
					var commit = new TreeCommit(Repository, item.Name, parent, FileStatus.Cached, name);
					parent.AddCommit(commit);
					break;
			}
		}
	}

	public void Refresh()
	{
		if(Repository.IsEmpty) return;
		var tree = Repository.Accessor.QueryTreeContent.Invoke(
			new QueryTreeContentRequest(_treeHash, true, false));
		SetContent(tree);
	}

	public byte[] GetBlobContent(string blobPath)
	{
		Verify.Argument.IsNeitherNullNorWhitespace(blobPath);

		return Repository.Accessor.QueryBlobBytes.Invoke(
			new QueryBlobBytesRequest()
			{
				Treeish    = TreeHash,
				ObjectName = blobPath,
			});
	}

	public Task<byte[]> GetBlobContentAsync(string blobPath,
		IProgress<OperationProgress>? progress = default, CancellationToken cancellationToken = default)
	{
		Verify.Argument.IsNeitherNullNorWhitespace(blobPath);

		progress?.Report(new OperationProgress(Resources.StrsFetchingBlob.AddEllipsis()));
		return Repository.Accessor.QueryBlobBytes.InvokeAsync(
			new QueryBlobBytesRequest()
			{
				Treeish    = TreeHash,
				ObjectName = blobPath,
			},
			progress,
			cancellationToken);
	}
}
