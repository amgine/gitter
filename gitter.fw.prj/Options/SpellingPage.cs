namespace gitter.Framework.Options
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Globalization;
	using System.Windows.Forms;

	using gitter.Framework.Controls;
	using gitter.Framework.Services;

	using Resources = gitter.Framework.Properties.Resources;

	public partial class SpellingPage : PropertyPage, IExecutableDialog
	{
		public static readonly new Guid Guid = new Guid("ECAC85BA-3093-42E6-8142-EA4EEFC81D16");

		private const string NHunspellHomepage = @"http://nhunspell.sourceforge.net/";
		private const string DownloadUrl = @"http://ftp.osuosl.org/pub/openoffice/contrib/dictionaries/";

		public SpellingPage()
			: base(Guid)
		{
			InitializeComponent();

			Text = Resources.StrSpelling;

			_lstDictionaries.Style = GitterApplication.DefaultStyle;
			_lstDictionaries.Columns.Add(new CustomListBoxColumn(0, Resources.StrName) { SizeMode = ColumnSizeMode.Fill });

			var loaded = SpellingService.GetLoadedLocales();
			foreach(var locale in SpellingService.GetAvailableLocales())
			{
				string cultureName = locale;
				try
				{
					var ci = CultureInfo.GetCultureInfo(locale.ToLower().Replace('_', '-'));
					cultureName = ci.DisplayName;
				}
				catch { }
				_lstDictionaries.Items.Add(new CustomListBoxRow<string>(locale, new TextSubItem(0, cultureName))
					{
						IsChecked = loaded.Contains(locale),
					});
			}
		}

		public bool Execute()
		{
			foreach(CustomListBoxRow<string> item in _lstDictionaries.Items)
			{
				bool loaded = SpellingService.IsLoaded(item.DataContext);
				if(loaded)
				{
					if(!item.IsChecked)
					{
						SpellingService.UnloadLocale(item.DataContext);
					}
				}
				else
				{
					if(item.IsChecked)
					{
						SpellingService.LoadLocale(item.DataContext);
					}
				}
			}
			return true;
		}

		private void _lnkDownload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Utility.OpenUrl(DownloadUrl);
		}

		private void _picLogo_Click(object sender, EventArgs e)
		{
			Utility.OpenUrl(NHunspellHomepage);
		}
	}
}
