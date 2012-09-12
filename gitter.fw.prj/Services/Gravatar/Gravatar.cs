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
		{
			var handler = Updated;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		#endregion

		public Gravatar(string email)
		{
			Verify.Argument.IsNotNull(email, "email");

			_email = email;
		}

		public string Email
		{
			get { return _email; }
			set
			{
				Verify.Argument.IsNotNull(value, "value");

				if(_email != value)
				{
					_email = value;
					_image = null;
					InvokeUpdated();
				}
			}
		}

		#region IAvatar

		public Image Image
		{
			get { return _image; }
		}

		public bool IsLoaded
		{
			get { return _image != null; }
		}

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
			Verify.Argument.IsNotNull(ar, "ar");
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
