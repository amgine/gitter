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

namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Text;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Configuration;

	public sealed class ViewDockService
	{
		#region Data

		private readonly Dictionary<Guid, IViewFactory> _factories;
		private readonly IWorkingEnvironment _environment;
		private readonly ViewDockGrid _grid;
		private readonly Section _section;

		private ViewBase _activeView;

		#endregion

		public event EventHandler ActiveViewChanged;

		private void InvokeActiveViewChanged()
		{
			var handler = ActiveViewChanged;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		public ViewDockService(IWorkingEnvironment environment, ViewDockGrid grid, Section section)
		{
			Verify.Argument.IsNotNull(environment, "environment");
			Verify.Argument.IsNotNull(grid, "grid");
			Verify.Argument.IsNotNull(section, "section");

			_environment = environment;
			_grid = grid;
			_section = section;
			_factories = new Dictionary<Guid, IViewFactory>();

			RegisterFactory(new WebBrowserViewFactory());
		}

		public ViewDockGrid Grid
		{
			get { return _grid; }
		}

		public ICollection<IViewFactory> ViewFactories
		{
			get { return _factories.Values; }
		}

		public ViewBase ActiveView
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

		public T GetFactory<T>()
			where T : class, IViewFactory
		{
			foreach(var factory in ViewFactories)
			{
				var res = factory as T;
				if(res != null) return res;
			}
			return default(T);
		}

		public void RegisterFactory(IViewFactory factory)
		{
			Verify.Argument.IsNotNull(factory, "factory");

			_factories.Add(factory.Guid, factory);
		}

		public void UnregisterFactory(IViewFactory factory)
		{
			Verify.Argument.IsNotNull(factory, "factory");

			UnregisterFactory(factory.Guid);
		}

		public void UnregisterFactory(Guid guid)
		{
			_factories.Remove(guid);
		}

		private void OnViewClosing(object sender, EventArgs e)
		{
			var view = (ViewBase)sender;
			view.Closing -= OnViewClosing;
			var section = _section.GetCreateSection(GetViewConfigId(view));
			view.SaveViewTo(section);
			if(_activeView == view) _activeView = null;
		}

		private void FindAppropriateViewHost(IViewFactory factory, ViewBase view)
		{
			var host = _grid.RootHost;
			if(!factory.IsSingleton)
			{
				foreach(var v in factory.CreatedViews)
				{
					if(v.Host != null && v.Host.IsDocumentWell)
					{
						host = v.Host;
					}
				}
				host.AddView(view);
			}
			else
			{
				switch(factory.DefaultViewPosition)
				{
					case ViewPosition.SecondaryDocumentHost:
						lock(ViewHost.ViewHosts)
						{
							foreach(var h in ViewHost.ViewHosts)
							{
								if(h != host && h.IsDocumentWell)
								{
									h.AddView(view);
									return;
								}
							}
						}
						host = new ViewHost(_grid, false, true, new[] { view })
						{
							Size = _grid.RootHost.Size
						};
						_grid.RootHost.PerformDock(host, DockResult.Right);
						break;

					case ViewPosition.Left:
						host = new ViewHost(_grid, false, false, new[] { view });
						_grid.PerformDock(host, DockResult.Left);
						break;
					case ViewPosition.Top:
						host = new ViewHost(_grid, false, false, new[] { view });
						_grid.PerformDock(host, DockResult.Top);
						break;
					case ViewPosition.Right:
						host = new ViewHost(_grid, false, false, new[] { view });
						_grid.PerformDock(host, DockResult.Right);
						break;
					case ViewPosition.Bottom:
						host = new ViewHost(_grid, false, false, new[] { view });
						_grid.PerformDock(host, DockResult.Bottom);
						break;

					case ViewPosition.LeftAutoHide:
						host = new ViewHost(_grid, false, false, new[] { view });
						host.UnpinFromLeft();
						break;
					case ViewPosition.TopAutoHide:
						host = new ViewHost(_grid, false, false, new[] { view });
						host.UnpinFromTop();
						break;
					case ViewPosition.RightAutoHide:
						host = new ViewHost(_grid, false, false, new[] { view });
						host.UnpinFromRight();
						break;
					case ViewPosition.BottomAutoHide:
						host = new ViewHost(_grid, false, false, new[] { view });
						host.UnpinFromBottom();
						break;

					case ViewPosition.Float:
						host = new ViewHost(_grid, false, false, new[] { view });
						var form = host.PrepareFloatingMode();
						form.Location = _grid.PointToScreen(new Point(20, 20));
						form.Show(_grid.TopLevelControl);
						break;

					default:
						_grid.RootHost.AddView(view);
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
				if(_activeView != null)
				{

				}
				if(_activeView != view)
				{
					_activeView = view;
					ActiveViewChanged.Raise(this);
				}
			}
			else
			{
				view.Host.SetActiveView(view);
			}
		}

		public WebBrowserView ShowWebBrowserView(string url)
		{
			return ShowWebBrowserView(url, true);
		}

		public WebBrowserView ShowWebBrowserView(string url, bool activate)
		{
			return (WebBrowserView)ShowView(
				WebBrowserViewFactory.Guid,
				new WebBrowserViewModel(url),
				activate);
		}

		public ViewBase ShowView(Guid guid)
		{
			return ShowView(guid, true);
		}

		private IViewFactory GetViewFactoryByGuid(Guid guid)
		{
			IViewFactory factory;
			Verify.Argument.IsTrue(
				_factories.TryGetValue(guid, out factory),
				"guid",
				string.Format(
					CultureInfo.InvariantCulture,
					"Unknown view factory GUID: {0}",
					guid));
			return factory;
		}

		public ViewBase ShowView(Guid guid, bool activate)
		{
			var factory = GetViewFactoryByGuid(guid);
			if(factory.IsSingleton)
			{
				ViewBase existing = null;
				foreach(var view in factory.CreatedViews)
				{
					existing = view;
					break;
				}
				if(existing == null)
				{
					existing = factory.CreateView(_environment);
					var section = _section.TryGetSection(GetViewConfigId(existing));
					if(section != null)
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
				ViewBase existing = null;
				foreach(var view in factory.CreatedViews)
				{
					if(object.Equals(view.ViewModel, null))
					{
						existing = view;
						break;
					}
				}
				if(existing == null)
				{
					existing = factory.CreateView(_environment);
					var section = _section.TryGetSection(GetViewConfigId(existing));
					if(section != null)
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
				ViewBase existing = null;
				foreach(var view in factory.CreatedViews)
				{
					existing = view;
					break;
				}
				if(existing == null)
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
				ViewBase existing = null;
				foreach(var view in factory.CreatedViews)
				{
					if(object.Equals(view.ViewModel, viewModel))
					{
						existing = view;
						break;
					}
				}
				if(existing == null)
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

		public ViewBase FindView(Guid guid)
		{
			IViewFactory factory;
			if(!_factories.TryGetValue(guid, out factory))
			{
				return null;
			}
			foreach(var view in factory.CreatedViews)
			{
				return view;
			}
			return null;
		}

		public ViewBase FindView(Guid guid, object viewModel)
		{
			IViewFactory factory;
			if(!_factories.TryGetValue(guid, out factory))
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
			IViewFactory factory;
			if(!_factories.TryGetValue(guid, out factory))
			{
				return new ViewBase[0];
			}
			return factory.CreatedViews;
		}

		public IEnumerable<ViewBase> FindViews(Guid guid, object viewModel)
		{
			IViewFactory factory;
			if(!_factories.TryGetValue(guid, out factory))
			{
				return new ViewBase[0];
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
}
