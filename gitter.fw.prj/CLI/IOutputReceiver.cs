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

namespace gitter.Framework.CLI
{
	using System;
	using System.Diagnostics;
	using System.IO;

	/// <summary>Receives output from stderr/stdout.</summary>
	public interface IOutputReceiver
	{
		/// <summary>Gets a value indicating whether this instance is initialized.</summary>
		/// <value><c>true</c> if this instance is initialized; otherwise, <c>false</c>.</value>
		bool IsInitialized { get; }

		/// <summary>Initializes output reader.</summary>
		/// <param name="process">Process to read from.</param>
		/// <param name="reader">StreamReader to read from.</param>
		void Initialize(Process process, StreamReader reader);

		/// <summary>Notifies receiver that output is no longer required.</summary>
		/// <remarks>Reader should still receive bytes, but disable any stream processing.</remarks>
		void NotifyCanceled();

		/// <summary>Waits for outpuit stream end.</summary>
		void WaitForEndOfStream();
	}
}
