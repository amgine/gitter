﻿#region Copyright Notice
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

namespace gitter.Git.Gui.Controls;

using System;

using gitter.Framework;
using gitter.Framework.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

/// <summary><see cref="CustomListBox"/> for displaying <see cref="ReflogRecordListItem"/>.</summary>
public sealed class ReflogListBox : CustomListBox
{
	public ReflogListBox()
	{
	}

	public Reflog Reflog { get; private set; }

	public void Load(Reflog reflog)
	{
		if(Reflog == reflog) return;

		if(Reflog is not null)
		{
			DetachFromReflog();
		}
		Reflog = reflog;
		if(Reflog is not null)
		{
			AttachToReflog();
		}
	}

	private void AttachToReflog()
	{
		lock(Reflog)
		{
			foreach(var record in Reflog)
			{
				Items.Add(new ReflogRecordListItem(record));
			}
			Reflog.RecordAdded += OnRecordAdded;
		}
	}

	private void DetachFromReflog()
	{
		Reflog.RecordAdded -= OnRecordAdded;
		Items.Clear();
	}

	private void OnRecordAdded(object sender, ReflogRecordEventArgs e)
	{
		var item = new ReflogRecordListItem(e.Object);
		Items.InsertSafe(e.Object.Index, item);
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			if(Reflog is not null)
			{
				DetachFromReflog();
				Reflog = null;
			}
		}
		base.Dispose(disposing);
	}
}
