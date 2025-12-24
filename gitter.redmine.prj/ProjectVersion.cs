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

public sealed class ProjectVersion : NamedRedmineObject
{
	#region Static

	public static readonly RedmineObjectProperty<Project>       ProjectProperty     = new("project",     nameof(Project));
	public static readonly RedmineObjectProperty<string>        DescriptionProperty = new("description", nameof(Description));
	public static readonly RedmineObjectProperty<VersionStatus> StatusProperty      = new("status",      nameof(Status));
	public static readonly RedmineObjectProperty<DateTime?>     DueDateProperty     = new("due_date",    nameof(DueDate));
	public static readonly RedmineObjectProperty<DateTime>      CreatedOnProperty   = new("created_on",  nameof(CreatedOn));
	public static readonly RedmineObjectProperty<DateTime>      UpdatedOnProperty   = new("updated_on",  nameof(UpdatedOn));

	#endregion

	#region Data

	private Project _project;
	private string _description;
	private VersionStatus _status;
	private DateTime? _dueDate;
	private DateTime _createdOn;
	private DateTime _updatedOn;

	#endregion

	#region .ctor

	internal ProjectVersion(RedmineServiceContext context, int id, string name)
		: base(context, id, name)
	{
	}

	internal ProjectVersion(RedmineServiceContext context, XmlNode node)
		: base(context, node)
	{
		_project		= RedmineUtility.LoadNamedObject(node[ProjectProperty.XmlNodeName], context.Projects.Lookup);
		_description	= RedmineUtility.LoadString(node[DescriptionProperty.XmlNodeName]);
		_status			= RedmineUtility.LoadVersionStatus(node[StatusProperty.XmlNodeName]);
		_dueDate		= RedmineUtility.LoadDate(node[DueDateProperty.XmlNodeName]);
		_createdOn		= RedmineUtility.LoadDateRequired(node[CreatedOnProperty.XmlNodeName]);
		_updatedOn		= RedmineUtility.LoadDateRequired(node[UpdatedOnProperty.XmlNodeName]);
	}

	#endregion

	#region Methods

	internal override void Update(XmlNode node)
	{
		base.Update(node);

		Project		= RedmineUtility.LoadNamedObject(node[ProjectProperty.XmlNodeName], Context.Projects.Lookup);
		Description	= RedmineUtility.LoadString(node[DescriptionProperty.XmlNodeName]);
		Status		= RedmineUtility.LoadVersionStatus(node[StatusProperty.XmlNodeName]);
		DueDate		= RedmineUtility.LoadDate(node[DueDateProperty.XmlNodeName]);
		CreatedOn	= RedmineUtility.LoadDateRequired(node[CreatedOnProperty.XmlNodeName]);
		UpdatedOn	= RedmineUtility.LoadDateRequired(node[UpdatedOnProperty.XmlNodeName]);
	}

	public override Task UpdateAsync(CancellationToken cancellationToken = default)
	{
		var url = string.Format(CultureInfo.InvariantCulture,
			@"versions/{0}.xml", Id);
		return Context.ProjectVersions.FetchSingleItemAsync(url, cancellationToken);
	}

	#endregion

	#region Properties

	public Project Project
	{
		get => _project;
		private set => UpdatePropertyValue(ref _project, value, ProjectProperty);
	}

	public string Description
	{
		get => _description;
		private set => UpdatePropertyValue(ref _description, value, DescriptionProperty);
	}

	public VersionStatus Status
	{
		get => _status;
		private set => UpdatePropertyValue(ref _status, value, StatusProperty);
	}

	public DateTime? DueDate
	{
		get => _dueDate;
		private set => UpdatePropertyValue(ref _dueDate, value, DueDateProperty);
	}

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
}
