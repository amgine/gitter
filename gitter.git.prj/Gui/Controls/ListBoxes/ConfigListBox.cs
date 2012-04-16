namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Properties.Resources;

	public sealed class ConfigListBox : CustomListBox
	{
		private Repository _repository;

		/// <summary>Create <see cref="ConfigListBox"/>.</summary>
		public ConfigListBox()
		{
			Columns.Add(new NameColumn(Resources.StrParameter)
			{
				SizeMode = ColumnSizeMode.Sizeable,
				Width = 200,
			});
			Columns.Add(new ConfigParameterValueColumn());

			Items.Comparison = ConfigParameterListItem.CompareByName;
		}

		public void LoadData(Repository repository)
		{
			if(_repository != null)
				DetachFromRepository();
			_repository = repository;
			if(_repository != null)
				AttachToRepository();
		}

		public void LoadData(ConfigFile configFile)
		{
			if(configFile != ConfigFile.System && configFile != ConfigFile.User)
			{
				throw new ArgumentException("configFile");
			}
			if(_repository != null)
			{
				DetachFromRepository();
			}
			_repository = null;
			BeginUpdate();
			Items.Clear();
			var config = RepositoryProvider.Git.QueryConfig(
				new AccessLayer.QueryConfigParameters(configFile));
			foreach(var p in config)
			{
				Items.Add(new ConfigParameterListItem(ObjectFactories.CreateConfigParameter(p)));
			}
			EndUpdate();
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

		private void OnParamCreated(object sender, ConfigParameterEventArgs e)
		{
			Items.AddSafe(new ConfigParameterListItem(e.Object));
		}
	}
}
