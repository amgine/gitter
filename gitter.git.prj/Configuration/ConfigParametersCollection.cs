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
using System.Collections.Generic;
using System.Threading.Tasks;

using gitter.Framework;
using gitter.Git.AccessLayer;

using Resources = gitter.Git.Properties.Resources;

/// <summary>Repository configuration.</summary>
public sealed class ConfigParametersCollection : GitObject, IEnumerable<ConfigParameter>
{
	#region Events

	/// <summary>New parameter created/detected.</summary>
	public event EventHandler<ConfigParameterEventArgs> ParameterCreated;

	/// <summary>Parameter removed/lost.</summary>
	public event EventHandler<ConfigParameterEventArgs> ParameterDeleted;

	/// <summary>Invokes <see cref="ParameterCreated"/> event.</summary>
	/// <param name="parameter">New branch.</param>
	private void InvokeParameterCreated(ConfigParameter parameter)
	{
		ParameterCreated?.Invoke(this, new ConfigParameterEventArgs(parameter));
	}

	/// <summary>Invokes <see cref="ParameterDeleted"/> &amp; other related events.</summary>
	/// <param name="parameter">Deleted parameter.</param>
	private void InvokeParameterDeleted(ConfigParameter parameter)
	{
		parameter.MarkAsDeleted();
		ParameterDeleted?.Invoke(this, new ConfigParameterEventArgs(parameter));
	}

	#endregion

	#region Data

	private readonly Dictionary<string, ConfigParameter> _parameters = new();

	#endregion

	#region .ctor

	/// <summary>Create <see cref="ConfigParametersCollection"/>.</summary>
	/// <param name="repository">Related <see cref="Repository"/>.</param>
	internal ConfigParametersCollection(Repository repository)
		: base(repository)
	{
	}

	#endregion

	#region Properties

	public IEnumerable<string> Names => _parameters.Keys;

	public ConfigParameter this[string name]
	{
		get
		{
			ConfigParameter res;
			lock(SyncRoot)
			{
				Verify.Argument.IsTrue(
					_parameters.TryGetValue(name, out res),
					"name", "Parameter not found.");
			}
			return res;
		}
	}

	public bool Exists(string name)
	{
		lock(SyncRoot)
		{
			return _parameters.ContainsKey(name);
		}
	}

	/// <summary>Returns parameter count.</summary>
	/// <value>Parameter count.</value>
	public int Count
	{
		get
		{
			lock(SyncRoot)
			{
				return _parameters.Count;
			}
		}
	}

	/// <summary>Object used for cross-thread synchronization.</summary>
	public object SyncRoot => _parameters;

	/// <summary>Gets/sets default merge tool.</summary>
	public MergeTool MergeTool
	{
		get
		{
			var toolName = TryGetParameterValue(GitConstants.MergeToolParameter);
			return toolName != null
				? MergeTool.GetCreateByName(toolName)
				: default;
		}
		set
		{
			var toolName = value == null ? "" : value.Name;
			if(TryGetParameter(GitConstants.MergeToolParameter, out var parameter))
			{
				parameter.Value = toolName;
			}
			else
			{
				CreateParameter(GitConstants.MergeToolParameter, toolName);
			}
		}
	}

	#endregion

