﻿#region Copyright Notice
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

namespace gitter.Framework.Controls
{
	using System;
	using System.ComponentModel;

	/// <summary>Collection of <see cref="FlowPanel"/>'s hosted in <see cref="FlowLayoutControl"/>.</summary>
	public sealed class FlowPanelCollection : SafeNotifySortedCollection<FlowPanel>
	{
		/// <summary>Create <see cref="FlowPanelCollection"/>.</summary>
		/// <param name="control">Host <see cref="FlowLayoutControl"/>.</param>
		internal FlowPanelCollection(FlowLayoutControl control)
		{
			Verify.Argument.IsNotNull(control, nameof(control));

			FlowLayoutControl = control;
		}

		/// <summary>Host <see cref="FlowLayoutControl"/>.</summary>
		public FlowLayoutControl FlowLayoutControl { get; }

		protected override ISynchronizeInvoke SynchronizeInvoke => FlowLayoutControl;

		protected override void FreeItem(FlowPanel item)
			=> item.FlowControl = null;

		protected override void AcquireItem(FlowPanel item)
			=> item.FlowControl = FlowLayoutControl;

		protected override bool VerifyItem(FlowPanel item)
			=> item != null && item.FlowControl == null;
	}
}
