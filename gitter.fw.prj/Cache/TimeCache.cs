namespace gitter.Framework
{
	using System;

	public class TimeCache<T> : PersistentCache<T>
	{
		#region Data

		private TimeSpan _lifetime;
		private DateTime _cacheTime;

		#endregion

		public TimeCache(Func<T> onReevaluate, TimeSpan lifetime)
			: base(onReevaluate)
		{
			Verify.Argument.IsNotNull(onReevaluate, "onReevaluate");

			_lifetime = lifetime;
		}

		public TimeCache(Func<T> onReevaluate, TimeSpan lifetime, T value)
			: base(onReevaluate, value)
		{
			_cacheTime = DateTime.Now;
		}

		public override bool IsCached
		{
			get { return base.IsCached && (DateTime.Now - _cacheTime < _lifetime); }
		}

		protected override void Reevaluate()
		{
			base.Reevaluate();
			_cacheTime = DateTime.Now;
		}
	}
}
