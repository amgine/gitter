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

namespace gitter.Git.Gui.Dialogs;

using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
public partial class SelectResetModeDialog : GitDialogBase
{
	const int ButtonHeight  = 66;
	const int ButtonSpacing = 16;

	sealed class SizeImpl(List<CommandLink> buttons) : IDpiBoundValue<Size>
	{
		public Size GetValue(Dpi dpi)
		{
			var conv = DpiConverter.FromDefaultTo(dpi);
			var bh = conv.ConvertY(ButtonHeight);
			var bs = conv.ConvertY(ButtonSpacing);
			var h = buttons.Count * bh + bs * 2 + bs * (buttons.Count - 1);
			return new(conv.ConvertX(350), h);
		}
	}

	private static readonly ResetMode[] ResetModes =
		[
			ResetMode.Soft,
			ResetMode.Mixed,
			ResetMode.Hard,
			ResetMode.Merge,
			ResetMode.Keep,
		];

	private List<CommandLink> _buttons;

	public SelectResetModeDialog(ResetMode availableModes)
	{
		SuspendLayout();
		Name = nameof(SelectResetModeDialog);
		Text = Resources.StrReset;
		AutoScaleDimensions = Dpi.Default;

		AvailableModes = availableModes;
		ResetMode = ResetMode.Mixed;

		_buttons = new List<CommandLink>(ResetModes.Length);
		foreach(var resetMode in ResetModes)
		{
			if((AvailableModes & resetMode) == resetMode)
			{
				_buttons.Add(CreateResetButton(resetMode));
			}
		}

		const int margin = 16;

		var location = new Point(margin, margin);
		foreach(var button in _buttons)
		{
			button.Location = location;
			button.Parent = this;
			location.Y += button.Height + ButtonSpacing;
		}

		ScalableSize = new SizeImpl(_buttons);
		Size = ScalableSize.GetValue(Dpi.Default);

		ResumeLayout(false);
		PerformLayout();
	}

	public SelectResetModeDialog()
		: this(ResetMode.Soft | ResetMode.Mixed | ResetMode.Hard)
	{
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; }

	public ResetMode AvailableModes { get; }

	public ResetMode ResetMode { get; set; }

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrReset;

	/// <inheritdoc/>
	public override DialogButtons OptimalButtons => DialogButtons.Cancel;

	static ResetMode GetResetMode(object? sender)
		=> (ResetMode)((Control)sender!).Tag!;

	private CommandLink CreateResetButton(ResetMode mode)
	{
		var text = string.Empty;
		var desc = string.Empty;

		switch(mode)
		{
			case ResetMode.Soft:
				text = Resources.StrSoft;
				desc = Resources.TipSoftReset;
				break;
			case ResetMode.Mixed:
				text = Resources.StrMixed;
				desc = Resources.TipMixedReset;
				break;
			case ResetMode.Hard:
				text = Resources.StrHard;
				desc = Resources.TipHardReset;
				break;
			case ResetMode.Merge:
				text = Resources.StrMerge;
				desc = Resources.TipMergeReset;
				break;
			case ResetMode.Keep:
				text = Resources.StrKeep;
				desc = Resources.TipKeepReset;
				break;
			default:
				throw new ArgumentException($"Unknown reset mode: {mode}", nameof(mode));
		}

		var button = new CommandLink()
		{
			Size        = new Size(319, ButtonHeight),
			Text        = text,
			Description = desc,
			Tag         = mode,
		};

		button.Click += (s, _) =>
			{
				ResetMode = GetResetMode(s);
				ClickOk();
			};

		return button;
	}

	/// <inheritdoc/>
	protected override void OnShown()
	{
		base.OnShown();

		if(_buttons is { Count: > 0 })
		{
			foreach(var btn in _buttons)
			{
				if(GetResetMode(btn) == ResetMode)
				{
					btn.Focus();
					return;
				}
			}
			_buttons[0].Focus();
		}
	}
}
