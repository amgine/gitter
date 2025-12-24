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

namespace gitter.Framework.Services;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Media;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using gitter.Framework.Controls;
using gitter.Framework.Layout;

[DesignerCategory("")]
public partial class MessageBoxForm : Form
{
	static readonly IDpiBoundValue<Padding> ContentPadding = DpiBoundValue.Padding(new(20));

	private const int MaxClientWidth = 481;

	private readonly string _title;
	private readonly string _message;
	private readonly MessageBoxIcon _mbIcon;
	private readonly Icon? _icon;
	private readonly Label _lblMessage;
	private bool _buttonClick;
	private bool _hasCancelButton;
	private readonly IButtonWidget[] _buttons;
	private readonly Panel? _picIcon;

	public MessageBoxForm(MessageBoxButton button, MessageBoxIcon icon, string message, string caption)
		: this([button], icon, message, caption)
	{
	}

	public MessageBoxForm(IReadOnlyList<MessageBoxButton> buttons, MessageBoxIcon icon, string message, string title)
	{
		SuspendLayout();

		_title   = title;
		_message = message;
		_mbIcon  = icon;
		_icon    = GetSystemIcon(icon);

		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode = AutoScaleMode.Dpi;
		Font = GitterApplication.FontManager.UIFont;
		FormBorderStyle = FormBorderStyle.FixedDialog;
		MaximizeBox = false;
		MinimizeBox = false;
		Name = nameof(MessageBoxForm);
		Text = title;
		ShowIcon = false;
		ShowInTaskbar = false;
		StartPosition = FormStartPosition.CenterParent;
		KeyPreview = true;

		Panel pnlButtons;

		var style = GitterApplication.Style;
		var colors = style.Colors;

		BackColor = colors.Window;
		ForeColor = colors.WindowText;

		_ = new ControlLayout(this)
		{
			Content = new Grid(
				rows:
				[
					SizeSpec.Everything(),
					SizeSpec.Absolute(1),
					SizeSpec.Absolute(39),
				],
				content:
				[
					_icon is not null
						? new GridContent(new Grid(
							padding: ContentPadding,
							columns:
							[
								SizeSpec.Absolute(32),
								SizeSpec.Absolute(6),
								SizeSpec.Everything(),
							],
							rows:
							[
								SizeSpec.Absolute(32),
								SizeSpec.Everything(),
							],
							content:
							[
								new GridContent(new ControlContent(_picIcon = new()
								{
									TabStop = false,
									Parent = this,
								}, marginOverride: LayoutConstants.NoMargin), row: 0, column: 0),
								new GridContent(new ControlContent(_lblMessage = new()
								{
									AutoSize = false,
									Text = message,
									Parent = this,
								}, marginOverride: LayoutConstants.NoMargin), rowSpan: 2, column: 2),
							]), row: 0)
						: new GridContent(new ControlContent(_lblMessage = new()
							{
								AutoSize = false,
								Text = message,
								Parent = this,
							}, marginOverride: ContentPadding)),

					new GridContent(new ControlContent(new Panel
					{
						BackColor = colors.WindowFooterSeparator,
						Parent    = this,
					},
					marginOverride: LayoutConstants.NoMargin,
					horizontalContentAlignment: HorizontalContentAlignment.Stretch,
					verticalContentAlignment:   VerticalContentAlignment.Stretch),
					row: 1),
					new GridContent(new ControlContent(pnlButtons = new Panel
					{
						BackColor = colors.WindowFooter,
						Parent    = this,
					},
					marginOverride: LayoutConstants.NoMargin,
					horizontalContentAlignment: HorizontalContentAlignment.Stretch,
					verticalContentAlignment:   VerticalContentAlignment.Stretch),
					row: 2),
				]),
		};

		if(_picIcon is not null && _icon is not null)
		{
			_picIcon.Paint += OnIconPaint;
		}

		const int ButtonHeight  = 23;
		const int TopMargin     =  8;
		const int RightMargin   =  6;

		const int firstOffset = 1;

		static GridContent WrapButton(IButtonWidget button, int index)
			=> new(new ControlContent(button.Control,
				marginOverride: LayoutConstants.NoMargin,
				horizontalContentAlignment: HorizontalContentAlignment.Stretch,
				verticalContentAlignment: VerticalContentAlignment.Stretch),
				row: 1, column: index * 2 + firstOffset);

		var btnCount       = buttons.Count;
		var buttonFactory  = GitterApplication.Style.ButtonFactory;
		var buttonsContent = new GridContent[btnCount];

		var buttonWidth   = SizeSpec.Absolute(75);
		var buttonSpacing = SizeSpec.Absolute(6);

		_buttons = new IButtonWidget[btnCount];

		var tabIndex = 0;
		var columns = new ISizeSpec[2 + btnCount * 2 - 1];
		columns[0] = SizeSpec.Everything();
		for(var i = 0; i < btnCount; ++i)
		{
			columns[i * 2 + firstOffset + 0] = buttonWidth;
			if(i < btnCount - 1)
			{
				columns[i * 2 + firstOffset + 1] = buttonSpacing;
			}

			var b = buttons[i];
			var button = buttonFactory.Create();
			button.Tag  = b;
			button.Text = b.DisplayLabel;
			button.TabIndex = tabIndex++;
			button.Click += OnButtonClick;
			button.Parent = pnlButtons;
			if(b.IsDefault)
			{
				AcceptButton = button.Control as IButtonControl;
			}
			if(b.DialogResult == DialogResult.Cancel)
			{
				_hasCancelButton = true;
			}
			if(CancelButton is null &&
				b.DialogResult is DialogResult.Abort
				               or DialogResult.Cancel
				               or DialogResult.No)
			{
				CancelButton = button.Control as IButtonControl;
			}
			buttonsContent[i] = WrapButton(button, i);
			_buttons[i] = button;
		}
		columns[columns.Length - 1] = SizeSpec.Absolute(RightMargin);

		_ = new ControlLayout(pnlButtons)
		{
			Content = new Grid(
				rows:
				[
					SizeSpec.Absolute(TopMargin),
					SizeSpec.Absolute(ButtonHeight),
					SizeSpec.Everything(),
				],
				columns: columns,
				content: buttonsContent),
		};

		ResumeLayout(performLayout: false);
		PerformLayout();
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	protected override void OnHandleCreated(EventArgs e)
	{
		this.EnableImmersiveDarkModeIfNeeded();
		if(!_hasCancelButton && _buttons is { Length: > 1 })
		{
			this.DisableCloseButton();
		}
		base.OnHandleCreated(e);
	}

	/// <inheritdoc/>
	protected override void OnDpiChanged(DpiChangedEventArgs e)
	{
		base.OnDpiChanged(e);
		UpdateClientSize();
	}

	private void UpdateClientSize()
	{
		var conv = DpiConverter.FromDefaultTo(this);
		var h = 0;
		var p = ContentPadding.GetValue(conv.To);

		h += conv.ConvertY(1);
		h += conv.ConvertY(39);
		h += p.Vertical;

		var suggestedWidth = conv.ConvertX(MaxClientWidth);
		var size = TextRenderer.MeasureText(
			_message, Font, new(suggestedWidth, short.MaxValue),
			TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak);

		bool centerMessage = false;

		var iconH = conv.ConvertY(32);
		if(size.Height < iconH)
		{
			centerMessage = true;
			size.Height = iconH;
		}

		var minWidth = conv.ConvertX(200);
		if(size.Width < minWidth)
		{
			size.Width = minWidth;
		}

		h += size.Height;
		int maxMessageHeight = Screen.GetBounds(this).Height * 3 / 4;

		if(h > maxMessageHeight) h = maxMessageHeight;

		_lblMessage.TextAlign = centerMessage ? ContentAlignment.MiddleLeft : ContentAlignment.TopLeft;
		ClientSize = new(size.Width, h);
	}

	private static Icon? GetSystemIcon(MessageBoxIcon icon)
		=> icon switch
		{
			MessageBoxIcon.Information => SystemIcons.Information,
			MessageBoxIcon.Error       => SystemIcons.Error,
			MessageBoxIcon.Exclamation => SystemIcons.Exclamation,
			MessageBoxIcon.Question    => SystemIcons.Question,
			_ => null,
		};

	private void OnButtonClick(object? sender, EventArgs e)
	{
		if(sender is not IButtonWidget { Tag: MessageBoxButton button }) return;

		DialogResult = button.DialogResult;
		_buttonClick = true;
		Close();
	}

	/// <inheritdoc/>
	protected override void OnFormClosing(FormClosingEventArgs e)
	{
		base.OnFormClosing(e);
		if(!_buttonClick)
		{
			DialogResult = DialogResult.Cancel;
		}
	}

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		UpdateClientSize();
		PlaySystemSound();
		base.OnLoad(e);
	}

