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
	using System.Collections.Generic;
	using System.Xml;

	public abstract class TeamCityObject
	{
		#region Static

		public static readonly TeamCityObjectProperty<string> IdProperty =
			new TeamCityObjectProperty<string>("id", "Id");
		public static readonly TeamCityObjectProperty<string> WebUrlProperty =
			new TeamCityObjectProperty<string>("webUrl", "WebUrl");

		#endregion

		#region Data

		private readonly TeamCityServiceContext _context;
		private readonly string _id;
		private string _webUrl;

		#endregion

		#region Events

		public event EventHandler<TeamCityObjectPropertyChangedEventArgs> PropertyChanged;

		protected void OnPropertyChanged(TeamCityObjectProperty property)
		{
			var handler = PropertyChanged;
			if(handler != null)
			{
				handler(this, new TeamCityObjectPropertyChangedEventArgs(property));
			}
		}

		#endregion

		#region .ctor

		protected TeamCityObject(TeamCityServiceContext context, string id)
		{
			Verify.Argument.IsNotNull(context, nameof(context));

			_context = context;
			_id = id;
		}

		protected TeamCityObject(TeamCityServiceContext context, XmlNode node)
		{
			Verify.Argument.IsNotNull(context, nameof(context));
			Verify.Argument.IsNotNull(node, nameof(node));

			_context	= context;
			_id			= TeamCityUtility.LoadString(node.Attributes[IdProperty.XmlNodeName]);
			_webUrl		= TeamCityUtility.LoadString(node.Attributes[WebUrlProperty.XmlNodeName]);
		}

		#endregion

		#region Methods

		internal virtual void Update(XmlNode node)
		{
			Verify.Argument.IsNotNull(node, nameof(node));

			WebUrl = TeamCityUtility.LoadString(node.Attributes[WebUrlProperty.XmlNodeName]);
		}

		public virtual void Update()
		{
			throw new NotSupportedException();
		}

		public object GetValue(TeamCityObjectProperty property)
		{
			Verify.Argument.IsNotNull(property, nameof(property));

			return GetType().GetProperty(property.Name).GetValue(this, null);
		}

		protected void UpdatePropertyValue<T>(ref T field, T value, TeamCityObjectProperty<T> property)
		{
			if(!EqualityComparer<T>.Default.Equals(field, value))
			{
				field = value;
				OnPropertyChanged(property);
			}
		}

		#endregion

		#region Properties

		public string Id
		{
			get { return _id; }
		}

		public TeamCityServiceContext Context
		{
			get { return _context; }
		}

		public string WebUrl
		{
			get { return _webUrl; }
			private set { UpdatePropertyValue(ref _webUrl, value, WebUrlProperty); }
		}

		#endregion
	}
}
