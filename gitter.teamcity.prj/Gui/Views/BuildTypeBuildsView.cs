namespace gitter.TeamCity.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Linq;
	using System.Text;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.TeamCity.Properties.Resources;

	sealed class BuildTypeBuildsView : TeamCityViewBase
	{
		private BuildType _buildType;
		private CustomListBox _lstBuilds;

		public BuildTypeBuildsView(IWorkingEnvironment environment, IDictionary<string, object> parameters)
			: base(Guids.BuildTypeBuildsViewGuid, environment, parameters)
		{
			_lstBuilds = new CustomListBox();
			_lstBuilds.BorderStyle = System.Windows.Forms.BorderStyle.None;
			_lstBuilds.Columns.AddRange(new CustomListBoxColumn[]
				{
					new CustomListBoxColumn((int)ColumnId.Id, Resources.StrId) { Width = 50 },
					new CustomListBoxColumn((int)ColumnId.Number, Resources.StrNumber) { Width = 150 },
					new DateColumn((int)ColumnId.StartDate, Resources.StrStartDate, true) { Width = 150 },
					new CustomListBoxColumn((int)ColumnId.Status, Resources.StrStatus) { Width = 100 },
				});
			_lstBuilds.Bounds = this.ClientRectangle;
			_lstBuilds.Anchor = System.Windows.Forms.AnchorStyles.Left |
								System.Windows.Forms.AnchorStyles.Top |
								System.Windows.Forms.AnchorStyles.Right |
								System.Windows.Forms.AnchorStyles.Bottom;
			_lstBuilds.Parent = this;

			ApplyParameters(parameters);
		}

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgBuildType"]; }
		}

		public override void ApplyParameters(IDictionary<string, object> parameters)
		{
			base.ApplyParameters(parameters);

			object buildType;
			if(parameters.TryGetValue("BuildType", out buildType) && buildType != null)
			{
				_buildType = buildType as BuildType;
			}
			else
			{
				_buildType = null;
			}
			if(_buildType != null)
			{
				Text = _buildType.Name;
			}
			else
			{
				Text = string.Empty;
			}
			if(ServiceContext != null)
			{
				RefreshContent();
			}
		}

		protected override void OnContextAttached()
		{
			base.OnContextAttached();
			RefreshContent();
		}

		protected override void OnContextDetached()
		{
			base.OnContextDetached();
			RefreshContent();
		}

		public override void RefreshContent()
		{
			base.RefreshContent();

			if(ServiceContext != null && _buildType != null)
			{
				_lstBuilds.BeginUpdate();
				_lstBuilds.Items.Clear();
				_buildType.Builds.Refresh();
				lock(_buildType.Builds.SyncRoot)
				{
					foreach(var build in _buildType.Builds)
					{
						_lstBuilds.Items.Add(new BuildListItem(build));
					}
				}
				_lstBuilds.EndUpdate();
			}
			else
			{
				_lstBuilds.Items.Clear();
			}
		}
	}
}
