namespace gitter.TeamCity
{
	using System;

	public sealed class TeamCityObjectPropertyChangedEventArgs : EventArgs
	{
		private readonly TeamCityObjectProperty _property;

		public TeamCityObjectPropertyChangedEventArgs(TeamCityObjectProperty property)
		{
			_property = property;
		}

		public TeamCityObjectProperty Property
		{
			get { return _property; }
		}
	}
}
