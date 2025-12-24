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
using System.Threading.Tasks;
using System.Xml;

public sealed class IssueCreation : RedmineObjectCreation<Issue>
{
	#region Data

	private Project _project;
	private Issue _parent;

	private IssueTracker _tracker;
	private IssueStatus _status;
	private IssuePriority _priority;

	private User _assignedTo;
	private IssueCategory _category;
	private ProjectVersion _fixedVersion;

	private string _subject;
	private string _description;

	private DateTime? _startDate;
	private DateTime? _dueDate;

	private double _doneRatio;
	private double _estimatedHours;

	private readonly CustomFieldsDefinition _customFields;

	#endregion

	#region Properties

	public Project Project
	{
		get { return _project; }
		set { _project = value; }
	}

	public Issue Parent
	{
		get { return _parent; }
		set { _parent = value; }
	}

	public IssueTracker Tracker
	{
		get { return _tracker; }
		set { _tracker = value; }
	}

	public IssueStatus Status
	{
		get { return _status; }
		set { _status = value; }
	}

	public IssuePriority Priority
	{
		get { return _priority; }
		set { _priority = value; }
	}

	public User AssignedTo
	{
		get { return _assignedTo; }
		set { _assignedTo = value; }
	}

	public IssueCategory Category
	{
		get { return _category; }
		set { _category = value; }
	}

	public ProjectVersion FixedVersion
	{
		get { return _fixedVersion; }
		set { _fixedVersion = value; }
	}

	public string Subject
	{
		get { return _subject; }
		set { _subject = value; }
	}

	public string Description
	{
		get { return _description; }
		set { _description = value; }
	}

	public DateTime? StartDate
	{
		get { return _startDate; }
		set { _startDate = value; }
	}

	public DateTime? DueDate
	{
		get { return _dueDate; }
		set { _dueDate = value; }
	}

	public double DoneRatio
	{
		get { return _doneRatio; }
		set { _doneRatio = value; }
	}

	public double EstimatedHours
	{
		get { return _estimatedHours; }
		set { _estimatedHours = value; }
	}

	public CustomFieldsDefinition CustomFields => _customFields;

	#endregion

	internal IssueCreation(RedmineServiceContext context)
		: base(context)
	{
		_customFields = new(null);
	}

	protected override void ResetCore()
	{
		_project		= null;
		_parent			= null;

		_tracker		= null;
		_status			= null;
		_priority		= null;

		_assignedTo		= null;
		_category		= null;
		_fixedVersion	= null;

		_subject		= null;
		_description	= null;

		_startDate		= null;
		_dueDate		= null;

		_doneRatio		= 0;
		_estimatedHours	= 0;

		_customFields?.Reset();
	}

	protected override Task CommitCore()
	{
		var xml = new XmlDocument();
		var root = xml.CreateElement("issue");

		EmitIfChanged(null,			Project,		xml, root, "project_id",		RedmineUtility.EmitObjectId);
		EmitIfChanged(null,			Parent,			xml, root, "parent_issue_id",	RedmineUtility.EmitObjectId);

		EmitIfChanged(null,			Tracker,		xml, root, "tracker_id",		RedmineUtility.EmitObjectId);
		EmitIfChanged(null,			Status,			xml, root, "status_id",			RedmineUtility.EmitObjectId);
		EmitIfChanged(null,			Priority,		xml, root, "priority_id",		RedmineUtility.EmitObjectId);

		EmitIfChanged(null,			AssignedTo,		xml, root, "assigned_to_id",	RedmineUtility.EmitObjectId);
		EmitIfChanged(null,			Category,		xml, root, "category_id",		RedmineUtility.EmitObjectId);
		EmitIfChanged(null,			FixedVersion,	xml, root, "fixed_version_id",	RedmineUtility.EmitObjectId);

		EmitIfChanged(null,			Subject,		xml, root, "subject",			RedmineUtility.EmitString);
		EmitIfChanged(null,			Description,	xml, root, "description",		RedmineUtility.EmitString);

		EmitIfChanged(null,			StartDate,		xml, root, "start_date",		RedmineUtility.EmitDate);
		EmitIfChanged(null,			DueDate,		xml, root, "due_date",			RedmineUtility.EmitDate);

		EmitIfChanged(0,			DoneRatio,		xml, root, "done_ratio",		RedmineUtility.EmitDouble);
		EmitIfChanged(0,			EstimatedHours,	xml, root, "estimated_hours",	RedmineUtility.EmitDouble);

		var cfv = xml.CreateElement("custom_fields");
		_customFields.EmitChanged(cfv);
		if(cfv.ChildNodes.Count != 0)
		{
			var attr = xml.CreateAttribute("type");
			attr.Value = "array";
			cfv.Attributes.Append(attr);
			root.AppendChild(cfv);
		}

		xml.AppendChild(root);

		const string url = "issues.xml";
		return Context.PostXmlAsync(url, xml);
	}
}
