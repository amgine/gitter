﻿#region Copyright Notice
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

using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace gitter.Framework.Controls;

/// <summary>TextBox which displays grayed hint text if no text is entered.</summary>
[DesignerCategory("")]
public class HintTextBox : TextBox
{
	private bool _userTextEntered;
	private string _hint;

	public HintTextBox()
	{
		EnterHintMode();
		_hint = string.Empty;
	}

	private static bool TextIsValid(string text)
	{
		return !string.IsNullOrWhiteSpace(text);
	}

	private void EnterHintMode()
	{
		_userTextEntered = false;
		ForeColor = HintForeColor;
		base.Text = Hint;
	}

	private void EnterNormalMode()
	{
		_userTextEntered = true;
		ForeColor = TextForeColor;
	}

	protected override void OnGotFocus(EventArgs e)
	{
		base.OnGotFocus(e);
		if(!_userTextEntered)
		{
			EnterNormalMode();
			base.Text = string.Empty;
		}
	}

	protected override void OnLostFocus(EventArgs e)
	{
		base.OnLostFocus(e);
		if(TextIsValid(base.Text))
		{
			_userTextEntered = true;
		}
		else
		{
			EnterHintMode();
		}
	}

	[DefaultValue("GrayText")]
	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public Color HintForeColor { get; set; } = SystemColors.GrayText;

	[DefaultValue("WindowText")]
	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public Color TextForeColor { get; set; } = SystemColors.WindowText;

	[DefaultValue("")]
	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Description("Text to display on unfocused control when no text is entered.")]
	public string Hint
	{
		get => _hint;
		set
		{
			_hint = value;
			if(!_userTextEntered)
			{
				base.Text = value;
			}
		}
	}

	[DefaultValue("")]
	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Description("Current text in the HintTextBox. If no text is entered, hint text is not returned.")]
	public override string Text
	{
		get => _userTextEntered ? base.Text : string.Empty;
		set
		{
			if(TextIsValid(value))
			{
				_userTextEntered = true;
				base.Text = value;
			}
			else
			{
				EnterHintMode();
			}
		}
	}
}
