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

namespace gitter.Framework.Mvc.WinForms;

using System.Collections.Generic;
using System.Windows.Forms;

using gitter.Framework.Services;

public class UserInputErrorNotifier : IUserInputErrorNotifier
{
	private readonly INotificationService _notificationService;
	private readonly IEnumerable<IUserInputSource> _inputSources;

	public UserInputErrorNotifier(INotificationService notificationService, IEnumerable<IUserInputSource> inputSources)
	{
		Verify.Argument.IsNotNull(notificationService);
		Verify.Argument.IsNotNull(inputSources);

		_notificationService = notificationService;
		_inputSources = inputSources;
	}

	private Control? GetControlFromInputSource(IUserInputSource userInputSource)
	{
		if(userInputSource is null) return null;

		foreach(var inputSource in _inputSources)
		{
			if(inputSource == userInputSource)
			{
				return (inputSource as IWin32ControlInputSource)?.Control;
			}
		}
		return default;
	}

	public void NotifyError(IUserInputSource userInputSource, UserInputError userInputError)
	{
		if(userInputError is null) return;

		var control = GetControlFromInputSource(userInputSource);
		if(control is not null)
		{
			_notificationService.NotifyInputError(control, userInputError.Title, userInputError.Message);
		}
	}
}
