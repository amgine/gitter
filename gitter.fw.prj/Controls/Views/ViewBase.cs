﻿#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

using gitter.Framework.Configuration;
using gitter.Framework.Services;

/// <summary>Represents control with advanced docking capabilities.</summary>
[ToolboxItem(false)]
[DesignerCategory("")]
public class ViewBase : UserControl
{
	protected static ISearchToolBarController CreateSearchToolbarController<TView, TBar, TOptions>(TView view)
		where TView    : ViewBase, ISearchableView<TOptions>
		where TOptions : SearchOptions
		where TBar : SearchToolBar<TOptions>, new()
		=> new SearchToolBarController<TView, TBar, TOptions>(view);

	protected interface ISearchToolBarController
	{
		bool IsVisible { get; set; }

		void Show();

		void Hide();
	}

	sealed class SearchToolBarController<TView, TBar, TOptions> : ISearchToolBarController
		where TView    : ViewBase, ISearchableView<TOptions>
		where TBar     : SearchToolBar<TOptions>, new()
		where TOptions : SearchOptions
	{
		private readonly TView _view;
		private TOptions _options;
		private TBar _toolbar;

		public SearchToolBarController(TView view)
		{
			_view = view;
		}

		public bool IsVisible
		{
			get => _toolbar is { Visible: true };
			set
			{
				if(value)
				{
					Show();
				}
				else
				{
					Hide();
				}
			}
		}

		public void Show()
		{
			if(_toolbar == null)
			{
				_toolbar = new TBar
				{
					Options = _options,
					View    = _view,
				};
				_view.AddBottomToolStrip(_toolbar);
			}
			_toolbar.Visible = true;
			_toolbar.FocusSearchTextBox();
		}

		public void Hide()
		{
			if(_toolbar != null)
			{
				_options = _toolbar.Options;
				_view.RemoveToolStrip(_toolbar);
				_toolbar.Dispose();
				_toolbar = default;
			}
		}
	}

	#region Data

	private object _viewModel;
	private INotificationService _notificationService;
	private IToolTipService _toolTipService;

	#endregion

	#region Events

	private static readonly object ClosingEvent = new();
	public event EventHandler Closing
	{
		add    => Events.AddHandler    (ClosingEvent, value);
		remove => Events.RemoveHandler (ClosingEvent, value);
	}

	public new event EventHandler TextChanged
	{
		add    => base.TextChanged += value;
		remove => base.TextChanged -= value;
	}

	protected virtual void OnClosing()
		=> ((EventHandler)Events[ClosingEvent])?.Invoke(this, EventArgs.Empty);

	#endregion

	#region .ctor

	/// <summary>To keep designer happy.</summary>
	public ViewBase()
	{
		SuspendLayout();
		Size = new Size(555, 362);
		if(LicenseManager.UsageMode == LicenseUsageMode.Runtime)
		{
			Font      = GitterApplication.FontManager.UIFont;
			BackColor = GitterApplication.Style.Colors.Window;
			ForeColor = GitterApplication.Style.Colors.WindowText;
		}
		else
		{
			Font = SystemFonts.MessageBoxFont ?? SystemFonts.DefaultFont;
		}
		ResumeLayout(performLayout: false);
	}

	/// <summary>Create <see cref="ViewBase"/>.</summary>
	public ViewBase(Guid guid, IWorkingEnvironment environment)
		: this()
	{
		Verify.Argument.IsNotNull(environment);

		Guid        = guid;
		WorkingEnvironment = environment;
	}

	#endregion

	public IWorkingEnvironment WorkingEnvironment { get; }

	private static readonly IDpiBoundValue<Size> _defaultScalableSize = DpiBoundValue.Size(new(555, 362));

	public virtual IDpiBoundValue<Size> DefaultScalableSize => _defaultScalableSize;

	protected INotificationService NotificationService
		=> _notificationService ??= new BalloonNotificationService();

	protected IToolTipService ToolTipService
		=> _toolTipService ??= new DefaultToolTipService();

	//protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
	//{
	//    switch(e.KeyCode)
	//    {
	//        case Keys.F4:
	//            if(e.Control)
	//            {
	//                e.IsInputKey = true;
	//            }
	//            break;
	//    }
	//    base.OnPreviewKeyDown(e);
	//}

	//protected override void OnKeyDown(KeyEventArgs e)
	//{
	//    switch(e.KeyCode)
	//    {
	//        case Keys.F4:
	//            if(e.Control)
	//            {
	//                Close();
	//            }
	//            break;
	//    }
	//    base.OnKeyDown(e);
	//}

	public Guid Guid { get; }

	public object ViewModel
	{
		get => _viewModel;
		set
		{
			if(!object.Equals(_viewModel, value))
			{
				if(_viewModel != null)
				{
					DetachViewModel(_viewModel);
				}
				_viewModel = value;
				if(_viewModel != null)
				{
					AttachViewModel(_viewModel);
				}
			}
		}
	}

	public virtual bool IsDocument => false;

