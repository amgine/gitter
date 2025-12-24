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
using System.Drawing;
using System.Threading.Tasks;

public sealed class Gravatar : IAvatar
{
	private static readonly Lazy<Bitmap> _failedImage = new(() => new Bitmap(1, 1), System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);

	#region Data

	private string _email;
	private Task<Image?>? _task;

	#endregion

	#region Events

	public event EventHandler? Updated;

	private void InvokeUpdated()
		=> Updated?.Invoke(this, EventArgs.Empty);

	#endregion

	public Gravatar(string email)
	{
		Verify.Argument.IsNotNull(email);

		_email = email;
	}

	public string Email
	{
		get => _email;
		set
		{
			Verify.Argument.IsNotNull(value);

			if(_email != value)
			{
				_email = value;
				Image = null;
				InvokeUpdated();
			}
		}
	}

	public bool IsAvailable => !string.IsNullOrEmpty(Email);

	#region IAvatar

	public Image? Image { get; private set; }

	public bool IsLoaded => Image is not null;

	public Task<Image?> UpdateAsync()
	{
		if(_task is not null) return _task;
		_task = UpdateCoreAsync();
		return _task;
	}

	private async Task<Image?> UpdateCoreAsync()
	{
		try
		{
			Image = await GravatarService
				.GetGravatarAsync(_email, DefaultGravatarType.wavatar, GravatarRating.g, 60)
				.ConfigureAwait(continueOnCapturedContext: false);
		}
		catch
		{
			Image = _failedImage.Value;
		}
		return Image;
	}

	#endregion
}
