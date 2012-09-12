namespace gitter.Framework
{
	using System;

	public class GCCache<T> : Cache<T>
		where T : class
	{
		#region Data

		private readonly Func<T> _onReevaluate;
		private WeakReference _weakRef;

		#endregion

		public GCCache(Func<T> onReevaluate)
		{
			Verify.Argument.IsNotNull(onReevaluate, "onReevaluate");

			_onReevaluate = onReevaluate;
		}

		public GCCache(Func<T> onReevaluate, T value)
		{
			Verify.Argument.IsNotNull(onReevaluate, "onReevaluate");

			_onReevaluate = onReevaluate;
			if(value != null) _weakRef = new WeakReference(value);
		}

		public override bool IsCached
		{
			get { return _weakRef != null && _weakRef.IsAlive; }
		}

		public override T Value
		{
			get
			{
				if(_weakRef == null)
				{
					return Reevaluate();
				}
				else
				{
					var value = (T)_weakRef.Target;
					if(value == null)
					{
						return Reevaluate();
					}
					else
					{
						return value;
					}
				}
			}
		}

		public override void Invalidate()
		{
			_weakRef = null;
		}

		private T Reevaluate()
		{
			var value = _onReevaluate();
			_weakRef = new WeakReference(value);
			return value;
		}
	}
}
