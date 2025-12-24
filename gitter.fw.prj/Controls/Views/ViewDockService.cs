#region Copyright Notice
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
using System.Collections.Generic;
using System.Globalization;
using System.Drawing;

using gitter.Framework.Configuration;

public sealed class ViewDockService
{
	#region Data

	private readonly Dictionary<Guid, IViewFactory> _factories;
	private readonly IWorkingEnvironment _environment;
	private readonly Section _section;

	private ViewBase? _activeView;

	#endregion

	public event EventHandler? ActiveViewChanged;

	private void InvokeActiveViewChanged()
		=> ActiveViewChanged?.Invoke(this, EventArgs.Empty);

	public ViewDockService(IWorkingEnvironment environment, DockPanel dockPanel, Section section)
	{
		Verify.Argument.IsNotNull(environment);
		Verify.Argument.IsNotNull(dockPanel);
		Verify.Argument.IsNotNull(section);

		_environment = environment;
		DockPanel = dockPanel;
		_section = section;
		_factories = new Dictionary<Guid, IViewFactory>();

		RegisterFactory(new WebBrowserViewFactory());
	}

	public DockPanel DockPanel { get; }

	public ICollection<IViewFactory> ViewFactories
		=> _factories.Values;

	public ViewBase? ActiveView
	{
		get
		{
			foreach(var factory in ViewFactories)
			{
				foreach(var view in factory.CreatedViews)
				{
					var host = view.Host;
					if(host != null && host.IsActive)
					{
						_activeView = view;
						break;
					}
				}
			}
			return _activeView;
		}
	}

	private static string GetViewConfigId(ViewBase view)
	{
		return view.IdentificationString + "-" + view.Guid.ToString();
	}

	public IViewFactory GetFactory(Guid guid)
	{
		return _factories[guid];
	}

	public T? GetFactory<T>()
		where T : class, IViewFactory
	{
		foreach(var factory in ViewFactories)
		{
			if(factory is T res) return res;
		}
		return default;
	}

	public void RegisterFactory(IViewFactory factory)
	{
		Verify.Argument.IsNotNull(factory);

		_factories.Add(factory.Guid, factory);
	}

	public void UnregisterFactory(IViewFactory factory)
	{
		Verify.Argument.IsNotNull(factory);

		UnregisterFactory(factory.Guid);
	}

	public void UnregisterFactory(Guid guid)
	{
		_factories.Remove(guid);
	}

	private void OnViewClosing(object? sender, EventArgs e)
	{
		var view = (ViewBase)sender!;
		view.Closing -= OnViewClosing;
		var section = _section.GetCreateSection(GetViewConfigId(view));
		view.SaveViewTo(section);
		if(_activeView == view) _activeView = null;
	}

	private void Rescale(ViewBase view)
	{
		view.Size = view.DefaultScalableSize.GetValue(new Dpi(DockPanel.DeviceDpi));
	}

	private void FindAppropriateViewHost(IViewFactory factory, ViewBase view)
	{
		var host = DockPanel.RootHost;
		if(!factory.IsSingleton)
		{
			foreach(var v in factory.CreatedViews)
			{
				if(v.Host is { IsDocumentWell: true })
				{
					host = v.Host;
				}
			}
			view.Size = DpiConverter.FromDefaultTo(new Dpi(host.DeviceDpi)).Convert(view.Size);
			host.AddView(view);
		}
		else
		{
			switch(factory.DefaultViewPosition)
			{
				case ViewPosition.SecondaryDocumentHost:
					foreach(var h in DockElements<ViewHost>.Instances)
					{
						if(h != host && h.IsDocumentWell)
						{
							h.AddView(view);
							return;
						}
					}
					Rescale(view);
					host = new ViewHost(DockPanel, false, true, [view])
					{
						Size = DockPanel.RootHost.Size
					};
					DockPanel.RootHost.PerformDock(host, DockResult.Right);
					break;

				case ViewPosition.Left:
					Rescale(view);
					host = new ViewHost(DockPanel, false, false, [view]);
					DockPanel.PerformDock(host, DockResult.Left);
					break;
				case ViewPosition.Top:
					Rescale(view);
					host = new ViewHost(DockPanel, false, false, [view]);
					DockPanel.PerformDock(host, DockResult.Top);
					break;
				case ViewPosition.Right:
					Rescale(view);
					host = new ViewHost(DockPanel, false, false, [view]);
					DockPanel.PerformDock(host, DockResult.Right);
					break;
				case ViewPosition.Bottom:
					Rescale(view);
					host = new ViewHost(DockPanel, false, false, [view]);
					DockPanel.PerformDock(host, DockResult.Bottom);
					break;

				case ViewPosition.LeftAutoHide:
					Rescale(view);
					host = new ViewHost(DockPanel, false, false, [view]);
					host.UnpinFromLeft();
					break;
				case ViewPosition.TopAutoHide:
					Rescale(view);
					host = new ViewHost(DockPanel, false, false, [view]);
					host.UnpinFromTop();
					break;
				case ViewPosition.RightAutoHide:
					Rescale(view);
					host = new ViewHost(DockPanel, false, false, [view]);
					host.UnpinFromRight();
					break;
				case ViewPosition.BottomAutoHide:
					Rescale(view);
					host = new ViewHost(DockPanel, false, false, [view]);
					host.UnpinFromBottom();
					break;

				case ViewPosition.Float:
					Rescale(view);
					host = new ViewHost(DockPanel, false, false, [view]);
					var form = host.PrepareFloatingMode();
					form.Location = DockPanel.PointToScreen(new Point(20, 20));
					form.Show(DockPanel.TopLevelControl);
					break;

				default:
					Rescale(view);
					DockPanel.RootHost.AddView(view);
					break;
			}
		}
	}