	/// <inheritdoc/>
	protected override void OnKeyDown(KeyEventArgs e)
	{
		switch(e.KeyCode)
		{
			case Keys.C when e.Modifiers == Keys.Control:
				CopyToClipboard();
				SystemSounds.Beep.Play();
				e.Handled = true;
				break;
		}

		base.OnKeyDown(e);
	}

	private void PlaySystemSound()
	{
		var sound = _mbIcon switch
		{
			MessageBoxIcon.Information => SystemSounds.Asterisk,
			MessageBoxIcon.Error       => SystemSounds.Hand,
			MessageBoxIcon.Exclamation => SystemSounds.Exclamation,
			MessageBoxIcon.Question    => SystemSounds.Question,
			_ => default,
		};
		sound?.Play();
	}

	public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButton button, MessageBoxIcon icon)
	{
		return Show(owner, text, caption, [button], icon);
	}

	public static DialogResult Show(IWin32Window owner, string text, string caption, IReadOnlyList<MessageBoxButton> buttons, MessageBoxIcon icon)
	{
		using var form = new MessageBoxForm(buttons, icon, text, caption);
		form.ShowDialog(owner);
		return form.DialogResult;
	}

	private void OnIconPaint(object? sender, PaintEventArgs e)
	{
		if(sender is not Control iconDisplay) return;
		if(_icon is null) return;
		e.Graphics.DrawIcon(_icon, iconDisplay.ClientRectangle);
	}

	private void CopyToClipboard()
	{
		const string separator = "---------------------------";

		var sb = new StringBuilder();
		sb.AppendLine(separator);
		sb.AppendLine(_title);
		sb.AppendLine(separator);
		sb.AppendLine(_message);
		sb.AppendLine(separator);
		foreach(var button in _buttons)
		{
			sb.Append(button.Text);
			sb.Append("   ");
		}
		sb.AppendLine();
		sb.AppendLine(separator);
		ClipboardEx.TrySetTextSafe(sb.ToString());
	}
}
