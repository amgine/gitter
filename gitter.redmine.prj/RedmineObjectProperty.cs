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

namespace gitter.Redmine
{
	using System;

	public abstract class RedmineObjectProperty
	{
		private readonly string _xmlNodeName;
		private readonly string _name;

		internal RedmineObjectProperty(string xmlNodeName, string name)
		{
			_xmlNodeName = xmlNodeName;
			_name = name;
		}

		public string XmlNodeName
		{
			get { return _xmlNodeName; }
		}

		public string Name
		{
			get { return _name; }
		}

		public abstract Type Type { get; }

		public override string ToString()
		{
			return _name;
		}
	}

	public sealed class RedmineObjectProperty<T> : RedmineObjectProperty
	{
		internal RedmineObjectProperty(string xmlNodeName, string name)
			: base(xmlNodeName, name)
		{
		}

		public override Type Type
		{
			get { return typeof(T); }
		}

		public T GetValue(RedmineObject obj)
		{
			Verify.Argument.IsNotNull(obj, nameof(obj));

			return (T)obj.GetType().GetProperty(Name).GetValue(obj, null);
		}
	}
}
