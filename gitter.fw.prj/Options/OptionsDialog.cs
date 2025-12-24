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

namespace gitter.Framework.Options;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework.Controls;
using gitter.Framework.Layout;

using Resources = gitter.Framework.Properties.Resources;

[ToolboxItem(false)]
public partial class OptionsDialog : DialogBase, IExecutableDialog, IElevatedExecutableDialog
{
	readonly struct DialogControls
	{
		public readonly OptionsListBox _lstOptions;
		public readonly Panel _pnlPageContainer;

		public DialogControls(IGitterStyle style)
		{
			style ??= GitterApplication.Style;

			_lstOptions = new()
			{
				Style          = style,
				HeaderStyle    = HeaderStyle.Hidden,
				ItemActivation = gitter.Framework.Controls.ItemActivation.SingleClick,
				ShowTreeLines  = true,
			};
			_pnlPageContainer = new();
		}

		public void Layout(Control parent)
		{
			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					columns:
					[
						SizeSpec.Absolute(160),
						LayoutConstants.ColumnSpacing,
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_lstOptions,       marginOverride: LayoutConstants.NoMargin), column: 0),
						new GridContent(new ControlContent(_pnlPageContainer, marginOverride: LayoutConstants.NoMargin), column: 2),
					])
			};

			var tabIndex = 0;
			_lstOptions.TabIndex = tabIndex++;
			_pnlPageContainer.TabIndex = tabIndex++;

			_lstOptions.Parent = parent;
			_pnlPageContainer.Parent = parent;
		}
	}

	private readonly DialogControls _controls;
	private readonly Dictionary<Guid, PropertyPage?> _propertyPages;
	private readonly IWorkingEnvironment _environment;
	private PropertyPage? _activePage;

	public OptionsDialog(IWorkingEnvironment environment, IPropertyPageProvider propertyPageProvider)
	{
		Verify.Argument.IsNotNull(environment);
		Verify.Argument.IsNotNull(propertyPageProvider);

		_environment = environment;

		Name = nameof(OptionsDialog);
		Text = Resources.StrOptions;

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Size                = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.Style);
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		_controls._lstOptions.Load(propertyPageProvider);
		_propertyPages = [];

		_controls._lstOptions.ItemActivated += OnItemActivated;
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			_activePage = null;
			foreach(var page in _propertyPages.Values)
			{
				page?.Dispose();
			}
			_propertyPages.Clear();
		}
		base.Dispose(disposing);
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(619, 381));

	/// <inheritdoc/>
	public override DialogButtons OptimalButtons => DialogButtons.All;

	protected override void OnShown()
	{
		if(_controls._lstOptions.Items.Count != 0)
		{
			var item = _controls._lstOptions.Items[0];
			item.IsSelected = true;
			item.Activate();
		}
	}

	private void OnItemActivated(object? sender, ItemEventArgs e)
	{
		if(e.Item is not PropertyPageItem item) return;

		var desc = item.DataContext;
		if(_activePage is not null)
		{
			if(_activePage.Guid == desc.Guid) return;
		}
		if(!_propertyPages.TryGetValue(desc.Guid, out var page))
		{
			page = desc.CreatePropertyPage(_environment);
			bool raiseElevatedChanged = false;
			if(page is IElevatedExecutableDialog elevated)
			{
				elevated.RequireElevationChanged += OnRequireElevationChanged;
				if(!RequireElevation && elevated.RequireElevation)
				{
					raiseElevatedChanged = true;
				}
			}
			_propertyPages.Add(desc.Guid, page);
			if(raiseElevatedChanged)
			{
				RequireElevationChanged?.Invoke(this, EventArgs.Empty);
			}
		}
		if(page is not null)
		{
			page.Dock = DockStyle.Fill;
			page.Parent = _controls._pnlPageContainer;
			page.InvokeOnShown();
		}
		if(_activePage is not null)
		{
			_activePage.Parent = null;
		}
		_activePage = page;
	}

	private void OnRequireElevationChanged(object? sender, EventArgs e)
	{
		bool require = false;
		foreach(var page in _propertyPages.Values)
		{
			if(page != sender && page is IElevatedExecutableDialog { RequireElevation: true })
			{
				require = true;
				break;
			}
		}
		if(require) return;
		RequireElevationChanged?.Invoke(this, EventArgs.Empty);
	}

	#region IExecutableDialog Members

	public bool Execute()
	{
		bool res = true;
		foreach(var page in _propertyPages.Values)
		{
			if(page is IExecutableDialog executable)
			{
				if(!executable.Execute())
				{
					res = false;
				}
			}
		}
		return res;
	}

	#endregion

	#region IElevatedExecutableDialog Members

	public event EventHandler? RequireElevationChanged;

	public bool RequireElevation
	{
		get
		{
			foreach(var page in _propertyPages.Values)
			{
				if(page is IElevatedExecutableDialog { RequireElevation: true })
				{
					return true;
				}
			}
			return false;
		}
	}

	public string[]? ElevatedExecutionActions
	{
		get
		{
			var list = default(List<string>);
			foreach(var page in _propertyPages.Values)
			{
				if(page is IElevatedExecutableDialog { RequireElevation: true, ElevatedExecutionActions: { Length: not 0 } actions })
				{
					(list ??= []).AddRange(actions);
				}
			}
			return list?.ToArray();
		}
	}

	#endregion
}
