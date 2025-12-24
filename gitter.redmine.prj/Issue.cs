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

/// <summary>Represents redmine issue object.</summary>
public sealed class Issue : RedmineObject
{
	#region Static

	public static readonly RedmineObjectProperty<Project>        ProjectProperty        = new("project",         nameof(Project));
	public static readonly RedmineObjectProperty<Issue>          ParentProperty         = new("parent",          nameof(Parent));
	public static readonly RedmineObjectProperty<IssueTracker>   TrackerProperty        = new("tracker",         nameof(Tracker));
	public static readonly RedmineObjectProperty<IssueStatus>    StatusProperty         = new("status",          nameof(Status));
	public static readonly RedmineObjectProperty<IssuePriority>  PriorityProperty       = new("priority",        nameof(Priority));
	public static readonly RedmineObjectProperty<User>           AuthorProperty         = new("author",          nameof(Author));
	public static readonly RedmineObjectProperty<User>           AssignedToProperty     = new("assigned_to",     nameof(AssignedTo));
	public static readonly RedmineObjectProperty<IssueCategory>  CategoryProperty       = new("category",        nameof(Category));
	public static readonly RedmineObjectProperty<ProjectVersion> FixedVersionProperty   = new("fixed_version",   nameof(FixedVersion));
	public static readonly RedmineObjectProperty<string>         SubjectProperty        = new("subject",         nameof(Subject));
	public static readonly RedmineObjectProperty<string>         DescriptionProperty    = new("description",     nameof(Description));
	public static readonly RedmineObjectProperty<DateTime?>      StartDateProperty      = new("start_date",      nameof(StartDate));
	public static readonly RedmineObjectProperty<DateTime?>      DueDateProperty        = new("due_date",        nameof(DueDate));
	public static readonly RedmineObjectProperty<double>         DoneRatioProperty      = new("done_ratio",      nameof(DoneRatio));
	public static readonly RedmineObjectProperty<double>         EstimatedHoursProperty = new("estimated_hours", nameof(EstimatedHours));
	public static readonly RedmineObjectProperty<CustomFields>   CustomFieldsProperty   = new("custom_fields",   nameof(CustomFields));
	public static readonly RedmineObjectProperty<DateTime>       CreatedOnProperty      = new("created_on",      nameof(CreatedOn));
	public static readonly RedmineObjectProperty<DateTime>       UpdatedOnProperty      = new("updated_on",      nameof(UpdatedOn));

	#endregion

	#region Data

	private Project _project;

	private Issue _parent;

	private IssueTracker _tracker;
	private IssueStatus _status;
	private IssuePriority _priority;

	private User _author;
	private User _assignedTo;
	private IssueCategory _category;
	private ProjectVersion _fixedVersion;

	private string _subject;
	private string _description;

	private DateTime? _startDate;
	private DateTime? _dueDate;

	private double _doneRatio;
	private double _estimatedHours;

	private CustomFields _customFields;

	private DateTime _createdOn;
	private DateTime _updatedOn;

	#endregion

	#region .ctor

	internal Issue(RedmineServiceContext context, int id)
		: base(context, id)
	{
		_customFields	= new CustomFields();
	}

	internal Issue(RedmineServiceContext context, XmlNode node)
		: base(context, node)
	{
		_parent         = RedmineUtility.LoadObject(node[ParentProperty.XmlNodeName],            context.Issues.Lookup);

		_project        = RedmineUtility.LoadNamedObject(node[ProjectProperty.XmlNodeName],      context.Projects.Lookup);
		_tracker        = RedmineUtility.LoadNamedObject(node[TrackerProperty.XmlNodeName],      context.Trackers.Lookup);
		_status         = RedmineUtility.LoadNamedObject(node[StatusProperty.XmlNodeName],       context.IssueStatuses.Lookup);
		_priority       = RedmineUtility.LoadNamedObject(node[PriorityProperty.XmlNodeName],     context.IssuePriorities.Lookup);

		_author         = RedmineUtility.LoadNamedObject(node[AuthorProperty.XmlNodeName],       context.Users.Lookup);
		_assignedTo     = RedmineUtility.LoadNamedObject(node[AssignedToProperty.XmlNodeName],   context.Users.Lookup);

		_category       = RedmineUtility.LoadNamedObject(node[CategoryProperty.XmlNodeName],     context.IssueCategories.Lookup);
		_fixedVersion   = RedmineUtility.LoadNamedObject(node[FixedVersionProperty.XmlNodeName], context.ProjectVersions.Lookup);

		_subject        = RedmineUtility.LoadString(node[SubjectProperty.XmlNodeName]);
		_description    = RedmineUtility.LoadString(node[DescriptionProperty.XmlNodeName]);

		_startDate      = RedmineUtility.LoadDate(node[StartDateProperty.XmlNodeName]);
		_dueDate        = RedmineUtility.LoadDate(node[DueDateProperty.XmlNodeName]);

		_doneRatio      = RedmineUtility.LoadDouble(node[DoneRatioProperty.XmlNodeName]);
		_estimatedHours = RedmineUtility.LoadDouble(node[EstimatedHoursProperty.XmlNodeName]);

		_customFields   = RedmineUtility.LoadCustomFields(node[CustomFieldsProperty.XmlNodeName], context.CustomFields.Lookup);

		_createdOn      = RedmineUtility.LoadDateRequired(node[CreatedOnProperty.XmlNodeName]);
		_updatedOn      = RedmineUtility.LoadDateRequired(node[UpdatedOnProperty.XmlNodeName]);
	}

