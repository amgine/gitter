#if !NET5_0_OR_GREATER && CS_10_OR_GREATER
namespace System.Runtime.CompilerServices
{
	/// <summary>Allows capturing of the expressions passed to a method.</summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
	sealed class CallerArgumentExpressionAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="CallerArgumentExpressionAttribute"/> class.</summary>
		/// <param name="parameterName">The name of the targeted parameter.</param>
		public CallerArgumentExpressionAttribute(string parameterName)
		{
			ParameterName = parameterName;
		}

		/// <summary>The name of the targeted parameter.</summary>
		/// <value>The name of the targeted parameter of the <see cref="CallerArgumentExpressionAttribute"/>.</value>
		public string ParameterName { get; }
	}
}
#endif

#if !NET5_0_OR_GREATER
namespace System.Runtime.CompilerServices
{
	[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
	internal class IsExternalInit { }

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	internal sealed class ModuleInitializerAttribute : Attribute { }
}
#endif
