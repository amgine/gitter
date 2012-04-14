namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;

	/// <summary>Factory for <see cref="ViewBase"/> objects.</summary>
	public interface IViewFactory
	{
		/// <summary>View GUID.</summary>
		Guid Guid { get; }

		/// <summary>View name.</summary>
		string Name { get; }

		/// <summary>Tool icon.</summary>
		Image Image { get; }

		/// <summary>Only one instance of view should be maintained.</summary>
		bool Singleton { get; }

		/// <summary>List of views created by this factory.</summary>
		IEnumerable<ViewBase> CreatedViews { get; }

		/// <summary>Closes all views, created by this factory.</summary>
		void CloseAllViews();

		/// <summary>Create new view with default parameters.</summary>
		/// <returns>Created tool.</returns>
		ViewBase CreateView();

		/// <summary>Create new view with specified parameters.</summary>
		/// <param name="parameters">Creation parameters.</param>
		/// <returns>Created view.</returns>
		ViewBase CreateView(IDictionary<string, object> parameters);

		/// <summary>Default view position.</summary>
		ViewPosition DefaultViewPosition { get; }
	}
}
