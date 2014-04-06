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
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using Resources = gitter.Framework.Properties.Resources;

	[DefaultProperty("Text"), DefaultEvent("Expanded")]
	public partial class GroupSeparator : Control
	{
		#region Static Data

		private static readonly Bitmap ImgEH = Resources.ImgChevronExpandHover;
		private static readonly Bitmap ImgCH = Resources.ImgChevronCollapseHover;
		private static readonly Bitmap ImgEN = Resources.ImgChevronExpand;
		private static readonly Bitmap ImgCN = Resources.ImgChevronCollapse;

		#endregion

		#region Data

		private bool _showChevron;
		private bool _expanded;
		private bool _hover;

		#endregion

		#region Events

		private static readonly object ExpandedEvent;
		public event EventHandler Expanded
		{
			add { Events.AddHandler(ExpandedEvent, value); }
			remove { Events.RemoveHandler(ExpandedEvent, value); }
		}

		private static readonly object CollapsedEvent;
		public event EventHandler Collapsed
		{
			add { Events.AddHandler(CollapsedEvent, value); }
			remove { Events.RemoveHandler(CollapsedEvent, value); }
		}

		#endregion

		#region .ctor

		static GroupSeparator()
		{
			ExpandedEvent = new object();
			CollapsedEvent = new object();
		}

		/// <summary>Create <see cref="GroupSeparator"/>.</summary>
		public GroupSeparator()
		{
			InitializeComponent();
			_expanded = true;

			SetStyle(ControlStyles.FixedHeight, true);
			SetStyle(ControlStyles.Selectable, false);
		}

		#endregion

		#region Properties

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[DefaultValue("")]
		[Localizable(true)]
		public override string Text
		{
			get { return base.Text; }
			set
			{
				base.Text = value;
				_lblText.Text = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(typeof(Size), "0, 19")]
		public override Size MinimumSize
		{
			get { return base.MinimumSize; }
			set { base.MinimumSize = value; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(typeof(Size), "9999, 19")]
		public override Size MaximumSize
		{
			get { return base.MaximumSize; }
			set { base.MaximumSize = value; }
		}

		[DefaultValue(false)]
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public bool ShowChevron
		{
			get { return _showChevron; }
			set
			{
				if(_showChevron != value)
				{
					_showChevron = value;
					_picChevron.Visible = _showChevron;
					UpdateChevron();
					UpdateLineBounds();
				}
			}
		}

		[DefaultValue(true)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsExpanded
		{
			get { return _expanded; }
			set
			{
				if(_expanded != value)
				{
					_expanded = value;
					UpdateChevron();
				}
			}
		}

		#endregion

		private void UpdateLineBounds()
		{
			var x = _lblText.Right + 4;
			var w = Width - x;
			if(_showChevron) w -= 19+5;
			_line.SetBounds(x, (Height + 1) / 2, w, 1);
		}

		private void UpdateChevron()
		{
			if(_showChevron)
			{
				if(_hover)
				{
					_picChevron.Image = _expanded ? ImgCH : ImgEH;
				}
				else
				{
					_picChevron.Image = _expanded ? ImgCN : ImgEN;
				}
			}
			else
			{
				_picChevron.Image = null;
			}
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			UpdateLineBounds();
		}

		private void _lblText_Resize(object sender, EventArgs e)
		{
			UpdateLineBounds();
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			_hover = true;
			UpdateChevron();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			_hover = false;
			UpdateChevron();
		}

		private void OnClick(object sender, EventArgs e)
		{
			if(_showChevron)
			{
				IsExpanded = !IsExpanded;
				if(_expanded)
					Events.Raise(ExpandedEvent, this);
				else
					Events.Raise(CollapsedEvent, this);
			}
		}

		private void OnHostedControlMouseEnter(object sender, EventArgs e)
		{
			_hover = true;
			UpdateChevron();
		}

		private void OnHostedControlMouseLeave(object sender, EventArgs e)
		{
			_hover = false;
			UpdateChevron();
		}
	}
}
