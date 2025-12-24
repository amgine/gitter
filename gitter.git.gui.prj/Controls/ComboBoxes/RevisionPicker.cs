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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;

public class RevisionPicker : TextBoxDecoratorWithButton
{
	const int MaxDisplayedItems = 10;

	private ReferencesListBox _lstReferences;
	private RevisionToolTip _revisionToolTip;
	private Popup? _dropDown;
	private long _lastHidden;

	public RevisionPicker() : base(new())
	{
		_lstReferences = new ReferencesListBox()
		{
			HeaderStyle = HeaderStyle.Hidden,
			BorderStyle = BorderStyle.FixedSingle,
			ItemActivation = gitter.Framework.Controls.ItemActivation.SingleClick,
			Size = new Size(Width, 2 + 2 + 21 * 10),
			DisableContextMenus = true,
			Font = LicenseManager.UsageMode == LicenseUsageMode.Runtime ?
				GitterApplication.FontManager.UIFont.Font :
				SystemFonts.MessageBoxFont,
		};
		_lstReferences.ItemActivated += OnItemActivated;
		_lstReferences.DisplayedItemsCountChanged += OnReferencesDisplayedItemsCountChanged;
		_revisionToolTip = new RevisionToolTip()
		{
		};
	}

	private void OnReferencesDisplayedItemsCountChanged(object? sender, EventArgs e)
	{
		if(_dropDown is not { Visible: true }) return;
		var count = Math.Min(MaxDisplayedItems, Math.Max(_lstReferences.DisplayedItemsCount, 1));
		_dropDown.Height = _lstReferences.GetHeightToFitItems(count, Dpi.FromControl(this));
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			if(_dropDown is not null)
			{
				_dropDown.Closed -= OnDropDownClosed;
				_dropDown.Dispose();
				_dropDown = null;
			}
			if(_lstReferences is not null)
			{
				_lstReferences.LoadData(null);
				_lstReferences.ItemActivated -= OnItemActivated;
				_lstReferences.Dispose();
				_lstReferences = null!;
			}
			if(_revisionToolTip is not null)
			{
				_revisionToolTip.Dispose();
				_revisionToolTip = null!;
			}
		}
		base.Dispose(disposing);
	}

	public ReferencesListBox References => _lstReferences;

	public bool DroppedDown
	{
		get => _dropDown is { Visible: true };
		set
		{
			if(value)
			{
				ShowDropDownCore();
			}
			else
			{
				HideDropDownCore();
			}
		}
	}

	/// <inheritdoc/>
	protected override void OnSizeChanged(EventArgs e)
	{
		base.OnSizeChanged(e);
		if(_dropDown is not null)
		{
			_dropDown.Width = Width;
		}
	}

	protected override void PaintButton(Graphics graphics, Rectangle bounds)
	{
		var colors      = GetColorTable();
		var conv        = DpiConverter.FromDefaultTo(this);
		var glyphColor  = Decorated.Enabled ? IsMouseOverButton ? colors.Hover.ForeColor : colors.Normal.ForeColor : colors.Disabled.ForeColor;
		var glyphSize   = conv.ConvertX(8);

		var glyphBounds = new Rectangle(
			bounds.X + (bounds.Width  - glyphSize) / 2,
			bounds.Y + (bounds.Height - glyphSize) / 2,
			glyphSize, glyphSize);

		using(var pen = new Pen(glyphColor, conv.ConvertX(1.5f)))
		{
			var h  = glyphBounds.Height / 4;
			var y0 = glyphBounds.Y + h;
			var y1 = glyphBounds.Y + h * 3;
			var x0 = glyphBounds.X;
			var x1 = glyphBounds.X + glyphBounds.Width / 2;
			var x2 = glyphBounds.Right;
#if NET9_0_OR_GREATER
			Span<Point> points = stackalloc Point[3];
#else
			var points = new Point[3];
#endif
			points[0] = new(x0, y0);
			points[1] = new(x1, y1);
			points[2] = new(x2, y0);
			using(graphics.SwitchSmoothingMode(System.Drawing.Drawing2D.SmoothingMode.HighQuality))
			{
				graphics.DrawLines(pen, points);
			}
		}
	}

	private void ShowDropDownCore()
	{
		if(IsDisposed || Disposing) return;
		if(References.DisplayedItemsCount == 0) return;
		if(_dropDown is null)
		{
			_dropDown = new Popup(References)
			{
				PopupAnimation = PopupAnimations.Slide | PopupAnimations.TopToBottom,
				Width          = Width,
				AutoClose      = true,
			};
			_dropDown.Closed += OnDropDownClosed;
		}
		var count = Math.Min(MaxDisplayedItems, References.DisplayedItemsCount);
		_dropDown.Height = References.GetHeightToFitItems(count, Dpi.FromControl(this));
		_dropDown.Show(this);
	}

	private void HideDropDownCore(ToolStripDropDownCloseReason reason = ToolStripDropDownCloseReason.ItemClicked)
		=> _dropDown?.Close(reason);

	private void OnDropDownClosed(object? sender, ToolStripDropDownClosedEventArgs e)
#if NETCOREAPP
		=> _lastHidden = Environment.TickCount64;
#else
		=> _lastHidden = Environment.TickCount;
#endif

	protected override void OnButtonClick()
	{
#if NETCOREAPP
		var delta = Environment.TickCount64 - _lastHidden;
#else
		var delta = Environment.TickCount - _lastHidden;
#endif
		if(delta < 0 || delta > 20)
		{
			ShowDropDownCore();
		}
	}

	private void OnItemActivated(object? sender, ItemEventArgs e)
	{
		switch(e.Item)
		{
			case BranchListItem branch:
				Text = branch.DataContext.Name;
				HideDropDownCore();
				break;
			case RemoteBranchListItem remoteBranch:
				Text = remoteBranch.DataContext.Name;
				HideDropDownCore();
				break;
			case TagListItem tag:
				Text = tag.DataContext.Name;
				HideDropDownCore();
				break;
		}
	}

	protected override void OnDecoratedMouseEnter(object? sender, EventArgs e)
	{
		base.OnDecoratedMouseEnter(sender, e);
		var repository = References.Repository;
		if(repository is null) return;
		var refspec = Text?.Trim();
		if(refspec is not { Length: not 0 }) return;
		try
		{
			var pointer = repository.GetRevisionPointer(refspec);
			var revision = pointer.Dereference();
			if(revision is { IsLoaded: false })
			{
				revision.Load();
			}
			ShowRevisionToolTip(revision);
		}
		catch(Exception exc) when(!exc.IsCritical)
		{
			ShowRevisionToolTip(default);
		}
	}

	private void ShowRevisionToolTip(Revision? revision)
	{
		if(IsDisposed) return;

		_revisionToolTip.Revision = revision;
		if(revision is not null)
		{
			_revisionToolTip.Show(this, new Point(0, Height + 1));
		}
		else
		{
			_revisionToolTip.Hide(this);
		}
	}

	protected override void OnDecoratedMouseLeave(object? sender, EventArgs e)
	{
		base.OnDecoratedMouseLeave(sender, e);
		_revisionToolTip.Hide(this);
	}

	/// <inheritdoc/>
	public int DropDownHeight
	{
		get => References.Height;
		set => References.Height = value;
	}
}
