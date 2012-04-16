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
		public RedmineViewBase(Guid guid, IWorkingEnvironment environment, IDictionary<string, object> parameters)
			: base(guid, environment, parameters)
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
