namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	public sealed class ConfigListBox : CustomListBox
	{
		private Repository _repository;
		private ConfigurationFile _configurationFile;

		/// <summary>Create <see cref="ConfigListBox"/>.</summary>
		public ConfigListBox()
		{
			Columns.Add(
				new NameColumn(Resources.StrParameter)
				{
					SizeMode = ColumnSizeMode.Sizeable,
					Width = 200,
				});
			Columns.Add(new ConfigParameterValueColumn());

			Items.Comparison = ConfigParameterListItem.CompareByName;
		}

		public void LoadData(Repository repository)
		{
			if(_configurationFile != null)
			{
				DetachFromConfigFile();
				_configurationFile = null;
			}
			if(_repository != repository)
			{
				if(_repository != null)
				{
					DetachFromRepository();
				}
				_repository = repository;
				if(_repository != null)
				{
					AttachToRepository();
				}
			}
		}

		public void LoadData(ConfigurationFile configurationFile)
		{
			if(_repository != null)
			{
				DetachFromRepository();
				_repository = null;
			}
			if(_configurationFile != configurationFile)
			{
				if(_configurationFile != null)
				{
					DetachFromConfigFile();
				}
				_configurationFile = configurationFile;
				if(_configurationFile != null)
				{
					AttachToConfigurationFile();
				}
			}
		}

		public void Clear()
		{
			if(_repository != null)
			{
				DetachFromRepository();
				_repository = null;
			}
			if(_configurationFile != null)
			{
				DetachFromConfigFile();
				_configurationFile = null;
			}
		}

		private void AttachToRepository()
		{
			_repository.Configuration.ParameterCreated += OnParamCreated;
			BeginUpdate();
			Items.Clear();
			lock(_repository.Configuration.SyncRoot)
			{
				foreach(var p in _repository.Configuration)
				{
					Items.Add(new ConfigParameterListItem(p));
				}
			}
			EndUpdate();
		}

		private void DetachFromRepository()
		{
			_repository.Configuration.ParameterCreated -= OnParamCreated;
			Items.Clear();
		}

		private void AttachToConfigurationFile()
		{
			_configurationFile.ParameterCreated += OnParamCreated;
			BeginUpdate();
			Items.Clear();
			lock(_configurationFile.SyncRoot)
			{
				foreach(var parameter in _configurationFile)
				{
					Items.Add(new ConfigParameterListItem(parameter));
				}
			}
			EndUpdate();
		}

		private void DetachFromConfigFile()
		{
			_configurationFile.ParameterCreated -= OnParamCreated;
			Items.Clear();
		}

		private void OnParamCreated(object sender, ConfigParameterEventArgs e)
		{
			Items.AddSafe(new ConfigParameterListItem(e.Object));
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				Clear();
			}
			base.Dispose(disposing);
		}
	}
}
