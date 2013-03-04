namespace gitter.Controls
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Framework.Options;

	using Resources = gitter.Properties.Resources;

	public class ServicePicker : CustomPopupComboBox
	{
		private static readonly StringFormat StringFormat = new StringFormat(StringFormat.GenericTypographic)
		{
			LineAlignment = StringAlignment.Center,
		};

		private CustomListBox _lstServiceProviders;
		private IRepositoryServiceProvider _selectedRemote;

		/// <summary>Initializes a new instance of the <see cref="RemotePicker"/> class.</summary>
		public ServicePicker()
		{
			_lstServiceProviders = new CustomListBox()
			{
				Style = GitterApplication.DefaultStyle,
				HeaderStyle = HeaderStyle.Hidden,
				BorderStyle = BorderStyle.FixedSingle,
				ItemActivation = gitter.Framework.Controls.ItemActivation.SingleClick,
				Size = new Size(Width, 2 + 2 + 21 * 5),
				DisableContextMenus = true,
				Font = LicenseManager.UsageMode == LicenseUsageMode.Runtime?
					GitterApplication.FontManager.UIFont.Font:
					SystemFonts.MessageBoxFont,
			};
			_lstServiceProviders.Columns.Add(
				new CustomListBoxColumn(0, Resources.StrName)
				{
					SizeMode = ColumnSizeMode.Auto,
				});
			_lstServiceProviders.ItemActivated += OnItemActivated;

			DropDownStyle = ComboBoxStyle.DropDownList;
			DrawMode = DrawMode.OwnerDrawFixed;

			DropDownControl = _lstServiceProviders;
		}

		public IRepositoryServiceProvider SelectedIssueTracker
		{
			get { return _selectedRemote; }
			set
			{
				if(_selectedRemote != value)
				{
					_selectedRemote = value;
					if(_selectedRemote == null)
					{
						Text = Resources.StrlNone.SurroundWith('<', '>');
						ForeColor = SystemColors.GrayText;
					}
					else
					{
						Text = _selectedRemote.Name;
						ForeColor = SystemColors.WindowText;
					}
					OnSelectedItemChanged(EventArgs.Empty);
					OnSelectedIndexChanged(EventArgs.Empty);
				}
			}
		}

		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			var graphics = e.Graphics;
			var bounds = e.Bounds;
			var iconBounds = e.Bounds;
			iconBounds.Width = 16;
			var d = (iconBounds.Height - 16);
			iconBounds.Y += d / 2;
			iconBounds.Height = 16;
			bounds.X += iconBounds.Width + 3;
			bounds.Width -= iconBounds.Width + 3;

			e.DrawBackground();
			graphics.TextRenderingHint = Utility.TextRenderingHint;
			graphics.TextContrast = Utility.TextContrast;
			if(_selectedRemote == null)
			{
				GitterApplication.TextRenderer.DrawText(
					graphics, Resources.StrlNone.SurroundWith('<', '>'), Font,
					((e.State & DrawItemState.Selected) == DrawItemState.Selected) ?
					SystemBrushes.HighlightText : SystemBrushes.GrayText, bounds, StringFormat);
			}
			else
			{
				var icon = _selectedRemote.Icon;
				if(icon != null)
				{
					graphics.DrawImage(icon, iconBounds, new Rectangle(0, 0, icon.Width, icon.Height), GraphicsUnit.Pixel);
				}
				GitterApplication.TextRenderer.DrawText(
					graphics, _selectedRemote.DisplayName, Font,
					((e.State & DrawItemState.Selected) == DrawItemState.Selected) ?
					SystemBrushes.HighlightText : SystemBrushes.WindowText, bounds, StringFormat);
			}
		}

		public CustomListBox ServiceProviders
		{
			get { return _lstServiceProviders; }
		}

		private void OnItemActivated(object sender, ItemEventArgs e)
		{
			var remoteItem = e.Item as ServiceProviderListItem;
			if(remoteItem != null)
			{
				SelectedIssueTracker = remoteItem.DataContext;
				HideDropDown();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_lstServiceProviders != null)
				{
					_lstServiceProviders.ItemActivated -= OnItemActivated;
					_lstServiceProviders.Dispose();
					_lstServiceProviders = null;
				}
				_selectedRemote = null;
			}
			base.Dispose(disposing);
		}
	}
}
