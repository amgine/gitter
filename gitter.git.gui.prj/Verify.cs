#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Globalization;

	using gitter.Git;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>
	/// A static class for retail validated assertions.
	/// Instead of breaking into the debugger an exception is thrown.
	/// </summary>
	internal static partial class Verify
	{
		/// <summary>Argument verification methods.</summary>
		public static partial class Argument
		{
			/// <summary>Checks if supplied git object is valid.</summary>
			/// <param name="gitObject">Git object to validate.</param>
			/// <param name="repository">Repository associated with caller.</param>
			/// <param name="parameterName">The name of the parameter that will be presented if an exception is thrown.</param>
			[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
			[DebuggerStepThrough]
			#if FW_4_5
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			#endif
			public static void IsValidGitObject(GitNamedObjectWithLifetime gitObject, Repository repository, string parameterName)
			{
				Assert.IsNotNull(repository);
				Assert.IsNeitherNullNorWhitespace(parameterName);

				if(gitObject == null)
				{
					throw new ArgumentNullException(parameterName);
				}
				if(gitObject.Repository != repository)
				{
					throw new ArgumentException(
						string.Format(
							CultureInfo.InvariantCulture,
							Resources.ExcSuppliedObjectIsNotHandledByThisRepository,
							gitObject.GetType().Name),
						parameterName);
				}
				if(gitObject.IsDeleted)
				{
					throw new ArgumentException(
						string.Format(
							CultureInfo.InvariantCulture,
							Resources.ExcSuppliedObjectIsDeleted,
							gitObject.GetType().Name),
						parameterName);
				}
			}

			/// <summary>Checks if supplied git object is valid.</summary>
			/// <param name="gitObject">Git object to validate.</param>
			/// <param name="repository">Repository associated with caller.</param>
			/// <param name="parameterName">The name of the parameter that will be presented if an exception is thrown.</param>
			[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
			[DebuggerStepThrough]
			#if FW_4_5
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			#endif
			public static void IsValidGitObject(GitNamedObjectWithLifetime gitObject, string parameterName)
			{
				Assert.IsNeitherNullNorWhitespace(parameterName);

				if(gitObject == null)
				{
					throw new ArgumentNullException(parameterName);
				}
				if(gitObject.IsDeleted)
				{
					throw new ArgumentException(
						string.Format(
							CultureInfo.InvariantCulture,
							Resources.ExcSuppliedObjectIsDeleted,
							gitObject.GetType().Name),
						parameterName);
				}
			}

			/// <summary>Checks if supplied revision pointer is valid.</summary>
			/// <param name="revision">Revision pointer to validate.</param>
			/// <param name="repository">Repository associated with caller.</param>
			/// <param name="parameterName">The name of the parameter that will be presented if an exception is thrown.</param>
			[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
			[DebuggerStepThrough]
			#if FW_4_5
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			#endif
			public static void IsValidRevisionPointer(IRevisionPointer revision, Repository repository, string parameterName)
			{
				Assert.IsNotNull(repository);
				Assert.IsNeitherNullNorWhitespace(parameterName);

				if(revision == null)
				{
					throw new ArgumentNullException(parameterName);
				}
				if(revision.Repository != repository)
				{
					throw new ArgumentException(
						string.Format(
							CultureInfo.InvariantCulture,
							Resources.ExcSuppliedObjectIsNotHandledByThisRepository,
							revision.GetType().Name),
						parameterName);
				}
				if(revision.IsDeleted)
				{
					throw new ArgumentException(
						string.Format(
							CultureInfo.InvariantCulture,
							Resources.ExcSuppliedObjectIsDeleted,
							revision.GetType().Name),
						parameterName);
				}
				Assert.IsNotNull(revision.Repository);
			}

			/// <summary>Checks if supplied revision pointer is valid.</summary>
			/// <param name="revision">Revision pointer to validate.</param>
			/// <param name="parameterName">The name of the parameter that will be presented if an exception is thrown.</param>
			[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
			[DebuggerStepThrough]
			#if FW_4_5
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			#endif
			public static void IsValidRevisionPointer(IRevisionPointer revision, string parameterName)
			{
				Assert.IsNeitherNullNorWhitespace(parameterName);

				if(revision == null)
				{
					throw new ArgumentNullException(parameterName);
				}
				if(revision.IsDeleted)
				{
					throw new ArgumentException(
						string.Format(
							CultureInfo.InvariantCulture,
							Resources.ExcSuppliedObjectIsDeleted,
							revision.GetType().Name),
						parameterName);
				}
				Assert.IsNotNull(revision.Repository);
			}

			/// <summary>Checks if 2 supplied revision pointers are valid.</summary>
			/// <param name="revision1">First pointer.</param>
			/// <param name="revision2">Second pointer.</param>
			/// <param name="argName1">The name of the first pointer parameter that will be presented if an exception is thrown.</param>
			/// <param name="argName2">The name of the second pointer parameter that will be presented if an exception is thrown.</param>
			[DebuggerStepThrough]
			public static void AreValidRevisionPointers(
				IRevisionPointer revision1, IRevisionPointer revision2,
				string argName1 = "revision1", string argName2 = "revision2")
			{
				Verify.Argument.IsValidRevisionPointer(revision1, argName1);
				Verify.Argument.IsValidRevisionPointer(revision2, argName2);
				Verify.Argument.IsTrue(revision2.Repository == revision1.Repository, argName2,
					Resources.ExcAllObjectsMustBeHandledByThisRepository.UseAsFormat("revisions"));
			}

			/// <summary>Checks if supplied revision pointer is valid.</summary>
			/// <param name="revisions">Pointer sequence to validate.</param>
			/// <param name="repository">Repository associated with caller.</param>
			/// <param name="parameterName">The name of the parameter that will be presented if an exception is thrown.</param>
			[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
			[DebuggerStepThrough]
			#if FW_4_5
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			#endif
			public static void IsValidRevisionPointerSequence(
				IEnumerable<IRevisionPointer> revisions, Repository repository, string parameterName)
			{
				Assert.IsNotNull(repository);
				Assert.IsNeitherNullNorWhitespace(parameterName);

				if(revisions == null)
				{
					throw new ArgumentNullException(parameterName);
				}
				foreach(var revision in revisions)
				{
					if(revision == null)
					{
						throw new ArgumentException(
							Resources.ExcCollectionMustNotContainNullElements,
							parameterName);
					}
					if(revision.Repository != repository)
					{
						throw new ArgumentException(
							string.Format(
								CultureInfo.InvariantCulture,
								Resources.ExcAllObjectsMustBeHandledByThisRepository,
								parameterName),
							parameterName);
					}
					if(revision.IsDeleted)
					{
						throw new ArgumentException(
							string.Format(
								CultureInfo.InvariantCulture,
								Resources.ExcAtLeastOneOfSuppliedObjectIsDeleted,
								parameterName),
							parameterName);
					}
				}
			}

			/// <summary>Checks if supplied revision pointer is valid.</summary>
			/// <param name="revisions">Pointer sequence to validate.</param>
			/// <param name="parameterName">The name of the parameter that will be presented if an exception is thrown.</param>
			[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
			[DebuggerStepThrough]
			#if FW_4_5
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			#endif
			public static void IsValidRevisionPointerSequence(
				IEnumerable<IRevisionPointer> revisions, string parameterName)
			{
				Assert.IsNeitherNullNorWhitespace(parameterName);

				Repository repository = null;
				if(revisions == null)
				{
					throw new ArgumentNullException(parameterName);
				}
				foreach(var revision in revisions)
				{
					if(revision == null)
					{
						throw new ArgumentException(
							Resources.ExcCollectionMustNotContainNullElements,
							parameterName);
					}
					if(repository == null)
					{
						repository = revision.Repository;
					}
					else
					{
						if(revision.Repository != repository)
						{
							throw new ArgumentException(
								string.Format(
									CultureInfo.InvariantCulture,
									Resources.ExcAllObjectsMustBeHandledByThisRepository,
									parameterName),
								parameterName);
						}
					}
					if(revision.IsDeleted)
					{
						throw new ArgumentException(
							string.Format(
								CultureInfo.InvariantCulture,
								Resources.ExcAtLeastOneOfSuppliedObjectIsDeleted,
								parameterName),
							parameterName);
					}
				}
			}

			/// <summary>Validates the reference name.</summary>
			/// <param name="referenceName">Reference name.</param>
			/// <param name="referenceType">Reference type.</param>
			/// <param name="parameterName">The name of the parameter that will be presented if an exception is thrown.</param>
			/// <exception cref="ArgumentNullException">
			/// <paramref name="referenceName"/> is <c>null</c>.
			/// </exception>
			/// <exception cref="ArgumentException">
			/// <paramref name="referenceName"/> is not a valid reference name.
			/// </exception>
			[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
			[DebuggerStepThrough]
			#if FW_4_5
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			#endif
			public static void IsValidReferenceName(string referenceName, ReferenceType referenceType, string parameterName)
			{
				Assert.IsNeitherNullNorWhitespace(parameterName);

				if(referenceName == null)
				{
					throw new ArgumentNullException(parameterName);
				}
				string msg;
				if(!Reference.ValidateName(referenceName, referenceType, out msg))
				{
					throw new ArgumentException(msg, parameterName);
				}
			}

			/// <summary>Validates SHA1 hash.</summary>
			/// <param name="value">SHA1 hash to validate.</param>
			/// <param name="parameterName">The name of the parameter that will be presented if an exception is thrown.</param>
			/// <exception cref="ArgumentNullException">
			/// <paramref name="value"/> is <c>null</c>.
			/// </exception>
			/// <exception cref="ArgumentException">
			/// <paramref name="parameterName"/> is not a valid SHA1 hash.
			/// </exception>
			[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
			[DebuggerStepThrough]
			#if FW_4_5
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			#endif
			public static void IsValidSHA1(string value, string parameterName)
			{
				Assert.IsNeitherNullNorWhitespace(parameterName);

				const int SHA1_LENGTH = 40;
				if(value == null)
				{
					throw new ArgumentNullException(parameterName);
				}
				if(value.Length != SHA1_LENGTH)
				{
					throw new ArgumentException(
						string.Format(
							CultureInfo.InvariantCulture,
							"String representation of SHA1 hash must be {0} characters long.",
							SHA1_LENGTH),
						parameterName);
				}
				for(int i = 0; i < SHA1_LENGTH; ++i)
				{
					if(!Uri.IsHexDigit(value[i]))
					{
						throw new ArgumentException(
							string.Format(
								CultureInfo.InvariantCulture,
								"Invalid SHA1 (unexpected character '{0}' at position {1}).",
								value[i], i),
							parameterName);
					}
				}
			}
		}
	}
}
