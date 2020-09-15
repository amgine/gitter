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

namespace gitter.TeamCity
{
	using System;
	using System.Xml;

	public sealed class Build : TeamCityObject
	{
		#region Static

		public static readonly TeamCityObjectProperty<BuildStatus> StatusProperty =
			new TeamCityObjectProperty<BuildStatus>("status", nameof(Status));
		public static readonly TeamCityObjectProperty<BuildType> BuildTypeProperty =
			new TeamCityObjectProperty<BuildType>("buildTypeId", nameof(BuildType));
		public static readonly TeamCityObjectProperty<string> NumberProperty =
			new TeamCityObjectProperty<string>("number", nameof(Number));
		public static readonly TeamCityObjectProperty<DateTime> StartDateProperty =
			new TeamCityObjectProperty<DateTime>("startDate", nameof(StartDate));

		#endregion

		#region Data

		private BuildType _buildtype;
		private BuildStatus _status;
		private string _number;
		private DateTime _startDate;

		#endregion

		#region .ctor

		internal Build(TeamCityServiceContext context, string id)
			: base(context, id)
		{
		}

		internal Build(TeamCityServiceContext context, XmlNode node)
			: base(context, node)
		{
			_status		= TeamCityUtility.LoadBuildStatus(node.Attributes[StatusProperty.XmlNodeName]);
			_number		= TeamCityUtility.LoadString(node.Attributes[NumberProperty.XmlNodeName]);
			_startDate	= TeamCityUtility.LoadDateForSure(node.Attributes[StartDateProperty.XmlNodeName]);
			_buildtype	= Context.BuildTypes.Lookup(node.Attributes[BuildTypeProperty.XmlNodeName].InnerText);
		}

		#endregion

		#region Methods

		internal override void Update(XmlNode node)
		{
			Status		= TeamCityUtility.LoadBuildStatus(node.Attributes[StatusProperty.XmlNodeName]);
			Number		= TeamCityUtility.LoadString(node.Attributes[NumberProperty.XmlNodeName]);
			StartDate	= TeamCityUtility.LoadDateForSure(node.Attributes[StartDateProperty.XmlNodeName]);
			BuildType	= Context.BuildTypes.Lookup(node.Attributes[BuildTypeProperty.XmlNodeName].InnerText);
		}

		public BuildLocator CreateLocator()
		{
			return new BuildLocator() { Id = Id };
		}

		private string ReadSingleField(string fieldName)
		{
			return Context.GetPlainText("builds/" + "id:" + Id + "/" + fieldName);
		}

		#endregion

		#region Properties

		public BuildStatus Status
		{
			get { return _status; }
			private set { UpdatePropertyValue(ref _status, value, StatusProperty); }
		}

		public string Number
		{
			get { return _number; }
			private set { UpdatePropertyValue(ref _number, value, NumberProperty); }
		}

		public DateTime StartDate
		{
			get { return _startDate; }
			private set { UpdatePropertyValue(ref _startDate, value, StartDateProperty); }
		}

		public BuildType BuildType
		{
			get { return _buildtype; }
			private set { UpdatePropertyValue(ref _buildtype, value, BuildTypeProperty); }
		}

		#endregion
	}
}
