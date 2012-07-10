namespace gitter.Git.Integration
{
	using System;
	using System.Drawing;

	using gitter.Framework;
	using gitter.Framework.Configuration;

	public abstract class IntegrationFeature : INamedObject
	{
		#region Data

		private readonly string _name;
		private readonly bool _defaultEnabled;
		private readonly string _displayText;
		private readonly Bitmap _icon;
		private bool _enabled;

		#endregion

		public event EventHandler EnabledChanged;

		protected IntegrationFeature(string name, string displayText, Bitmap icon, bool defaultEnabled)
		{
			_name = name;
			_displayText = displayText;
			_icon = icon;
			_defaultEnabled = defaultEnabled;
			_enabled = defaultEnabled;
		}

		public string Name
		{
			get { return _name; }
		}

		public string DisplayText
		{
			get { return _displayText; }
		}

		public Bitmap Icon
		{
			get { return _icon; }
		}

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if(_enabled != value)
				{
					_enabled = value;
					EnabledChanged.Raise(this);
				}
			}
		}

		public void SaveTo(Section section)
		{
			section.SetValue("Enabled", _enabled);
			SaveMoreTo(section);
		}

		protected virtual void SaveMoreTo(Section section)
		{
		}

		public void LoadFrom(Section section)
		{
			Enabled = section.GetValue("Enabled", _defaultEnabled);
			LoadMoreFrom(section);
		}

		protected virtual void LoadMoreFrom(Section section)
		{
		}
	}
}
