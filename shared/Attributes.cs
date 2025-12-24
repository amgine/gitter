#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

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
	internal class IsExternalInit;

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	internal sealed class ModuleInitializerAttribute : Attribute;
}
#endif

#if !NET5_0_OR_GREATER
namespace System.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
	internal sealed class NotNullAttribute : Attribute;

	/// <summary>Specifies that when a method returns <see cref="ReturnValue"/>, the parameter may be null even if the corresponding type disallows it.</summary>
	[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
	internal sealed class MaybeNullWhenAttribute : Attribute
	{
		/// <summary>Initializes the attribute with the specified return value condition.</summary>
		/// <param name="returnValue">
		/// The return value condition. If the method returns this value, the associated parameter may be null.
		/// </param>
		public MaybeNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;

		/// <summary>Gets the return value condition.</summary>
		public bool ReturnValue { get; }
	}

	/// <summary>Specifies that when a method returns <see cref="ReturnValue"/>, the parameter will not be null even if the corresponding type allows it.</summary>
	[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
	internal sealed class NotNullWhenAttribute : Attribute
	{
		/// <summary>Initializes the attribute with the specified return value condition.</summary>
		/// <param name="returnValue">
		/// The return value condition. If the method returns this value, the associated parameter will not be null.
		/// </param>
		public NotNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;

		/// <summary>Gets the return value condition.</summary>
		public bool ReturnValue { get; }
	}

	/// <summary>Specifies that the method or property will ensure that the listed field and property members have not-null values.</summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	internal sealed class MemberNotNullAttribute : Attribute
	{
		/// <summary>Initializes the attribute with a field or property member.</summary>
		/// <param name="member">
		/// The field or property member that is promised to be not-null.
		/// </param>
		public MemberNotNullAttribute(string member) => Members = [member];

		/// <summary>Initializes the attribute with the list of field and property members.</summary>
		/// <param name="members">
		/// The list of field and property members that are promised to be not-null.
		/// </param>
		public MemberNotNullAttribute(params string[] members) => Members = members;

		/// <summary>Gets field or property member names.</summary>
		public string[] Members { get; }
	}

	/// <summary>Specifies that the method or property will ensure that the listed field and property members have not-null values when returning with the specified return value condition.</summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	internal sealed class MemberNotNullWhenAttribute : Attribute
	{
		/// <summary>Initializes the attribute with the specified return value condition and a field or property member.</summary>
		/// <param name="returnValue">
		/// The return value condition. If the method returns this value, the associated field or property member will not be null.
		/// </param>
		/// <param name="member">
		/// The field or property member that is promised to be not-null.
		/// </param>
		public MemberNotNullWhenAttribute(bool returnValue, string member)
		{
			ReturnValue = returnValue;
			Members = [member];
		}

		/// <summary>Initializes the attribute with the specified return value condition and list of field and property members.</summary>
		/// <param name="returnValue">
		/// The return value condition. If the method returns this value, the associated field and property members will not be null.
		/// </param>
		/// <param name="members">
		/// The list of field and property members that are promised to be not-null.
		/// </param>
		public MemberNotNullWhenAttribute(bool returnValue, params string[] members)
		{
			ReturnValue = returnValue;
			Members = members;
		}

		/// <summary>Gets the return value condition.</summary>
		public bool ReturnValue { get; }

		/// <summary>Gets field or property member names.</summary>
		public string[] Members { get; }
	}

	/// <summary>Specifies that the method will not return if the associated Boolean parameter is passed the specified value.</summary>
	[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
	internal sealed class DoesNotReturnIfAttribute : Attribute
	{
		/// <summary>Initializes the attribute with the specified parameter value.</summary>
		/// <param name="parameterValue">
		/// The condition parameter value. Code after the method will be considered unreachable by diagnostics if the argument to
		/// the associated parameter matches this value.
		/// </param>
		public DoesNotReturnIfAttribute(bool parameterValue) => ParameterValue = parameterValue;

		/// <summary>Gets the condition parameter value.</summary>
		public bool ParameterValue { get; }
	}
}
#endif
