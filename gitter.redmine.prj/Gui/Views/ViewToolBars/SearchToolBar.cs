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

namespace gitter.Redmine.Gui
{
	using System;
	using System.Drawing;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;

	using Resources = gitter.Redmine.Properties.Resources;

	[ToolboxItem(false)]
	internal abstract class SearchToolBar<TView, TOptions> : ToolStrip
		where TView : RedmineViewBase, ISearchableView<TOptions>
		where TOptions : SearchOptions
	{
		#region Data

		private readonly TView _view;
		private readonly ToolStripTextBox _textBox;
		private readonly ToolStripButton _btnNext;
		private readonly ToolStripButton _btnPrev;
		private bool _result;

		#endregion

		protected SearchToolBar(TView view)
		{
			Verify.Argument.IsNotNull(view, nameof(view));

			_view = view;

			_result = true;

			Items.Add(new ToolStripButton(Resources.StrClose, CachedResources.Bitmaps["ImgSearchClose"], (sender, e) =>
			{
				_view.SearchToolBarVisible = false;
			})
			{
				DisplayStyle = ToolStripItemDisplayStyle.Image,
			});
			Items.Add(new ToolStripLabel(Resources.StrFind.AddColon(), null));
			Items.Add(_textBox = new ToolStripTextBox()
			{
				AutoSize = false,
				Width = 200,
			});
			Items.Add(_btnNext = new ToolStripButton(Resources.StrNext, CachedResources.Bitmaps["ImgSearchNext"], (sender, e) =>
			{
				_view.SearchNext(CreateSearchOptions());
			})
			{
				DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
				Enabled = false,
			});
			Items.Add(_btnPrev = new ToolStripButton(Resources.StrPrevious, CachedResources.Bitmaps["ImgSearchPrevious"], (sender, e) =>
			{
				_view.SearchPrevious(CreateSearchOptions());
			})
			{
				DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
				Enabled = false,
			});
			/*
			Items.Add(new ToolStripButton(Resources.StrMatchCase));
			*/

			_textBox.TextBox.PreviewKeyDown += (sender, e) =>
			{
				if(e.KeyCode == Keys.Escape)
				{
					_view.SearchToolBarVisible = false;
					e.IsInputKey = true;
				}
			};

			_textBox.TextChanged += (sender, e) =>
			{
				var result = _view.SearchFirst(CreateSearchOptions());
				HandleSearchResult(result);
				_btnNext.Enabled = _textBox.TextLength > 0;
				_btnPrev.Enabled = _textBox.TextLength > 0;
			};
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
					catch
					{
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

		public ToolStripButton NextButton
		{
			get { return _btnNext; }
		}

		public ToolStripButton PrevButton
		{
			get { return _btnNext; }
		}
	}
}
