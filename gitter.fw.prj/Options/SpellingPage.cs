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

namespace gitter.Framework.Options;

using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using gitter.Framework.Controls;
using gitter.Framework.Layout;
using gitter.Framework.Services;

using Resources = gitter.Framework.Properties.Resources;

public partial class SpellingPage : PropertyPage, IExecutableDialog
{
	protected override bool ScaleChildren => false;

	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(521, 328));

	public static readonly new Guid Guid = new("ECAC85BA-3093-42E6-8142-EA4EEFC81D16");

	private const string NHunspellHomepage = @"http://nhunspell.sourceforge.net/";
	private const string DownloadUrl = @"http://ftp.osuosl.org/pub/openoffice/contrib/dictionaries/";

	readonly struct DialogControls
	{
		private readonly LabelControl label1;
		public readonly CustomListBox _lstDictionaries;
		//public readonly LinkLabel _lnkDownload;

		public DialogControls(IGitterStyle style)
		{
			style ??= GitterApplication.Style;

			label1 = new();
			_lstDictionaries = new()
			{
				Style          = style,
				HeaderStyle    = HeaderStyle.Hidden,
				ShowCheckBoxes = true,
				Text           = "No dictionaries found",
			};


			_lstDictionaries.Columns.Add(new CustomListBoxColumn(0, Resources.StrName) { SizeMode = ColumnSizeMode.Fill });
		}

		public void Localize()
		{
			label1.Text = "Dictionaries:";
		}

		public void Layout(Control parent)
		{
			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					rows:
					[
						LayoutConstants.LabelRowHeight,
						LayoutConstants.LabelRowSpacing,
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(label1,           marginOverride: LayoutConstants.NoMargin), row: 0),
						new GridContent(new ControlContent(_lstDictionaries, marginOverride: LayoutConstants.NoMargin), row: 2),
					]),
			};
			label1.Parent = parent;
			_lstDictionaries.Parent = parent;
		}
	}

	private readonly DialogControls _controls;

	public SpellingPage()
		: base(Guid)
	{
		Name = nameof(SpellingPage);
		Text = Resources.StrSpelling;

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Size                = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.Style);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		//_controls._lnkDownload.LinkClicked += _lnkDownload_LinkClicked;

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
			_controls._lstDictionaries.Items.Add(new CustomListBoxRow<string>(locale, new TextSubItem(0, cultureName))
				{
					IsChecked = loaded.Contains(locale),
				});
		}
	}

	public bool Execute()
	{
		foreach(CustomListBoxRow<string> item in _controls._lstDictionaries.Items)
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
