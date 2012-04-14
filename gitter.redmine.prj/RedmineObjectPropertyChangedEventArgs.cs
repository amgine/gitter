namespace gitter.Redmine
{
	using System;

	public sealed class RedmineObjectPropertyChangedEventArgs : EventArgs
	{
		private readonly RedmineObjectProperty _property;

		public RedmineObjectPropertyChangedEventArgs(RedmineObjectProperty property)
		{
			_property = property;
		}

		public RedmineObjectProperty Property
		{
			get { return _property; }
		}
	}
}
