namespace gitter.Git.Gui.Views
{
	using System;
	using System.Globalization;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	internal sealed class DiffToolbar : ToolStrip
	{
		#region Data

		private readonly DiffView _diffView;
		private readonly ToolStripTextBox _contextTextBox;
		private readonly ToolStripDropDownButton _ddbOptions;
		private readonly ToolStripMenuItem _mnuIgnoreWhitespace;
		private readonly ToolStripMenuItem _mnuUsePatienceAlgorithm;

		#endregion

		public DiffToolbar(DiffView diffView)
		{
			Verify.Argument.IsNotNull(diffView, "diffView");

			_diffView = diffView;

			Items.Add(new ToolStripButton(Resources.StrRefresh, CachedResources.Bitmaps["ImgRefresh"],
				(sender, e) =>
				{
					_diffView.RefreshContent();
				})
			{
				DisplayStyle = ToolStripItemDisplayStyle.Image,
			});
			Items.Add(new ToolStripSeparator());
			Items.Add(new ToolStripLabel(Resources.StrContext.AddColon(), null));
			Items.Add(new ToolStripButton(Resources.StrLessContext, CachedResources.Bitmaps["ImgLessContext"], (sender, e) => DecrementContext())
				{
					DisplayStyle = ToolStripItemDisplayStyle.Image,
				});
			Items.Add(_contextTextBox = new ToolStripTextBox()
				{
					Text = _diffView.DiffOptions.Context.ToString(CultureInfo.InvariantCulture),
					TextBoxTextAlign = HorizontalAlignment.Right,
					ControlAlign = ContentAlignment.MiddleCenter,
					AutoSize = false,
					Size = new Size(40, 20),
					MaxLength = 4,
					ShortcutsEnabled = false,
				});
			Items.Add(new ToolStripButton(Resources.StrMoreContext, CachedResources.Bitmaps["ImgMoreContext"], (sender, e) => IncrementContext())
				{
					DisplayStyle = ToolStripItemDisplayStyle.Image,
				});
			Items.Add(new ToolStripSeparator());
			Items.Add(_ddbOptions = new ToolStripDropDownButton(Resources.StrOptions, CachedResources.Bitmaps["ImgConfig"])
				{
				});
			_ddbOptions.DropDownItems.Add(_mnuIgnoreWhitespace = new ToolStripMenuItem(Resources.StrsIgnoreWhitespace)
				{
					Checked = _diffView.DiffOptions.IgnoreWhitespace,
				});
			_ddbOptions.DropDownItems.Add(_mnuUsePatienceAlgorithm = new ToolStripMenuItem(Resources.StrsUsePatienceDiffAlgorithm)
				{
					Checked = _diffView.DiffOptions.UsePatienceAlgorithm,
				});

			_contextTextBox.TextChanged += (sender, e) =>
				{
					int context;
					if(int.TryParse(_contextTextBox.Text,
						NumberStyles.None, CultureInfo.InvariantCulture, out context))
					{
						SetContext(context);
					}
				};
			_contextTextBox.KeyPress += (sender, e) =>
				{
					e.Handled = !char.IsNumber(e.KeyChar);
				};
			_mnuIgnoreWhitespace.Click += (sender, e) =>
				{
					_diffView.DiffOptions.IgnoreWhitespace = !_diffView.DiffOptions.IgnoreWhitespace;
					_mnuIgnoreWhitespace.Checked = _diffView.DiffOptions.IgnoreWhitespace;
					_diffView.Reload();
				};
			_mnuUsePatienceAlgorithm.Click += (sender, e) =>
				{
					_diffView.DiffOptions.UsePatienceAlgorithm = !_diffView.DiffOptions.UsePatienceAlgorithm;
					_mnuUsePatienceAlgorithm.Checked = _diffView.DiffOptions.UsePatienceAlgorithm;
					_diffView.Reload();
				};
		}

		private void SetContext(int context)
		{
			if(context < 0)
				context = 0;
			if(context > 9999)
				context = 9999;
			if(_diffView.DiffOptions.Context != context)
			{
				_diffView.DiffOptions.Context = context;
				_diffView.Reload();
				_contextTextBox.Text = context.ToString(CultureInfo.InvariantCulture);
			}
		}

		private void IncrementContext()
		{
			var context = _diffView.DiffOptions.Context;
			if(context == 9999) return;
			++context;
			_contextTextBox.Text = context.ToString(CultureInfo.InvariantCulture);
		}

		private void DecrementContext()
		{
			var context = _diffView.DiffOptions.Context;
			if(context == 0) return;
			--context;
			_contextTextBox.Text = context.ToString(CultureInfo.InvariantCulture);
		}
	}
}
