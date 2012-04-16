namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;

	public abstract class ViewFactoryBase : IViewFactory
	{
		#region Data

		private readonly Guid _guid;
		private readonly string _name;
		private readonly Image _image;
		private readonly bool _singleton;
		private readonly LinkedList<ViewBase> _createdViews;
		private ViewPosition _viewPosition;

		#endregion

		#region .ctor

		protected ViewFactoryBase(Guid guid, string name, Image image, bool singleton)
		{
			_guid = guid;
			_name = name;
			_image = image;
			_singleton = singleton;
			_createdViews = new LinkedList<ViewBase>();
		}

		protected ViewFactoryBase(Guid guid, string name, Image image)
			: this(guid, name, image, false)
		{
		}

		#endregion

		#region Properties

		public Guid Guid
		{
			get { return _guid; }
		}

		public string Name
		{
			get { return _name; }
		}

		public Image Image
		{
			get { return _image; }
		}

		public virtual bool Singleton
		{
			get { return _singleton; }
		}

		public IEnumerable<ViewBase> CreatedViews
		{
			get { return _createdViews; }
		}

		public ViewPosition DefaultViewPosition
		{
			get { return _viewPosition; }
			protected set { _viewPosition = value; }
		}

		#endregion

		/// <summary>Closes all tools, created by this factory.</summary>
		public void CloseAllViews()
		{
			while(_createdViews.Count != 0)
				_createdViews.First.Value.Close();
		}

		private void OnViewDisposed(object sender, EventArgs e)
		{
			var view = (ViewBase)sender;
			view.Disposed -= OnViewDisposed;
			lock(_createdViews)
			{
				_createdViews.Remove(view);
			}
		}

		/// <summary>Create new view with specified parameters.</summary>
		/// <param name="environment">Application working environment.</param>
		/// <param name="parameters">Creation parameters.</param>
		/// <returns>Created view.</returns>
		protected abstract ViewBase CreateViewCore(IWorkingEnvironment environment, IDictionary<string, object> parameters);

		/// <summary>Create new view with default parameters.</summary>
		/// <param name="environment">Application working environment.</param>
		/// <returns>Created view.</returns>
		public ViewBase CreateView(IWorkingEnvironment environment)
		{
			if(environment == null) throw new ArgumentNullException("environment");

			var view = CreateViewCore(environment, null);
			if(view != null)
			{
				lock(_createdViews)
				{
					_createdViews.AddLast(view);
					view.Disposed += OnViewDisposed;
				}
			}
			return view;
		}

		/// <summary>Create new view with specified parameters.</summary>
		/// <param name="environment">Application working environment.</param>
		/// <param name="parameters">Creation parameters.</param>
		/// <returns>Created view.</returns>
		public ViewBase CreateView(IWorkingEnvironment environment, IDictionary<string, object> parameters)
		{
			if(environment == null) throw new ArgumentNullException("environment");

			var view = CreateViewCore(environment, parameters);
			if(view != null)
			{
				lock(_createdViews)
				{
					_createdViews.AddLast(view);
					view.Disposed += OnViewDisposed;
				}
			}
			return view;
		}
	}
}
