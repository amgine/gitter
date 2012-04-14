namespace gitter.Redmine
{
	using System;

	public sealed class CustomFieldValue
	{
		#region Data

		private readonly CustomField _field;
		private string _value;

		#endregion

		#region Events

		public event EventHandler ValueChanged;

		#endregion

		#region .ctor

		public CustomFieldValue(CustomField field, string value)
		{
			_field = field;
			_value = value;
		}

		#endregion

		public CustomField Field
		{
			get { return _field; }
		}

		public string Value
		{
			get { return _value; }
			internal set
			{
				if(_value != value)
				{
					_value = value;
					var handler = ValueChanged;
					if(handler != null) handler(this, EventArgs.Empty);
				}
			}
		}
	}
}
