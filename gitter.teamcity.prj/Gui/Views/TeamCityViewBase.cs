namespace gitter.TeamCity.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework;
	using gitter.Framework.Controls;

	public class TeamCityViewBase : ViewBase
	{
		private TeamCityServiceContext _serviceContext;

		/// <summary>Create <see cref="RedmineViewBase"/>.</summary>
		public TeamCityViewBase()
		{
		}

		/// <summary>Create <see cref="RedmineViewBase"/>.</summary>
		public TeamCityViewBase(Guid guid, IWorkingEnvironment environment, IDictionary<string, object> parameters)
			: base(guid, environment, parameters)
		{
		}

		public TeamCityServiceContext ServiceContext
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
