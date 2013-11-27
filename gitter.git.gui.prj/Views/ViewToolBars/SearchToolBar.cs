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

namespace gitter.Git.Gui.Views
{
	using System;
	using System.Drawing;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	internal abstract class SearchToolBar<TView, TOptions> : ToolStrip
		where TView : GitViewBase, ISearchableView<TOptions>
		where TOptions : SearchOptions
	{
		#region Data

		private readonly TView _view;
		private readonly ToolStripTextBox _textBox;
		private readonly ToolStripButton _btnNext;
		private readonly ToolStripButton _btnPrev;
		private readonly ToolStripButton _btnMatchCase;
		private bool _result;
		private TOptions _options;

		#endregion

		protected SearchToolBar(TView view)
		{
			Verify.Argument.IsNotNull(view, "view");

			_view = view;
			_result = true;

			Items.AddRange(new ToolStripItem[]
				{
					new ToolStripButton(Resources.StrClose, CachedResources.Bitmaps["ImgSearchClose"], OnCloseButtonClick)
					{
						DisplayStyle = ToolStripItemDisplayStyle.Image,
					},
					new ToolStripLabel(Resources.StrFind.AddColon(), null),
					_textBox = new ToolStripTextBox()
					{
						AutoSize = false,
						Width = 200,
					},
					_btnNext = new ToolStripButton(Resources.StrNext, CachedResources.Bitmaps["ImgSearchNext"], OnNextClick)
					{
						DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
						Enabled = false,
					},
					_btnPrev = new ToolStripButton(Resources.StrPrevious, CachedResources.Bitmaps["ImgSearchPrevious"], OnPreviousClick)
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
			_textBox.TextBox.PreviewKeyDown += OnSearchTextBoxPreviewKeyDown;
			_textBox.TextChanged += OnSearchTextChanged;
			_btnMatchCase.CheckedChanged += OnMatchCaseCheckedChanged;
		}

		private void OnCloseButtonClick(object sender, EventArgs e)
		{
			_view.SearchToolBarVisible = false;
		}

		private void OnSearchTextChanged(object sender, EventArgs e)
		{
			Options = CreateSearchOptions();
			HandleSearchResult(_view.Search.Current(Options));
			_btnNext.Enabled = _textBox.TextLength > 0;
			_btnPrev.Enabled = _textBox.TextLength > 0;
		}

		private void OnSearchTextBoxPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if(e.KeyCode == Keys.Escape)
			{
				_view.SearchToolBarVisible = false;
				e.IsInputKey = true;
			}
		}

		private void OnNextClick(object sender, EventArgs e)
		{
			HandleSearchResult(_view.Search.Next(Options));
		}

		private void OnPreviousClick(object sender, EventArgs e)
		{
			HandleSearchResult(_view.Search.Previous(Options));
		}

		private void OnMatchCaseClick(object sender, EventArgs e)
		{
			_btnMatchCase.Checked = !_btnMatchCase.Checked;
		}

		private void OnMatchCaseCheckedChanged(object sender, EventArgs e)
		{
			Options = CreateSearchOptions();
			HandleSearchResult(_view.Search.Current(Options));
		}

		private TOptions Options
		{
			get
			{
				if(_options == null)
				{
					_options = CreateSearchOptions();
				}
				return _options;
			}
			set { _options = value; }
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
					catch(Exception exc)
					{
						if(exc.IsCritical())
						{
							throw;
						}
					}
				}
				_result = result;
			}
		}

		public TView View
		{
			get { return _view; }
		}

		public void FocusSearchTextBox()
		{
			_textBox.SelectAll();
			_textBox.Focus();
		}

		public string SearchText
		{
			get { return _textBox.Text; }
			set { _textBox.Text = value; }
		}

		public bool MatchCase
		{
			get { return _btnMatchCase.Checked; }
			set { _btnMatchCase.Checked = value; }
		}

		public ToolStripButton NextButton
		{
			get { return _btnNext; }
		}

		public ToolStripButton PrevButton
		{
			get { return _btnNext; }
		}

		public ToolStripButton MatchCaseButton
		{
			get { return _btnMatchCase; }
		}
	}
}
