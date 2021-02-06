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

namespace gitter.GitLab.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Globalization;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using gitter.GitLab.Api;

	using Resources = gitter.GitLab.Properties.Resources;

	sealed class PipelineListItem : CustomListBoxItem<Pipeline>
	{
		private static int CompareString(string data1, string data2)
		{
			if(data1 == data2) return 0;
			if(data1 == null) return 1;
			else if(data2 == null) return -1;
			return string.Compare(data1, data2);
		}

		public static int CompareById(PipelineListItem item1, PipelineListItem item2)
		{
			var data1 = item1.DataContext.Id;
			var data2 = item2.DataContext.Id;
			return data1.CompareTo(data2);
		}

		public static int CompareById(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			if(item1 is not PipelineListItem i1) return 0;
			if(item2 is not PipelineListItem i2) return 0;
			return CompareById(i1, i2);
		}

		public static int CompareByCreatedAt(PipelineListItem item1, PipelineListItem item2)
		{
			var data1 = item1.DataContext.CreatedAt;
			var data2 = item2.DataContext.CreatedAt;
			if(!data1.HasValue || !data2.HasValue) return 0;
			return data1.Value.CompareTo(data2.Value);
		}

		public static int CompareByCreatedAt(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			if(item1 is not PipelineListItem i1) return 0;
			if(item2 is not PipelineListItem i2) return 0;
			return CompareByCreatedAt(i1, i2);
		}

		public static int CompareByUpdatedAt(PipelineListItem item1, PipelineListItem item2)
		{
			var data1 = item1.DataContext.UpdatedAt;
			var data2 = item2.DataContext.UpdatedAt;
			if(!data1.HasValue || !data2.HasValue) return 0;
			return data1.Value.CompareTo(data2.Value);
		}

		public static int CompareByUpdatedAt(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			if(item1 is not PipelineListItem i1) return 0;
			if(item2 is not PipelineListItem i2) return 0;
			return CompareByUpdatedAt(i1, i2);
		}

		public static int CompareByRef(PipelineListItem item1, PipelineListItem item2)
			=> CompareString(item1.DataContext.Ref, item2.DataContext.Ref);

		public static int CompareByRef(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			if(item1 is not PipelineListItem i1) return 0;
			if(item2 is not PipelineListItem i2) return 0;
			return CompareByRef(i1, i2);
		}

		public static int CompareByHash(PipelineListItem item1, PipelineListItem item2)
			=> CompareString(item1.DataContext.Sha, item2.DataContext.Sha);

		public static int CompareByHash(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			if(item1 is not PipelineListItem i1) return 0;
			if(item2 is not PipelineListItem i2) return 0;
			return CompareByHash(i1, i2);
		}

		public static int CompareByStatus(PipelineListItem item1, PipelineListItem item2)
		{
			var data1 = item1.DataContext.Status;
			var data2 = item2.DataContext.Status;
			return ((int)data1).CompareTo((int)data2);
		}

		public static int CompareByStatus(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			if(item1 is not PipelineListItem i1) return 0;
			if(item2 is not PipelineListItem i2) return 0;
			return CompareByStatus(i1, i2);
		}

		public PipelineListItem(Pipeline pipeline)
			: base(pipeline)
		{
			Verify.Argument.IsNotNull(pipeline, nameof(pipeline));
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var menu = new HyperlinkContextMenu(DataContext.WebUrl.ToString());
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}
	}
}
