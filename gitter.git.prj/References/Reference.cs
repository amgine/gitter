#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Symbolic reference.</summary>
	public class Reference : GitNamedObjectWithLifetime, IRevisionPointer
	{
		#region Data

		/// <summary>Object pointed by this <see cref="Reference"/>.</summary>
		private IRevisionPointer _pointer;
		/// <summary><see cref="WeakReference"/> to this <see cref="Reference"/>'s <see cref="Reflog"/>.</summary>
		private WeakReference _reflogRef;
		/// <summary>Reflog access sync object.</summary>
		private readonly object _reflogSync;

		#endregion

		#region Events

		/// <summary>Reference changed pointer.</summary>
		public event EventHandler<RevisionPointerChangedEventArgs> PointerChanged;

		/// <summary>Reference points to another <see cref="Revision"/>.</summary>
		public event EventHandler<RevisionChangedEventArgs> PositionChanged;

		/// <summary>Invoke <see cref="PointerChanged"/>.</summary>
		private void InvokePointerChanged(IRevisionPointer oldPos, IRevisionPointer newPos)
		{
			var handler = PointerChanged;
			if(handler != null) handler(this, new RevisionPointerChangedEventArgs(oldPos, newPos));
		}

		/// <summary>Invoke <see cref="PositionChanged"/>.</summary>
		protected void InvokePositionChanged(Revision oldPos, Revision newPos)
		{
			var handler = PositionChanged;
			if(handler != null) handler(this, new RevisionChangedEventArgs(oldPos, newPos));
		}

		#endregion

		#region Nested Types

		/// <summary>Reference to a reflog record.</summary>
		private sealed class ReflogReference : IRevisionPointer
		{
			#region Data

			/// <summary>Reflog selector.</summary>
			private readonly string _reflogSelector;
			/// <summary>Owner reference.</summary>
			private readonly Reference _reference;

			#endregion

			/// <summary>Initializes a new instance of the <see cref="ReflogReference"/> class.</summary>
			/// <param name="reference">Owner reference.</param>
			/// <param name="reflogSelector">Reflog selector.</param>
			public ReflogReference(Reference reference, string reflogSelector)
			{
				Verify.Argument.IsNotNull(reference, "reference");
				Verify.Argument.IsNotNull(reflogSelector, "reflogSelector");

				_reference = reference;
				_reflogSelector = reflogSelector;
			}

			/// <summary>Gets the reflog selector.</summary>
			/// <value>Reflog selector.</value>
			public string ReflogSelector
			{
				get { return _reflogSelector; }
			}

			/// <summary>Gets the host repository.</summary>
			/// <value>Host repository</value>
			/// <remarks>Never returns <c>null</c>.</remarks>
			public Repository Repository
			{
				get { return _reference.Repository; }
			}

			/// <summary><see cref="ReferenceType"/>.</summary>
			/// <value><see cref="ReferenceType.ReflogRecord"/>.</value>
			public ReferenceType Type
			{
				get { return ReferenceType.ReflogRecord; }
			}

			/// <summary>
			/// Revision expression (reference name, sha1, relative expression, etc.).
			/// </summary>
			public string Pointer
			{
				get { return _reference.Name + "@{" + _reflogSelector + "}"; }
			}

			/// <summary>
			/// Returns full non-ambiguous revision name.
			/// </summary>
			public string FullName
			{
				get { return _reference.FullName + "@{" + _reflogSelector + "}"; ; }
			}

			/// <summary>
			/// Evaluate commit which is targeted by this <see cref="IRevisionPointer"/>.
			/// </summary>
			/// <returns>
			/// Commit which is pointed by this <see cref="IRevisionPointer"/>.
			/// </returns>
			public Revision Dereference()
			{
				var revisionData = _reference.Repository.Accessor.Dereference.Invoke(
					new DereferenceParameters(FullName)
					{
						LoadRevisionData = true,
					});
				return ObjectFactories.CreateRevision(_reference.Repository, revisionData);
			}

			/// <summary>
			/// Object is deleted and not valid anymore.
			/// </summary>
			public bool IsDeleted
			{
				get { return false; }
			}
		}

		#endregion

		#region Static

		public static string GetReferenceTypeName(ReferenceType referenceType)
		{
			switch(referenceType)
			{
				case ReferenceType.Branch:
					return Resources.StrBranch;
				case ReferenceType.Tag:
					return Resources.StrTag;
				case ReferenceType.RemoteBranch:
					return Resources.StrBranch;
				case ReferenceType.Remote:
					return Resources.StrRemote;
				default:
					return string.Empty;
			}
		}

		/// <summary>Validates the reference name.</summary>
		/// <param name="name">Reference name.</param>
		/// <param name="referenceType">Reference type.</param>
		/// <param name="errorMessage">Error message.</param>
		/// <returns><c>true</c> if <paramref name="name"/> is a valid reference name; otherwise, <c>false</c>.</returns>
		public static bool ValidateName(string name, ReferenceType referenceType, out string errorMessage)
		{
			/*
			   1. They can include slash / for hierarchical (directory) grouping, but no slash-separated component can begin with a dot ..
			   2. They must contain at least one /. This enforces the presence of a category like heads/, tags/ etc. but the actual names are not restricted.
			   3. They cannot have two consecutive dots .. anywhere.
			   4. They cannot have ASCII control characters (i.e. bytes whose values are lower than \040, or \177 DEL), space, tilde ~, caret ^, colon :, question-mark ?, asterisk *, or open bracket [ anywhere.
			   5. They cannot end with a slash / nor a dot ..
			   6. They cannot end with the sequence .lock.
			   7. They cannot contain a sequence @{.
			   8. They cannot contain a \\.
			*/
			if(string.IsNullOrWhiteSpace(name))
			{
				errorMessage = string.Format(CultureInfo.InvariantCulture,
					Resources.ErrNameCannotBeEmpty, GetReferenceTypeName(referenceType));
				return false;
			}
			name = name.Trim();
			if(name[0] == '-')
			{
				errorMessage = string.Format(CultureInfo.InvariantCulture,
					Resources.ErrNameCannotBeginWithCharacter, GetReferenceTypeName(referenceType), "-");
				return false;
			}
			for(int i = 0; i < name.Length; ++i)
			{
				bool lastchar = i == name.Length - 1;
				var c = name[i];
				if(c < 32 || c == 127)
				{
					errorMessage = string.Format(CultureInfo.InvariantCulture,
						Resources.ErrNameCannotContainASCIIControlCharacters, GetReferenceTypeName(referenceType));
					return false;
				}
				switch(c)
				{
					case '/':
						if(i == 0)
						{
							errorMessage = string.Format(CultureInfo.InvariantCulture,
								Resources.ErrNameCannotBeginWithCharacter, GetReferenceTypeName(referenceType), "/");
							return false;
						}
						if(lastchar)
						{
							errorMessage = string.Format(CultureInfo.InvariantCulture,
								Resources.ErrNameCannotEndWithCharacter, GetReferenceTypeName(referenceType), "/");
							return false;
						}
						if(name[i + 1] == '.')
						{
							errorMessage = string.Format(CultureInfo.InvariantCulture,
								Resources.ErrSlashSeparatedComponentCannotBeginWithCharacter, ".");
							return false;
						}
						if(name[i + 1] == '/')
						{
							errorMessage = string.Format(CultureInfo.InvariantCulture,
								Resources.ErrNameCannotContainSequence, GetReferenceTypeName(referenceType), "//");
							return false;
						}
						break;
					case '.':
						if(i == 0)
						{
							errorMessage = string.Format(CultureInfo.InvariantCulture,
								Resources.ErrNameCannotBeginWithCharacter, GetReferenceTypeName(referenceType), ".");
							return false;
						}
						if(lastchar)
						{
							errorMessage = string.Format(CultureInfo.InvariantCulture,
								Resources.ErrNameCannotEndWithCharacter, GetReferenceTypeName(referenceType), ".");
							return false;
						}
						if(!lastchar && name[i + 1] == '.')
						{
							errorMessage = string.Format(CultureInfo.InvariantCulture,
								Resources.ErrNameCannotContainSequence, GetReferenceTypeName(referenceType), "..");
							return false;
						}
						if(i == name.Length - 5 && name.IndexOf("lock", i + 1, 4) != -1)
						{
							errorMessage = string.Format(CultureInfo.InvariantCulture,
								Resources.ErrNameCannotEndWithSequence, GetReferenceTypeName(referenceType), ".lock");
							return false;
						}
						break;
					case '@':
						if(!lastchar && (name[i + 1] == '{'))
						{
							errorMessage = string.Format(CultureInfo.InvariantCulture,
								Resources.ErrNameCannotContainSequence, GetReferenceTypeName(referenceType), "@{");
							return false;
						}
						break;
					case ' ':
					case '\\':
					case '~':
					case '^':
					case ':':
					case '?':
					case '*':
					case '[':
						errorMessage = string.Format(CultureInfo.InvariantCulture,
							Resources.ErrNameCannotContainCharacter, referenceType, c);
						return false;
				}
			}
			errorMessage = string.Empty;
			return true;
		}

		#endregion

		#region .ctor

		/// <summary>Create <see cref="Reference"/>.</summary>
		/// <param name="repository">Host <see cref="Repository"/>.</param>
		/// <param name="name">Reference name.</param>
		/// <param name="pointer">Referenced object.</param>
		internal Reference(Repository repository, string name, IRevisionPointer pointer)
			: base(repository, name)
		{
			Verify.Argument.IsNotNull(pointer, "pointer");

			_pointer = PrepareInputPointer(pointer);
			_reflogSync = new object();
			EnterPointer(_pointer);
			var rev = _pointer.Dereference();
			if(rev != null)
			{
				EnterRevision(_pointer.Dereference());
			}
		}

		#endregion

		/// <summary>Returns object pointed by this <see cref="Reference"/>.</summary>
		public IRevisionPointer Pointer
		{
			get { return _pointer; }
			internal set
			{
				Verify.Argument.IsNotNull(value, "value");

				var newPointer = PrepareInputPointer(value);
				if(_pointer != newPointer)
				{
					var oldPointer = _pointer;

					var oldPos = oldPointer.Dereference();
					var newPos = newPointer.Dereference();

					if(oldPos != null && oldPos != newPos)
					{
						LeaveRevision(oldPos);
					}
					
					LeavePointer(oldPointer);
					_pointer = newPointer;
					EnterPointer(newPointer);
					InvokePointerChanged(oldPointer, newPointer);

					if(newPos != null && oldPos != newPos)
					{
						EnterRevision(newPos);
					}

					if(oldPos != newPos)
					{
						InvokePositionChanged(oldPos, newPos);
					}
				}
			}
		}

		/// <summary>Get a reference to reflog record.</summary>
		/// <param name="index">Reflog record index.</param>
		/// <value>Pointer to reflog record.</value>
		/// <returns>Pointer to reflog record.</returns>
		public IRevisionPointer this[int index]
		{
			get
			{
				Verify.Argument.IsNotNegative(index, "index");

				if(index == 0) return this;
				return new ReflogReference(this, index.ToString(CultureInfo.InvariantCulture));
			}
		}

		/// <summary>Gets the <see cref="Revision"/>, pointed by this <see cref="Reference"/>.</summary>
		/// <value><see cref="Revision"/>, pointed by this <see cref="Reference"/>.</value>
		public Revision Revision
		{
			get { return _pointer.Dereference(); }
		}

		/// <summary>Gets this <see cref="Reference"/>'s reflog - history of pointer modifications.</summary>
		/// <value>Reflog of this <see cref="Reference"/>.</value>
		public Reflog Reflog
		{
			get
			{
				Reflog reflog;
				lock(_reflogSync)
				{
					if(_reflogRef == null)
					{
						reflog = new Reflog(this);
						_reflogRef = new WeakReference(reflog);
					}
					else
					{
						var obj = _reflogRef.Target;
						if(obj == null)
						{
							reflog = new Reflog(this);
							_reflogRef = new WeakReference(reflog);
						}
						else
						{
							reflog = (Reflog)obj;
						}
					}
				}
				return reflog;
			}
		}

		/// <summary>Filter <see cref="IRevisionPointer"/> to types supported by this <see cref="Reference"/>.</summary>
		/// <param name="pointer">Raw pointer.</param>
		/// <returns>Valid pointer.</returns>
		protected virtual IRevisionPointer PrepareInputPointer(IRevisionPointer pointer)
		{
			return pointer;
		}

		/// <summary>Called when this <see cref="Reference"/> is moved away from <paramref name="pointer"/>.</summary>
		/// <param name="pointer">Object, which this <see cref="Reference"/> was pointing to.</param>
		protected virtual void LeavePointer(IRevisionPointer pointer)
		{
		}

		/// <summary>Called when this <see cref="Reference"/> is moved to <paramref name="pointer"/>.</summary>
		/// <param name="pointer">Object, which this <see cref="Reference"/> will be pointing to.</param>
		protected virtual void EnterPointer(IRevisionPointer pointer)
		{
		}

		/// <summary>Called when this <see cref="Reference"/> is moved away from <paramref name="revision"/>.</summary>
		/// <param name="pointer"><see cref="Revision"/>, which this <see cref="Reference"/> was pointing to.</param>
		protected virtual void LeaveRevision(Revision revision)
		{
			revision.References.Remove(this);
		}

		/// <summary>Called when this <see cref="Reference"/> is moved to <paramref name="Revision"/>.</summary>
		/// <param name="pointer"><see cref="Revision"/>, which this <see cref="Reference"/> will be pointing to.</param>
		protected virtual void EnterRevision(Revision revision)
		{
			revision.References.Add(this);
		}

		/// <summary>Called after marking this <see cref="Reference"/> as deleted.</summary>
		protected override void OnDeleted()
		{
			var rev = _pointer.Dereference();
			LeavePointer(_pointer);
			if(rev != null)
			{
				LeaveRevision(rev);
			}
		}

		/// <summary>Notifies about reflog modification.</summary>
		internal virtual void NotifyRelogRecordAdded()
		{
			lock(_reflogSync)
			{
				if(_reflogRef != null)
				{
					var obj = _reflogRef.Target;
					if(obj == null)
					{
						_reflogRef = null;
					}
					else
					{
						var reflog = (Reflog)obj;
						reflog.NotifyRecordAdded();
					}
				}
			}
		}

		/// <summary><see cref="ReferenceType"/>.</summary>
		/// <value>Reference type.</value>
		public virtual ReferenceType Type
		{
			get { return ReferenceType.Reference; }
		}

		/// <summary>Gets the full reference name.</summary>
		/// <value>Full reference name.</value>
		public virtual string FullName
		{
			get { return Name; }
		}

		/// <summary>
		/// Returns revision expression (reference name, sha1, relative expression, etc.).
		/// </summary>
		/// <value>Revision expression.</value>
		string IRevisionPointer.Pointer
		{
			get { return Name; }
		}

		/// <summary>
		/// Evaluate commit which is targeted by this <see cref="IRevisionPointer"/>.
		/// </summary>
		/// <returns>
		/// Commit which is pointed by this <see cref="IRevisionPointer"/>.
		/// </returns>
		Revision IRevisionPointer.Dereference()
		{
			return _pointer.Dereference();
		}
	}
}
