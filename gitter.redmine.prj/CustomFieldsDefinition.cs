namespace gitter.Redmine
{
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
				if(field == null) throw new ArgumentNullException("field");
				CustomFieldValue value;
				if(_values.TryGetValue(field.Id, out value)) return value.Value;
				return null;
			}
			set
			{
				if(field == null) throw new ArgumentNullException("field");
				if(value == null)
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
