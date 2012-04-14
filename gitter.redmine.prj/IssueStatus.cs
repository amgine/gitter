namespace gitter.Redmine
{
	using System;
	using System.Xml;

	public sealed class IssueStatus : NamedRedmineObject
	{
		#region Static

		public static readonly RedmineObjectProperty IsDefaultProperty =
			new RedmineObjectProperty("is_default", "IsDefault");
		public static readonly RedmineObjectProperty IsClosedProperty =
			new RedmineObjectProperty("is_closed", "IsClosed");

		#endregion

		#region Data

		private bool _isDefault;
		private bool _isClosed;

		#endregion

		#region .ctor

		internal IssueStatus(RedmineServiceContext context, int id, string name)
			: base(context, id, name)
		{
		}

		internal IssueStatus(RedmineServiceContext context, XmlNode node)
			: base(context, node)
		{
			_isDefault	= RedmineUtility.LoadBoolean(node[IsDefaultProperty.XmlNodeName]);
			_isClosed	= RedmineUtility.LoadBoolean(node[IsClosedProperty.XmlNodeName]);
		}

		#endregion

		#region Methods

		internal override void Update(XmlNode node)
		{
			base.Update(node);

			Name		= RedmineUtility.LoadString(node[NameProperty.XmlNodeName]);
			IsDefault	= RedmineUtility.LoadBoolean(node[IsDefaultProperty.XmlNodeName]);
			IsClosed	= RedmineUtility.LoadBoolean(node[IsClosedProperty.XmlNodeName]);
		}

		#endregion

		#region Properties

		public bool IsDefault
		{
			get { return _isDefault; }
			private set
			{
				if(_isDefault != value)
				{
					_isDefault = value;
					OnPropertyChanged(IsDefaultProperty);
				}
			}
		}

		public bool IsClosed
		{
			get { return _isClosed; }
			private set
			{
				if(_isClosed != value)
				{
					_isClosed = value;
					OnPropertyChanged(IsClosedProperty);
				}
			}
		}

		#endregion
	}
}
