namespace gitter.Framework.Services
{
	using System;

	public sealed class TrackingEventArgs : EventArgs
	{
		#region Data

		private readonly int _index;
		private readonly bool _tracked;

		#endregion

		#region .ctor

		public TrackingEventArgs(bool tracked, int index)
		{
			_tracked = tracked;
			_index = index;
		}

		#endregion

		#region Properties

		public bool IsTracked
		{
			get { return _tracked; }
		}

		public int Index
		{
			get { return _index; }
		}

		#endregion
	}

	public sealed class TrackingEventArgs<T> : EventArgs
	{
		#region Data

		private readonly int _index;
		private readonly T _element;
		private readonly bool _tracked;

		#endregion

		#region .ctor

		public TrackingEventArgs(bool tracked, int index, T element)
		{
			_tracked = tracked;
			_index = index;
			_element = element;
		}

		#endregion

		#region Properties

		public bool IsTracked
		{
			get { return _tracked; }
		}

		public int Index
		{
			get { return _index; }
		}

		public T Item
		{
			get { return _element; }
		}

		#endregion
	}
}
