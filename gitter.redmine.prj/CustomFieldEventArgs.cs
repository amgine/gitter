namespace gitter.Redmine
{
	using System;

	public sealed class CustomFieldEventArgs : EventArgs
	{
		private readonly CustomField _customField;

		public CustomFieldEventArgs(CustomField customField)
		{
			_customField = customField;
		}

		public CustomField CustomField
		{
			get { return _customField; }
		}
	}
}
