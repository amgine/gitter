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

namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Repository tags collection ($GIT_DIR/refs/tags cache).</summary>
	public sealed class RefsTagsCollection : GitObjectsCollection<Tag, TagEventArgs>
	{
		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="RefsTagsCollection"/> class.</summary>
		/// <param name="repository">Host repository.</param>
		/// <exception cref="ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
		internal RefsTagsCollection(Repository repository)
			: base(repository)
		{
		}

		#endregion

		#region Create()

		/// <summary>Creates lightweight tag <paramref name="name"/> at commit pointed by <paramref name="revision"/>.</summary>
		/// <param name="name">Tag name.</param>
		/// <param name="revision">Revision which is being tagged.</param>
		/// <returns>Created tag.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="name"/> == null or <paramref name="revision"/> == <c>null</c>.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="revision"/> is not handled by this repository or deleted.
		/// <paramref name="name"/> is not a valid reference name.
		/// <paramref name="name"/> already exists.
		/// </exception>
		/// <exception cref="T:gitter.Git.GitException">Failed to dereference <paramref name="revision"/> or failed to create a tag.</exception>
		public Tag Create(string name, IRevisionPointer revision)
		{
			Verify.Argument.IsValidReferenceName(name, ReferenceType.Tag, "name");
			Verify.Argument.IsValidRevisionPointer(revision, Repository, "revision");
			Verify.Argument.IsFalse(ContainsObjectName(name), "name",
				Resources.ExcObjectWithThisNameAlreadyExists.UseAsFormat("Tag"));

			var rev = revision.Dereference();
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.TagChanged))
			{
				Repository.Accessor.CreateTag.Invoke(
					new CreateTagParameters(name, revision.Pointer));
			}
			var tag = new Tag(Repository, name, rev, TagType.Lightweight);
			AddObject(tag);
			return tag;
		}

		/// <summary>Creates annotated tag <paramref name="name"/> at commit pointed by <paramref name="revision"/>.</summary>
		/// <param name="name">Tag name.</param>
		/// <param name="revision">Revision which is being tagged.</param>
		/// <param name="message">Message.</param>
		/// <param name="sign">Create tag signed by default email key.</param>
		/// <returns>Created tag.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name"/> == null or <paramref name="revision"/> == null or <paramref name="message"/> == null.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="revision"/> is not handled by this repository or deleted.
		/// <paramref name="name"/> is not a valid reference name.
		/// <paramref name="name"/> already exists.
		/// </exception>
		/// <exception cref="T:gitter.Git.GitException">Failed to dereference <paramref name="revision"/> or failed to create a tag.</exception>
		public Tag Create(string name, IRevisionPointer revision, string message, bool sign)
		{
			Verify.Argument.IsValidReferenceName(name, ReferenceType.Tag, "name");
			Verify.Argument.IsValidRevisionPointer(revision, Repository, "revision");
			Verify.Argument.IsFalse(ContainsObjectName(name), "name",
				Resources.ExcObjectWithThisNameAlreadyExists.UseAsFormat("Tag"));
			Verify.Argument.IsNotNull(message, "message");

			var rev = revision.Dereference();
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.TagChanged))
			{
				Repository.Accessor.CreateTag.Invoke(
					new CreateTagParameters(name, revision.Pointer, message, sign));
			}
			var tag = new Tag(Repository, name, rev, TagType.Annotated);
			AddObject(tag);
			return tag;
		}

		/// <summary>Creates signed tag <paramref name="name"/> at commit pointed by <paramref name="revision"/>.</summary>
		/// <param name="name">Tag name.</param>
		/// <param name="revision">Revision which is being tagged.</param>
		/// <param name="message">Message.</param>
		/// <param name="keyId">GPG key ID.</param>
		/// <returns>Created tag.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name"/> == null or <paramref name="revision"/> == null or <paramref name="message"/> == null or <paramref name="keyId"/> == null.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="revision"/> is not handled by this repository or deleted.
		/// <paramref name="name"/> is not a valid reference name.
		/// <paramref name="name"/> already exists.
		/// </exception>
		/// <exception cref="T:gitter.Git.GitException">Failed to dereference <paramref name="revision"/> or failed to create a tag.</exception>
		public Tag Create(string name, IRevisionPointer revision, string message, string keyId)
		{
			Verify.Argument.IsValidReferenceName(name, ReferenceType.Tag, "name");
			Verify.Argument.IsValidRevisionPointer(revision, Repository, "revision");
			Verify.Argument.IsFalse(ContainsObjectName(name), "name",
				Resources.ExcObjectWithThisNameAlreadyExists.UseAsFormat("Tag"));
			Verify.Argument.IsNotNull(message, "message");
			Verify.Argument.IsNotNull(keyId, "keyId");

			var rev = revision.Dereference();
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.TagChanged))
			{
				Repository.Accessor.CreateTag.Invoke(
					new CreateTagParameters(name, revision.Pointer, message, keyId));
			}
			var tag = new Tag(Repository, name, rev, TagType.Annotated);
			AddObject(tag);
			return tag;
		}

		#endregion

		#region Delete()

		/// <summary>Delete tag.</summary>
		/// <param name="tag">Tag to delete.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="tag"/> == null.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="revision"/> is not handled by this repository or deleted.
		/// </exception>
		/// <exception cref="T:gitter.Git.GitException">
		/// Failed to delete <paramref name="tag"/>.
		/// </exception>
		internal void Delete(Tag tag)
		{
			Verify.Argument.IsValidGitObject(tag, Repository, "tag");

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.TagChanged))
			{
				Repository.Accessor.DeleteTag.Invoke(
					new DeleteTagParameters(tag.Name));
			}
			RemoveObject(tag);
		}

		#endregion

		#region Refresh()

		private void RefreshInternal(IEnumerable<TagData> tagDataList)
		{
			lock(SyncRoot)
			{
				CacheUpdater.UpdateObjectDictionary(
					ObjectStorage,
					null,
					null,
					tagDataList,
					tagData => ObjectFactories.CreateTag(Repository, tagData),
					ObjectFactories.UpdateTag,
					InvokeObjectAdded,
					InvokeObjectRemoved,
					true);
			}
		}

		/// <summary>Sync information on tags: removes non-existent, adds new, verifies positions.</summary>
		public void Refresh()
		{
			var tags = Repository.Accessor.QueryTags.Invoke(
				new QueryTagsParameters());
			RefreshInternal(tags);
		}

		internal void Refresh(IEnumerable<TagData> tagDataList)
		{
			Verify.Argument.IsNotNull(tagDataList, "tagDataList");

			RefreshInternal(tagDataList);
		}

		/// <summary>Refresh tag's position (remove tag if it doesn't exist anymore and recreate if position differs).</summary>
		/// <param name="tag">Tag to refresh.</param>
		internal void Refresh(Tag tag)
		{
			Verify.Argument.IsValidGitObject(tag, Repository, "tag");

			var tagData = Repository.Accessor.QueryTag.Invoke(
				new QueryTagParameters(tag.Name));
			if(tagData != null)
			{
				ObjectFactories.UpdateTag(tag, tagData);
			}
			else
			{
				RemoveObject(tag);
			}
		}

		#endregion

		#region Load()

		/// <summary>Perform initial load of tags.</summary>
		/// <param name="tagDataList">List of tag data containers.</param>
		internal void Load(IEnumerable<TagData> tagDataList)
		{
			ObjectStorage.Clear();
			if(tagDataList != null)
			{
				foreach(var tagData in tagDataList)
				{
					AddObject(ObjectFactories.CreateTag(Repository, tagData));
				}
			}
		}

		#endregion

		#region Notify()

		/// <summary>Notifies that tag was created externally.</summary>
		/// <param name="tagData">Created tag data.</param>
		/// <returns>Created tag.</returns>
		internal Tag NotifyCreated(TagData tagData)
		{
			var tag = ObjectFactories.CreateTag(Repository, tagData);
			AddObject(tag);
			return tag;
		}

		#endregion

		#region Overrides

		/// <summary>Fixes the input tag name.</summary>
		/// <param name="name">Input value.</param>
		/// <returns>Fixed name value.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> == <c>null</c>.</exception>
		protected override string FixInputName(string name)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(name, "name");

			if(name.StartsWith(GitConstants.TagPrefix) && !ContainsObjectName(name))
			{
				return name.Substring(GitConstants.TagPrefix.Length);
			}
			return name;
		}

		/// <summary>Creates the event args for specified <paramref name="item"/>.</summary>
		/// <param name="item">Item to create event args for.</param>
		/// <returns>Created event args.</returns>
		protected override TagEventArgs CreateEventArgs(Tag item)
		{
			return new TagEventArgs(item);
		}

		#endregion
	}
}
