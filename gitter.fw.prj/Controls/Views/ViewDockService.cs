namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Configuration;

	public class ViewDockService
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
			if(environment == null) throw new ArgumentNullException("environment");
			if(grid == null) throw new ArgumentNullException("grid");
			if(section == null) throw new ArgumentNullException("section");

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

		public ICollection<IViewFactory> Factories
		{
			get { return _factories.Values; }
		}

		public ViewBase ActiveView
		{
			get
			{
				foreach(var f in _factories.Values)
				{
					foreach(var tool in f.CreatedViews)
					{
						var host = tool.Host;
						if(host != null && host.IsActive)
						{
							_activeView = tool;
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
			foreach(var f in _factories.Values)
			{
				var res = f as T;
				if(res != null) return res;
			}
			return default(T);
		}

		public void RegisterFactory(IViewFactory factory)
		{
			if(factory == null) throw new ArgumentNullException("factory");
			_factories.Add(factory.Guid, factory);
		}

		public void UnregisterFactory(IViewFactory factory)
		{
			if(factory == null) throw new ArgumentNullException("factory");
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
			if(!factory.Singleton)
			{
				foreach(var v in factory.CreatedViews)
				{
					if(v.Host != null && v.Host.IsDocumentWell)
						host = v.Host;
				}
				host.AddView(view);
			}
			else
			{
				switch(factory.DefaultViewPosition)
				{
					case ViewPosition.SecondaryDocumentHost:
						foreach(var h in ViewHost.ViewHosts)
						{
							if(h != host && h.IsDocumentWell)
							{
								h.AddView(view);
								return;
							}
						}
						host = new ViewHost(_grid, false, true, new[] { view })
						{
							Size = _grid.RootHost.Size
						};
						_grid.RootHost.PerformDock(host, DockResult.Right);
						break;

					case ViewPosition.LeftTool:
						host = new ViewHost(_grid, false, false, new[] { view });
						_grid.PerformDock(host, DockResult.Left);
						break;
					case ViewPosition.TopTool:
						host = new ViewHost(_grid, false, false, new[] { view });
						_grid.PerformDock(host, DockResult.Top);
						break;
					case ViewPosition.RightTool:
						host = new ViewHost(_grid, false, false, new[] { view });
						_grid.PerformDock(host, DockResult.Right);
						break;
					case ViewPosition.BottomTool:
						host = new ViewHost(_grid, false, false, new[] { view });
						_grid.PerformDock(host, DockResult.Bottom);
						break;

					case ViewPosition.LeftAutoHideTool:
						host = new ViewHost(_grid, false, false, new[] { view });
						host.UnpinFromLeft();
						break;
					case ViewPosition.TopAutoHideTool:
						host = new ViewHost(_grid, false, false, new[] { view });
						host.UnpinFromTop();
						break;
					case ViewPosition.RightAutoHideTool:
						host = new ViewHost(_grid, false, false, new[] { view });
						host.UnpinFromRight();
						break;
					case ViewPosition.BottomAutoHideTool:
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
			return (WebBrowserView)ShowView(WebBrowserViewFactory.Guid, new Dictionary<string, object>() { { "url", url } }, activate);
		}

		public ViewBase ShowView(Guid guid)
		{
			return ShowView(guid, true);
		}

		public ViewBase ShowView(Guid guid, bool activate)
		{
			IViewFactory factory;
			if(!_factories.TryGetValue(guid, out factory))
				throw new ArgumentException("Unknown GUID.", "guid");
			if(factory.Singleton)
			{
				ViewBase existing = null;
				foreach(var tool in factory.CreatedViews)
				{
					existing = tool;
					break;
				}
				if(existing == null)
				{
					existing = factory.CreateView(_environment);
					var section = _section.TryGetSection(GetViewConfigId(existing));
					if(section != null)
						existing.LoadViewFrom(section);
					existing.Closing += OnViewClosing;
					ShowNewView(factory, existing, activate);
				}
				else
				{
					existing.ApplyParameters(null);
					ShowExistingView(existing, activate);
				}
				return existing;
			}
			else
			{
				ViewBase existing = null;
				foreach(var tool in factory.CreatedViews)
				{
					if(tool.ParametersIdentical(null))
					{
						existing = tool;
						break;
					}
				}
				if(existing == null)
				{
					existing = factory.CreateView(_environment);
					var section = _section.TryGetSection(GetViewConfigId(existing));
					if(section != null)
						existing.LoadViewFrom(section);
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

		public ViewBase ShowView(Guid guid, IDictionary<string, object> parameters)
		{
			return ShowView(guid, parameters, true);
		}

		public ViewBase ShowView(Guid guid, IDictionary<string, object> parameters, bool activate)
		{
			IViewFactory factory;
			if(!_factories.TryGetValue(guid, out factory))
				throw new ArgumentException("Unknown GUID.", "guid");
			if(factory.Singleton)
			{
				ViewBase existing = null;
				foreach(var tool in factory.CreatedViews)
				{
					existing = tool;
					break;
				}
				if(existing == null)
				{
					existing = factory.CreateView(_environment, parameters);
					existing.Closing += OnViewClosing;
					ShowNewView(factory, existing, activate);
				}
				else
				{
					existing.ApplyParameters(parameters);
					ShowExistingView(existing, activate);
				}
				return existing;
			}
			else
			{
				ViewBase existing = null;
				foreach(var tool in factory.CreatedViews)
				{
					if(tool.ParametersIdentical(parameters))
					{
						existing = tool;
						break;
					}
				}
				if(existing == null)
				{
					existing = factory.CreateView(_environment, parameters);
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
				return null;
			foreach(var tool in factory.CreatedViews)
			{
				return tool;
			}
			return null;
		}

		public ViewBase FindView(Guid guid, IDictionary<string, object> parameters)
		{
			IViewFactory factory;
			if(!_factories.TryGetValue(guid, out factory))
				return null;
			foreach(var tool in factory.CreatedViews)
			{
				if(tool.ParametersIdentical(parameters))
					return tool;
			}
			return null;
		}

		public IEnumerable<ViewBase> FindViews(Guid guid)
		{
			IViewFactory factory;
			if(!_factories.TryGetValue(guid, out factory))
				return new ViewBase[0];
			return factory.CreatedViews;
		}

		public IEnumerable<ViewBase> FindViews(Guid guid, IDictionary<string, object> parameters)
		{
			IViewFactory factory;
			if(!_factories.TryGetValue(guid, out factory))
				return new ViewBase[0];
			var list = new List<ViewBase>();
			foreach(var tool in factory.CreatedViews)
			{
				if(tool.ParametersIdentical(parameters))
					list.Add(tool);
			}
			return list;
		}

		public void SaveSettings()
		{
			foreach(var factory in _factories.Values)
			{
				foreach(var tool in factory.CreatedViews)
				{
					var section = _section.GetCreateSection(GetViewConfigId(tool));
					tool.SaveViewTo(section);
				}
			}
		}
	}
}
