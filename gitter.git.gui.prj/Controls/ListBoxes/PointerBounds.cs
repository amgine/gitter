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

namespace gitter.Git.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Git.Gui.Controls;

	public struct PointerBounds
	{
		#region Static

		public static ContextMenuStrip GetContextMenu(IEnumerable<PointerBounds> pointers, int x, int y)
		{
			if(pointers == null) return null;
			foreach(var ptr in pointers)
			{
				if(x >= ptr.Bounds.X && x < ptr.Bounds.Right)
				{
					return ptr.GetContextMenu();
				}
			}
			return null;
		}

		#endregion

		#region .ctor

		public PointerBounds(IRevisionPointer revisionPointer, Rectangle bounds)
		{
			RevisionPointer = revisionPointer;
			Bounds          = bounds;
		}

		#endregion

		#region Properties

		public IRevisionPointer RevisionPointer { get; }

		public Rectangle Bounds { get; }

		#endregion

		#region Methods

		public ContextMenuStrip GetContextMenu()
			=> RevisionPointer switch
			{
				Tag          tag    => new TagMenu(tag),
				BranchBase   branch => new BranchMenu(branch),
				StashedState stash  => new StashedStateMenu(stash),
				Head         head   => new HeadMenu(head),
				_ => default,
			};

		#endregion
	}
}
