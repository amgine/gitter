namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>Extender for <see cref="UserColumn"/>.</summary>
	[ToolboxItem(false)]
	public partial class UserColumnExtender : BaseExtender
	{
		private readonly UserColumn _column;
		private bool _disableEvents;

		/// <summary>Create <see cref="UserColumnExtender"/>.</summary>
		/// <param name="column">Related column.</param>
		public UserColumnExtender(UserColumn column)
		{
			if(column == null) throw new ArgumentNullException("column");
			_column = column;

			InitializeComponent();

			_chkShowEmail.Text = Resources.StrShowEmail;
			_chkShowEmail.Image = CachedResources.Bitmaps["ImgMail"];

			ShowEmail = column.ShowEmail;
			SubscribeToColumnEvents();
		}

		private void SubscribeToColumnEvents()
		{
			_column.ShowEmailChanged += OnColumnShowEmailChanged;
		}

		private void UnsubscribeFromColumnEvents()
		{
			_column.ShowEmailChanged -= OnColumnShowEmailChanged;
		}

		private void OnColumnShowEmailChanged(object sender, EventArgs e)
		{
			ShowEmail = _column.ShowEmail;
		}

		public bool ShowEmail
		{
			get { return _chkShowEmail.Checked; }
			set
			{
				_disableEvents = true;
				_chkShowEmail.Checked = value;
				_disableEvents = false;
			}
		}

		private void OnShowEmailCheckedChanged(object sender, EventArgs e)
		{
			if(!_disableEvents)
			{
				_column.ShowEmail = ((CheckBox)sender).Checked;
			}
		}
	}
}
