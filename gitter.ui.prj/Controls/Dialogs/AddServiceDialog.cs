namespace gitter.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;

	using gitter.Framework;

	using Resources = gitter.Properties.Resources;

	public partial class AddServiceDialog : DialogBase, IExecutableDialog
	{
		#region Data

		private readonly IWorkingEnvironment _environment;
		private Dictionary<IRepositoryServiceProvider, Control> _setupControlCache;
		private Control _activeSetupControl;

		#endregion

		public AddServiceDialog(IWorkingEnvironment environment)
		{
			Verify.Argument.IsNotNull(environment, "environment");

			_environment = environment;
			_setupControlCache = new Dictionary<IRepositoryServiceProvider, Control>();

			InitializeComponent();

			Text = Resources.StrAddService;

			var hs = new HashSet<IRepositoryServiceProvider>(environment.ActiveIssueTrackerProviders);
			foreach(var prov in _environment.IssueTrackerProviders)
			{
				if(!hs.Contains(prov))
				{
					var item = new ServiceProviderListItem(prov);
					_servicePicker.ServiceProviders.Items.Add(item);
				}
			}

			_servicePicker.SelectedIndexChanged += OnSelectedProviderChanged;

			if(_servicePicker.ServiceProviders.Items.Count != 0)
			{
				var item = _servicePicker.ServiceProviders.Items[0];
				item.IsSelected = true;
				item.Activate();
			}
			else
			{
				_servicePicker.Enabled = false;
				_lblProvider.Enabled = false;
			}
		}

		private void OnSelectedProviderChanged(object sender, EventArgs e)
		{
			var prov = _servicePicker.SelectedIssueTracker;
			if(prov == null) return;
			Control setupControl;
			if(!_setupControlCache.TryGetValue(prov, out setupControl))
			{
				setupControl = prov.CreateSetupControl(_environment.ActiveRepository);
				_setupControlCache.Add(prov, setupControl);
			}
			int d = 0;
			if(_activeSetupControl != null)
			{
				d -= _activeSetupControl.Height;
				_activeSetupControl.Parent = null;
				_activeSetupControl = null;
			}
			_activeSetupControl = setupControl;
			if(_activeSetupControl != null)
			{
				d += _activeSetupControl.Height;
			}
			Height += d;
			if(_activeSetupControl != null)
			{
				_activeSetupControl.SetBounds(0, _servicePicker.Bottom + 3, Width, 0,
					BoundsSpecified.X | BoundsSpecified.Y | BoundsSpecified.Width);
				_activeSetupControl.Parent = this;
			}
		}

		protected override string ActionVerb
		{
			get { return Resources.StrAdd; }
		}

		public bool Execute()
		{
			var prov = _servicePicker.SelectedIssueTracker;
			if(prov == null) return false;
			var ctl = _activeSetupControl as IExecutableDialog;
			if(ctl != null)
			{
				if(!ctl.Execute()) return false;
			}
			if(!_environment.TryLoadIssueTracker(prov))
			{
				return false;
			}
			return true;
		}
	}
}
