namespace gitter.Redmine
{
	using System;
	using System.Xml;

	public sealed class IssueStatus : NamedRedmineObject
	{
		#region Static

		public static readonly RedmineObjectProperty<bool> IsDefaultProperty =
			new RedmineObjectProperty<bool>("is_default", "IsDefault");
		public static readonly RedmineObjectProperty<bool> IsClosedProperty =
			new RedmineObjectProperty<bool>("is_closed", "IsClosed");

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
			private set { UpdatePropertyValue(ref _isDefault, value, IsDefaultProperty); }
		}

		public bool IsClosed
		{
			get { return _isClosed; }
			private set { UpdatePropertyValue(ref _isDefault, value, IsClosedProperty); }
		}

		#endregion
	}
}
