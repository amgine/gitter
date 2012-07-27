namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework.Configuration;

	using gitter.Git.AccessLayer;

	public sealed class LogOptions
	{
		public event EventHandler Changed;

		private HashSet<Reference> _allowedReferences;
		private RevisionQueryOrder _order;
		private LogReferenceFilter _filter;
		private int _maxCount;
		private int _skip;

		public LogOptions()
		{
			_order = RevisionQueryOrder.DateOrder;
			_filter = LogReferenceFilter.All;
			_allowedReferences = new HashSet<Reference>();
			_skip = 0;
			_maxCount = 500;
		}

		public IEnumerable<Reference> AllowedReferences
		{
			get { return _allowedReferences; }
		}

		public void AllowReference(Reference reference)
		{
			if(reference == null) throw new ArgumentNullException("reference");

			if(_allowedReferences.Add(reference))
			{
				if(_filter == LogReferenceFilter.Allowed)
				{
					Changed.Raise(this);
				}
			}
		}

		public void DisallowReference(Reference reference)
		{
			if(reference == null) throw new ArgumentNullException("reference");

			if(_allowedReferences.Remove(reference))
			{
				if(_filter == LogReferenceFilter.Allowed)
				{
					Changed.Raise(this);
				}
			}
		}

		public LogReferenceFilter Filter
		{
			get { return _filter; }
			set
			{
				if(_filter != value)
				{
					_filter = value;
					Changed.Raise(this);
				}
			}
		}

		public RevisionQueryOrder Order
		{
			get { return _order; }
			set
			{
				if(_order != value)
				{
					_order = value;
					Changed.Raise(this);
				}
			}
		}

		public int MaxCount
		{
			get { return _maxCount; }
			set
			{
				if(_maxCount != value)
				{
					_maxCount = value;
					Changed.Raise(this);
				}
			}
		}

		public int Skip
		{
			get { return _skip; }
			set
			{
				if(_skip != value)
				{
					_skip = value;
					Changed.Raise(this);
				}
			}
		}

		public void Reset()
		{
			_filter = LogReferenceFilter.All;
			_order = RevisionQueryOrder.Default;
			_allowedReferences.Clear();
			_filter = LogReferenceFilter.All;
			_maxCount = 0;
			_skip = 0;
			Changed.Raise(this);
		}

		internal QueryRevisionsParameters GetLogParameters()
		{
			var p = new QueryRevisionsParameters()
			{
				MaxCount = MaxCount,
				Skip = Skip,
				Order = Order,
			};
			switch(_filter)
			{
				case LogReferenceFilter.All:
					p.All = true;
					break;
				case LogReferenceFilter.Allowed:
					var l = new List<string>();
					foreach(var reference in _allowedReferences)
					{
						l.Add(reference.FullName);
					}
					p.References = l;
					break;
				case LogReferenceFilter.HEAD:
					p.References = new List<string>() { GitConstants.HEAD };
					break;
			}
			return p;
		}

		public void SaveTo(Section section)
		{
			if(section == null) throw new ArgumentNullException("section");

			section.SetValue("Order", _order);
			section.SetValue("MaxCount", _maxCount);
			section.SetValue("Skip", _skip);
		}

		public void LoadFrom(Section section)
		{
			if(section == null) throw new ArgumentNullException("section");

			bool changed = false;
			var order = section.GetValue("Order", RevisionQueryOrder.DateOrder);
			if(_order != order)
			{
				_order = order;
				changed = true;
			}
			var maxCount = section.GetValue("MaxCount", 0);
			if(_maxCount != maxCount)
			{
				_maxCount = maxCount;
				changed = true;
			}
			var skip = section.GetValue("Skip", 0);
			if(_skip != skip)
			{
				_skip = skip;
				changed = true;
			}
			if(changed) Changed.Raise(this);
		}
	}
}
