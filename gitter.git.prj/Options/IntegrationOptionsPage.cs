namespace gitter.Git
{
	using System;
	using System.Drawing;

	using gitter.Framework;
	using gitter.Framework.Options;
	using gitter.Framework.Controls;

	using gitter.Git.Integration;
	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Properties.Resources;

	public partial class IntegrationOptionsPage : PropertyPage, IExecutableDialog
	{
		public static readonly new Guid Guid = new Guid("3C8876D2-3F45-438D-AD23-977364DA8922");

		private sealed class FeatureItem : CustomListBoxItem<IntegrationFeature>
		{
			public FeatureItem(IntegrationFeature feature)
				: base(feature)
			{
				CheckedState = feature.Enabled ? CheckedState.Checked : CheckedState.Unchecked;
			}

			protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
			{
				if((ColumnId)measureEventArgs.SubItemId == ColumnId.Name)
				{
					return measureEventArgs.MeasureImageAndText(DataContext.Icon, DataContext.DisplayText);
				}
				return Size.Empty;
			}

			protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
			{
				if((ColumnId)paintEventArgs.SubItemId == ColumnId.Name)
				{
					paintEventArgs.PaintImageAndText(DataContext.Icon, DataContext.DisplayText);
				}
			}
		}

		public IntegrationOptionsPage()
			: base(Guid)
		{
			InitializeComponent();

			Text = Resources.StrIntegration;

			_lstFeatures.BeginUpdate();
			_lstFeatures.Columns.Add(new NameColumn());
			foreach(var feature in RepositoryProvider.Integration)
			{
				_lstFeatures.Items.Add(new FeatureItem(feature));
			}
			_lstFeatures.EndUpdate();
		}

		#region IExecutableDialog Members

		public bool Execute()
		{
			foreach(FeatureItem item in _lstFeatures.Items)
			{
				item.DataContext.Enabled = item.IsChecked;
			}
			return true;
		}

		#endregion
	}
}
