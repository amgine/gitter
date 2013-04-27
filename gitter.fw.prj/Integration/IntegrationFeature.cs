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

namespace gitter.Framework
{
	using System;
	using System.Drawing;

	using gitter.Framework.Configuration;

	public abstract class IntegrationFeature : IIntegrationFeature
	{
		#region Data

		private readonly string _name;
		private readonly bool _defaultEnabled;
		private readonly string _displayText;
		private readonly Bitmap _icon;
		private bool _enabled;

		#endregion

		#region Events

		public event EventHandler IsEnabledChanged;

		protected virtual void OnIsEnabledChanged()
		{
			var handler = IsEnabledChanged;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		#endregion

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

		public bool IsEnabled
		{
			get { return _enabled; }
			set
			{
				if(_enabled != value)
				{
					_enabled = value;
					OnIsEnabledChanged();
				}
			}
		}

		public virtual bool AdministratorRightsRequired
		{
			get { return false; }
		}

		public Action GetEnableAction(bool enable)
		{
			return () => IsEnabled = enable;
		}

		public bool HasConfiguration
		{
			get { return true; }
		}

		public void SaveTo(Section section)
		{
			Verify.Argument.IsNotNull(section, "section");

			section.SetValue("Enabled", _enabled);
			SaveMoreTo(section);
		}

		protected virtual void SaveMoreTo(Section section)
		{
		}

		public void LoadFrom(Section section)
		{
			Verify.Argument.IsNotNull(section, "section");

			IsEnabled = section.GetValue("Enabled", _defaultEnabled);
			LoadMoreFrom(section);
		}

		protected virtual void LoadMoreFrom(Section section)
		{
		}
	}
}
