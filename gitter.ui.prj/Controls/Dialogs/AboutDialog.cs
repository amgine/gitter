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

			_pnlUpdates.Visible = Utility.CheckIfCanLaunchUpdater();
			_btnUpdate.ShowUACShield();

			_updateChannel = new gitter.Git.GitRepositoryUpdateChannel("git@github.com:amgine/gitter.git");

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
					AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
					if(titleAttribute.Title != "")
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
					return "";
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
					return "";
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
					return "";
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
					return "";
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
					_lblUpdateStatus.Text = "Your version is up to date";
				}
				else
				{
					_lblUpdateStatus.Text = "v." + version.ToString() + " is available";
					_btnUpdate.Visible = true;
				}
			}
			else
			{
				_lblUpdateStatus.Text = "Check failed";
			}
		}

		private void OnCheckForUpdatesClick(object sender, EventArgs e)
		{
			_btnCheckForUpdates.Visible = false;
			_lblUpdateStatus.Text = "Checking for updates...";
			_lblUpdateStatus.Visible = true;
			var f = new Func<Version>(_updateChannel.CheckVersion);
			f.BeginInvoke(OnVersionCheckCompleted, f);
		}

		private void OnUpdateClick(object sender, EventArgs e)
		{
			if(!Utility.CheckIfUpdaterIsRunning())
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
