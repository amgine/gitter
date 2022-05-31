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

namespace gitter.Framework;

using System;
using System.Drawing;

using gitter.Framework.Configuration;

public abstract class IntegrationFeature : IIntegrationFeature
{
	#region Data

	private readonly bool _defaultEnabled;
	private bool _enabled;

	#endregion

	#region Events

	public event EventHandler IsEnabledChanged;

	protected virtual void OnIsEnabledChanged() => IsEnabledChanged?.Invoke(this, EventArgs.Empty);

	#endregion

	protected IntegrationFeature(string name, string displayText, IImageProvider icon, bool defaultEnabled)
	{
		Name = name;
		DisplayText = displayText;
		Icon = icon;
		_defaultEnabled = defaultEnabled;
		_enabled = defaultEnabled;
	}

	public string Name { get; }

	public string DisplayText { get; }

	public IImageProvider Icon { get; }

	public bool IsEnabled
	{
		get => _enabled;
		set
		{
			if(_enabled != value)
			{
				_enabled = value;
				OnIsEnabledChanged();
			}
		}
	}

	public virtual bool AdministratorRightsRequired => false;

	public string GetEnableAction(bool enable) => default;

	public bool HasConfiguration => true;

	public void SaveTo(Section section)
	{
		Verify.Argument.IsNotNull(section);

		section.SetValue("Enabled", _enabled);
		SaveMoreTo(section);
	}

	protected virtual void SaveMoreTo(Section section)
	{
	}

	public void LoadFrom(Section section)
	{
		Verify.Argument.IsNotNull(section);

		IsEnabled = section.GetValue("Enabled", _defaultEnabled);
		LoadMoreFrom(section);
	}

	protected virtual void LoadMoreFrom(Section section)
	{
	}
}
