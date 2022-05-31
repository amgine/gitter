#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Controls;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;

using Resources = gitter.Properties.Resources;

public abstract class PickerDialog<TPicker, TValue> : DialogBase, IExecutableDialog
	where TPicker : Control, IPicker<TValue>, new()
	where TValue : class
{
	const int ControlSpacing = 3;

	sealed class ScalableSizeImpl : IDpiBoundValue<Size>
	{
		public IDpiBoundValue<Size> DisplayedControlSize { get; set; }

		public Size GetValue(Dpi dpi)
		{
			var width  = 400 * dpi.X / 96;
			var height =  30 * dpi.Y / 96;

			if(DisplayedControlSize is not null)
			{
				height += ControlSpacing * dpi.Y / 96;
				height += DisplayedControlSize.GetValue(dpi).Height;
			}

			return new(width, height);
		}
	}

	private readonly Dictionary<TValue, Control> _controlCache;
	private readonly Label _label;
	private readonly TPicker _picker;
	private readonly ScalableSizeImpl _size = new();

	protected PickerDialog(string displayName)
	{
		_controlCache = new Dictionary<TValue, Control>();
		_label = new Label();
		_picker = new TPicker();
		SuspendLayout();
		// 
		// _label
		// 
		_label.Name = "_label";
		_label.AutoSize = true;
		_label.Bounds = new System.Drawing.Rectangle(0, 6, 54, 15);
		_label.Text = displayName;
		// 
		// _picker
		// 
		_picker.Name = "_picker";
		_picker.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		_picker.Bounds = new System.Drawing.Rectangle(94, 3, 303, 23);
		_picker.TabIndex = 0;
		_picker.SelectedValueChanged += OnSelectedValueChanged;
		// 
		// PickerDialog
		// 
		AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
		Controls.Add(_label);
		Controls.Add(_picker);
		Size = new System.Drawing.Size(400, 30);
		ResumeLayout(false);
		PerformLayout();
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize => _size;

	public TValue SelectedValue => _picker.SelectedValue;

	protected virtual int MinimumSelectableItems => 1;

	protected Control SelectedControl { get; private set; }

	protected abstract void LoadItems(TPicker picker);

	protected abstract Control CreateControl(TValue item);

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);

		LoadItems(_picker);

		if(_picker.DropDownItems.Count != 0)
		{
			var item = _picker.DropDownItems[0];
			item.IsSelected = true;
			item.Activate();
			if(_picker.DropDownItems.Count < MinimumSelectableItems)
			{
				_picker.Enabled = false;
			}
		}
		else
		{
			_picker.Enabled = false;
			_label.Enabled = false;
		}
	}

	protected Control GetOrCreateControl(TValue item)
	{
		if(!_controlCache.TryGetValue(item, out var control))
		{
			control = CreateControl(item);
			_controlCache.Add(item, control);
		}
		return control;
	}

	private void OnSelectedValueChanged(object sender, EventArgs e)
	{
		var item = _picker.SelectedValue;
		var control = item is not null ? GetOrCreateControl(item) : default;
		_size.DisplayedControlSize = (control as DialogBase)?.ScalableSize;
		if(SelectedControl is not null)
		{
			SelectedControl.Parent = null;
			SelectedControl = null;
		}
		SelectedControl = control;
		if(SelectedControl is not null)
		{
			SelectedControl.SetBounds(0, _picker.Bottom + ControlSpacing * DeviceDpi / 96, Width, 0,
				BoundsSpecified.X | BoundsSpecified.Y | BoundsSpecified.Width);
			SelectedControl.Parent = this;
		}
		OnScalableSizeChanged(EventArgs.Empty);
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			if(_controlCache is not null)
			{
				foreach(var ctl in _controlCache.Values)
				{
					if(ctl is { IsDisposed : false })
					{
						ctl.Dispose();
					}
				}
				_controlCache.Clear();
			}
		}
		base.Dispose(disposing);
	}

	/// <inheritdoc/>
	public virtual bool Execute()
		=> SelectedControl is IExecutableDialog ctl ? ctl.Execute() : true;
}
