namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	using gitter.Git.AccessLayer.CLI;

	internal sealed partial class RepositoryCLI : ITagAccessor
	{
		private static TagData ParseTag(string strTag, ref int pos)
		{
			var strHash = strTag.Substring(pos, 40);
			int pos2 = strTag.IndexOf('\n', pos);
			if(pos2 < 0) pos2 = strTag.Length;
			pos += 41;
			var strName = strTag.Substring(pos + 10, pos2 - pos - 10);
			pos = pos2 + 1;
			var type = TagType.Lightweight;
			if(pos < strTag.Length)
			{
				pos2 = strTag.IndexOf('\n', pos);
				if(pos2 != -1)
				{
					if(strTag[pos2 - 3] == '^')
					{
						type = TagType.Annotated;
						strHash = strTag.Substring(pos, 40);
						pos = pos2 + 1;
					}
				}
			}

			return new TagData(strName, strHash, type);
		}

		/// <summary>Check if tag exists and get its position.</summary>
		/// <param name="parameters"><see cref="QueryTagParameters"/>.</param>
		/// <returns><see cref="TagData"/> or null, if requested tag doesn't exist.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public TagData QueryTag(QueryTagParameters parameters)
		{
			if(parameters == null) throw new ArgumentNullException("parameters");

			string fullName = GitConstants.TagPrefix + parameters.TagName;
			var cmd = new ShowRefCommand(
					ShowRefCommand.Tags(),
					ShowRefCommand.Dereference(),
					ShowRefCommand.Hash(),
					ShowRefCommand.NoMoreOptions(),
					new CommandArgument(fullName));

			var output = _executor.ExecCommand(cmd);
			var tag = output.Output;
			if(output.ExitCode == 0 && tag.Length >= 40)
			{
				string hash;
				TagType type;
				if(tag.Length >= 81)
				{
					hash = output.Output.Substring(41, 40);
					type = TagType.Annotated;
				}
				else
				{
					hash = output.Output.Substring(0, 40);
					type = TagType.Lightweight;
				}
				return new TagData(parameters.TagName, hash, type);
			}
			else
			{
				return null;
			}
		}

		/// <summary>Query tag message.</summary>
		/// <param name="tag">Tag name or object hash.</param>
		/// <returns>Tag message.</returns>
		public string QueryTagMessage(string tag)
		{
			var cmd = new CatFileCommand(new CommandArgument[]
				{ new CommandArgument("tag"), new CommandArgument(tag) });
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
			var parser = new GitParser(output.Output);
			while(!parser.IsAtEndOfLine)
			{
				parser.SkipLine();
			}
			parser.SkipLine();
			if(parser.RemainingSymbols > 1)
			{
				var message = parser.ReadStringUpTo(parser.Length - 1);
				const char c = '�';
				if(message.ContainsAnyOf(c))
				{
					output = _executor.ExecCommand(cmd, System.Text.Encoding.Default);
					output.ThrowOnBadReturnCode();
					parser = new GitParser(output.Output);
					while(!parser.IsAtEndOfLine)
					{
						parser.SkipLine();
					}
					parser.SkipLine();
					if(parser.RemainingSymbols > 1)
					{
						message = parser.ReadStringUpTo(parser.Length - 1);
					}
					else
					{
						message = string.Empty;
					}
				}
				return message;
			}
			else
			{
				return string.Empty;
			}
		}

		/// <summary>Query tag list.</summary>
		/// <param name="parameters"><see cref="QueryTagsParameters"/>.</param>
		/// <returns>List of requested tags.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<TagData> QueryTags(QueryTagsParameters parameters)
		{
			if(parameters == null) throw new ArgumentNullException("parameters");

			var cmd = new ShowRefCommand(
				ShowRefCommand.Dereference(),
				ShowRefCommand.Tags());
			var output = _executor.ExecCommand(cmd);
			if(output.ExitCode != 0)
				return new TagData[0];

			var tags = output.Output;
			int pos = 0;
			int l = tags.Length;
			var res = new List<TagData>();
			while(pos < l)
			{
				var t = ParseTag(tags, ref pos);
				if(t != null) res.Add(t);
			}
			return res;
		}

		/// <summary>Create new tag object.</summary>
		/// <param name="parameters"><see cref="CreateTagParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void CreateTag(CreateTagParameters parameters)
		{
			if(parameters == null) throw new ArgumentNullException("parameters");

			var args = new List<CommandArgument>(5);
			if(parameters.TagType == TagType.Annotated)
			{
				if(parameters.Signed)
				{
					if(parameters.KeyId != null)
					{
						args.Add(TagCommand.SignByKey(parameters.KeyId));
					}
					else
					{
						args.Add(TagCommand.SignByEmail());
					}
				}
				else
				{
					args.Add(TagCommand.Annotate());
				}
			}
			if(parameters.Force)
			{
				args.Add(TagCommand.Force());
			}
			if(!string.IsNullOrEmpty(parameters.Message))
			{
				args.Add(TagCommand.Message(parameters.Message));
			}
			else if(!string.IsNullOrEmpty(parameters.MessageFile))
			{
				args.Add(TagCommand.MessageFromFile(parameters.MessageFile));
			}
			args.Add(new CommandArgument(parameters.TagName));
			if(parameters.TaggedObject != null)
			{
				args.Add(new CommandArgument(parameters.TaggedObject));
			}
			var cmd = new TagCommand(args);
			var output = _executor.ExecCommand(cmd);
			if(output.ExitCode != 0)
			{
				if(IsUnknownRevisionError(output.Error, parameters.TaggedObject))
				{
					throw new UnknownRevisionException(parameters.TaggedObject);
				}
				if(IsTagAlreadyExistsError(output.Error, parameters.TagName))
				{
					throw new TagAlreadyExistsException(parameters.TagName);
				}
				if(IsInvalidTagNameError(output.Error, parameters.TagName))
				{
					throw new InvalidTagNameException(parameters.TagName);
				}
				output.Throw();
			}
		}

		/// <summary>Delete tag.</summary>
		/// <param name="parameters"><see cref="DeleteTagParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void DeleteTag(DeleteTagParameters parameters)
		{
			if(parameters == null) throw new ArgumentNullException("parameters");

			var cmd = new TagCommand(
				TagCommand.Delete(),
				new CommandArgument(parameters.TagName));

			var output = _executor.ExecCommand(cmd);
			if(output.ExitCode != 0)
			{
				if(IsTagNotFoundError(output.Error, parameters.TagName))
				{
					throw new TagNotFoundException(parameters.TagName);
				}
				output.Throw();
			}
		}

		/// <summary>Verify tags GPG signatures.</summary>
		/// <param name="parameters"><see cref="VerifyTagsParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void VerifyTags(VerifyTagsParameters parameters)
		{
			if(parameters == null) throw new ArgumentNullException("parameters");

			var args = new List<CommandArgument>(1 + parameters.TagNames.Count);
			args.Add(TagCommand.Verify());
			foreach(var tagName in parameters.TagNames)
			{
				args.Add(new CommandArgument(tagName));
			}

			var cmd = new TagCommand(args);
			var output = _executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
		}

		public string Describe(DescribeParameters parameters)
		{
			if(parameters == null) throw new ArgumentNullException("parameters");

			var args = new List<CommandArgument>(2);
			args.Add(DescribeCommand.Tags());
			if(parameters.Revision != null)
			{
				args.Add(new CommandArgument(parameters.Revision));
			}
			var cmd = new DescribeCommand(args);

			var output = _executor.ExecCommand(cmd);
			if(output.ExitCode != 0)
			{
				if(parameters.Revision != null)
				{
					if(IsUnknownRevisionError(output.Error, parameters.Revision))
					{
						throw new UnknownRevisionException(parameters.Revision);
					}
				}
				output.Throw();
			}
			if(output.Output.Length == 0) return null;

			return output.Output;
		}
	}
}
