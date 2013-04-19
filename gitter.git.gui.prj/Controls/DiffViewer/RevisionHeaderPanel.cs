namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	/// <summary><see cref="FlowPanel"/> which displays basic commit information: author, hash, date, subject, etc.</summary>
	public sealed class RevisionHeaderPanel : FlowPanel
	{
		#region Constants

		private const int SelectionMargin = 5;

		#endregion

		#region Data

		private RevisionHeaderContent _content;
		private Revision _revision;
		private bool _isSelectable;
		private bool _isSelected;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="RevisionHeaderPanel"/>.</summary>
		public RevisionHeaderPanel()
		{
			_content = new RevisionHeaderContent();
			_content.Invalidated += OnContentInvalidated;
			_content.SizeChanged += OnContentSizeChanged;
			_content.ContextMenuRequested += OnContentContextMenuRequested;
			_content.CursorChanged += OnContentCursorChanged;
		}

		#endregion

		private void OnContentContextMenuRequested(object sender, ContentContextMenuEventArgs e)
		{
			int x = e.Position.X;
			int y = e.Position.Y;
			if(IsSelectable)
			{
				x += SelectionMargin;
			}
			ShowContextMenu(e.ContextMenu, x, y);
		}

		private void OnContentInvalidated(object sender, ContentInvalidatedEventArgs e)
		{
			var bounds = e.Bounds;
			if(IsSelectable)
			{
				bounds.X += SelectionMargin;
			}
			InvalidateSafe(bounds);
		}

		private void OnContentSizeChanged(object sender, EventArgs e)
		{
			InvalidateSize();
		}

		private void OnContentCursorChanged(object sender, EventArgs e)
		{
			FlowControl.Cursor = _content.Cursor;
		}

		protected override void OnMouseMove(int x, int y)
		{
			if(IsSelectable)
			{
				x -= SelectionMargin;
			}
			if(x < 0)
			{
				_content.OnMouseLeave();
			}
			else
			{
				_content.OnMouseMove(x, y);
			}
			base.OnMouseMove(x, y);
		}

		protected override void OnMouseLeave()
		{
			base.OnMouseLeave();
			_content.OnMouseLeave();
		}

		public bool IsSelectable
		{
			get { return _isSelectable; }
			set { _isSelectable = value; }
		}

		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				if(_isSelected != value)
				{
					_isSelected = value;
					Invalidate();
					if(value && FlowControl != null)
					{
						foreach(var p in FlowControl.Panels)
						{
							if(p != this)
							{
								var rhp = p as RevisionHeaderPanel;
								if(rhp != null)
								{
									rhp.IsSelected = false;
								}
							}
						}
					}
				}
			}
		}

		/// <summary>Displayed <see cref="T:gitter.Git.Revision"/>.</summary>
		public Revision Revision
		{
			get { return _revision; }
			set
			{
				if(_revision != value)
				{
					_revision = value;
					if(FlowControl != null)
					{
						_content.Revision = _revision;
					}
				}
			}
		}

		protected override void OnFlowControlAttached()
		{
			_content.Revision = _revision;
			base.OnFlowControlAttached();
		}

		protected override void OnFlowControlDetached()
		{
			_content.Revision = null;
			base.OnFlowControlDetached();
		}

		protected override void OnMouseDown(int x, int y, MouseButtons button)
		{
			base.OnMouseDown(x, y, button);
			if(IsSelectable)
			{
				if(button == MouseButtons.Left)
				{
					IsSelected = true;
				}
				x -= SelectionMargin;
			}
			_content.OnMouseDown(x, y, button);
		}

		protected override Size OnMeasure(FlowPanelMeasureEventArgs measureEventArgs)
		{
			Assert.IsNotNull(measureEventArgs);

			var width = measureEventArgs.Width;
			if(IsSelectable)
			{
				width -= SelectionMargin;
			}
			_content.Style = Style;
			var size = _content.OnMeasure(measureEventArgs.Graphics, width);
			if(IsSelectable)
			{
				size.Width += SelectionMargin;
			}
			return size;
		}

		protected override void OnPaint(FlowPanelPaintEventArgs paintEventArgs)
		{
			Assert.IsNotNull(paintEventArgs);

			var bounds		= paintEventArgs.Bounds;
			var graphics	= paintEventArgs.Graphics;
			graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
			if(IsSelectable)
			{
				if(IsSelected)
				{
					graphics.FillRectangle(
						SystemBrushes.Highlight, new Rectangle(bounds.X, bounds.Y, SelectionMargin, bounds.Height));
				}
				bounds.Width -= SelectionMargin;
				bounds.X += SelectionMargin;
			}
			_content.Style = Style;
			var clip = Rectangle.Intersect(paintEventArgs.ClipRectangle, bounds);
			if(clip.Width > 0 && clip.Height > 0)
			{
				graphics.SetClip(clip);
				_content.OnPaint(graphics, bounds);
			}
		}
	}
}
