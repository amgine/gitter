namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;

	using Resources = gitter.Git.Gui.Properties.Resources;

	partial class RemoteView : GitViewBase
	{
		#region Data

		private RemoteToolbar _toolbar;
		private Remote _remote;

		#endregion

		public RemoteView(IDictionary<string, object> parameters, GuiProvider gui)
			: base(Guids.RemoteViewGuid, gui, parameters)
		{
			InitializeComponent();

			Text = Resources.StrRemote;

			ApplyParameters(parameters);

			AddTopToolStrip(_toolbar = new RemoteToolbar(this));
		}

		public override void ApplyParameters(IDictionary<string, object> parameters)
		{
			if(parameters != null)
			{
				object remote;
				parameters.TryGetValue("Remote", out remote);
				Remote = remote as Remote;
			}
			else
			{
				Remote = null;
			}
		}

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgRemote"]; }
		}

		public override bool IsDocument
		{
			get { return true; }
		}

		public Remote Remote
		{
			get { return _remote; }
			private set
			{
				if(IsDisposed) throw new ObjectDisposedException(GetType().Name);

				if(_remote != value)
				{
					if(_remote != null)
					{
						DetachRemote(_remote);
					}
					_remote = value;
					if(_remote != null)
					{
						AttachRemote(_remote);
					}

					UpdateText();
					FetchData();
				}
			}
		}

		public override void RefreshContent()
		{
			FetchData();
		}

		private void DetachRemote(Remote remote)
		{
			remote.Deleted -= OnRemoteDeleted;
			remote.Renamed -= OnRemoteRenamed;
		}

		private void AttachRemote(Remote remote)
		{
			remote.Deleted += OnRemoteDeleted;
			remote.Renamed += OnRemoteRenamed;
		}

		private void FetchData()
		{
			if(Remote == null)
			{
				_lstRemoteReferences.Load(null);
			}
			else
			{
				var action = AsyncAction.Create(
					Remote,
					(remote, monitor) =>
					{
						_lstRemoteReferences.Load(remote);
						_lstRemoteReferences.FetchData();
					},
					Resources.StrFetchingDataFrom.UseAsFormat(_remote.Name),
					string.Empty);
				action.BeginInvoke(this, _lstRemoteReferences.ProgressMonitor, OnFethCompleted, action);
			}
		}

		private void OnFethCompleted(IAsyncResult ar)
		{
			var action = (AsyncAction<Remote>)ar.AsyncState;
			try
			{
				action.EndInvoke(ar);
			}
			catch(GitException exc)
			{
				if(action.Data == Remote)
				{
					if(!_lstRemoteReferences.IsDisposed)
					{
						try
						{
							_lstRemoteReferences.BeginInvoke(new Action<string>(
								msg =>
								{
									if(!_lstRemoteReferences.IsDisposed)
									{
										_lstRemoteReferences.Text = msg;
									}
								}), exc.Message);
						}
						catch(ObjectDisposedException)
						{
						}
					}
				}
			}
		}

		private void UpdateText()
		{
			if(!IsDisposed)
			{
				if(Remote != null)
				{
					Text = Remote.Name;
				}
				else
				{
					Text = Resources.StrRemote;
				}
			}
		}

		private void OnRemoteRenamed(object sender, NameChangeEventArgs e)
		{
			if(!IsDisposed)
			{
				if(InvokeRequired)
				{
					try
					{
						BeginInvoke(new MethodInvoker(UpdateText));
					}
					catch(ObjectDisposedException)
					{
					}
				}
				else
				{
					UpdateText();
				}
			}
		}

		private void OnRemoteDeleted(object sender, EventArgs e)
		{
			if(!IsDisposed)
			{
				if(InvokeRequired)
				{
					try
					{
						BeginInvoke(new MethodInvoker(Close));
					}
					catch(ObjectDisposedException)
					{
					}
				}
				else
				{
					Close();
				}
			}
		}
	}
}
