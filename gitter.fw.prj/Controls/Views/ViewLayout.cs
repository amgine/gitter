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

namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Configuration;

	/// <summary>Dock layout storage.</summary>
	public sealed class ViewLayout
	{
		/// <summary>Describes <see cref="ViewBase"/>.</summary>
		private sealed class ViewEntry
		{
			private readonly Guid _guid;

			public ViewEntry(ViewBase view)
			{
				Verify.Argument.IsNotNull(view, nameof(view));

				_guid = view.Guid;
			}

			public ViewEntry(Section section)
			{
				Verify.Argument.IsNotNull(section, nameof(section));

				_guid = section.GetValue<Guid>("Guid");
			}

			public void SaveTo(Section section)
			{
				Verify.Argument.IsNotNull(section, nameof(section));

				section.SetValue("Guid", _guid);
			}
		}

		/// <summary>Describes <see cref="ViewHost"/>.</summary>
		private sealed class HostEntry : ILayoutBase
		{
			#region Data

			private readonly bool _isRoot;
			private readonly bool _isDocumentWell;
			private readonly List<ViewEntry> _views;

			#endregion

			/// <summary>Initializes a new instance of the <see cref="HostEntry"/> class.</summary>
			private HostEntry()
			{
				_views = new List<ViewEntry>();
			}

			public HostEntry(ViewHost viewHost)
				: this()
			{
				Verify.Argument.IsNotNull(viewHost, nameof(viewHost));

				_isRoot = viewHost.IsRoot;
				_isDocumentWell = viewHost.IsDocumentWell;
				foreach(var view in viewHost.Views)
				{
					_views.Add(new ViewEntry(view));
				}
			}

			public HostEntry(Section section)
				: this()
			{
				Verify.Argument.IsNotNull(section, nameof(section));

				_isRoot = section.GetValue<bool>("IsRoot");
				_isDocumentWell = section.GetValue<bool>("IsDocumentHost");
				var tools = section.GetSection("Views");
				foreach(var t in tools.Sections)
				{
					_views.Add(new ViewEntry(t));
				}
			}

			public void SaveTo(Section section)
			{
				Verify.Argument.IsNotNull(section, nameof(section));

				section.SetValue("Type", "Host");
				section.SetValue("IsRoot", _isRoot);
				section.SetValue("IsDocumentHost", _isDocumentWell);
				var tools = section.CreateSection("Views");
				for(int i = 0; i < _views.Count; ++i)
				{
					_views[i].SaveTo(tools.CreateSection("View_" + i.ToString(
						System.Globalization.CultureInfo.InvariantCulture)));
				}
			}
		}

		/// <summary>Describes <see cref="ViewSplit"/>.</summary>
		private sealed class SplitEntry : ILayoutBase
		{
			#region Data

			private readonly List<ILayoutBase> _cells;
			private readonly List<double> _splitters;
			private Orientation _orientation;

			#endregion

			private SplitEntry()
			{
				_cells = new List<ILayoutBase>();
				_splitters = new List<double>();
			}

			public SplitEntry(ViewSplit split)
				: this()
			{
				Verify.Argument.IsNotNull(split, nameof(split));

				_orientation = split.Orientation;
				foreach(var control in split)
				{
					_cells.Add(ToLayout(control));
				}
				foreach(var splitter in split.Positions)
				{
					_splitters.Add(splitter);
				}
			}

			public SplitEntry(Section section)
				: this()
			{
				Verify.Argument.IsNotNull(section, nameof(section));

				_orientation = section.GetValue<Orientation>("Orientation");
				var splitters = section.GetSection("Splitters");
				foreach(var s in splitters.Parameters)
				{
					_splitters.Add((double)s.Value);
				}
				var cells = section.GetSection("Cells");
				foreach(var c in cells.Sections)
				{
					_cells.Add(ToLayout(c));
				}
			}

			public void SaveTo(Section section)
			{
				Verify.Argument.IsNotNull(section, nameof(section));

				section.SetValue("Type", "Split");
				section.SetValue("Orientation", _orientation);
				var splitters = section.CreateSection("Splitters");
				for(int i = 0; i < _splitters.Count; ++i)
				{
					splitters.SetValue("Splitter_" + i.ToString(
						System.Globalization.CultureInfo.InvariantCulture), _splitters[i]);
				}
				var cells = section.CreateSection("Cells");
				for(int i = 0; i < _cells.Count; ++i)
				{
					_cells[i].SaveTo(cells.CreateSection("Cell_" + i.ToString(
						System.Globalization.CultureInfo.InvariantCulture)));
				}
			}
		}

		/// <summary>Describes <see cref="ViewDockSide"/>.</summary>
		private sealed class SideEntry
		{
			#region Data

			private readonly AnchorStyles _side;
			private readonly List<HostEntry> _hosts;

			#endregion

			/// <summary>
			/// Initializes a new instance of the <see cref="SideEntry"/> class.
			/// </summary>
			private SideEntry()
			{
				_hosts = new List<HostEntry>();
			}

			public SideEntry(ViewDockSide side)
				: this()
			{
				Verify.Argument.IsNotNull(side, nameof(side));

				_side = side.Side;
				foreach(var host in side)
				{
					_hosts.Add(new HostEntry(host));
				}
			}

			public SideEntry(AnchorStyles side, Section section)
			{
				Verify.Argument.IsNotNull(section, nameof(section));

				_side = side;
				foreach(var h in section.Sections)
				{
					_hosts.Add(new HostEntry(h));
				}
			}

			public void SaveTo(Section section)
			{
				Verify.Argument.IsNotNull(section, nameof(section));

				for(int i = 0; i < _hosts.Count; ++i)
				{
					_hosts[i].SaveTo(section.CreateSection("Host_" + i.ToString(
						System.Globalization.CultureInfo.InvariantCulture)));
				}
			}
		}

		/// <summary>Describes <see cref="FloatingViewForm"/>.</summary>
		private sealed class FloatEntry
		{
			private readonly Rectangle _bounds;
			private readonly ILayoutBase _root;

			public FloatEntry(FloatingViewForm floatingForm)
			{
				Verify.Argument.IsNotNull(floatingForm, nameof(floatingForm));

				_bounds = floatingForm.Bounds;
				_root = ToLayout(floatingForm.RootControl);
			}

			public FloatEntry(Section section)
			{
				Verify.Argument.IsNotNull(section, nameof(section));

				_bounds = section.GetValue<Rectangle>("Bounds");
				_root = ToLayout(section.GetSection("Root"));
			}

			public void SaveTo(Section section)
			{
				Verify.Argument.IsNotNull(section, nameof(section));

				section.SetValue("Bounds", _bounds);
				_root.SaveTo(section.CreateSection("Root"));
			}
		}

		private interface ILayoutBase
		{
			void SaveTo(Section section);
		}

		private static ILayoutBase ToLayout(Control control)
		{
			Verify.Argument.IsNotNull(control, nameof(control));

			if(control is ViewSplit split)
			{
				return new SplitEntry(split);
			}
			else if(control is ViewHost host)
			{
				return new HostEntry(host);
			}
			else
			{
				throw new ArgumentException("control");
			}
		}

		private static ILayoutBase ToLayout(Section section)
		{
			Verify.Argument.IsNotNull(section, nameof(section));

			switch(section.GetValue<string>("Type"))
			{
				case "Split":
					return new SplitEntry(section);
				case "Host":
					return new HostEntry(section);
				default:
					throw new ArgumentException("section");
			}
		}

		#region Data

		private ILayoutBase _root;
		private SideEntry _left;
		private SideEntry _top;
		private SideEntry _right;
		private SideEntry _bottom;
		private List<FloatEntry> _floats;

		#endregion

		/// <summary>Capture current tool layout of specified <paramref name="viewDockService"/>.</summary>
		/// <param name="viewDockService">The tool dock service.</param>
		/// <exception cref="ArgumentNullException"><paramref name="viewDockService"/> == <c>null</c>.</exception>
		public ViewLayout(ViewDockService viewDockService)
		{
			Verify.Argument.IsNotNull(viewDockService, nameof(viewDockService));

			var grid = viewDockService.Grid;

			_root = ToLayout(grid.RootControl);

			if(grid.LeftSide != null)
			{
				_left = new SideEntry(grid.LeftSide);
			}
			if(grid.TopSide != null)
			{
				_top = new SideEntry(grid.TopSide);
			}
			if(grid.RightSide != null)
			{
				_right = new SideEntry(grid.RightSide);
			}
			if(grid.BottomSide != null)
			{
				_bottom = new SideEntry(grid.BottomSide);
			}
			_floats = new List<FloatEntry>();
			foreach(var floatingViewForm in grid.FloatingViewForms)
			{
				_floats.Add(new FloatEntry(floatingViewForm));
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewLayout"/> class.
		/// </summary>
		/// <param name="section">The section.</param>
		public ViewLayout(Section section)
		{
			Verify.Argument.IsNotNull(section, nameof(section));

			_root = ToLayout(section.GetSection("Root"));

			var sides = section.TryGetSection("Sides");
			if(sides != null)
			{
				var left = sides.TryGetSection("Left");
				if(left != null)
				{
					_left = new SideEntry(AnchorStyles.Left, left);
				}
				var top = sides.TryGetSection("Top");
				if(top != null)
				{
					_top = new SideEntry(AnchorStyles.Top, top);
				}
				var right = sides.TryGetSection("Right");
				if(right != null)
				{
					_right = new SideEntry(AnchorStyles.Right, right);
				}
				var bottom = sides.TryGetSection("Bottom");
				if(bottom != null)
				{
					_bottom = new SideEntry(AnchorStyles.Bottom, bottom);
				}
			}
			var floats = section.TryGetSection("Floats");
			if(floats != null)
			{
				foreach(var f in floats.Sections)
				{
					_floats.Add(new FloatEntry(f));
				}
			}
		}

		/// <summary>Applies this layout to the specified tool dock service.</summary>
		/// <param name="viewDockService">Tool dock service.</param>
		/// <exception cref="ArgumentNullException"><paramref name="viewDockService"/> == <c>null</c>.</exception>
		public void ApplyTo(ViewDockService viewDockService)
		{
			Verify.Argument.IsNotNull(viewDockService, nameof(viewDockService));

		}

		/// <summary>Saves this layout to the specified section.</summary>
		/// <param name="section">Configuration section.</param>
		public void SaveTo(Section section)
		{
			Verify.Argument.IsNotNull(section, nameof(section));

			section.Clear();

			_root.SaveTo(section.CreateSection("Root"));

			if(_left != null || _top != null || _right != null || _bottom != null)
			{
				var sides = section.CreateSection("Sides");
				if(_left != null)
				{
					_left.SaveTo(sides.CreateSection("Left"));
				}
				if(_top != null)
				{
					_top.SaveTo(sides.CreateSection("Top"));
				}
				if(_right != null)
				{
					_right.SaveTo(sides.CreateSection("Right"));
				}
				if(_bottom != null)
				{
					_bottom.SaveTo(sides.CreateSection("Bottom"));
				}
			}
			
			if(_floats != null && _floats.Count != 0)
			{
				var floats = section.CreateSection("Floats");
				for(int i = 0; i < _floats.Count; ++i)
				{
					_floats[i].SaveTo(section.CreateSection("Float_" + i.ToString(
						System.Globalization.CultureInfo.InvariantCulture)));
				}
			}
		}
	}
}
