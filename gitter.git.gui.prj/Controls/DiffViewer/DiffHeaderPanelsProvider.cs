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

namespace gitter.Git.Gui.Controls;

using System;
using System.Collections.Generic;

using gitter.Framework.Controls;

public static class DiffHeaderPanelsProvider
{
	public static event EventHandler<CreatingPanelsEventArgs>? CreatingPanels;

	internal static IReadOnlyList<FlowPanel> GetSourceSpecificPanels(IDiffSource diffSource)
	{
		Assert.IsNotNull(diffSource);

		var panels = new List<FlowPanel>();
		switch(diffSource)
		{
			case IRevisionDiffSource revisionSource:
				panels.Add(new RevisionHeaderPanel { Revision = revisionSource.Revision.Dereference() });
				panels.Add(new FlowPanelSeparator  { SeparatorStyle = FlowPanelSeparatorStyle.Line });
				break;
			case IIndexDiffSource indexSource when !indexSource.Cached:
				var panel = new UntrackedFilesPanel(indexSource.Repository.Status);
				if(panel.Count != 0)
				{
					panels.Add(panel);
					panels.Add(new FlowPanelSeparator { Height = 5 });
				}
				break;
		}
		CreatingPanels?.Invoke(null, new CreatingPanelsEventArgs(diffSource, panels));
		return panels;
	}
}
