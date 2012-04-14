namespace gitter.Framework.Configuration
{
	using System;

	/// <summary>Configuration parameter.</summary>
	public sealed class Parameter : INamedObject
	{
		#region Data

		/// <summary>Parameter name.</summary>
		private readonly string _name;
		/// <summary>Parameter type.</summary>
		private Type _type;
		/// <summary>Parameter value.</summary>
		private object _value;
		/// <summary>Parameter modification flag.</summary>
		private bool _isModified;

		#endregion

		/// <summary>Initializes a new instance of the <see cref="Parameter"/> class.</summary>
		/// <param name="name">Parameter name.</param>
		/// <param name="type">Parameter type.</param>
		/// <param name="value">Parameter value.</param>
		public Parameter(string name, Type type, object value)
		{
			_name = name;
			_type = type;
			_value = value;
		}

		#region Properties

		/// <summary>Gets parameter name.</summary>
		/// <value>Parameter name.</value>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>Gets parameter type.</summary>
		/// <value>Parameter type.</value>
		public Type Type
		{
			get { return _type; }
		}

		/// <summary>Gets or sets parameter value.</summary>
		/// <value>Parameter value.</value>
		public object Value
		{
			get { return _value; }
			set
			{
				_value = value;
				_isModified = true;
			}
		}

		/// <summary>Gets a value indicating whether this parameter is modified.</summary>
		/// <value><c>true</c> if this parameter is modified; otherwise, <c>false</c>.</value>
		public bool IsModified
		{
			get { return _isModified; }
		}

		#endregion

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0} = {1}", _name, _value);
		}
	}
}
