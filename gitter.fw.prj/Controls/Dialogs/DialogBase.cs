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

namespace gitter.Framework;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework.Mvc;
using gitter.Framework.Mvc.WinForms;
using gitter.Framework.Services;

using Resources = gitter.Framework.Properties.Resources;

[ToolboxItem(false)]
[DesignerCategory("")]
public abstract partial class DialogBase : UserControl
{
	private static readonly object ScalableSizeChangedEvent = new();

	public event EventHandler ScalableSizeChanged
	{
		add    => Events.AddHandler    (ScalableSizeChangedEvent, value);
		remove => Events.RemoveHandler (ScalableSizeChangedEvent, value);
	}

	protected virtual void OnScalableSizeChanged(EventArgs e)
		=> Events.Raise(ScalableSizeChangedEvent, this, e);

	protected static readonly IDpiBoundValue<Padding> DefaultScalableMargin = DpiBoundValue.Padding(new(10));

	protected const int DefaultWidth = 385;

	public virtual IDpiBoundValue<Padding> ScalableMargin => DefaultScalableMargin;

	public abstract IDpiBoundValue<Size> ScalableSize { get; }

	/// <summary>Initializes a new instance of the <see cref="DialogBase"/> class.</summary>
	public DialogBase()
	{
		NotificationService = new BalloonNotificationService();
		ToolTipService = new DefaultToolTipService();
		SuspendLayout();
#if !NETCOREAPP
		Font = SystemFonts.MessageBoxFont ?? SystemFonts.DefaultFont;
#endif
		AutoScaleMode = AutoScaleMode.Dpi;
		AutoScaleDimensions = new SizeF(96F, 96F);
		Margin = new Padding(10);
		ResumeLayout(false);
		PerformLayout();
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			ToolTipService.Dispose();
			NotificationService.Dispose();
		}
		base.Dispose(disposing);
	}

	public void InvokeOnShown()
	{
		OnShown();
	}

	public void InvokeOnClosed(DialogResult result)
	{
		OnClosed(result);
	}

	protected virtual void OnShown()
	{
	}

	protected virtual void OnClosed(DialogResult result)
	{
	}

	public virtual DialogButtons OptimalButtons => DialogButtons.OkCancel;

	public MouseCursor MouseCursor
	{
		get => MouseCursorConverter.Convert(Cursor);
		set => Cursor = MouseCursorConverter.Convert(value);
	}

	/// <summary>Runs this dialog.</summary>
	/// <param name="owner">Owner window.</param>
	/// <returns><see cref="DialogResult"/>.</returns>
	public DialogResult Run(IWin32Window owner)
	{
		using var form = new DialogForm(this, OptimalButtons)
		{
			OKButtonText     = ActionVerb,
			CancelButtonText = CancelVerb,
		};
		return form.ShowDialog(owner);
	}

	/// <summary>Verb, describing operation.</summary>
	protected virtual string ActionVerb => Resources.StrOk;

	protected virtual string CancelVerb => Resources.StrCancel;

	protected void ClickOk()
	{
		if(ParentForm is DialogForm form)
		{
			form.ClickOk();
		}
		else
		{
			ParentForm.DialogResult = DialogResult.OK;
			ParentForm.Close();
		}
	}

	protected void ClickCancel()
	{
		if(ParentForm is DialogForm form)
		{
			form.ClickCancel();
		}
		else
		{
			ParentForm.DialogResult = DialogResult.Cancel;
			ParentForm.Close();
		}
	}

	protected void ClickApply()
	{
		if(ParentForm is DialogForm form) form.ClickApply();
	}

	protected void SetOkEnabled(bool enabled)
	{
		if(ParentForm is DialogForm form) form.OkButtonEnabled = enabled;
	}

	protected void SetCancelEnabled(bool enabled)
	{
		if(ParentForm is DialogForm form) form.CancelButtonEnabled = enabled;
	}

	protected void SetApplyEnabled(bool enabled)
	{
		if(ParentForm is DialogForm form) form.ApplyButtonEnabled = enabled;
	}

	protected INotificationService NotificationService { get; }

	protected IToolTipService ToolTipService { get; }
}
