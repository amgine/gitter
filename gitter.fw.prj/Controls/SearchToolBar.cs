#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

using gitter.Framework;

using Resources = gitter.Framework.Properties.Resources;

[ToolboxItem(false)]
[DesignerCategory("")]
public abstract class SearchToolBar<TOptions> : ToolStrip
	where TOptions : SearchOptions
{
	#region Data

	private ISearchableView<TOptions> _view;
	private readonly ToolStripTextBox _textBox;
	private readonly ToolStripButton _btnNext;
	private readonly ToolStripButton _btnPrev;
	private readonly ToolStripButton _btnMatchCase;
	private bool _result;
	private TOptions _options;

	#endregion

	protected SearchToolBar()
	{
		_result = true;
		var dpiBindings = new DpiBindings(this);

		var searchClose = new ToolStripButton(Resources.StrClose, null, OnCloseButtonClick)
		{
			DisplayStyle = ToolStripItemDisplayStyle.Image,
		};

		Items.AddRange(new ToolStripItem[]
			{
				searchClose,
				new ToolStripLabel(Resources.StrFind.AddColon(), null),
				_textBox = new ToolStripTextBox()
				{
					AutoSize = false,
					Width = 200,
				},
				_btnNext = new ToolStripButton(Resources.StrNext, null, OnNextClick)
				{
					DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
					Enabled = false,
				},
				_btnPrev = new ToolStripButton(Resources.StrPrevious, null, OnPreviousClick)
				{
					DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
					Enabled = false,
				},
				_btnMatchCase = new ToolStripButton(Resources.StrMatchCase, null, OnMatchCaseClick)
				{
					DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
					Enabled = true,
				},
			});

		dpiBindings.BindImage(searchClose, CommonIcons.SearchClose);
		dpiBindings.BindImage(_btnNext, CommonIcons.SearchNext);
		dpiBindings.BindImage(_btnPrev, CommonIcons.SearchPrev);

		_textBox.TextBox.PreviewKeyDown += OnSearchTextBoxPreviewKeyDown;
		_textBox.TextBox.KeyDown        += OnSearchTextBoxKeyDown;
		_textBox.TextChanged            += OnSearchTextChanged;
		_btnMatchCase.CheckedChanged    += OnMatchCaseCheckedChanged;
	}

	private void OnCloseButtonClick(object sender, EventArgs e)
	{
		if(View is null) return;
		View.SearchToolBarVisible = false;
	}

	private void OnSearchTextChanged(object sender, EventArgs e)
	{
		if(View != null)
		{
			Options = CreateSearchOptions();
			HandleSearchResult(View.Search.Current(Options));
		}
		_btnNext.Enabled = _textBox.TextLength > 0;
		_btnPrev.Enabled = _textBox.TextLength > 0;
	}

	private void OnSearchTextBoxPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		Assert.IsNotNull(e);

		if(View is null) return;

		switch(e.KeyCode)
		{
			case Keys.Enter:
			case Keys.F3 when e.Modifiers == Keys.Shift:
			case Keys.F3 when e.Modifiers == Keys.None:
			case Keys.Escape:
				e.IsInputKey = true;
				break;
		}
	}

	private void OnSearchTextBoxKeyDown(object sender, KeyEventArgs e)
	{
		Assert.IsNotNull(e);

		if(View is null) return;

		switch(e.KeyCode)
		{
			case Keys.Enter:
				HandleSearchResult(View.Search.Current(Options));
				e.Handled = true;
				break;
			case Keys.F3 when e.Modifiers == Keys.Shift:
				HandleSearchResult(View.Search.Previous(Options));
				e.Handled = true;
				break;
			case Keys.F3 when e.Modifiers == Keys.None:
				HandleSearchResult(View.Search.Next(Options));
				e.Handled = true;
				break;
			case Keys.Escape:
				View.SearchToolBarVisible = false;
				e.Handled = true;
				break;
		}
	}

	private void OnNextClick(object sender, EventArgs e)
	{
		if(View is null) return;
		HandleSearchResult(View.Search.Next(Options));
	}

	private void OnPreviousClick(object sender, EventArgs e)
	{
		if(View is null) return;
		HandleSearchResult(View.Search.Previous(Options));
	}

	private void OnMatchCaseClick(object sender, EventArgs e)
	{
		_btnMatchCase.Checked = !_btnMatchCase.Checked;
	}

	private void OnMatchCaseCheckedChanged(object sender, EventArgs e)
	{
		if(View is null) return;
		Options = CreateSearchOptions();
		HandleSearchResult(View.Search.Current(Options));
	}

	public TOptions Options
	{
		get => _options ??= CreateSearchOptions();
		set
		{
			if(_options != value)
			{
				_options = value;
				if(value is not null)
				{
					SearchText = value.Text;
					MatchCase  = value.MatchCase;
				}
			}
		}
	}

	protected abstract TOptions CreateSearchOptions();

	private void HandleSearchResult(bool result)
	{
		if(result != _result)
		{
			if(result)
			{
				_textBox.TextBox.BackColor = GitterApplication.Style.Colors.Window;
			}
			else
			{
				var color = GitterApplication.Style.Colors.Window;
				int r = color.R + 50;
				if(r > 255) r = 255;
				_textBox.TextBox.BackColor = Color.FromArgb(r, color.G, color.B);
				try
				{
					System.Media.SystemSounds.Beep.Play();
				}
				catch(Exception exc) when(!exc.IsCritical())
				{
				}
			}
			_result = result;
		}
	}

	public ISearchableView<TOptions> View
	{
		get => _view;
		set
		{
			if(_view != value)
			{
				_view = value;
			}
		}
	}

	public void FocusSearchTextBox()
	{
		_textBox.SelectAll();
		_textBox.Focus();
	}

	public string SearchText
	{
		get => _textBox.Text;
		set => _textBox.Text = value;
	}

	public bool MatchCase
	{
		get => _btnMatchCase.Checked;
		set => _btnMatchCase.Checked = value;
	}

	public ToolStripButton NextButton => _btnNext;

	public ToolStripButton PrevButton => _btnNext;

	public ToolStripButton MatchCaseButton => _btnMatchCase;
}
