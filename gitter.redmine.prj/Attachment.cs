namespace gitter.Redmine
{
	using System;
	using System.Globalization;
	using System.Xml;

	public sealed class Attachment : RedmineObject
	{
		#region Static

		public static readonly RedmineObjectProperty FileNameProperty =
			new RedmineObjectProperty("filename", "FileName");
		public static readonly RedmineObjectProperty FileSizeProperty =
			new RedmineObjectProperty("filesize", "FileSize");
		public static readonly RedmineObjectProperty DescriptionProperty =
			new RedmineObjectProperty("description", "Description");
		public static readonly RedmineObjectProperty ContentTypeProperty =
			new RedmineObjectProperty("content_type", "ContentType");
		public static readonly RedmineObjectProperty ContentUrlProperty =
			new RedmineObjectProperty("content_url", "ContentUrl");
		public static readonly RedmineObjectProperty AuthorProperty =
			new RedmineObjectProperty("author", "Author");
		public static readonly RedmineObjectProperty CreatedOnProperty =
			new RedmineObjectProperty("created_on", "CreatedOn");

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
			_createdOn		= RedmineUtility.LoadDateForSure(node[CreatedOnProperty.XmlNodeName]);
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
			CreatedOn	= RedmineUtility.LoadDateForSure(node[CreatedOnProperty.XmlNodeName]);
		}

		public override void Update()
		{
			var url = string.Format(CultureInfo.InvariantCulture,
				@"attachments/{0}.xml", Id);
			Context.Attachments.FetchSingleItem(url);
		}

		#endregion

		#region Properties

		public string FileName
		{
			get { return _fileName; }
			private set
			{
				if(_fileName != value)
				{
					_fileName = value;
					OnPropertyChanged(FileNameProperty);
				}
			}
		}

		public int FileSize
		{
			get { return _fileSize; }
			private set
			{
				if(_fileSize != value)
				{
					_fileSize = value;
					OnPropertyChanged(FileSizeProperty);
				}
			}
		}

		public string ContentType
		{
			get { return _contentType; }
			private set
			{
				if(_contentType != value)
				{
					_contentType = value;
					OnPropertyChanged(ContentTypeProperty);
				}
			}
		}

		public string Description
		{
			get { return _description; }
			private set
			{
				if(_description != value)
				{
					_description = value;
					OnPropertyChanged(DescriptionProperty);
				}
			}
		}

		public string ContentUrl
		{
			get { return _contentUrl; }
			private set
			{
				if(_contentUrl != value)
				{
					_contentUrl = value;
					OnPropertyChanged(ContentUrlProperty);
				}
			}
		}

		public User Author
		{
			get { return _author; }
			private set
			{
				if(_author != value)
				{
					_author = value;
					OnPropertyChanged(AuthorProperty);
				}
			}
		}

		public DateTime CreatedOn
		{
			get { return _createdOn; }
			private set
			{
				if(_createdOn != value)
				{
					_createdOn = value;
					OnPropertyChanged(CreatedOnProperty);
				}
			}
		}

		#endregion
	}
}
