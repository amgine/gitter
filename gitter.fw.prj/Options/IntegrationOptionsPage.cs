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

namespace gitter.Framework.Options;

using System;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;

using Resources = gitter.Framework.Properties.Resources;

public partial class IntegrationOptionsPage : PropertyPage, IExecutableDialog, IElevatedExecutableDialog
{
	public static readonly new Guid Guid = new("3C8876D2-3F45-438D-AD23-977364DA8922");

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
				var image = DataContext.Icon?.GetImage(measureEventArgs.Dpi.X * 16 / 96);
				return measureEventArgs.MeasureImageAndText(image, DataContext.DisplayText);
			}
			return Size.Empty;
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			if(paintEventArgs.SubItemId == 0)
			{
				var image = DataContext.Icon?.GetImage(paintEventArgs.Dpi.X * 16 / 96);
				paintEventArgs.PaintImageAndText(image, DataContext.DisplayText);
			}
		}
	}

	readonly struct DialogControls
	{
		public readonly CustomListBox _lstFeatures;
		public readonly LabelControl _lblIntegrationFeatures;

		public DialogControls(IGitterStyle style)
		{
			style ??= GitterApplication.Style;
			_lstFeatures = new CustomListBox
			{
				Style = style,
				ShowCheckBoxes = true,
				HeaderStyle = HeaderStyle.Hidden,
			};
			_lblIntegrationFeatures = new();
		}

		public void Localize()
		{
			_lblIntegrationFeatures.Text = Resources.StrsIntegrationFeatures.AddColon();
		}

		public void Layout(Control parent)
		{
			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					rows:
					[
						LayoutConstants.LabelRowHeight,
						LayoutConstants.LabelRowSpacing,
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_lblIntegrationFeatures, marginOverride: LayoutConstants.NoMargin), row: 0),
						new GridContent(new ControlContent(_lstFeatures, marginOverride: LayoutConstants.NoMargin), row: 2),
					]),
			};

			var tabIndex = 0;
			_lblIntegrationFeatures.TabIndex = tabIndex++;
			_lstFeatures.TabIndex = tabIndex++;

			_lblIntegrationFeatures.Parent = parent;
			_lstFeatures.Parent = parent;
		}
	}

	private readonly DialogControls _controls;
	private int _adminFeaturesChanged;

	public IntegrationOptionsPage()
		: base(Guid)
	{
		Name = nameof(IntegrationOptionsPage);
		Text = Resources.StrIntegration;

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Size                = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.Style);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(false);
		PerformLayout();

		_controls._lstFeatures.BeginUpdate();
		_controls._lstFeatures.Columns.Add(new CustomListBoxColumn(0, Resources.StrName) { SizeMode = ColumnSizeMode.Fill });
		foreach(var feature in GitterApplication.IntegrationFeatures)
		{
			var item = new FeatureItem(feature);
			if(feature.AdministratorRightsRequired)
			{
				item.CheckedStateChanged += OnItemCheckedStateChanged;
			}
			_controls._lstFeatures.Items.Add(item);
		}
		_controls._lstFeatures.EndUpdate();
	}

	protected override bool ScaleChildren => false;

	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(447, 354));

	private void OnItemCheckedStateChanged(object? sender, EventArgs e)
	{
		var item = (FeatureItem)sender!;
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
		foreach(FeatureItem item in _controls._lstFeatures.Items)
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

	public event EventHandler? RequireElevationChanged;

	private void OnRequireElevationChanged()
		=> RequireElevationChanged?.Invoke(this, EventArgs.Empty);

	public bool RequireElevation
	{
		get => _adminFeaturesChanged != 0;
	}

	public string[]? ElevatedExecutionActions
	{
		get
		{
			if(RequireElevation)
			{
				var d = new string[_adminFeaturesChanged];
				int i = 0;
				foreach(FeatureItem item in _controls._lstFeatures.Items)
				{
					if(item.DataContext.AdministratorRightsRequired && item.IsChecked != item.DataContext.IsEnabled)
					{
						d[i++] = item.DataContext.GetEnableAction(item.IsChecked) ?? "";
					}
				}
				return d;
			}
			else
			{
				return null;
			}
		}
	}

	#endregion

}
