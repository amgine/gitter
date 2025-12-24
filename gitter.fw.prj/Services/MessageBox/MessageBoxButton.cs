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

namespace gitter.Framework.Services;

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Resources = gitter.Framework.Properties.Resources;

public sealed class MessageBoxButton(DialogResult dialogResult, int resultOption, string displayLabel, bool isDefault)
{
	public static MessageBoxButton Yes    { get; } = new(DialogResult.Yes,    0, Resources.StrYes,    true);
	public static MessageBoxButton No     { get; } = new(DialogResult.No,     0, Resources.StrNo,     false);
	public static MessageBoxButton Ok     { get; } = new(DialogResult.OK,     0, Resources.StrOk,     true);
	public static MessageBoxButton Close  { get; } = new(DialogResult.OK,     0, Resources.StrClose,  true);
	public static MessageBoxButton Cancel { get; } = new(DialogResult.Cancel, 0, Resources.StrCancel, false);
	public static MessageBoxButton Abort  { get; } = new(DialogResult.Abort,  0, Resources.StrAbort,  false);
	public static MessageBoxButton Retry  { get; } = new(DialogResult.Retry,  0, Resources.StrRetry,  true);
	public static MessageBoxButton Ignore { get; } = new(DialogResult.Ignore, 0, Resources.StrIgnore, false);

	public static IReadOnlyList<MessageBoxButton> RetryCancel      { get; } = [Retry, Cancel];
	public static IReadOnlyList<MessageBoxButton> AbortRetryIgnore { get; } = [Abort, Retry, Ignore];
	public static IReadOnlyList<MessageBoxButton> YesNo            { get; } = [Yes, No];
	public static IReadOnlyList<MessageBoxButton> YesNoCancel      { get; } = [Yes, No, Cancel];
	public static IReadOnlyList<MessageBoxButton> OkCancel         { get; } = [Ok, Cancel];

	public static MessageBoxButton GetOk(string label, int resultOption = 0)
		=> new(DialogResult.OK, resultOption, label, isDefault: true);

	public static MessageBoxButton GetYes(string label, int resultOption = 0)
		=> new(DialogResult.Yes, resultOption, label, isDefault: true);

	public static IReadOnlyList<MessageBoxButton> GetButtons(MessageBoxButtons buttons)
		=> buttons switch
		{
			MessageBoxButtons.OK               => [Ok],
			MessageBoxButtons.OKCancel         => OkCancel,
			MessageBoxButtons.YesNo            => YesNo,
			MessageBoxButtons.YesNoCancel      => YesNoCancel,
			MessageBoxButtons.AbortRetryIgnore => AbortRetryIgnore,
			MessageBoxButtons.RetryCancel      => RetryCancel,
			_ => throw new ArgumentException($"Unknown MessageBoxButtons value: {buttons}", nameof(buttons)),
		};

	public string DisplayLabel { get; } = displayLabel;

	public int ResultOption { get; } = resultOption;

	public DialogResult DialogResult { get; } = dialogResult;

	public bool IsDefault { get; } = isDefault;
}
