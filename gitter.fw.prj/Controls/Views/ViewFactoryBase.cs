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
	using System.Drawing;

	public abstract class ViewFactoryBase : IViewFactory
	{
		private readonly bool _singleton;
		private readonly LinkedList<ViewBase> _createdViews;

		protected ViewFactoryBase(Guid guid, string name, Image image, bool singleton)
		{
			Guid = guid;
			Name = name;
			Image = image;
			_singleton = singleton;
			_createdViews = new LinkedList<ViewBase>();
		}

		protected ViewFactoryBase(Guid guid, string name, Image image)
			: this(guid, name, image, false)
		{
		}

		public Guid Guid { get; }

		public string Name { get; }

		public Image Image { get; }

		public virtual bool IsSingleton => _singleton;

		public IEnumerable<ViewBase> CreatedViews => _createdViews;

		public ViewPosition DefaultViewPosition { get; protected set; }

		/// <summary>Closes all tools, created by this factory.</summary>
		public void CloseAllViews()
		{
			while(_createdViews.Count != 0)
			{
				_createdViews.First.Value.Close();
			}
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
		/// <param name="viewModel">View model.</param>
		/// <returns>Created view.</returns>
		protected abstract ViewBase CreateViewCore(IWorkingEnvironment environment);

		/// <summary>Create new view with default parameters.</summary>
		/// <param name="environment">Application working environment.</param>
		/// <returns>Created view.</returns>
		public ViewBase CreateView(IWorkingEnvironment environment)
		{
			Verify.Argument.IsNotNull(environment, nameof(environment));

			var view = CreateViewCore(environment);
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
