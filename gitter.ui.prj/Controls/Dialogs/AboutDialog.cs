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

namespace gitter;

using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Services;

using Resources = gitter.Properties.Resources;

partial class AboutDialog : DialogBase
{
	private readonly IUpdateChannel _updateChannel;
	private IUpdateVersion? _latestVersion;

	public AboutDialog(IUpdateChannel updateChannel)
	{
		_updateChannel = updateChannel;

		InitializeComponent();

		if(GitterApplication.Style.Type == GitterStyleType.DarkBackground)
		{
			BackColor = Color.FromArgb(37, 37, 37);
		}

		linkLabel1.LinkColor       = GitterApplication.Style.Colors.HyperlinkText;
		linkLabel1.ActiveLinkColor = GitterApplication.Style.Colors.HyperlinkTextHotTrack;

		Text = Resources.StrAbout;
		labelVersion.Text = $"v{AssemblyVersion}";

		_pnlUpdates.Visible = HelperExecutables.CheckIfCanLaunchUpdater();

		Margin = Padding.Empty;
	}

	public override DialogButtons OptimalButtons => DialogButtons.Ok;

	#region Assembly Attribute Accessors

	private static T? GetAssemblyAttribute<T>() where T : Attribute
		=> Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(T), false) is { Length: > 0 } attributes
			? attributes[0] as T
			: default;

	public string AssemblyTitle
		=> GetAssemblyAttribute<AssemblyTitleAttribute>()?.Title
		?? System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);

	public string? AssemblyVersion => typeof(AboutDialog).Assembly.GetName().Version?.ToString();

	public string AssemblyDescription
		=> GetAssemblyAttribute<AssemblyDescriptionAttribute>()?.Description ?? string.Empty;

	public string AssemblyProduct
		=> GetAssemblyAttribute<AssemblyProductAttribute>()?.Product ?? string.Empty;

	public string AssemblyCopyright
		=> GetAssemblyAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? string.Empty;

	public string AssemblyCompany
		=> GetAssemblyAttribute<AssemblyCompanyAttribute>()?.Company ?? string.Empty;

	#endregion

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(500, 252));

	/// <inheritdoc/>
	public override IDpiBoundValue<Padding> ScalableMargin { get; } = DpiBoundValue.Constant(Padding.Empty);

	private static void OnEmailLinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
		=> Utility.OpenUrl(@"mailto://" + ((LinkLabel)sender!).Text);

	private async void OnCheckForUpdatesClick(object? sender, EventArgs e)
	{
		_btnCheckForUpdates.Visible = false;
		_lblUpdateStatus.Text = Resources.StrsCheckingForUpdates.AddEllipsis();
		_lblUpdateStatus.Visible = true;
		_latestVersion = default;
		try
		{
			_latestVersion = await _updateChannel.GetLatestVersionAsync();
		}
		catch
		{
		}
		if(IsDisposed) return;
		if(_latestVersion is not null)
		{
			var assembly   = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
			var asmVersion = assembly.GetName().Version;
			if(_latestVersion.Version <= asmVersion)
			{
				_lblUpdateStatus.Text = Resources.StrsYourVersionIsUpToDate;
			}
			else
			{
				_lblUpdateStatus.Text = Resources.StrsVersionIsAvailable.UseAsFormat(_latestVersion.Version);
				_btnUpdate.Visible = true;
			}
		}
		else
		{
			_lblUpdateStatus.Text = Resources.StrsCheckFailed;
		}
	}

	private void OnUpdateClick(object? sender, EventArgs e)
	{
		if(!HelperExecutables.CheckIfUpdaterIsRunning())
		{
			try
			{
				_latestVersion?.Update();
				_btnUpdate.Enabled = false;
			}
			catch(Exception exc) when(!exc.IsCritical)
			{
			}
		}
	}
}
