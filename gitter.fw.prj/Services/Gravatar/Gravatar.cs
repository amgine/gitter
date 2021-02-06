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
	using System.Drawing;

	public sealed class Gravatar : IAvatar
	{
		#region Data

		private volatile IAsyncResult _avatarLoading;
		private readonly object _sync = new object();
		private Bitmap _image;
		private string _email;

		#endregion

		#region Events

		public event EventHandler Updated;

		private void InvokeUpdated()
			=> Updated?.Invoke(this, EventArgs.Empty);

		#endregion

		public Gravatar(string email)
		{
			Verify.Argument.IsNotNull(email, nameof(email));

			_email = email;
		}

		public string Email
		{
			get { return _email; }
			set
			{
				Verify.Argument.IsNotNull(value, nameof(value));

				if(_email != value)
				{
					_email = value;
					_image = null;
					InvokeUpdated();
				}
			}
		}

		#region IAvatar

		public Image Image => _image;

		public bool IsLoaded => _image != null;

		public IAsyncResult BeginUpdate()
		{
			lock(_sync)
			{
				if(_avatarLoading != null) return _avatarLoading;
				_avatarLoading = GravatarService.BeginGetGravatar(
					OnAvatarLoaded, _email, DefaultGravatarType.wavatar, GravatarRating.g, 60);
				return _avatarLoading;
			}
		}

		public void EndUpdate(IAsyncResult ar)
		{
			Verify.Argument.IsNotNull(ar, nameof(ar));
			Verify.State.IsTrue(_avatarLoading != null, "No async operation is running.");

			GravatarService.EndGetGravatar(ar);
		}

		public void Update()
		{
			lock(_sync)
			{
				Verify.State.IsTrue(_avatarLoading != null, "Async update is already running.");
				try
				{
					_image = GravatarService.GetGravatar(
						_email, DefaultGravatarType.wavatar, GravatarRating.g, 60);
				}
				catch
				{
					_image = new Bitmap(1, 1);
				}
			}
			InvokeUpdated();
		}

		#endregion

		private void OnAvatarLoaded(IAsyncResult ar)
		{
			lock(_sync)
			{
				_avatarLoading = null;
				try
				{
					_image = GravatarService.EndGetGravatar(ar);
				}
				catch
				{
					_image = new Bitmap(1, 1);
				}
			}
			InvokeUpdated();
		}
	}
}
