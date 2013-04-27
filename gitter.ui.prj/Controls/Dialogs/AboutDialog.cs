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

namespace gitter
{
	using System;
	using System.Text;
	using System.Reflection;
	using System.Windows.Forms;
	using System.Drawing;
	using System.Drawing.Drawing2D;

	using gitter.Framework;
	using gitter.Framework.Services;

	using Resources = gitter.Properties.Resources;

	public partial class AboutDialog : DialogBase
	{
		private IUpdateChannel _updateChannel;

		public AboutDialog()
		{
			InitializeComponent();

			this.Text = Resources.StrAbout;
			this.labelVersion.Text = String.Format("v{0}", AssemblyVersion);

			_pnlUpdates.Visible = HelperExecutables.CheckIfCanLaunchUpdater();

			_updateChannel = new gitter.Git.GitRepositoryUpdateChannel("git://github.com/amgine/gitter.git");

			Margin = new Padding(0, 0, 0, 0);
		}

		public override DialogButtons OptimalButtons
		{
			get { return DialogButtons.Ok; }
		}

		#region Assembly Attribute Accessors

		public string AssemblyTitle
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				if(attributes.Length > 0)
				{
					var titleAttribute = (AssemblyTitleAttribute)attributes[0];
					if(titleAttribute.Title != string.Empty)
					{
						return titleAttribute.Title;
					}
				}
				return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
			}
		}

		public string AssemblyVersion
		{
			get
			{
				return Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
		}

		public string AssemblyDescription
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
				if(attributes.Length == 0)
				{
					return string.Empty;
				}
				return ((AssemblyDescriptionAttribute)attributes[0]).Description;
			}
		}

		public string AssemblyProduct
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
				if(attributes.Length == 0)
				{
					return string.Empty;
				}
				return ((AssemblyProductAttribute)attributes[0]).Product;
			}
		}

		public string AssemblyCopyright
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
				if(attributes.Length == 0)
				{
					return string.Empty;
				}
				return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
			}
		}

		public string AssemblyCompany
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
				if(attributes.Length == 0)
				{
					return string.Empty;
				}
				return ((AssemblyCompanyAttribute)attributes[0]).Company;
			}
		}

		#endregion

		private static void OnEmailLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Utility.OpenUrl(@"mailto://" + ((LinkLabel)sender).Text);
		}

		private void OnVersionCheckCompleted(IAsyncResult ar)
		{
			var f = (Func<Version>)ar.AsyncState;
			Version version = null;
			try
			{
				version = f.EndInvoke(ar);
			}
			catch
			{
			}
			if(IsDisposed) return;
			try
			{
				BeginInvoke(new Action<Version>(OnVersionCheckCompleted), version);
			}
			catch
			{
			}
		}

		private void OnVersionCheckCompleted(Version version)
		{
			if(IsDisposed) return;
			if(version != null)
			{
				var asmVersion = Assembly.GetExecutingAssembly().GetName().Version;
				if(version <= asmVersion)
				{
					_lblUpdateStatus.Text = Resources.StrsYourVersionIsUpToDate;
				}
				else
				{
					_lblUpdateStatus.Text = Resources.StrsVersionIsAvailable.UseAsFormat(version);
					_btnUpdate.Visible = true;
				}
			}
			else
			{
				_lblUpdateStatus.Text = Resources.StrsCheckFailed;
			}
		}

		private void OnCheckForUpdatesClick(object sender, EventArgs e)
		{
			_btnCheckForUpdates.Visible = false;
			_lblUpdateStatus.Text = Resources.StrsCheckingForUpdates.AddEllipsis();
			_lblUpdateStatus.Visible = true;
			var f = new Func<Version>(_updateChannel.CheckVersion);
			f.BeginInvoke(OnVersionCheckCompleted, f);
		}

		private void OnUpdateClick(object sender, EventArgs e)
		{
			if(!HelperExecutables.CheckIfUpdaterIsRunning())
			{
				try
				{
					_updateChannel.Update();
					_btnUpdate.Enabled = false;
				}
				catch
				{
				}
			}
		}
	}
}
