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
	using System.Text;
	using System.Threading;
	using System.Drawing;
	using System.Reflection;
	using System.Windows.Forms;

	using Resources = gitter.Framework.Properties.Resources;

	public partial class ExceptionDialog : DialogBase
	{
		private const string ReportUrl = @"https://github.com/amgine/gitter/issues/new";
		private DateTime _date;

		/// <summary>Create <see cref="ExceptionDialog"/>.</summary>
		/// <param name="exception">Related exception.</param>
		public ExceptionDialog(Exception exception)
		{
			Verify.Argument.IsNotNull(exception, nameof(exception));

			Exception = exception;
			_date = DateTime.Now;

			InitializeComponent();

			Text = Resources.StrError;

			GitterApplication.FontManager.InputFont.Apply(_txtStack);

			var f = GitterApplication.FontManager.UIFont.Font;

			_lblExceptionName.Font = new Font(f.FontFamily, 15, FontStyle.Regular, f.Unit, f.GdiCharSet);

			_lblSTack.Text = Resources.StrStack.AddColon();
			_lnkCopyToClipboard.Text = Resources.StrCopyToClipboard;
			_lnkSendBugReport.Text = Resources.StrSendBugreport;

			_lblExceptionName.Text = exception.GetType().FullName;
			_lblMessage.Text = exception.Message;
			string stackTrace;
			if(exception.Data.Contains("OriginalStackTrace"))
			{
				stackTrace = (string)exception.Data["OriginalStackTrace"];
			}
			else
			{
				stackTrace = exception.StackTrace;
			}
			_txtStack.Text = stackTrace;
		}

		protected override string ActionVerb => Resources.StrClose;

		public override DialogButtons OptimalButtons => DialogButtons.Ok;

		public Exception Exception { get; }

		private string GetMessage()
		{
			var str = new StringBuilder();
			str.Append("Date: ");
			str.AppendLine(_date.FormatISO8601());
			AppendExceptionInfo("Exception", Exception, str);
			if(Exception.InnerException != null)
			{
				AppendInnerException(Exception.InnerException, str);
			}
			AppendLoadedAssemblies(str);
			return str.ToString();
		}

		private static void AppendInnerException(Exception exception, StringBuilder str)
		{
			AppendExceptionInfo("Inner Exception", exception, str);
			if(exception.InnerException != null)
			{
				AppendInnerException(exception.InnerException, str);
			}
		}

		private static void AppendExceptionInfo(string header, Exception exception, StringBuilder str)
		{
			str.Append(header);
			str.Append(": ");
			str.AppendLine(exception.GetType().FullName);
			str.AppendLine("Message:");
			str.AppendLine(exception.Message);
			str.AppendLine();
			str.AppendLine("Call Stack:");
			str.AppendLine(exception.StackTrace);
			str.AppendLine();
		}

		private static void AppendLoadedAssemblies(StringBuilder str)
		{
			var asms = AppDomain.CurrentDomain.GetAssemblies();
			Array.Sort(asms, (asm1, asm2) =>
				{
					if(asm1.GlobalAssemblyCache != asm2.GlobalAssemblyCache)
						return asm1.GlobalAssemblyCache ? 1 : -1;
					else
						return string.Compare(asm1.FullName, asm2.FullName);
				});
			str.AppendLine("Loaded Assemblies:");
			for(int i = 0; i < asms.Length; ++i)
			{
				AppendAssemblyInfo(asms[i], str);
			}
		}

		private static void AppendAssemblyInfo(Assembly asm, StringBuilder str)
		{
			if(asm.GlobalAssemblyCache)
			{
				str.Append("[GAC] ");
			}
			str.AppendLine(asm.FullName);
		}

		private void OnCopyToClipboardLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			CopyToClipboardSafe();
		}

		private void OnSendBugReportLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Utility.OpenUrl(ReportUrl);
		}

		private void CopyToClipboardSafe()
		{
			if(Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
			{
				CopyToClipboard();
			}
			else
			{
				var thread = new Thread(CopyToClipboard);
				if(thread.TrySetApartmentState(ApartmentState.STA))
				{
					thread.Start();
					thread.Join();
				}
			}
		}

		private void CopyToClipboard()
		{
			var message = GetMessage();
			ClipboardEx.SetTextSafe(message);
		}
	}
}
