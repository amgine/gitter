namespace gitter.Framework.Configuration
{
	using System;

	static class TypeHelpers
	{
		public static Type GetType<T>(T value)
		{
			if(value != null) return value.GetType();
			return typeof(T);
		}

		public static Type GetType(Type type, object value)
		{
			if(value != null) return value.GetType();
			return type;
		}

		public static T UnpackValue<T>(object value)
		{
			if(value == null) return default(T);
			return (T)value;
		}

		public static object PackValue<T>(T value)
		{
			if(value == null) return null;
			return value;
		}
	}
}
