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

namespace gitter.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Properties.Resources;

	public abstract class PickerDialog<TPicker, TValue> : DialogBase, IExecutableDialog
		where TPicker : Control, IPicker<TValue>, new()
		where TValue : class
	{
		#region Data

		private Dictionary<TValue, Control> _controlCache;
		private Control _selectedControl;
		private Label _label;
		private TPicker _picker;

		#endregion

		#region .ctor

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

		#endregion

		#region Properties

		public TValue SelectedValue
		{
			get { return _picker.SelectedValue; }
		}

		protected virtual int MinimumSelectableItems
		{
			get { return 1; }
		}

		protected Control SelectedControl
		{
			get { return _selectedControl; }
		}

		#endregion

		#region Methods

		protected abstract void LoadItems(TPicker picker);

		protected abstract Control CreateControl(TValue item);

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
			Control control;
			if(!_controlCache.TryGetValue(item, out control))
			{
				control = CreateControl(item);
				_controlCache.Add(item, control);
			}
			return control;
		}

		private void OnSelectedValueChanged(object sender, EventArgs e)
		{
			var item = _picker.SelectedValue;
			if(item == null)
			{
				return;
			}
			var control = GetOrCreateControl(item);
			int d = 0;
			if(_selectedControl != null)
			{
				d -= _selectedControl.Height;
				_selectedControl.Parent = null;
				_selectedControl = null;
				d -= 3;
			}
			_selectedControl = control;
			if(_selectedControl != null)
			{
				d += _selectedControl.Height;
				d += 3;
			}
			Height += d;
			if(_selectedControl != null)
			{
				_selectedControl.SetBounds(0, _picker.Bottom + 3, Width, 0,
					BoundsSpecified.X | BoundsSpecified.Y | BoundsSpecified.Width);
				_selectedControl.Parent = this;
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_controlCache != null)
				{
					foreach(var ctl in _controlCache.Values)
					{
						if(ctl != null && !ctl.IsDisposed)
						{
							ctl.Dispose();
						}
					}
					_controlCache.Clear();
				}
			}
			base.Dispose(disposing);
		}

		#endregion

		#region IExecutableDialog Members

		public virtual bool Execute()
		{
			if(_selectedControl != null)
			{
				var ctl = _selectedControl as IExecutableDialog;
				if(ctl != null)
				{
					if(!ctl.Execute())
					{
						return false;
					}
				}
			}
			return true;
		}

		#endregion
	}
}
