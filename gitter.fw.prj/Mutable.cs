namespace gitter.Framework
{
	using System;

	public sealed class Mutable<T>
	{
		private T _value;

		public Mutable()
		{
		}

		public Mutable(T value)
		{
			_value = value;
		}

		public T Value
		{
			get { return _value; }
			set
			{
				_value = value;
			}
		}

		public static implicit operator Mutable<T>(T value)
		{
			return new Mutable<T>(value);
		}

		public static implicit operator T(Mutable<T> mutable)
		{
			if(mutable == null) return default(T);
			return mutable._value;
		}
	}
}
