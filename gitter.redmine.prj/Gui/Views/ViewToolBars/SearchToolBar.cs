namespace gitter.Redmine.Gui
{
	using System;
	using System.Drawing;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Windows.Forms;

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
			if(view == null) throw new ArgumentNullException("view");
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
				if(result != _result)
				{
					if(result)
					{
						_textBox.TextBox.BackColor = SystemColors.Window;
					}
					else
					{
						_textBox.TextBox.BackColor = Color.FromArgb(255, 200, 200);
						System.Media.SystemSounds.Beep.Play();
					}
					_result = result;
				}
				_btnNext.Enabled = _textBox.TextLength > 0;
				_btnPrev.Enabled = _textBox.TextLength > 0;
			};
		}

		protected abstract TOptions CreateSearchOptions();

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
