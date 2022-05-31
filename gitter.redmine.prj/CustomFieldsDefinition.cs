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
using System.Collections.Generic;
using System.Xml;

public sealed class CustomFieldsDefinition : IEnumerable<CustomFieldValue>
{
	#region Data

	private readonly Dictionary<int, CustomFieldValue> _original;
	private readonly Dictionary<int, CustomFieldValue> _values;

	#endregion

	internal CustomFieldsDefinition(CustomFields original)
	{
		_original = new Dictionary<int, CustomFieldValue>();
		if(original != null)
		{
			foreach(var value in original)
			{
				_original.Add(value.Field.Id, new CustomFieldValue(value.Field, value.Value));
				_values.Add(value.Field.Id, new CustomFieldValue(value.Field, value.Value));
			}
		}
		_values = new Dictionary<int, CustomFieldValue>();
	}

	public string this[CustomField field]
	{
		get
		{
			Verify.Argument.IsNotNull(field);
			if(_values.TryGetValue(field.Id, out var value)) return value.Value;
			return null;
		}
		set
		{
			Verify.Argument.IsNotNull(field);
			if(value is null)
			{
				_values.Remove(field.Id);
			}
			else
			{
				_values[field.Id] = new CustomFieldValue(field, value);
			}
		}
	}

	internal void EmitChanged(XmlElement root)
	{
		var xml = root.OwnerDocument;
		foreach(var v in _original.Values)
		{
			if(!_values.ContainsKey(v.Field.Id))
			{
				var e = xml.CreateElement("custom_field");
				var attr = xml.CreateAttribute("id");
				attr.Value = XmlConvert.ToString(v.Field.Id);
				e.Attributes.Append(attr);
				root.AppendChild(e);
			}
		}
		foreach(var v in _values.Values)
		{
			CustomFieldValue original;
			if(_original.TryGetValue(v.Field.Id, out original) && original.Value == v.Value)
			{
				continue;
			}
			var e = xml.CreateElement("custom_field");
			var attr = xml.CreateAttribute("id");
			attr.Value = XmlConvert.ToString(v.Field.Id);
			e.Attributes.Append(attr);
			var ev = xml.CreateElement("value");
			RedmineUtility.EmitString(ev, v.Value);
			e.AppendChild(ev);
			root.AppendChild(e);
		}
	}

	internal void Reset()
	{
		_values.Clear();
		foreach(var kvp in _original)
		{
			_values.Add(kvp.Key, kvp.Value);
		}
	}

	public int Count => _values.Count;

	public IEnumerator<CustomFieldValue> GetEnumerator()
		=> _values.Values.GetEnumerator();

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		=> _values.Values.GetEnumerator();
}