	public void Activate()
	{
		if(Host != null)
		{
			if(Host.Status == ViewHostStatus.AutoHide)
			{
				Host.DockSide.ActivateView(this);
			}
			else
			{
				Host.Activate(this);
			}
		}
		OnActivated();
	}

	public void Close()
	{
		if(Host != null)
		{
			Host.RemoveView(this);
			Dispose();
		}
	}

	protected virtual void DetachViewModel(object viewModel)
	{
	}

	protected virtual void AttachViewModel(object viewModel)
	{
	}

	internal ViewHost Host { get; set; }

	internal bool IsHosted =>  Host != null;

	public virtual void OnActivated()
	{
	}

	/// <summary>View's identification string.</summary>
	public virtual string IdentificationString => GetType().Name;

	/// <summary>View's text.</summary>
	public override string Text
	{
		get => base.Text;
		set => base.Text = value;
	}

	/// <summary>View's image provider.</summary>
	public virtual IImageProvider ImageProvider => default;

	protected void AddTopToolStrip(ToolStrip toolStrip)
	{
		Verify.Argument.IsNotNull(toolStrip);

		toolStrip.GripStyle = ToolStripGripStyle.Hidden;
		toolStrip.Stretch = true;
		toolStrip.Padding = new Padding(2);
		toolStrip.Dock = DockStyle.Top;
		Controls.Add(toolStrip);
		toolStrip.MouseDown += OnToolStripClick;
		foreach(Control c in Controls)
		{
			if(c.Dock == DockStyle.None)
			{
				if(c.Anchor.HasFlag(AnchorStyles.Top))
				{
					c.Top += toolStrip.Height;
					if(c.Anchor.HasFlag(AnchorStyles.Bottom))
					{
						c.Height -= toolStrip.Height;
					}
				}
			}
		}
	}

	protected void AddBottomToolStrip(ToolStrip toolStrip)
	{
		Verify.Argument.IsNotNull(toolStrip);

		toolStrip.GripStyle = ToolStripGripStyle.Hidden;
		toolStrip.Stretch = true;
		toolStrip.Padding = new Padding(2);
		toolStrip.Dock = DockStyle.Bottom;
		Controls.Add(toolStrip);
		toolStrip.MouseDown += OnToolStripClick;
		foreach(Control c in Controls)
		{
			if(c.Dock == DockStyle.None)
			{
				if(c.Anchor.HasFlag(AnchorStyles.Bottom))
				{
					if(c.Anchor.HasFlag(AnchorStyles.Top))
					{
						c.Height -= toolStrip.Height;
					}
					else
					{
						c.Top -= toolStrip.Height;
					}
				}
			}
		}
	}

	protected void RemoveToolStrip(ToolStrip toolStrip)
	{
		Verify.Argument.IsNotNull(toolStrip);
		Verify.Argument.IsTrue(toolStrip.Parent == this, nameof(toolStrip), "ToolStrip is not hosted in this " + GetType().Name + ".");

		var dock = toolStrip.Dock;
		toolStrip.Parent = null;
		toolStrip.Click -= OnToolStripClick;
		switch(dock)
		{
			case DockStyle.Top:
				foreach(Control c in Controls)
				{
					if(c.Dock == DockStyle.None)
					{
						if(c.Anchor.HasFlag(AnchorStyles.Top))
						{
							if(c.Anchor.HasFlag(AnchorStyles.Bottom))
							{
								c.SuspendLayout();
								c.Top -= toolStrip.Height;
								c.Height += toolStrip.Height;
								c.ResumeLayout();
							}
							else
							{
								c.Top -= toolStrip.Height;
							}
						}
					}
				}
				break;
			case DockStyle.Bottom:
				foreach(Control c in Controls)
				{
					if(c.Dock == DockStyle.None)
					{
						if(c.Anchor.HasFlag(AnchorStyles.Bottom))
						{
							if(c.Anchor.HasFlag(AnchorStyles.Top))
							{
								c.Height += toolStrip.Height;
							}
							else
							{
								c.Top += toolStrip.Height;
							}
						}
					}
				}
				break;
			default:
				throw new ArgumentException("Invalid ToolStrip.Dock value.", nameof(toolStrip));
		}
	}

	private void OnToolStripClick(object sender, EventArgs e)
	{
		Focus();
	}

	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			OnClosing();
			if(_notificationService != null)
			{
				_notificationService.Dispose();
				_notificationService = null;
			}
			if(_toolTipService != null)
			{
				_toolTipService.Dispose();
				_toolTipService = null;
			}
			_viewModel = null;
		}
		base.Dispose(disposing);
	}

	protected virtual void SaveMoreViewTo(Section section)
	{
	}

	public void SaveViewTo(Section section)
	{
		Verify.Argument.IsNotNull(section);

		SaveMoreViewTo(section);
	}

	protected virtual void LoadMoreViewFrom(Section section)
	{
	}

	public void LoadViewFrom(Section section)
	{
		Verify.Argument.IsNotNull(section);

		LoadMoreViewFrom(section);
	}
}
