#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
using System.Threading.Tasks;

using gitter.Git.AccessLayer;

/// <summary>git tag object.</summary>
public sealed class Tag : Reference
{
	/// <summary>Validates the tag name.</summary>
	/// <param name="name">Tag name.</param>
	/// <param name="errorMessage">Error message.</param>
	/// <returns><c>true</c> if <paramref name="name"/> is a valid tag name; otherwise, <c>false</c>.</returns>
	public static bool ValidateName(string? name, out string? errorMessage)
		=> Reference.ValidateName(name, ReferenceType.Tag, out errorMessage);

	/// <summary>Annotated tag message.</summary>
	private string? _message;

	/// <summary>Initializes a new instance of the <see cref="Tag"/> class.</summary>
	/// <param name="repository">Host repository.</param>
	/// <param name="name">Tag name.</param>
	/// <param name="pointer">Commit which is pointed by tag.</param>
	/// <param name="type">The type.</param>
	internal Tag(Repository repository, string name, IRevisionPointer pointer, TagType type)
		: base(repository, name, pointer)
	{
		TagType = type;
	}

	/// <summary><see cref="ReferenceType"/>.</summary>
	/// <value><see cref="ReferenceType.Tag"/>.</value>
	public override ReferenceType Type => ReferenceType.Tag;

	/// <summary>Gets or sets the type of this tag.</summary>
	/// <value>Type of this tag.</value>
	public TagType TagType { get; internal set; }

	private string? _cachedFullName;

	/// <summary>Gets the full tag name.</summary>
	/// <value>Full tag name.</value>
	public override string FullName
		=> _cachedFullName ??= GitConstants.TagPrefix + Name;

	/// <summary>Gets the message of annotated tag.</summary>
	/// <value>Message of annotated tag.</value>
	public string? Message
	{
		get
		{
			if(TagType == Git.TagType.Lightweight) return default;
			if(_message is null)
			{
				try
				{
					_message = Repository.Accessor.QueryTagMessage.Invoke(
						new QueryTagMessageRequest { TagName = FullName });
				}
				catch(Exception exc) when(!exc.IsCritical)
				{
					_message = string.Empty;
				}
			}
			return _message;
		}
	}

	/// <summary>Filter <see cref="IRevisionPointer"/> to types supported by this <see cref="Reference"/>.</summary>
	/// <param name="pointer">Raw pointer.</param>
	/// <returns>Valid pointer.</returns>
	protected override IRevisionPointer PrepareInputPointer(IRevisionPointer pointer)
	{
		Verify.Argument.IsNotNull(pointer);

		return pointer.Dereference() ?? pointer;
	}

	/// <summary>Deletes this <see cref="Tag"/>.</summary>
	public void Delete()
	{
		Verify.State.IsNotDeleted(this);

		Repository.Refs.Tags.Delete(this);
	}

	/// <summary>Deletes this <see cref="Tag"/>.</summary>
	public Task DeleteAsync()
	{
		Verify.State.IsNotDeleted(this);

		return Repository.Refs.Tags.DeleteAsync(this);
	}

	/// <summary>Refreshes this instance's cached data.</summary>
	public void Refresh()
	{
		Verify.State.IsNotDeleted(this);

		Repository.Refs.Tags.Refresh(this);
	}

	/// <summary>Refreshes this instance's cached data.</summary>
	public Task RefreshAsync()
	{
		Verify.State.IsNotDeleted(this);

		return Repository.Refs.Tags.RefreshAsync(this);
	}
}
