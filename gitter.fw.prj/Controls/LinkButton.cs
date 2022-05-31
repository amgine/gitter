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

namespace gitter.Framework.Controls;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

[DefaultEvent("LinkClicked")]
[DefaultProperty("Text")]
[DesignerCategory("")]
public class LinkButton : UserControl
{
	private Font _underlineFont;

	private static readonly object LinkClickedEvent = new();
	public event EventHandler LinkClicked
	{
		add    => Events.AddHandler   (LinkClickedEvent, value);
		remove => Events.RemoveHandler(LinkClickedEvent, value);
	}

	protected virtual void OnLinkClicked()
		=> ((EventHandler)Events[LinkClickedEvent])?.Invoke(this, EventArgs.Empty);

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			_underlineFont?.Dispose();
		}
		base.Dispose(disposing);
	}

	private readonly Label _lblText;
	private readonly PictureBox _picImage;

	public LinkButton()
	{
		SuspendLayout();
		_lblText  = new();
		_picImage = new();
		((ISupportInitialize)_picImage).BeginInit();
		_lblText.AutoSize = true;
		_lblText.Cursor = Cursors.Hand;
		_lblText.Location = new(27, 0);
		_lblText.Name = nameof(_lblText);
		_lblText.Size = new(256, 13);
		_lblText.TabIndex = 1;
		_lblText.TextAlign = ContentAlignment.MiddleLeft;
		_lblText.Click += OnTextLabelClick;
		_lblText.MouseEnter += OnInteractivePartMouseEnter;
		_lblText.MouseLeave += OnInteractivePartMouseLeave;
		_picImage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
		_picImage.Cursor = Cursors.Hand;
		_picImage.Location = new(0, 0);
		_picImage.Margin = Padding.Empty;
		_picImage.Name = nameof(_picImage);
		_picImage.Size = new(24, 26);
		_picImage.SizeMode = PictureBoxSizeMode.CenterImage;
		_picImage.TabIndex = 0;
		_picImage.TabStop = false;
		_picImage.Click += OnImageClick;
		_picImage.MouseEnter += OnInteractivePartMouseEnter;
		_picImage.MouseLeave += OnInteractivePartMouseLeave;
		AutoScaleDimensions = new(96F, 96F);
		AutoScaleMode = AutoScaleMode.Dpi;
		Controls.Add(_lblText);
		Controls.Add(_picImage);
		Name = nameof(LinkButton);
		Size = new(256, 26);
		((ISupportInitialize)_picImage).EndInit();
		ResumeLayout(false);
		PerformLayout();

		_lblText.Text = Text;
		_lblText.ForeColor = GitterApplication.Style.Colors.HyperlinkText;

		if(LicenseManager.UsageMode == LicenseUsageMode.Designtime)
		{
			Font = SystemFonts.MessageBoxFont ?? SystemFonts.DefaultFont;
		}
		else
		{
			Font = GitterApplication.FontManager.UIFont;
		}
	}

	/// <inheritdoc/>
	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);
		var conv = new DpiConverter(this);
		var x = _picImage.Right + conv.ConvertX(3);
		_lblText.SetBounds(x, 0, Width - x, Height);
	}

	/// <inheritdoc/>
	protected override void OnFontChanged(EventArgs e)
	{
		base.OnFontChanged(e);
		RefreshUnderlineFont();
	}

	private void RefreshUnderlineFont()
	{
		if(_underlineFont is not null)
		{
			_underlineFont.Dispose();
			_underlineFont = null;
		}
		try
		{
			_underlineFont = new Font(Font, FontStyle.Underline);
		}
		catch(Exception exc) when(!exc.IsCritical())
		{
			_underlineFont = (Font)Font.Clone();
		}
	}

	public Image Image
	{
		get => _picImage.Image;
		set => _picImage.Image = value;
	}

	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public override string Text
	{
		get => base.Text;
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
		if(_underlineFont is null)
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
