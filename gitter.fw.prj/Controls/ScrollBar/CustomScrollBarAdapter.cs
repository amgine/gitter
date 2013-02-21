namespace gitter.Framework.Controls
{
	using System;
	using System.Windows.Forms;

	public sealed class CustomScrollBarAdapter : IScrollBarWidget
	{
		#region Data

		private readonly CustomScrollBar _scrollBar;

		#endregion

		#region Events

		public event EventHandler<ScrollEventArgs> Scroll;
		public event EventHandler ValueChanged;

		#endregion

		#region .ctor

		public CustomScrollBarAdapter(Orientation orientation, CustomScrollBarRenderer renderer)
		{
			switch(orientation)
			{
				case Orientation.Vertical:
					_scrollBar = new CustomVScrollbar() { Renderer = renderer };
					break;
				case Orientation.Horizontal:
					_scrollBar = new CustomHScrollbar() { Renderer = renderer };
					break;
				default:
					throw new ArgumentException("orientation");
			}
			_scrollBar.Scroll += OnScrollBarScroll;
			_scrollBar.ValueChanged += OnScrollBarValueChanged;
		}

		#endregion

		#region Event Handlers

		private void OnScrollBarScroll(object sender, ScrollEventArgs e)
		{
			var handler = Scroll;
			if(handler != null) handler(this, e);
		}

		private void OnScrollBarValueChanged(object sender, EventArgs e)
		{
			var handler = ValueChanged;
			if(handler != null) handler(this, e);
		}

		#endregion

		#region IScrollBarWidget Members

		public Control Control
		{
			get { return _scrollBar; }
		}

		public Orientation Orientation
		{
			get { return _scrollBar.Orientation; }
		}

		public int Value
		{
			get { return _scrollBar.Value; }
			set { _scrollBar.Value = value; }
		}

		public int Minimum
		{
			get { return _scrollBar.Minimum; }
			set { _scrollBar.Minimum = value; }
		}

		public int Maximum
		{
			get { return _scrollBar.Maximum; }
			set { _scrollBar.Maximum = value; }
		}

		public int SmallChange
		{
			get { return _scrollBar.SmallChange; }
			set { _scrollBar.SmallChange = value; }
		}

		public int LargeChange
		{
			get { return _scrollBar.LargeChange; }
			set { _scrollBar.LargeChange = value; }
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			_scrollBar.Dispose();
		}

		#endregion
	}
}
