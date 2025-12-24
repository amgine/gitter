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
using System.Xml;

public sealed class News : RedmineObject
{
	#region Static

	public static readonly RedmineObjectProperty<Project>  ProjectProperty     = new("project",     nameof(Project));
	public static readonly RedmineObjectProperty<User>     AuthorProperty      = new("author",      nameof(Author));
	public static readonly RedmineObjectProperty<string>   TitleProperty       = new("title",       nameof(Title));
	public static readonly RedmineObjectProperty<string>   SummaryProperty     = new("summary",     nameof(Summary));
	public static readonly RedmineObjectProperty<string>   DescriptionProperty = new("description", nameof(Description));
	public static readonly RedmineObjectProperty<DateTime> CreatedOnProperty   = new("created_on",  nameof(CreatedOn));

	#endregion

	#region Data

	private Project _project;
	private User _author;
	private string _title;
	private string _description;
	private string _summary;
	private DateTime _createdOn;

	#endregion

	#region .ctor

	internal News(RedmineServiceContext context, int id)
		: base(context, id)
	{
	}

	internal News(RedmineServiceContext context, XmlNode node)
		: base(context, node)
	{
		_project		= RedmineUtility.LoadNamedObject(node[ProjectProperty.XmlNodeName], context.Projects.Lookup);
		_author			= RedmineUtility.LoadNamedObject(node[AuthorProperty.XmlNodeName], context.Users.Lookup);
		_title			= RedmineUtility.LoadString(node[TitleProperty.XmlNodeName]);
		_description	= RedmineUtility.LoadString(node[SummaryProperty.XmlNodeName]);
		_summary		= RedmineUtility.LoadString(node[SummaryProperty.XmlNodeName]);
		_createdOn		= RedmineUtility.LoadDateRequired(node[CreatedOnProperty.XmlNodeName]);
	}

	#endregion

	#region Methods

	internal override void Update(XmlNode node)
	{
		base.Update(node);

		Project     = RedmineUtility.LoadNamedObject(node[ProjectProperty.XmlNodeName], Context.Projects.Lookup);
		Author      = RedmineUtility.LoadNamedObject(node[AuthorProperty.XmlNodeName], Context.Users.Lookup);
		Title       = RedmineUtility.LoadString(node[TitleProperty.XmlNodeName]);
		Description = RedmineUtility.LoadString(node[SummaryProperty.XmlNodeName]);
		Summary     = RedmineUtility.LoadString(node[SummaryProperty.XmlNodeName]);
		CreatedOn   = RedmineUtility.LoadDateRequired(node[CreatedOnProperty.XmlNodeName]);
	}

	#endregion

	#region Properties

	public Project Project
	{
		get => _project;
		private set => UpdatePropertyValue(ref _project, value, ProjectProperty);
	}

	public User Author
	{
		get => _author;
		private set => UpdatePropertyValue(ref _author, value, AuthorProperty);
	}

	public string Title
	{
		get => _title;
		private set => UpdatePropertyValue(ref _title, value, TitleProperty);
	}

	public string Description
	{
		get => _description;
		private set => UpdatePropertyValue(ref _description, value, DescriptionProperty);
	}

	public string Summary
	{
		get => _summary;
		private set => UpdatePropertyValue(ref _summary, value, SummaryProperty);
	}

	public DateTime CreatedOn
	{
		get => _createdOn;
		private set => UpdatePropertyValue(ref _createdOn, value, CreatedOnProperty);
	}

	#endregion

	/// <inheritdoc/>
	public override string ToString() => Title;
}
