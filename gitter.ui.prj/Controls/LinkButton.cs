namespace gitter.Controls
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;

	[DefaultEvent("LinkClicked")]
	[DefaultProperty("Text")]
	public partial class LinkButton : UserControl
	{
		private Font _underlineFont;

		private static readonly object LinkClickedEvent = new object();
		public event EventHandler LinkClicked
		{
			add { Events.AddHandler(LinkClickedEvent, value); }
			remove { Events.RemoveHandler(LinkClickedEvent, value); }
		}

		protected virtual void OnLinkClicked()
		{
			var handler = (EventHandler)Events[LinkClickedEvent];
			if(handler != null) handler(this, EventArgs.Empty);
		}

		public LinkButton()
		{
			InitializeComponent();

			_lblText.Text = Text;
			_lblText.ForeColor = GitterApplication.Style.Colors.HyperlinkText;

			if(LicenseManager.UsageMode == LicenseUsageMode.Designtime)
			{
				Font = SystemFonts.MessageBoxFont;
			}
			else
			{
				Font = GitterApplication.FontManager.UIFont;
			}
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			_lblText.Top = (Height - _lblText.Height) / 2;
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			RefreshUnderlineFont();
		}

		private void RefreshUnderlineFont()
		{
			if(_underlineFont != null)
			{
				_underlineFont.Dispose();
			}
			try
			{
				_underlineFont = new Font(Font, FontStyle.Underline);
			}
			catch
			{
				_underlineFont = (Font)Font.Clone();
			}
		}

		public Image Image
		{
			get { return _picImage.Image; }
			set { _picImage.Image = value; }
		}

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override string Text
		{
			get { return base.Text; }
			set
			{
				base.Text = value;
				_lblText.Text = value;
			}
		}

		private void OnTextLabelClick(object sender, EventArgs e)
		{
			OnLinkClicked();
		}

		private void OnImageClick(object sender, EventArgs e)
		{
			OnLinkClicked();
		}

		private void OnInteractivePartMouseEnter(object sender, EventArgs e)
		{
			if(_underlineFont == null)
			{
				RefreshUnderlineFont();
			}
			_lblText.Font = _underlineFont;
			_lblText.ForeColor = GitterApplication.Style.Colors.HyperlinkTextHotTrack;
		}

		private void OnInteractivePartMouseLeave(object sender, EventArgs e)
		{
			_lblText.Font = null;
			_lblText.ForeColor = GitterApplication.Style.Colors.HyperlinkText;
		}
	}
}
