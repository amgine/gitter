namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>"Email" column.</summary>
	public class EmailColumn : CustomListBoxColumn
	{
		protected EmailColumn(int id, string name, bool visible)
			: base(id, name, visible)
		{
			Width = 80;
		}

		public EmailColumn(string name, bool visible)
			: this((int)ColumnId.Email, name, visible)
		{
		}

		public EmailColumn(bool visible)
			: this((int)ColumnId.Email, Resources.StrEmail, visible)
		{
		}

		public EmailColumn()
			: this((int)ColumnId.Email, Resources.StrEmail, true)
		{
		}

		public static Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs, string email)
		{
			return measureEventArgs.MeasureText(email);
		}

		public static void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs, string email)
		{
			paintEventArgs.PaintText(email);
		}

		public override string IdentificationString
		{
			get { return "Email"; }
		}
	}
}
