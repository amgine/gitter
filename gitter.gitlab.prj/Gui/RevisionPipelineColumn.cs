#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2020  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.GitLab.Gui
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework.Controls;
	using gitter.Git;
	using gitter.Git.Gui.Controls;

	class RevisionPipelineColumn : CustomListBoxColumn
	{
		private readonly Dictionary<Hash, List<Api.Pipeline>> _pipelines = new();

		public RevisionPipelineColumn()
			: base(-1000, "Pipeline", true)
		{
			Width = 80;
			//IsAvailable = false;
			//Init();
		}

		//private async void Init()
		//{
		//	foreach(var pipeline in await _api.GetPipelinesAsync())
		//	{
		//		if(!Hash.TryParse(pipeline.Sha, out var hash)) continue;

		//		if(!_pipelines.TryGetValue(hash, out var list))
		//		{
		//			list = new List<Api.Pipeline>();
		//			_pipelines.Add(hash, list);
		//		}
		//		list.Add(pipeline);
		//	}
		//}

		protected override void OnPaintSubItem(SubItemPaintEventArgs subItemPaintEventArgs)
		{
			Assert.IsNotNull(subItemPaintEventArgs);

			if(subItemPaintEventArgs.Item is not IRevisionPointerListItem revItem)
			{
				return;
			}
			var revision = revItem.RevisionPointer?.Dereference();
			if(revision == null) return;

			if(_pipelines.TryGetValue(revision.Hash, out var list) && list.Count > 0)
			{
				var pipeline = list[0];
				subItemPaintEventArgs.PaintText(pipeline.Status.ToString());
			}
		}
	}
}