	/// <summary>Create new configuration parameter.</summary>
	/// <param name="name">Parameter name.</param>
	/// <param name="value">Parameter value.</param>
	/// <returns>Created parameter.</returns>
	public ConfigParameter CreateParameter(string name, string value)
	{
		Verify.Argument.IsNeitherNullNorWhitespace(name);
		Verify.Argument.IsNotNull(value);

		lock(SyncRoot)
		{
			Verify.Argument.IsFalse(_parameters.ContainsKey(name), nameof(name), "Parameter already exists.");

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.ConfigUpdated))
			{
				Repository.Accessor.AddConfigValue.Invoke(
					new AddConfigValueParameters(name, value));
			}
			var parameter = new ConfigParameter(Repository, ConfigFile.Repository, name, value);
			_parameters.Add(name, parameter);
			InvokeParameterCreated(parameter);
			return parameter;
		}
	}

	/// <summary>Set value of a parameter or create a new one if it does not exist.</summary>
	/// <param name="name">Parameter name.</param>
	/// <param name="value">Parameter value.</param>
	/// <returns>Modified or created parameter.</returns>
	public ConfigParameter SetValue(string name, string value)
	{
		Verify.Argument.IsNeitherNullNorWhitespace(name);

		lock(SyncRoot)
		{
			if(_parameters.TryGetValue(name, out var parameter))
			{
				parameter.Value = value;
			}
			else
			{
				parameter = CreateParameter(name, value);
			}
			return parameter;
		}
	}

	public void SetUserIdentity(string userName, string userEmail)
	{
		Verify.Argument.IsNotNull(userName);
		Verify.Argument.IsNotNull(userEmail);

		try
		{
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.ConfigUpdated))
			{
				SetValue(GitConstants.UserNameParameter, userName);
				SetValue(GitConstants.UserEmailParameter, userEmail);
			}
		}
		finally
		{
			Repository.OnUserIdentityChanged();
		}
	}

	internal void Unset(ConfigParameter parameter)
	{
		Verify.Argument.IsNotNull(parameter);

		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.ConfigUpdated))
		{
			Repository.Accessor.UnsetConfigValue.Invoke(
				new UnsetConfigValueParameters(parameter.Name));
			Refresh(parameter);
		}
	}

	public bool TryGetParameter(string name, out ConfigParameter parameter)
	{
		lock(SyncRoot)
		{
			return _parameters.TryGetValue(name, out parameter);
		}
	}

	public bool TryGetParameterValue(string name, out string value)
	{
		lock(SyncRoot)
		{
			if(_parameters.TryGetValue(name, out var parameter))
			{
				value = parameter.Value;
				return true;
			}
			else
			{
				value = null;
				return false;
			}
		}
	}

	public ConfigParameter TryGetParameter(string name)
	{
		lock(SyncRoot)
		{
			if(_parameters.TryGetValue(name, out var parameter))
			{
				return parameter;
			}
		}
		return default;
	}

	public string TryGetParameterValue(string name)
	{
		lock(SyncRoot)
		{
			if(_parameters.TryGetValue(name, out var parameter))
			{
				return parameter.Value;
			}
		}
		return default;
	}

	#region Refresh()

	public void Refresh()
	{
		var config = Repository.Accessor.QueryConfig
			.Invoke(new QueryConfigParameters());

		lock(SyncRoot)
		{
			CacheUpdater.UpdateObjectDictionary<ConfigParameter, ConfigParameterData>(
				_parameters,
				null,
				null,
				config,
				configParameterData => ObjectFactories.CreateConfigParameter(Repository, configParameterData),
				ObjectFactories.UpdateConfigParameter,
				InvokeParameterCreated,
				InvokeParameterDeleted,
				true);
		}
	}

	public async Task RefreshAsync()
	{
		var config = await Repository.Accessor.QueryConfig
			.InvokeAsync(new QueryConfigParameters())
			.ConfigureAwait(continueOnCapturedContext: false);

		lock(SyncRoot)
		{
			CacheUpdater.UpdateObjectDictionary<ConfigParameter, ConfigParameterData>(
				_parameters,
				null,
				null,
				config,
				configParameterData => ObjectFactories.CreateConfigParameter(Repository, configParameterData),
				ObjectFactories.UpdateConfigParameter,
				InvokeParameterCreated,
				InvokeParameterDeleted,
				true);
		}
	}

	private void Refresh(ConfigParameter configParameter, ConfigParameterData configParameterData)
	{
		Assert.IsNotNull(configParameter);

		if(configParameterData == null)
		{
			lock(SyncRoot)
			{
				var name = configParameter.Name;
				_parameters.Remove(name.ToLowerInvariant());
				InvokeParameterDeleted(configParameter);
			}
		}
		else
		{
			lock(SyncRoot)
			{
				ObjectFactories.UpdateConfigParameter(configParameter, configParameterData);
			}
		}
	}

	internal void Refresh(ConfigParameter configParameter)
	{
		Verify.Argument.IsNotNull(configParameter);

		var configParameterData = Repository.Accessor.QueryConfigParameter
			.Invoke(new QueryConfigParameterParameters(configParameter.Name));
		Refresh(configParameter, configParameterData);
	}

	internal async Task RefreshAsync(ConfigParameter configParameter)
	{
		Verify.Argument.IsNotNull(configParameter);

		var configParameterData = await Repository.Accessor.QueryConfigParameter
			.InvokeAsync(new QueryConfigParameterParameters(configParameter.Name))
			.ConfigureAwait(continueOnCapturedContext: false);
		Refresh(configParameter, configParameterData);
	}

	#endregion

	#region IEnumerable<ConfigParameter>

	public Dictionary<string, ConfigParameter>.ValueCollection.Enumerator GetEnumerator()
		=> _parameters.Values.GetEnumerator();

	IEnumerator<ConfigParameter> IEnumerable<ConfigParameter>.GetEnumerator()
		=> _parameters.Values.GetEnumerator();

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		=> _parameters.Values.GetEnumerator();

	#endregion
}
