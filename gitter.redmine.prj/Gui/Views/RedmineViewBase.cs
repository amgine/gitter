#region Copyright Notice
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

namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework;
	using gitter.Framework.Controls;

	public class RedmineViewBase : ViewBase
	{
		private RedmineServiceContext _serviceContext;

		/// <summary>Create <see cref="RedmineViewBase"/>.</summary>
		public RedmineViewBase()
		{
		}

		/// <summary>Create <see cref="RedmineViewBase"/>.</summary>
		public RedmineViewBase(Guid guid, IWorkingEnvironment environment)
			: base(guid, environment)
		{
		}

		public RedmineServiceContext ServiceContext
		{
			get { return _serviceContext; }
			set
			{
				if(value != _serviceContext)
				{
					if(_serviceContext != null)
					{
						OnContextDetached();
					}
					_serviceContext = value;
					if(_serviceContext != null)
					{
						OnContextAttached();
					}
				}
			}
		}

		protected virtual void OnContextAttached()
		{
		}

		protected virtual void OnContextDetached()
		{
		}

		public virtual void RefreshContent()
		{
		}
	}
}
