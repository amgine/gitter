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

namespace gitter.Git.Gui.Controls;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

public sealed class ConfigListBox : CustomListBox
{
	private Repository? _repository;
	private ConfigurationFile? _configurationFile;

	/// <summary>Create <see cref="ConfigListBox"/>.</summary>
	public ConfigListBox()
	{
		Columns.Add(
			new NameColumn(Resources.StrParameter)
			{
				SizeMode = ColumnSizeMode.Sizeable,
				Width    = new(200, Dpi.Default),
			});
		Columns.Add(new ConfigParameterValueColumn());

		Items.Comparison = ConfigParameterListItem.CompareByName;
	}

	public void LoadData(Repository? repository)
	{
		if(_configurationFile is not null)
		{
			DetachFromConfigFile(_configurationFile);
			_configurationFile = null;
		}
		if(_repository != repository)
		{
			if(_repository is not null)
			{
				DetachFromRepository(_repository);
			}
			_repository = repository;
			if(_repository is not null)
			{
				AttachToRepository(_repository);
			}
		}
	}

	public void LoadData(ConfigurationFile? configurationFile)
	{
		if(_repository is not null)
		{
			DetachFromRepository(_repository);
			_repository = null;
		}
		if(_configurationFile != configurationFile)
		{
			if(_configurationFile is not null)
			{
				DetachFromConfigFile(_configurationFile);
			}
			_configurationFile = configurationFile;
			if(_configurationFile is not null)
			{
				AttachToConfigurationFile(_configurationFile);
			}
		}
	}

	public void Clear()
	{
		if(_repository is not null)
		{
			DetachFromRepository(_repository);
			_repository = null;
		}
		if(_configurationFile is not null)
		{
			DetachFromConfigFile(_configurationFile);
			_configurationFile = null;
		}
	}

	private void AttachToRepository(Repository repository)
	{
		repository.Configuration.ParameterCreated += OnParamCreated;
		BeginUpdate();
		Items.Clear();
		lock(repository.Configuration.SyncRoot)
		{
			foreach(var p in repository.Configuration)
			{
				Items.Add(new ConfigParameterListItem(p));
			}
		}
		EndUpdate();
	}

	private void DetachFromRepository(Repository repository)
	{
		repository.Configuration.ParameterCreated -= OnParamCreated;
		Items.Clear();
	}

	private void AttachToConfigurationFile(ConfigurationFile file)
	{
		file.ParameterCreated += OnParamCreated;
		BeginUpdate();
		Items.Clear();
		lock(file.SyncRoot)
		{
			foreach(var parameter in file)
			{
				Items.Add(new ConfigParameterListItem(parameter));
			}
		}
		EndUpdate();
	}

	private void DetachFromConfigFile(ConfigurationFile file)
	{
		file.ParameterCreated -= OnParamCreated;
		Items.Clear();
	}

	private void OnParamCreated(object? sender, ConfigParameterEventArgs e)
	{
		Items.AddSafe(new ConfigParameterListItem(e.Object));
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			Clear();
		}
		base.Dispose(disposing);
	}
}
