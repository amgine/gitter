namespace gitter.Framework
{
	using System;

	public class PersistentCache<T> : Cache<T>
	{
		#region Data

		private readonly Func<T> _onReevaluate;
		private T _value;
		private bool _isCached;

		#endregion

		public PersistentCache(Func<T> onReevaluate)
		{
			if(onReevaluate == null) throw new ArgumentNullException("onReevaluate");
			_onReevaluate = onReevaluate;
		}

		public PersistentCache(Func<T> onReevaluate, T value)
		{
			if(onReevaluate == null) throw new ArgumentNullException("onReevaluate");
			_onReevaluate = onReevaluate;
			_value = value;
			_isCached = true;
		}

		public override bool IsCached
		{
			get { return _isCached; }
		}

		public override T Value
		{
			get
			{
				if(!IsCached)
					Reevaluate();
				return _value;
			}
		}

		protected virtual void Reevaluate()
		{
			_onReevaluate();
			_isCached = true;
		}

		public override void Invalidate()
		{
			_isCached = false;
			_value = default(T);
		}
	}
}
