namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;

	public abstract class ViewFactory : IViewFactory
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

		protected ViewFactory(Guid guid, string name, Image image, bool singleton)
		{
			_guid = guid;
			_name = name;
			_image = image;
			_singleton = singleton;
			_createdViews = new LinkedList<ViewBase>();
		}

		protected ViewFactory(Guid guid, string name, Image image)
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

		protected abstract ViewBase CreateViewCore(IDictionary<string, object> parameters);

		public ViewBase CreateView()
		{
			var tool = CreateViewCore(null);
			lock(_createdViews)
			{
				_createdViews.AddLast(tool);
				tool.Disposed += OnViewDisposed;
			}
			return tool;
		}

		public ViewBase CreateView(IDictionary<string, object> parameters)
		{
			var tool = CreateViewCore(parameters);
			lock(_createdViews)
			{
				_createdViews.AddLast(tool);
				tool.Disposed += OnViewDisposed;
			}
			return tool;
		}
	}
}
