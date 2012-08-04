namespace gitter.Redmine
{
	using System;
	using System.Globalization;
	using System.Xml;

	public sealed class IssueRelation : RedmineObject
	{
		#region Static

		public static readonly RedmineObjectProperty<Issue> IssueProperty =
			new RedmineObjectProperty<Issue>("issue", "Issue");
		public static readonly RedmineObjectProperty<Issue> IssueToProperty =
			new RedmineObjectProperty<Issue>("issue_to", "IssueTo");
		public static readonly RedmineObjectProperty<IssueRelationType> TypeProperty =
			new RedmineObjectProperty<IssueRelationType>("relation_type", "Type");

		#endregion

		#region Data

		private Issue _issue;
		private Issue _issueTo;
		private IssueRelationType _type;

		#endregion

		#region .ctor

		internal IssueRelation(RedmineServiceContext context, int id)
			: base(context, id)
		{
		}

		internal IssueRelation(RedmineServiceContext context, XmlNode node)
			: base(context, node)
		{
			_issue		= RedmineUtility.LoadObject(node[IssueProperty.XmlNodeName], context.Issues.Lookup);
			_issueTo	= RedmineUtility.LoadObject(node[IssueToProperty.XmlNodeName], context.Issues.Lookup);
			_type		= RedmineUtility.LoadIssueRelationType(node[TypeProperty.XmlNodeName]);
		}

		#endregion

		#region Methods

		internal override void Update(XmlNode node)
		{
			base.Update(node);

			Issue	= RedmineUtility.LoadObject(node[IssueProperty.XmlNodeName], Context.Issues.Lookup);
			IssueTo	= RedmineUtility.LoadObject(node[IssueToProperty.XmlNodeName], Context.Issues.Lookup);
			Type	= RedmineUtility.LoadIssueRelationType(node[TypeProperty.XmlNodeName]);
		}

		public override void Update()
		{
			var url = string.Format(CultureInfo.InvariantCulture,
				@"relations/{0}.xml", Id);
			Context.Attachments.FetchSingleItem(url);
		}

		#endregion

		#region Properties

		public Issue Issue
		{
			get { return _issue; }
			private set { UpdatePropertyValue(ref _issue, value, IssueProperty); }
		}

		public Issue IssueTo
		{
			get { return _issueTo; }
			private set { UpdatePropertyValue(ref _issueTo, value, IssueToProperty); }
		}

		public IssueRelationType Type
		{
			get { return _type; }
			private set { UpdatePropertyValue(ref _type, value, TypeProperty); }
		}

		#endregion
	}
}
