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
	using System.Collections.Generic;
	using System.Xml;

	public sealed class CustomFields : IEnumerable<CustomFieldValue>
	{
		#region Data

		private readonly Dictionary<int, CustomFieldValue> _values;

		#endregion

		#region Events

		public event EventHandler<CustomFieldEventArgs> CustomFieldAdded;
		public event EventHandler<CustomFieldEventArgs> CustomFieldRemoved;

		#endregion

		internal CustomFields()
		{
			_values = new Dictionary<int, CustomFieldValue>();
		}

		internal CustomFields(XmlNode node, Func<int, string, CustomField> initializer)
		{
			Verify.Argument.IsNotNull(initializer, "initializer");

			_values = new Dictionary<int, CustomFieldValue>();
			if(node != null)
			{
				foreach(XmlNode childNode in node.ChildNodes)
				{
					var field = RedmineUtility.LoadNamedObject(childNode, initializer);
					var value = RedmineUtility.LoadString(childNode["value"]);
					_values.Add(field.Id, new CustomFieldValue(field, value));
				}
			}
		}

		internal void Update(XmlNode node, Func<int, string, CustomField> initializer)
		{
			if(node == null || node.ChildNodes.Count == 0)
			{
				_values.Clear();
			}
			else
			{
				var hs = Count != 0 ? new HashSet<int>(_values.Keys) : null;
				foreach(XmlNode childNode in node.ChildNodes)
				{
					var field = RedmineUtility.LoadNamedObject(childNode, initializer);
					var value = RedmineUtility.LoadString(childNode["value"]);
					CustomFieldValue cfv;
					if(_values.TryGetValue(field.Id, out cfv))
					{
						cfv.Value = value;
						hs.Remove(field.Id);
					}
					else
					{
						cfv = new CustomFieldValue(field, value);
						_values.Add(field.Id, cfv);
						var handler = CustomFieldAdded;
						if(handler != null) handler(this, new CustomFieldEventArgs(field));
					}
				}
				if(hs != null)
				{
					var handler2 = CustomFieldRemoved;
					foreach(var id in hs)
					{
						var cfv = _values[id];
						_values.Remove(id);
						if(handler2 != null)
						{
							handler2(this, new CustomFieldEventArgs(cfv.Field));
						}
					}
				}
			}
		}

		public string this[int id]
		{
			get
			{
				CustomFieldValue res;
				if(_values.TryGetValue(id, out res)) return res.Value;
				return null;
			}
		}

		public string this[CustomField field]
		{
			get
			{
				Verify.Argument.IsNotNull(field, "field");
				return this[field.Id];
			}
		}

		public int Count
		{
			get { return _values.Count; }
		}

		public IEnumerator<CustomFieldValue> GetEnumerator()
		{
			return _values.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _values.Values.GetEnumerator();
		}
	}
}
