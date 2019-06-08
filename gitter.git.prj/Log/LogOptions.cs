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

		public IEnumerable<Reference> AllowedReferences => _allowedReferences;

		public void AllowReference(Reference reference)
		{
			Verify.Argument.IsNotNull(reference, nameof(reference));

			if(_allowedReferences.Add(reference))
			{
				if(_filter == LogReferenceFilter.Allowed)
				{
					Changed?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		public void DisallowReference(Reference reference)
		{
			Verify.Argument.IsNotNull(reference, nameof(reference));

			if(_allowedReferences.Remove(reference))
			{
				if(_filter == LogReferenceFilter.Allowed)
				{
					Changed?.Invoke(this, EventArgs.Empty);
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
					Changed?.Invoke(this, EventArgs.Empty);
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
					Changed?.Invoke(this, EventArgs.Empty);
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
					Changed?.Invoke(this, EventArgs.Empty);
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
					Changed?.Invoke(this, EventArgs.Empty);
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
			Changed?.Invoke(this, EventArgs.Empty);
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
			Verify.Argument.IsNotNull(section, nameof(section));

			section.SetValue("Order", _order);
			section.SetValue("MaxCount", _maxCount);
			section.SetValue("Skip", _skip);
		}

		public void LoadFrom(Section section)
		{
			Verify.Argument.IsNotNull(section, nameof(section));

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
			if(changed) Changed?.Invoke(this, EventArgs.Empty);
		}
	}
}
