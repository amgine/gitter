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

namespace gitter.Framework.Services
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;

	using Resources = gitter.Framework.Properties.Resources;

	public sealed class MessageBoxButton
	{
		public static readonly MessageBoxButton Yes		= new(DialogResult.Yes,	0, Resources.StrYes,	true);
		public static readonly MessageBoxButton No		= new(DialogResult.No,		0, Resources.StrNo,		false);
		public static readonly MessageBoxButton Ok		= new(DialogResult.OK,		0, Resources.StrOk,		true);
		public static readonly MessageBoxButton Close	= new(DialogResult.OK,		0, Resources.StrClose,	true);
		public static readonly MessageBoxButton Cancel	= new(DialogResult.Cancel,	0, Resources.StrCancel,	false);
		public static readonly MessageBoxButton Abort	= new(DialogResult.Abort,	0, Resources.StrAbort,	false);
		public static readonly MessageBoxButton Retry	= new(DialogResult.Retry,	0, Resources.StrRetry,	true);
		public static readonly MessageBoxButton Ignore	= new(DialogResult.Ignore,	0, Resources.StrIgnore,	false);

		public static readonly IEnumerable<MessageBoxButton> RetryCancel      = new[] { Retry, Cancel };
		public static readonly IEnumerable<MessageBoxButton> AbortRetryIgnore = new[] { Abort, Retry, Ignore };
		public static readonly IEnumerable<MessageBoxButton> YesNo            = new[] { Yes, No };
		public static readonly IEnumerable<MessageBoxButton> YesNoCancel      = new[] { Yes, No, Cancel };
		public static readonly IEnumerable<MessageBoxButton> OkCancel         = new[] { Ok, Cancel };

		public static MessageBoxButton GetOk(string label)
		{
			return GetOk(label, 0);
		}

		public static MessageBoxButton GetOk(string label, int resultOption)
		{
			return new MessageBoxButton(DialogResult.OK, resultOption, label, true);
		}

		public static MessageBoxButton GetYes(string label)
		{
			return GetYes(label, 0);
		}

		public static MessageBoxButton GetYes(string label, int resultOption)
		{
			return new MessageBoxButton(DialogResult.OK, resultOption, label, true);
		}

		public static IEnumerable<MessageBoxButton> GetButtons(MessageBoxButtons buttons)
		{
			return buttons switch
			{
				MessageBoxButtons.OK               => new[] { Ok },
				MessageBoxButtons.OKCancel         => new[] { Ok, Cancel },
				MessageBoxButtons.YesNo            => new[] { Yes, No },
				MessageBoxButtons.YesNoCancel      => new[] { Yes, No, Cancel },
				MessageBoxButtons.AbortRetryIgnore => new[] { Abort, Retry, Ignore },
				MessageBoxButtons.RetryCancel      => new[] { Retry, Cancel },
				_ => throw new ArgumentException($"Unknown MessageBoxButtons value: {buttons}", nameof(buttons)),
			};
		}

		public MessageBoxButton(DialogResult dialogResult, int resultOption, string displayLabel, bool isDefault)
		{
			DialogResult = dialogResult;
			ResultOption = resultOption;
			DisplayLabel = displayLabel;
			IsDefault    = isDefault;
		}

		public string DisplayLabel { get; }

		public int ResultOption { get; }

		public DialogResult DialogResult { get; }

		public bool IsDefault { get; }
	}
}
