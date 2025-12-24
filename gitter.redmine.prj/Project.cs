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

public sealed class Project : NamedRedmineObject
{
	#region Static

	public static readonly RedmineObjectProperty<string>   IdentifierProperty  = new("identifier",  nameof(Identifier));
	public static readonly RedmineObjectProperty<string>   DescriptionProperty = new("description", nameof(Description));
	public static readonly RedmineObjectProperty<Project>  ParentProperty      = new("parent",      nameof(Parent));
	public static readonly RedmineObjectProperty<DateTime> CreatedOnProperty   = new("created_on",  nameof(CreatedOn));
	public static readonly RedmineObjectProperty<DateTime> UpdatedOnProperty   = new("updated_on",  nameof(UpdatedOn));

	#endregion

	#region Data

	private Project _parent;
	private string _identifier;
	private string _description;
	private DateTime _createdOn;
	private DateTime _updatedOn;

	#endregion

	#region .ctor

	internal Project(RedmineServiceContext context, int id, string name)
		: base(context, id, name)
	{
	}

	internal Project(RedmineServiceContext context, XmlNode node)
		: base(context, node)
	{
		_identifier		= RedmineUtility.LoadString(node[IdentifierProperty.XmlNodeName]);
		_description	= RedmineUtility.LoadString(node[DescriptionProperty.XmlNodeName]);
		_parent			= RedmineUtility.LoadNamedObject(node[ParentProperty.XmlNodeName], context.Projects.Lookup);
		_createdOn		= RedmineUtility.LoadDateRequired(node[CreatedOnProperty.XmlNodeName]);
		_updatedOn		= RedmineUtility.LoadDateRequired(node[UpdatedOnProperty.XmlNodeName]);
	}

	#endregion

	#region Methods

	internal override void Update(XmlNode node)
	{
		base.Update(node);

		Identifier		= RedmineUtility.LoadString(node[IdentifierProperty.XmlNodeName]);
		Description		= RedmineUtility.LoadString(node[DescriptionProperty.XmlNodeName]);
		Parent			= RedmineUtility.LoadNamedObject(node[ParentProperty.XmlNodeName], Context.Projects.Lookup);
		CreatedOn		= RedmineUtility.LoadDateRequired(node[CreatedOnProperty.XmlNodeName]);
		UpdatedOn		= RedmineUtility.LoadDateRequired(node[UpdatedOnProperty.XmlNodeName]);
	}

	public override Task UpdateAsync(CancellationToken cancellationToken = default)
	{
		var url = string.Format(CultureInfo.InvariantCulture,
			@"projects/{0}.xml", Id);
		return Context.Projects.FetchSingleItemAsync(url, cancellationToken);
	}

	#endregion

	#region Properties

	public string Description
	{
		get => _description;
		private set => UpdatePropertyValue(ref _description, value, DescriptionProperty);
	}

	public string Identifier
	{
		get => _identifier;
		private set => UpdatePropertyValue(ref _identifier, value, IdentifierProperty);
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

	public Project Parent
	{
		get => _parent;
		private set => UpdatePropertyValue(ref _parent, value, ParentProperty);
	}

	#endregion
}
