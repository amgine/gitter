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

namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;

	using gitter.Framework.Controls;
	using gitter.Framework.Configuration;

	using Resources = gitter.Git.Gui.Properties.Resources;
	using System.Drawing.Drawing2D;

	/// <summary>"Graph" column.</summary>
	public sealed class GraphColumn : CustomListBoxColumn
	{
		public const bool DefaultShowColors = false;

		#region Data

		private bool _showColors;
		private GraphColumnExtender _extender;

		#endregion

		#region Events

		public event EventHandler ShowColorsChanged;

		#endregion

		public GraphColumn()
			: base((int)ColumnId.Graph, Resources.StrGraph, true)
		{
			Width = 21;
			SizeMode = ColumnSizeMode.Fixed;

			_showColors = DefaultShowColors;
		}

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			_extender = new GraphColumnExtender(this);
			Extender = new Popup(_extender);
		}

		protected override void OnListBoxDetached()
		{
			Extender.Dispose();
			Extender = null;
			_extender.Dispose();
			_extender = null;
			base.OnListBoxDetached();
		}

		public bool ShowColors
		{
			get { return _showColors; }
			set
			{
				if(_showColors != value)
				{
					_showColors = value;
					InvalidateContent();
					ShowColorsChanged.Raise(this);
				}
			}
		}

		const int GraphCellWidth = 21;
		const int GraphCellHeight = 21;

		public static void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs, GraphAtom[] graph, RevisionGraphItemType itemType)
		{
			if(graph != null)
			{
				bool showColors;
				var rgc = paintEventArgs.Column as GraphColumn;
				if(rgc != null)
				{
					showColors = rgc.ShowColors;
				}
				else
				{
					showColors = GraphColumn.DefaultShowColors;
				}
				GlobalBehavior.GraphStyle.DrawGraph(
					paintEventArgs.Graphics,
					graph,
					paintEventArgs.Bounds,
					GraphCellWidth,
					itemType,
					showColors);
			}
		}

		public static Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs, GraphAtom[] graph)
		{
			return new Size((graph != null) ? (graph.Length * GraphCellWidth) : (0), GraphCellHeight);
		}

		protected override void SaveMoreTo(Section section)
		{
			base.SaveMoreTo(section);
			section.SetValue("ShowColors", ShowColors);
		}

		protected override void LoadMoreFrom(Section section)
		{
			base.LoadMoreFrom(section);
			ShowColors = section.GetValue("ShowColors", ShowColors);
		}

		public override string IdentificationString
		{
			get { return "Graph"; }
		}
	}
}
