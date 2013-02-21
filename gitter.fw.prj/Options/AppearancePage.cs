namespace gitter.Framework.Options
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using Resources = gitter.Framework.Properties.Resources;

	[ToolboxItem(false)]
	public partial class AppearancePage : PropertyPage, IExecutableDialog
	{
		private IGitterStyle _selectedStyle;

		public AppearancePage()
			: base(PropertyPageFactory.AppearanceGroupGuid)
		{
			InitializeComponent();

			if(GitterApplication.TextRenderer == GitterApplication.GdiPlusTextRenderer)
			{
				_radGdiPlus.Checked = true;
			}
			else if(GitterApplication.TextRenderer == GitterApplication.GdiTextRenderer)
			{
				_radGdi.Checked = true;
			}

			_selectedStyle = GitterApplication.StyleOnNextStartup;
			int yOffset = 3;
			foreach(var style in GitterApplication.Styles)
			{
				var themeRadioButton = new RadioButton()
				{
					Left		= 3,
					Top			= yOffset,
					Width		= _pnlThemesContainer.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 2,
					Text		= style.DisplayName,
					FlatStyle	= FlatStyle.System,
					Tag			= style,
					Checked		= style == SelectedStyle,
				};
				themeRadioButton.CheckedChanged += OnThemeRadioButtonCheckedChanged;
				_pnlThemesContainer.Controls.Add(themeRadioButton);
				yOffset += themeRadioButton.Height;
			}
			_pnlRestartRequiredWarning.Visible = SelectedStyle != GitterApplication.Style;
		}

		private void OnThemeRadioButtonCheckedChanged(object sender, EventArgs e)
		{
			var radioButton = (RadioButton)sender;
			if(radioButton.Checked)
			{
				var style = (IGitterStyle)radioButton.Tag;
				SelectedStyle = style;
			}
		}

		private IGitterStyle SelectedStyle
		{
			get { return _selectedStyle; }
			set
			{
				if(_selectedStyle != value)
				{
					_selectedStyle = value;
					_pnlRestartRequiredWarning.Visible = value != GitterApplication.Style;
				}
			}
		}

		public bool Execute()
		{
			if(_radGdi.Checked)
			{
				GitterApplication.TextRenderer = GitterApplication.GdiTextRenderer;
			}
			else if(_radGdiPlus.Checked)
			{
				GitterApplication.TextRenderer = GitterApplication.GdiPlusTextRenderer;
			}
			GitterApplication.StyleOnNextStartup = SelectedStyle;
			return true;
		}
	}
}
