﻿#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git.Gui.Controls;

using System.Drawing;

using gitter.Framework.Controls;

sealed class FakeRevisionEmailPainter : EmailPainterBase<FakeRevisionListItem>
{
	/// <inheritdoc/>
	protected override string GetEmail(FakeRevisionListItem item)
		=> item.Repository.UserIdentity.Email;

	/// <inheritdoc/>
	protected override bool TryGetTextBrush(SubItemPaintEventArgs paintEventArgs, out Brush textBrush, out bool disposeBrush)
		=> FakeRevisionPainterHelper.TryGetTextBrush(paintEventArgs, out textBrush, out disposeBrush);
}
