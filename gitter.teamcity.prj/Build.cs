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

namespace gitter.TeamCity;

using System;
using System.Xml;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

public sealed class Build : TeamCityObject
{
	public static class Properties
	{
		public static readonly TeamCityObjectProperty<BuildStatus>    Status     = new("status",      nameof(Status));
		public static readonly TeamCityObjectProperty<BuildState>     State      = new("state",       nameof(State));
		public static readonly TeamCityObjectProperty<BuildType?>     BuildType  = new("buildTypeId", nameof(BuildType));
		public static readonly TeamCityObjectProperty<string?>        Number     = new("number",      nameof(Number));
		public static readonly TeamCityObjectProperty<string?>        BranchName = new("branchName",  nameof(BranchName));
		public static readonly TeamCityObjectProperty<DateTimeOffset> StartDate  = new("startDate",   nameof(StartDate));
	}

	#region Data

	private BuildType? _buildtype;
	private BuildStatus _status;
	private BuildState _state;
	private string? _number;
	private DateTimeOffset _startDate;
	private string? _branchName;

	#endregion

	#region .ctor

	internal Build(TeamCityServiceContext context, string id)
		: base(context, id)
	{
	}

	internal Build(TeamCityServiceContext context, XmlNode node)
		: base(context, node)
	{
		var attributes = node.Attributes ?? throw new ArgumentException("XML element was expected.", nameof(node));
		_status     = TeamCityUtility.LoadBuildStatus(attributes[Properties.Status.XmlNodeName]);
		_state      = TeamCityUtility.LoadBuildState(attributes[Properties.State.XmlNodeName]);
		_number		= TeamCityUtility.LoadString(attributes[Properties.Number.XmlNodeName]);
		_startDate	= TeamCityUtility.LoadRequiredDate(node.ChildNodes.Cast<XmlNode>().First(n => n.Name == Properties.StartDate.XmlNodeName));
		_buildtype	= Context.BuildTypes.Lookup(attributes[Properties.BuildType.XmlNodeName]?.InnerText);
		_branchName	= TeamCityUtility.LoadString(attributes[Properties.BranchName.XmlNodeName]);
	}

	#endregion

	#region Methods

	internal override void Update(XmlNode node)
	{
		var attributes = node.Attributes ?? throw new ArgumentException("XML element was expected.", nameof(node));
		Status     = TeamCityUtility.LoadBuildStatus(attributes[Properties.Status.XmlNodeName]);
		State      = TeamCityUtility.LoadBuildState(attributes[Properties.State.XmlNodeName]);
		Number     = TeamCityUtility.LoadString(attributes[Properties.Number.XmlNodeName]);
		StartDate  = TeamCityUtility.LoadRequiredDate(node.ChildNodes.Cast<XmlNode>().First(n => n.Name == Properties.StartDate.XmlNodeName));
		BuildType  = Context.BuildTypes.Lookup(attributes[Properties.BuildType.XmlNodeName]?.InnerText);
		BranchName = TeamCityUtility.LoadString(attributes[Properties.BranchName.XmlNodeName]);
	}

	public BuildLocator CreateLocator() => new() { Id = Id };

	private Task<string> ReadSingleFieldAsync(string fieldName, CancellationToken cancellationToken = default)
		=> Context.GetPlainTextAsync("builds/" + "id:" + Id + "/" + fieldName, cancellationToken);

	#endregion

	#region Properties

	public BuildStatus Status
	{
		get => _status;
		private set => UpdatePropertyValue(ref _status, value, Properties.Status);
	}

	public BuildState State
	{
		get => _state;
		private set => UpdatePropertyValue(ref _state, value, Properties.State);
	}

	public string? Number
	{
		get => _number;
		private set => UpdatePropertyValue(ref _number, value, Properties.Number);
	}

	public DateTimeOffset StartDate
	{
		get => _startDate;
		private set => UpdatePropertyValue(ref _startDate, value, Properties.StartDate);
	}

	public BuildType? BuildType
	{
		get => _buildtype;
		private set => UpdatePropertyValue(ref _buildtype, value, Properties.BuildType);
	}

	public string? BranchName
	{
		get => _branchName;
		private set => UpdatePropertyValue(ref _branchName, value, Properties.BranchName);
	}

	#endregion
}
