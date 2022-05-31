#region Copyright Notice
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

namespace gitter.Git.Gui;

using System;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;

using Resources = gitter.Git.Gui.Properties.Resources;

partial class RevisionHeaderContent
{
	sealed class BodyElement : TextWithHyperlinksElementBase
	{
		public BodyElement(RevisionHeaderContent owner)
			: base(owner, Resources.StrBody.AddColon())
		{
		}

		public override bool IsAvailableFor(Revision revision)
		{
			return !string.IsNullOrEmpty(revision.Body);
		}

		public override Element Element => Element.Body;

		protected override string GetText(Revision revision) => revision.Body;
	}
}
