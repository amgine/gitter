namespace gitter.Redmine.Gui.ListBoxes
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	public class NewsListBox : CustomListBox
	{
		public NewsListBox()
		{
			Columns.AddRange(
				new CustomListBoxColumn[]
				{
					new NewsIdColumn(),
					new NewsCreatedOnColumn(),
					new NewsTitleColumn(),
					new NewsAuthorColumn(),
				});
		}
	}
}