	private void ShowNewView(IViewFactory factory, ViewBase view, bool activate)
	{
		FindAppropriateViewHost(factory, view);
		ShowExistingView(view, activate);
	}

	private void ShowExistingView(ViewBase view, bool activate)
	{
		if(activate)
		{
			view.Activate();
			if(_activeView is not null)
			{

			}
			if(_activeView != view)
			{
				_activeView = view;
				ActiveViewChanged?.Invoke(this, EventArgs.Empty);
			}
		}
		else
		{
			view.Host!.SetActiveView(view);
		}
	}

	public WebBrowserView ShowWebBrowserView(string url)
	{
		return ShowWebBrowserView(url, true);
	}

	public WebBrowserView ShowWebBrowserView(string url, bool activate)
		=> (WebBrowserView)ShowView(WebBrowserViewFactory.Guid,
			new WebBrowserViewModel(url), activate);

	private IViewFactory GetViewFactoryByGuid(Guid guid)
		=> _factories.TryGetValue(guid, out var factory)
			? factory
			: throw new ArgumentException($"Unknown view factory GUID: {guid}", nameof(guid));

	public ViewBase ShowView(Guid guid, bool activate = true)
	{
		var factory = GetViewFactoryByGuid(guid);
		if(factory.IsSingleton)
		{
			var existing = default(ViewBase);
			foreach(var view in factory.CreatedViews)
			{
				existing = view;
				break;
			}
			if(existing is null)
			{
				existing = factory.CreateView(_environment);
				var section = _section.TryGetSection(GetViewConfigId(existing));
				if(section is not null)
				{
					existing.LoadViewFrom(section);
				}
				existing.Closing += OnViewClosing;
				ShowNewView(factory, existing, activate);
			}
			else
			{
				existing.ViewModel = null;
				ShowExistingView(existing, activate);
			}
			return existing;
		}
		else
		{
			var existing = default(ViewBase);
			foreach(var view in factory.CreatedViews)
			{
				if(object.Equals(view.ViewModel, null))
				{
					existing = view;
					break;
				}
			}
			if(existing is null)
			{
				existing = factory.CreateView(_environment);
				var section = _section.TryGetSection(GetViewConfigId(existing));
				if(section is not null)
				{
					existing.LoadViewFrom(section);
				}
				existing.Closing += OnViewClosing;
				ShowNewView(factory, existing, activate);
			}
			else
			{
				ShowExistingView(existing, activate);
			}
			return existing;
		}
	}

	public ViewBase ShowView(Guid guid, object viewModel, bool activate = true)
	{
		var factory = GetViewFactoryByGuid(guid);
		if(factory.IsSingleton)
		{
			var existing = default(ViewBase);
			foreach(var view in factory.CreatedViews)
			{
				existing = view;
				break;
			}
			if(existing is null)
			{
				existing = factory.CreateView(_environment);
				existing.ViewModel = viewModel;
				existing.Closing += OnViewClosing;
				ShowNewView(factory, existing, activate);
			}
			else
			{
				existing.ViewModel = viewModel;
				ShowExistingView(existing, activate);
			}
			return existing;
		}
		else
		{
			var existing = default(ViewBase);
			foreach(var view in factory.CreatedViews)
			{
				if(object.Equals(view.ViewModel, viewModel))
				{
					existing = view;
					break;
				}
			}
			if(existing is null)
			{
				existing = factory.CreateView(_environment);
				existing.ViewModel = viewModel;
				existing.Closing += OnViewClosing;
				ShowNewView(factory, existing, activate);
			}
			else
			{
				ShowExistingView(existing, activate);
			}
			return existing;
		}
	}

	public ViewBase? FindView(Guid guid)
	{
		if(!_factories.TryGetValue(guid, out var factory))
		{
			return null;
		}
		foreach(var view in factory.CreatedViews)
		{
			return view;
		}
		return null;
	}

	public ViewBase? FindView(Guid guid, object viewModel)
	{
		if(!_factories.TryGetValue(guid, out var factory))
		{
			return null;
		}
		foreach(var view in factory.CreatedViews)
		{
			if(object.Equals(view.ViewModel, viewModel))
			{
				return view;
			}
		}
		return null;
	}

	public IEnumerable<ViewBase> FindViews(Guid guid)
	{
		if(!_factories.TryGetValue(guid, out var factory))
		{
			return Preallocated<ViewBase>.EmptyArray;
		}
		return factory.CreatedViews;
	}

	public IEnumerable<ViewBase> FindViews(Guid guid, object viewModel)
	{
		if(!_factories.TryGetValue(guid, out var factory))
		{
			return Preallocated<ViewBase>.EmptyArray;
		}
		var list = new List<ViewBase>();
		foreach(var view in factory.CreatedViews)
		{
			if(object.Equals(view.ViewModel, viewModel))
			{
				list.Add(view);
			}
		}
		return list;
	}

	public void SaveSettings()
	{
		foreach(var factory in _factories.Values)
		{
			foreach(var view in factory.CreatedViews)
			{
				var section = _section.GetCreateSection(GetViewConfigId(view));
				view.SaveViewTo(section);
			}
		}
	}
}
