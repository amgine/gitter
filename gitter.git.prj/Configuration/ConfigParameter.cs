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

namespace gitter.Git;

using System;
using System.Threading.Tasks;

using gitter.Git.AccessLayer;

/// <summary>git configuration parameter.</summary>
public sealed class ConfigParameter : GitNamedObjectWithLifetime
{
	#region Events

	public event EventHandler? ValueChanged;

	private void InvokeValueChanged()
		=> ValueChanged?.Invoke(this, EventArgs.Empty);

	#endregion

	#region Data

	private readonly IConfigAccessor _configAccessor;
	private readonly ConfigFile _configFile;
	private readonly string? _fileName;
	private string _value;

	#endregion

	#region .ctor

	/// <summary>Create <see cref="ConfigParameter"/>.</summary>
	/// <param name="repository">Related <see cref="Repository"/>.</param>
	/// <param name="configFile">Configuration file.</param>
	/// <param name="name">Parameter name.</param>
	/// <param name="value">Parameter value.</param>
	internal ConfigParameter(Repository repository, ConfigFile configFile, string name, string value)
		: base(repository, name)
	{
		_configAccessor = repository.Accessor;
		_configFile = configFile;
		_value = value;
	}

	/// <summary>Create <see cref="ConfigParameter"/>.</summary>
	/// <param name="configAccessor">Configuration accessor.</param>
	/// <param name="configFile">Configuration file.</param>
	/// <param name="name">Parameter name.</param>
	/// <param name="value">Parameter value.</param>
	internal ConfigParameter(IConfigAccessor configAccessor, ConfigFile configFile, string name, string value)
		: base(name)
	{
		Verify.Argument.IsNotNull(configAccessor);

		_configAccessor = configAccessor;
		_configFile = configFile;
		_value = value;
	}

	/// <summary>Create <see cref="ConfigParameter"/>.</summary>
	/// <param name="configAccessor">Configuration accessor.</param>
	/// <param name="fileName">Configuration file.</param>
	/// <param name="name">Parameter name.</param>
	/// <param name="value">Parameter value.</param>
	internal ConfigParameter(IConfigAccessor configAccessor, string fileName, string name, string value)
		: base(name)
	{
		Verify.Argument.IsNotNull(configAccessor);

		_configAccessor = configAccessor;
		_configFile = Git.ConfigFile.Other;
		_fileName = fileName;
		_value = value;
	}

	#endregion

	#region Properties

	/// <summary>Returns or sets parameter value.</summary>
	/// <value>Parameter value.</value>
	public string Value
	{
		get => _value;
		set
		{
			Verify.State.IsNotDeleted(this);

			if(_value == value) return;

			if(Repository is not null)
			{
				using(Repository.Monitor.BlockNotifications(
					RepositoryNotifications.ConfigUpdated))
				{
					CallGitSetValue(value);
				}
			}
			else
			{
				CallGitSetValue(value);
			}
			_value = value;
			InvokeValueChanged();
		}
	}

	private void CallGitSetValue(string value)
	{
		var request = new SetConfigValueRequest(Name, value)
		{
			FileName   = _fileName,
			ConfigFile = _configFile,
		};
		_configAccessor.SetConfigValue.Invoke(request);
	}

	public ConfigFile ConfigFile => _configFile;

	public string? FileName => _fileName;

	#endregion

	public void Unset()
	{
		if(_configFile == ConfigFile.Repository)
		{
			Repository.Configuration.Unset(this);
		}
		else
		{
			_configAccessor.UnsetConfigValue.Invoke(
				new UnsetConfigValueRequest(Name)
				{
					ConfigFile = _configFile,
				});
			Refresh();
		}
	}

	/// <summary>Update parameter value.</summary>
	public void Refresh()
	{
		if(_configFile == ConfigFile.Repository)
		{
			Repository.Configuration.Refresh(this);
		}
		else
		{
			ConfigParameterData? configParameterData;
			if(_configFile == ConfigFile.Other)
			{
				configParameterData = _configAccessor.QueryConfigParameter
					.Invoke(new QueryConfigParameterRequest(_fileName!, Name));
			}
			else
			{
				configParameterData = _configAccessor.QueryConfigParameter
					.Invoke(new QueryConfigParameterRequest(_configFile, Name));
			}
			if(configParameterData is null)
			{
				MarkAsDeleted();
			}
			else
			{
				ObjectFactories.UpdateConfigParameter(this, configParameterData);
			}
		}
	}

	/// <summary>Update parameter value.</summary>
	public async Task RefreshAsync()
	{
		if(_configFile == ConfigFile.Repository)
		{
			await Repository.Configuration
				.RefreshAsync(this)
				.ConfigureAwait(continueOnCapturedContext: false);
		}
		else
		{
			ConfigParameterData? configParameterData;
			if(_configFile == ConfigFile.Other)
			{
				configParameterData = await _configAccessor.QueryConfigParameter
					.InvokeAsync(new QueryConfigParameterRequest(_fileName!, Name))
					.ConfigureAwait(continueOnCapturedContext: false);
			}
			else
			{
				configParameterData = await _configAccessor.QueryConfigParameter
					.InvokeAsync(new QueryConfigParameterRequest(_configFile, Name))
					.ConfigureAwait(continueOnCapturedContext: false);
			}
			if(configParameterData is null)
			{
				MarkAsDeleted();
			}
			else
			{
				ObjectFactories.UpdateConfigParameter(this, configParameterData);
			}
		}
	}

	internal void SetValue(string value)
	{
		if(_value != value)
		{
			_value = value;
			InvokeValueChanged();
		}
	}

	/// <inheritdoc/>
	public override string ToString() => $"{Name} = {_value}";
}
