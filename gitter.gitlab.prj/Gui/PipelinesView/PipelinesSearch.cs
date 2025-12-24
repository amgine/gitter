#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.GitLab.Gui;

using System;
using System.Globalization;

using gitter.Framework.Controls;

class PipelinesSearch(CustomListBox listBox) : ListBoxSearch<PipelinesSearchOptions>(listBox)
{
	private static bool TestPipeline(Api.Pipeline pipeline, PipelinesSearchOptions search)
	{
		Assert.IsNotNull(pipeline);
		Assert.IsNotNull(search);

		if(TestString(pipeline.Ref, search)) return true;
		if(TestString(pipeline.Sha, search)) return true;

		return false;
	}

	protected override bool TestItem(CustomListBoxItem item, PipelinesSearchOptions search)
	{
		Assert.IsNotNull(item);
		Assert.IsNotNull(search);

		return item is PipelineListItem pli && TestPipeline(pli.DataContext, search);
	}
}
