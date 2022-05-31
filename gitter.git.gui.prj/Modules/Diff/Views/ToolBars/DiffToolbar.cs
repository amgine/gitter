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

namespace gitter.Git.Gui.Views;

using System;
using System.Globalization;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
[DesignerCategory("")]
internal sealed class DiffToolbar : ToolStrip
{
	static class Icons
	{
		static IImageProvider Init(string name) => new ScaledImageProvider(CachedResources.ScaledBitmaps, name);

		public static IImageProvider Refresh     = CommonIcons.Refresh;
		public static IImageProvider ContextLess = Init(@"context.less");
		public static IImageProvider ContextMore = Init(@"context.more");
		public static IImageProvider Options     = Init(@"config");
		public static IImageProvider DiffSingle  = Init(@"diff.single");
		public static IImageProvider DiffSplit   = Init(@"diff.split");
	}

	#region Constants

	private const int MinimumContext = 0;
	private const int MaximumContext = 9999;

	#endregion

	#region Data

	private readonly DiffView _diffView;
	private readonly ToolStripTextBox _contextTextBox;
	private readonly ToolStripButton _btnRefresh;
	private readonly ToolStripButton _btnMoreContext;
	private readonly ToolStripButton _btnLessContext;
	private readonly ToolStripDropDownButton _ddbOptions;
	private readonly ToolStripMenuItem _mnuIgnoreWhitespace;
	private readonly ToolStripMenuItem _mnuUsePatienceAlgorithm;
	private readonly ToolStripMenuItem _mnuBinaryDiff;
	private readonly ToolStripButton _btnSingleMode;
	private readonly ToolStripButton _btnSplitMode;
	private readonly DpiBindings _bindings;

	#endregion

	public DiffToolbar(DiffView diffView)
	{
		Verify.Argument.IsNotNull(diffView);

		_diffView = diffView;

		Items.Add(_btnRefresh = new ToolStripButton(Resources.StrRefresh, default,
			(_, _) => _diffView.RefreshContent())
		{
			DisplayStyle = ToolStripItemDisplayStyle.Image,
		});
		Items.Add(new ToolStripSeparator());
		Items.Add(new ToolStripLabel(Resources.StrContext.AddColon(), null));
		Items.Add(_btnLessContext = new ToolStripButton(Resources.StrLessContext, default, (_, _) => DecrementContext())
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
		Items.Add(_btnMoreContext = new ToolStripButton(Resources.StrMoreContext, default, (_, _) => IncrementContext())
			{
				DisplayStyle = ToolStripItemDisplayStyle.Image,
			});
		Items.Add(new ToolStripSeparator());
		Items.Add(_ddbOptions = new ToolStripDropDownButton(Resources.StrOptions)
			{
			});
		_ddbOptions.DropDownItems.Add(_mnuIgnoreWhitespace = new ToolStripMenuItem(Resources.StrsIgnoreWhitespace, null, OnIgnoreWhitespaceClick)
			{
				Checked = _diffView.DiffOptions.IgnoreWhitespace,
			});
		_ddbOptions.DropDownItems.Add(_mnuUsePatienceAlgorithm = new ToolStripMenuItem(Resources.StrsUsePatienceDiffAlgorithm, null, OnUsePatienceAlgorithmClick)
			{
				Checked = _diffView.DiffOptions.UsePatienceAlgorithm,
			});
		_ddbOptions.DropDownItems.Add(_mnuBinaryDiff = new ToolStripMenuItem(Resources.StrBinary, null, OnBinaryClick)
			{
				Checked = _diffView.DiffOptions.Binary,
			});

		Items.Add(_btnSplitMode = new ToolStripButton(Resources.StrDiffSplitView, default, (_, _) => _diffView.ViewMode = DiffViewMode.Split)
			{
				DisplayStyle = ToolStripItemDisplayStyle.Image,
				Alignment = ToolStripItemAlignment.Right,
				Checked = _diffView.ViewMode == DiffViewMode.Split,
			});
		Items.Add(_btnSingleMode = new ToolStripButton(Resources.StrDiffSingleView, default, (_, _) => _diffView.ViewMode = DiffViewMode.Single)
			{
				DisplayStyle = ToolStripItemDisplayStyle.Image,
				Alignment = ToolStripItemAlignment.Right,
				Checked = _diffView.ViewMode == DiffViewMode.Single,
			});

		_contextTextBox.TextChanged += OnContextTextChanged;
		_contextTextBox.KeyPress += static (_, e) => e.Handled = !char.IsNumber(e.KeyChar);
		_diffView.ViewModeChanged += OnDiffViewViewModeChanged;

		_bindings = new DpiBindings(this);
		_bindings.BindImage(_btnRefresh,     Icons.Refresh);
		_bindings.BindImage(_btnLessContext, Icons.ContextLess);
		_bindings.BindImage(_btnMoreContext, Icons.ContextMore);
		_bindings.BindImage(_ddbOptions,     Icons.Options);
		_bindings.BindImage(_btnSingleMode,  Icons.DiffSingle);
		_bindings.BindImage(_btnSplitMode,   Icons.DiffSplit);
	}

	private void OnDiffViewViewModeChanged(object sender, EventArgs e)
	{
		_btnSingleMode.Checked = _diffView.ViewMode == DiffViewMode.Single;
		_btnSplitMode.Checked  = _diffView.ViewMode == DiffViewMode.Split;
	}

	private void OnContextTextChanged(object sender, EventArgs e)
	{
		if(int.TryParse(_contextTextBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int context))
		{
			SetContext(context);
		}
	}

	private void OnIgnoreWhitespaceClick(object sender, EventArgs e)
	{
		_diffView.DiffOptions.IgnoreWhitespace = !_diffView.DiffOptions.IgnoreWhitespace;
		_mnuIgnoreWhitespace.Checked = _diffView.DiffOptions.IgnoreWhitespace;
		_diffView.RefreshContent();
	}

	private void OnUsePatienceAlgorithmClick(object sender, EventArgs e)
	{
		_diffView.DiffOptions.UsePatienceAlgorithm = !_diffView.DiffOptions.UsePatienceAlgorithm;
		_mnuUsePatienceAlgorithm.Checked = _diffView.DiffOptions.UsePatienceAlgorithm;
		_diffView.RefreshContent();
	}

	private void OnBinaryClick(object sender, EventArgs e)
	{
		_diffView.DiffOptions.Binary = !_diffView.DiffOptions.Binary;
		_mnuBinaryDiff.Checked = _diffView.DiffOptions.Binary;
		_diffView.RefreshContent();
	}

	private void SetContext(int context)
	{
		if(context < MinimumContext)
		{
			context = MinimumContext;
		}
		else if(context > MaximumContext)
		{
			context = MaximumContext;
		}
		if(_diffView.DiffOptions.Context != context)
		{
			_diffView.DiffOptions.Context = context;
			_diffView.RefreshContent();
			_contextTextBox.Text = context.ToString(CultureInfo.InvariantCulture);
		}
	}

	private void IncrementContext()
	{
		var context = _diffView.DiffOptions.Context;
		if(context >= MaximumContext) return;
		++context;
		_contextTextBox.Text = context.ToString(CultureInfo.InvariantCulture);
	}

	private void DecrementContext()
	{
		var context = _diffView.DiffOptions.Context;
		if(context <= MinimumContext) return;
		--context;
		_contextTextBox.Text = context.ToString(CultureInfo.InvariantCulture);
	}
}
