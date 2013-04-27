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

namespace gitter.Framework.Services
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;

	public sealed class RepositoryCollection : Collection<RepositoryLink>
	{
		public event EventHandler<RepositoryLinkEventArgs> Added;
		public event EventHandler<RepositoryLinkEventArgs> Deleted;
		public event EventHandler Cleared;

		protected override void InsertItem(int index, RepositoryLink item)
		{
			base.InsertItem(index, item);

			var handler = Added;
			if(handler != null) handler(this, new RepositoryLinkEventArgs(item));
		}

		protected override void RemoveItem(int index)
		{
			var item = Items[index];
			base.RemoveItem(index);
			item.InvokeDeleted();
			var handler = Deleted;
			if(handler != null) handler(this, new RepositoryLinkEventArgs(item));
		}

		protected override void ClearItems()
		{
			base.ClearItems();

			var handler = Cleared;
			if(handler != null) handler(this, EventArgs.Empty);
		}
	}
}
