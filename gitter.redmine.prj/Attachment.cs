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

namespace gitter.Redmine;

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

public sealed class Attachment : RedmineObject
{
	#region Static

	public static readonly RedmineObjectProperty<string>   FileNameProperty    = new("filename",     nameof(FileName));
	public static readonly RedmineObjectProperty<int>      FileSizeProperty    = new("filesize",     nameof(FileSize));
	public static readonly RedmineObjectProperty<string>   DescriptionProperty = new("description",  nameof(Description));
	public static readonly RedmineObjectProperty<string>   ContentTypeProperty = new("content_type", nameof(ContentType));
	public static readonly RedmineObjectProperty<string>   ContentUrlProperty  = new("content_url",  nameof(ContentUrl));
	public static readonly RedmineObjectProperty<User>     AuthorProperty      = new("author",       nameof(Author));
	public static readonly RedmineObjectProperty<DateTime> CreatedOnProperty   = new("created_on",   nameof(CreatedOn));

	#endregion

	#region Data

	private string _fileName;
	private int _fileSize;
	private string _description;
	private string _contentType;
	private string _contentUrl;
	private User _author;
	private DateTime _createdOn;

	#endregion

	#region .ctor

	internal Attachment(RedmineServiceContext context, int id)
		: base(context, id)
	{
	}

	internal Attachment(RedmineServiceContext context, XmlNode node)
		: base(context, node)
	{
		_fileName		= RedmineUtility.LoadString(node[FileNameProperty.XmlNodeName]);
		_fileSize		= RedmineUtility.LoadInt(node[FileSizeProperty.XmlNodeName]);
		_contentType	= RedmineUtility.LoadString(node[ContentTypeProperty.XmlNodeName]);
		_description	= RedmineUtility.LoadString(node[DescriptionProperty.XmlNodeName]);
		_contentUrl		= RedmineUtility.LoadString(node[ContentUrlProperty.XmlNodeName]);
		_author			= RedmineUtility.LoadNamedObject(node[AuthorProperty.XmlNodeName], context.Users.Lookup);
		_createdOn		= RedmineUtility.LoadDateRequired(node[CreatedOnProperty.XmlNodeName]);
	}

	#endregion

	#region Methods

	internal override void Update(XmlNode node)
	{
		base.Update(node);

		FileName	= RedmineUtility.LoadString(node[FileNameProperty.XmlNodeName]);
		FileSize	= RedmineUtility.LoadInt(node[FileSizeProperty.XmlNodeName]);
		ContentType	= RedmineUtility.LoadString(node[ContentTypeProperty.XmlNodeName]);
		Description	= RedmineUtility.LoadString(node[DescriptionProperty.XmlNodeName]);
		ContentUrl	= RedmineUtility.LoadString(node[ContentUrlProperty.XmlNodeName]);
		Author		= RedmineUtility.LoadNamedObject(node[AuthorProperty.XmlNodeName], Context.Users.Lookup);
		CreatedOn	= RedmineUtility.LoadDateRequired(node[CreatedOnProperty.XmlNodeName]);
	}

	public override Task UpdateAsync(CancellationToken cancellationToken = default)
	{
		var url = string.Format(CultureInfo.InvariantCulture,
			@"attachments/{0}.xml", Id);
		return Context.Attachments.FetchSingleItemAsync(url, cancellationToken);
	}

	#endregion

	#region Properties

	public string FileName
	{
		get => _fileName;
		private set => UpdatePropertyValue(ref _fileName, value, FileNameProperty);
	}

	public int FileSize
	{
		get => _fileSize;
		private set => UpdatePropertyValue(ref _fileSize, value, FileSizeProperty);
	}

	public string ContentType
	{
		get => _contentType;
		private set => UpdatePropertyValue(ref _contentType, value, ContentTypeProperty);
	}

	public string Description
	{
		get => _description;
		private set => UpdatePropertyValue(ref _description, value, DescriptionProperty);
	}

	public string ContentUrl
	{
		get => _contentUrl;
		private set => UpdatePropertyValue(ref _contentUrl, value, ContentUrlProperty);
	}

	public User Author
	{
		get => _author;
		private set => UpdatePropertyValue(ref _author, value, AuthorProperty);
	}

	public DateTime CreatedOn
	{
		get => _createdOn;
		private set => UpdatePropertyValue(ref _createdOn, value, CreatedOnProperty);
	}

	#endregion
}
