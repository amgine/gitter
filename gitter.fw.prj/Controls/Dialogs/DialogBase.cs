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

namespace gitter.Framework
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Mvc;
	using gitter.Framework.Mvc.WinForms;
	using gitter.Framework.Options;
	using gitter.Framework.Services;

	using Resources = gitter.Framework.Properties.Resources;

	[ToolboxItem(false)]
	public partial class DialogBase : UserControl
	{
		#region Data

		private INotificationService _notificationService;
		private IToolTipService _tooltipService;

		#endregion

		/// <summary>Initializes a new instance of the <see cref="DialogBase"/> class.</summary>
		public DialogBase()
		{
			_notificationService = new BalloonNotificationService();
			_tooltipService = new DefaultToolTipService();
			SuspendLayout();
			if(LicenseManager.UsageMode == LicenseUsageMode.Runtime)
			{
				Font = GitterApplication.FontManager.UIFont;
			}
			else
			{
				Font = SystemFonts.MessageBoxFont;
			}
			AutoScaleMode = AutoScaleMode.Dpi;
			AutoScaleDimensions = new SizeF(96F, 96F);
			ResumeLayout(false);

			Margin = new Padding(10);
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				_tooltipService.Dispose();
				_notificationService.Dispose();
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

		public virtual DialogButtons OptimalButtons
		{
			get { return DialogButtons.OkCancel; }
		}

		public MouseCursor MouseCursor
		{
			get { return MouseCursorConverter.Convert(Cursor); }
			set { Cursor = MouseCursorConverter.Convert(value); }
		}

		/// <summary>Runs this dialog.</summary>
		/// <param name="owner">Owner window.</param>
		/// <returns><see cref="DialogResult"/>.</returns>
		public DialogResult Run(IWin32Window owner)
		{
			using(var form = new DialogForm(this, OptimalButtons)
				{
					OKButtonText = ActionVerb,
					CancelButtonText = CancelVerb,
				})
			{
				return form.ShowDialog(owner);
			}
		}

		protected virtual string ActionVerb
		{
			get { return Resources.StrOk; }
		}

		protected virtual string CancelVerb
		{
			get { return Resources.StrCancel; }
		}

		protected void ClickOk()
		{
			var form = ParentForm as DialogForm;
			if(form != null)
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
			var form = ParentForm as DialogForm;
			if(form != null)
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
			var form = ParentForm as DialogForm;
			if(form != null) form.ClickApply();
		}

		protected void SetOkEnabled(bool enabled)
		{
			var form = ParentForm as DialogForm;
			if(form != null) form.OkButtonEnabled = enabled;
		}

		protected void SetCancelEnabled(bool enabled)
		{
			var form = ParentForm as DialogForm;
			if(form != null) form.CancelButtonEnabled = enabled;
		}

		protected void SetApplyEnabled(bool enabled)
		{
			var form = ParentForm as DialogForm;
			if(form != null) form.ApplyButtonEnabled = enabled;
		}

		protected INotificationService NotificationService
		{
			get { return _notificationService; }
		}

		protected IToolTipService ToolTipService
		{
			get { return _tooltipService; }
		}
	}
}
