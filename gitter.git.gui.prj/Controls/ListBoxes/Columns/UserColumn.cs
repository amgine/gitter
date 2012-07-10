namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;

	using gitter.Framework.Controls;
	using gitter.Framework.Configuration;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>"User" columnn.</summary>
	public class UserColumn : CustomListBoxColumn
	{
		public const bool DefaultShowEmail = false;

		private const string EmailFormat = "{0} <{1}>";

		#region Data

		private bool _showEmail;
		private UserColumnExtender _extender;

		#endregion

		#region Events

		public event EventHandler ShowEmailChanged;

		#endregion

		#region .ctor

		protected UserColumn(int id, string name, bool visible)
			: base(id, name, visible)
		{
			Width = 80;

			_showEmail = DefaultShowEmail;
		}

		public UserColumn(string name, bool visible)
			: this((int)ColumnId.User, name, visible)
		{
		}

		public UserColumn(bool visible)
			: this((int)ColumnId.User, Resources.StrUser, visible)
		{
		}

		public UserColumn()
			: this((int)ColumnId.User, Resources.StrUser, true)
		{
		}

		#endregion

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			_extender = new UserColumnExtender(this);
			Extender = new Popup(_extender);
		}

		protected override void OnListBoxDetached()
		{
			base.OnListBoxDetached();
			Extender.Dispose();
			Extender = null;
			_extender.Dispose();
			_extender = null;
		}

		public bool ShowEmail
		{
			get { return _showEmail; }
			set
			{
				if(_showEmail != value)
				{
					_extender.ShowEmail = value;
					_showEmail = value;
					AutoSize(80);
					ShowEmailChanged.Raise(this);
				}
			}
		}

		public static Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs, User user)
		{
			return OnMeasureSubItem(measureEventArgs, user.Name, user.Email);
		}

		public static Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs, string userName, string userEmail)
		{
			bool showEmail;
			var rcc = measureEventArgs.Column as UserColumn;
			if(rcc != null)
			{
				showEmail = rcc.ShowEmail;
			}
			else
			{
				showEmail = UserColumn.DefaultShowEmail;
			}
			return measureEventArgs.MeasureText(showEmail ? (string.Format(EmailFormat, userName, userEmail)) : (userName));
		}

		public static void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs, User user)
		{
			OnPaintSubItem(paintEventArgs, user.Name, user.Email);
		}

		public static void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs, User user, Brush textBrush)
		{
			OnPaintSubItem(paintEventArgs, user.Name, user.Email, textBrush);
		}

		public static void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs, string userName, string userEmail)
		{
			OnPaintSubItem(paintEventArgs, userName, userEmail, paintEventArgs.Column.ContentBrush);
		}

		public static void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs, string userName, string userEmail, Brush textBrush)
		{
			bool showEmail = false;
			var uc = paintEventArgs.Column as UserColumn;
			if(uc != null)
			{
				showEmail = uc.ShowEmail;
			}
			else
			{
				showEmail = UserColumn.DefaultShowEmail;
			}
			paintEventArgs.PaintText(showEmail ? (string.Format(EmailFormat, userName, userEmail)) : (userName), textBrush);
		}

		protected override void SaveMoreTo(Section section)
		{
			base.SaveMoreTo(section);
			section.SetValue("ShowEmail", ShowEmail);
		}

		protected override void LoadMoreFrom(Section section)
		{
			base.LoadMoreFrom(section);
			ShowEmail = section.GetValue("ShowEmail", ShowEmail);
		}

		public override string IdentificationString
		{
			get { return "User"; }
		}
	}
}
