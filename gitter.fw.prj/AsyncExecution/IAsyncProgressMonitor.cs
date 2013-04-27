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

namespace gitter.Framework
{
	using System;
	using System.Windows.Forms;

	/// <summary>Object used to track progress of async action.</summary>
	public interface IAsyncProgressMonitor
	{
		#region Events

		/// <summary>Monitor raises this event to stop monitored action execution.</summary>
		event EventHandler Canceled;

		/// <summary>Monitor raises this event when it is ready to receive Set() calls after Start() call.</summary>
		event EventHandler Started;

		#endregion

		#region Properties

		/// <summary>Executing action.</summary>
		IAsyncResult CurrentContext { get; }

		/// <summary>Async action name.</summary>
		string ActionName { get; set; }

		/// <summary>Determines if action can be cancelled.</summary>
		bool CanCancel { get; set; }

		/// <summary>Returns <c>true</c> if operation must be terminated.</summary>
		bool IsCancelRequested { get; }

		#endregion

		#region Start()

		/// <summary>Starts monitor.</summary>
		/// <param name="parent">Reference to parent window which is related to executing action.</param>
		/// <param name="context">Execution context.</param>
		/// <param name="blocking">Block calling thread until ProcessCompleted() is called.</param>
		void Start(IWin32Window parent, IAsyncResult context, bool blocking);

		#endregion

		#region Set()

		/// <summary>Sets the name of currently executed step.</summary>
		/// <param name="action">Step name.</param>
		void SetAction(string action);

		/// <summary>Sets progress range information.</summary>
		/// <param name="min">Minimum.</param>
		/// <param name="max">Maximum.</param>
		void SetProgressRange(int min, int max);

		/// <summary>Sets progress range information and current step name.</summary>
		/// <param name="min">Minimum.</param>
		/// <param name="max">Maximum.</param>
		/// <param name="action">Step name.</param>
		void SetProgressRange(int min, int max, string action);

		/// <summary>Sets current progress.</summary>
		/// <param name="val">Progress.</param>
		void SetProgress(int val);

		/// <summary>Sets current progress and step name.</summary>
		/// <param name="val">Progress.</param>
		/// <param name="action">Step name.</param>
		void SetProgress(int val, string action);

		/// <summary>Sets progress as unknown.</summary>
		void SetProgressIndeterminate();

		#endregion

		/// <summary>Notifies that action is completed or cancelled and monitor must be shut down.</summary>
		void ProcessCompleted();
	}
}