	#endregion

	#region Methods

	internal override void Update(XmlNode node)
	{
		base.Update(node);

		Parent         = RedmineUtility.LoadObject(node[ParentProperty.XmlNodeName],            Context.Issues.Lookup);

		Project        = RedmineUtility.LoadNamedObject(node[ProjectProperty.XmlNodeName],      Context.Projects.Lookup);
		Tracker        = RedmineUtility.LoadNamedObject(node[TrackerProperty.XmlNodeName],      Context.Trackers.Lookup);
		Status         = RedmineUtility.LoadNamedObject(node[StatusProperty.XmlNodeName],       Context.IssueStatuses.Lookup);
		Priority       = RedmineUtility.LoadNamedObject(node[PriorityProperty.XmlNodeName],     Context.IssuePriorities.Lookup);

		Author         = RedmineUtility.LoadNamedObject(node[AuthorProperty.XmlNodeName],       Context.Users.Lookup);
		AssignedTo     = RedmineUtility.LoadNamedObject(node[AssignedToProperty.XmlNodeName],   Context.Users.Lookup);

		Category       = RedmineUtility.LoadNamedObject(node[CategoryProperty.XmlNodeName],     Context.IssueCategories.Lookup);
		FixedVersion   = RedmineUtility.LoadNamedObject(node[FixedVersionProperty.XmlNodeName], Context.ProjectVersions.Lookup);

		Subject        = RedmineUtility.LoadString(node[SubjectProperty.XmlNodeName]);
		Description    = RedmineUtility.LoadString(node[DescriptionProperty.XmlNodeName]);

		StartDate      = RedmineUtility.LoadDate(node[StartDateProperty.XmlNodeName]);
		DueDate        = RedmineUtility.LoadDate(node[DueDateProperty.XmlNodeName]);

		DoneRatio      = RedmineUtility.LoadDouble(node[DoneRatioProperty.XmlNodeName]);
		EstimatedHours = RedmineUtility.LoadDouble(node[EstimatedHoursProperty.XmlNodeName]);

		_customFields.Update(node[CustomFieldsProperty.XmlNodeName], Context.CustomFields.Lookup);

		CreatedOn      = RedmineUtility.LoadDateRequired(node[CreatedOnProperty.XmlNodeName]);
		UpdatedOn      = RedmineUtility.LoadDateRequired(node[UpdatedOnProperty.XmlNodeName]);
	}

	public override Task UpdateAsync(CancellationToken cancellationToken = default)
	{
		var url = string.Format(CultureInfo.InvariantCulture,
			@"issues/{0}.xml", Id);
		return Context.Issues.FetchSingleItemAsync(url, cancellationToken);
	}

	public IssueModification Modify() => new(this);

	#endregion

	#region Prpoerties

	public Project Project
	{
		get => _project;
		private set => UpdatePropertyValue(ref _project, value, ProjectProperty);
	}

	public Issue Parent
	{
		get => _parent;
		private set => UpdatePropertyValue(ref _parent, value, ParentProperty);
	}

	public IssueTracker Tracker
	{
		get => _tracker;
		private set => UpdatePropertyValue(ref _tracker, value, TrackerProperty);
	}

	public IssueStatus Status
	{
		get => _status;
		private set => UpdatePropertyValue(ref _status, value, StatusProperty);
	}

	public IssuePriority Priority
	{
		get => _priority;
		private set => UpdatePropertyValue(ref _priority, value, PriorityProperty);
	}

	public User Author
	{
		get => _author;
		private set => UpdatePropertyValue(ref _author, value, AuthorProperty);
	}

	public User AssignedTo
	{
		get => _assignedTo;
		private set => UpdatePropertyValue(ref _assignedTo, value, AssignedToProperty);
	}

	public bool IsAssigned => AssignedTo is not null;

	public IssueCategory Category
	{
		get => _category;
		private set => UpdatePropertyValue(ref _category, value, CategoryProperty);
	}

	public ProjectVersion FixedVersion
	{
		get => _fixedVersion;
		private set => UpdatePropertyValue(ref _fixedVersion, value, FixedVersionProperty);
	}

	public string Subject
	{
		get => _subject;
		private set => UpdatePropertyValue(ref _subject, value, SubjectProperty);
	}

	public string Description
	{
		get => _description;
		private set => UpdatePropertyValue(ref _description, value, DescriptionProperty);
	}

	public DateTime? StartDate
	{
		get => _startDate;
		private set => UpdatePropertyValue(ref _startDate, value, StartDateProperty);
	}

	public DateTime? DueDate
	{
		get => _dueDate;
		private set => UpdatePropertyValue(ref _dueDate, value, DueDateProperty);
	}

	public double DoneRatio
	{
		get => _doneRatio;
		private set => UpdatePropertyValue(ref _doneRatio, value, DoneRatioProperty);
	}

	public double EstimatedHours
	{
		get => _estimatedHours;
		private set => UpdatePropertyValue(ref _estimatedHours, value, EstimatedHoursProperty);
	}

	public CustomFields CustomFields => _customFields;

	public DateTime CreatedOn
	{
		get => _createdOn;
		private set => UpdatePropertyValue(ref _createdOn, value, CreatedOnProperty);
	}

	public DateTime UpdatedOn
	{
		get => _updatedOn;
		private set => UpdatePropertyValue(ref _updatedOn, value, UpdatedOnProperty);
	}

	#endregion

	public override string ToString() => _subject;
}
