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

namespace gitter.GitLab.Gui;

using System;
using System.ComponentModel;

using gitter.Framework;
using gitter.Framework.Controls;

[DesignerCategory("")]
abstract class GitLabViewBase : ViewBase
{
	private GitLabServiceContext _serviceContext;

	protected GitLabViewBase(Guid guid, IWorkingEnvironment environment)
		: base(guid, environment)
	{
	}

	public GitLabServiceContext ServiceContext
	{
		get => _serviceContext;
		set
		{
			if(value != _serviceContext)
			{
				if(_serviceContext is not null)
				{
					OnContextDetached(_serviceContext);
				}
				_serviceContext = value;
				if(_serviceContext is not null)
				{
					OnContextAttached(_serviceContext);
				}
			}
		}
	}

	protected virtual void OnContextDetached(GitLabServiceContext serviceContext)
	{
	}

	protected virtual void OnContextAttached(GitLabServiceContext serviceContext)
	{
	}

	protected static bool Search<TItem, TOptions>(CustomListBoxItemsCollection items, Func<TItem, TOptions, bool> test, int start, TOptions search, int direction)
		where TItem    : CustomListBoxItem
		where TOptions : SearchOptions
	{
		if(search.Text.Length == 0) return true;
		int count = items.Count;
		if(count == 0) return false;
		int end;
		if(direction == 1)
		{
			start = (start + 1) % count;
			end = start - 1;
			if(end < 0) end += count;
		}
		else
		{
			start = (start - 1);
			if(start < 0) start += count;
			end = (start + 1) % count;
		}
		while(start != end)
		{
			if(items[start] is TItem item)
			{
				if(test(item, search))
				{
					item.FocusAndSelect();
					return true;
				}
			}
			if(direction == 1)
			{
				start = (start + 1) % count;
			}
			else
			{
				--start;
				if(start < 0) start = count - 1;
			}
		}
		return false;
	}
}
