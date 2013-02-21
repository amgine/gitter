namespace gitter.Framework.Options
{
	using System;
	using System.Drawing;

	using gitter.Framework;
	using gitter.Framework.Options;
	using gitter.Framework.Controls;

	using Resources = gitter.Framework.Properties.Resources;

	public partial class IntegrationOptionsPage : PropertyPage, IExecutableDialog, IElevatedExecutableDialog
	{
		public static readonly new Guid Guid = new Guid("3C8876D2-3F45-438D-AD23-977364DA8922");

		private sealed class FeatureItem : CustomListBoxItem<IIntegrationFeature>
		{
			public FeatureItem(IIntegrationFeature feature)
				: base(feature)
			{
				CheckedState = feature.IsEnabled ? CheckedState.Checked : CheckedState.Unchecked;
			}

			protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
			{
				if(measureEventArgs.SubItemId == 0)
				{
					return measureEventArgs.MeasureImageAndText(DataContext.Icon, DataContext.DisplayText);
				}
				return Size.Empty;
			}

			protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
			{
				if(paintEventArgs.SubItemId == 0)
				{
					paintEventArgs.PaintImageAndText(DataContext.Icon, DataContext.DisplayText);
				}
			}
		}

		private int _adminFeaturesChanged;

		public IntegrationOptionsPage()
			: base(Guid)
		{
			InitializeComponent();

			Text = Resources.StrIntegration;

			_lblIntegrationFeatures.Text = Resources.StrsIntegrationFeatures.AddColon();

			_lstFeatures.BeginUpdate();
			_lstFeatures.Style = GitterApplication.DefaultStyle;
			_lstFeatures.Columns.Add(new CustomListBoxColumn(0, Resources.StrName) { SizeMode = ColumnSizeMode.Fill });
			foreach(var feature in GitterApplication.IntegrationFeatures)
			{
				var item = new FeatureItem(feature);
				if(feature.AdministratorRightsRequired)
				{
					item.CheckedStateChanged += OnItemCheckedStateChanged;
				}
				_lstFeatures.Items.Add(item);
			}
			_lstFeatures.EndUpdate();
		}

		private void OnItemCheckedStateChanged(object sender, EventArgs e)
		{
			var item = (FeatureItem)sender;
			if(item.IsChecked != item.DataContext.IsEnabled)
			{
				++_adminFeaturesChanged;
				if(_adminFeaturesChanged == 1)
				{
					OnRequireElevationChanged();
				}
			}
			else
			{
				--_adminFeaturesChanged;
				if(_adminFeaturesChanged == 0)
				{
					OnRequireElevationChanged();
				}
			}
		}

		#region IExecutableDialog Members

		public bool Execute()
		{
			foreach(FeatureItem item in _lstFeatures.Items)
			{
				var feature = item.DataContext;
				if(feature.AdministratorRightsRequired)
				{
					item.CheckedStateChanged -= OnItemCheckedStateChanged;
					item.IsChecked = feature.IsEnabled;
					item.CheckedStateChanged += OnItemCheckedStateChanged;
				}
				else
				{
					feature.IsEnabled = item.IsChecked;
				}
			}
			if(_adminFeaturesChanged != 0)
			{
				_adminFeaturesChanged = 0;
				OnRequireElevationChanged();
			}
			return true;
		}

		#endregion

		#region IElevatedExecutableDialog Members

		public event EventHandler RequireElevationChanged;

		private void OnRequireElevationChanged()
		{
			var handler = RequireElevationChanged;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		public bool RequireElevation
		{
			get { return _adminFeaturesChanged != 0; }
		}

		public Action ElevatedExecutionAction
		{
			get
			{
				if(RequireElevation)
				{
					var d = new Delegate[_adminFeaturesChanged];
					int i = 0;
					foreach(FeatureItem item in _lstFeatures.Items)
					{
						if(item.DataContext.AdministratorRightsRequired && item.IsChecked != item.DataContext.IsEnabled)
						{
							d[i++] = item.DataContext.GetEnableAction(item.IsChecked);
						}
					}
					return d.Length == 1 ? (Action)d[0] : (Action)Delegate.Combine(d);
				}
				else
				{
					return null;
				}
			}
		}

		#endregion

	}
}
