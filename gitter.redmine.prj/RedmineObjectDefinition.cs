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

namespace gitter.Redmine
{
	using System;
	using System.Collections.Generic;
	using System.Xml;

	public abstract class RedmineObjectDefinition<T>
		where T : RedmineObject
	{
		protected RedmineObjectDefinition()
		{
		}

		protected static void EmitIfChanged<TValue>(TValue original, TValue current, XmlDocument doc, XmlElement root, string name, Action<XmlElement, TValue> emit)
		{
			if(!EqualityComparer<TValue>.Default.Equals(original, current))
			{
				var e = doc.CreateElement(name);
				emit(e, current);
			}
		}

		public bool IsCommitted { get; private set; }

		protected abstract void ResetCore();

		protected abstract void CommitCore();

		public void Reset()
		{
			if(IsCommitted) throw new InvalidOperationException();

			ResetCore();
		}

		public void Commit()
		{
			if(IsCommitted) throw new InvalidOperationException();

			CommitCore();
			IsCommitted = true;
		}
	}
}
