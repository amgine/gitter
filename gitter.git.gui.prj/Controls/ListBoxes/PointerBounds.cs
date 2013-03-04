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

		#region Data

		private readonly IRevisionPointer _revisionPointer;
		private readonly Rectangle _bounds;

		#endregion

		#region .ctor

		public PointerBounds(IRevisionPointer revisionPointer, Rectangle bounds)
		{
			_revisionPointer = revisionPointer;
			_bounds = bounds;
		}

		#endregion

		#region Properties

		public IRevisionPointer RevisionPointer
		{
			get { return _revisionPointer; }
		}

		public Rectangle Bounds
		{
			get { return _bounds; }
		}

		#endregion

		#region Methods

		public ContextMenuStrip GetContextMenu()
		{
			var tag = RevisionPointer as Tag;
			if(tag != null)
			{
				return new TagMenu(tag);
			}
			var branch = RevisionPointer as BranchBase;
			if(branch != null)
			{
				return new BranchMenu(branch);
			}
			var stash = RevisionPointer as StashedState;
			if(stash != null)
			{
				return new StashedStateMenu(stash);
			}
			var head = RevisionPointer as Head;
			if(head != null)
			{
				return new HeadMenu(head);
			}
			return null;
		}

		#endregion
	}
}
