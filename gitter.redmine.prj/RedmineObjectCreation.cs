using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gitter.Redmine
{
	public abstract class RedmineObjectCreation<T> : RedmineObjectDefinition<T>
		where T : RedmineObject
	{
		private readonly RedmineServiceContext _context;

		protected RedmineObjectCreation(RedmineServiceContext context)
		{
			if(context == null) throw new ArgumentNullException("context");

			_context = context;
			ResetCore();
		}

		protected RedmineServiceContext Context
		{
			get { return _context; }
		}
	}
}
