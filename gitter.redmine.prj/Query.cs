namespace gitter.Redmine
{
	using System;
	using System.Xml;

	public sealed class Query : NamedRedmineObject
	{
		#region Static

		public static readonly RedmineObjectProperty<bool> IsPublicProperty =
			new RedmineObjectProperty<bool>("is_public", "IsPublic");
		public static readonly RedmineObjectProperty<int> ProjectIdProperty =
			new RedmineObjectProperty<int>("project_id", "ProjectId");

		#endregion

		#region Data

		private int _projectId;
		private bool _isPublic;

		#endregion

		#region .ctor

		internal Query(RedmineServiceContext context, int id, string name)
			: base(context, id, name)
		{
		}

		internal Query(RedmineServiceContext context, XmlNode node)
			: base(context, node)
		{
			_projectId	= RedmineUtility.LoadInt(node[ProjectIdProperty.XmlNodeName]);
			_isPublic	= RedmineUtility.LoadBoolean(node[IsPublicProperty.XmlNodeName]);
		}

		#endregion

		#region Methods

		internal override void Update(XmlNode node)
		{
			base.Update(node);

			ProjectId	= RedmineUtility.LoadInt(node[ProjectIdProperty.XmlNodeName]);
			IsPublic	= RedmineUtility.LoadBoolean(node[IsPublicProperty.XmlNodeName]);
		}

		#endregion

		#region Properties

		public int ProjectId
		{
			get { return _projectId; }
			private set { UpdatePropertyValue(ref _projectId, value, ProjectIdProperty); }
		}

		public bool IsPublic
		{
			get { return _isPublic; }
			private set { UpdatePropertyValue(ref _isPublic, value, IsPublicProperty); }
		}

		#endregion
	}
}
