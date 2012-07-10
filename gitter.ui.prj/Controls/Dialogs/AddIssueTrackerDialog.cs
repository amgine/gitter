namespace gitter.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;

	using gitter.Framework;

	using Resources = gitter.Properties.Resources;

	public partial class AddIssueTrackerDialog : DialogBase, IExecutableDialog
	{
		private readonly IWorkingEnvironment _environment;
		private Dictionary<IIssueTrackerProvider, Control> _setupControlCache;
		private Control _activeSetupControl;

		public AddIssueTrackerDialog(IWorkingEnvironment environment)
		{
			if(environment == null) throw new ArgumentNullException("environment");
			_environment = environment;
			_setupControlCache = new Dictionary<IIssueTrackerProvider, Control>();

			InitializeComponent();

			Text = Resources.StrAddIssueTracker;

			var hs = new HashSet<IIssueTrackerProvider>(environment.ActiveIssueTrackerProviders);
			foreach(var prov in _environment.IssueTrackerProviders)
			{
				if(!hs.Contains(prov))
				{
					var item = new IssueTrackerListItem(prov);
					_issueTracker.IssueTrackers.Items.Add(item);
				}
			}

			_issueTracker.SelectedIndexChanged += OnSelectedProviderChanged;

			if(_issueTracker.IssueTrackers.Items.Count != 0)
			{
				var item = _issueTracker.IssueTrackers.Items[0];
				item.IsSelected = true;
				item.Activate();
			}
			else
			{
				_issueTracker.Enabled = false;
				_lblProvider.Enabled = false;
			}
		}

		private void OnSelectedProviderChanged(object sender, EventArgs e)
		{
			var prov = _issueTracker.SelectedIssueTracker;
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
				_activeSetupControl.SetBounds(0, _issueTracker.Bottom + 3, Width, 0,
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
			var prov = _issueTracker.SelectedIssueTracker;
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
