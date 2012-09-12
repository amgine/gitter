namespace gitter.Framework.Options
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Framework.Properties.Resources;

	[ToolboxItem(false)]
	public partial class OptionsDialog : DialogBase, IExecutableDialog, IElevatedExecutableDialog
	{
		private readonly Dictionary<Guid, PropertyPage> _propertyPages;
		private readonly IWorkingEnvironment _environment;
		private PropertyPage _activePage;

		public OptionsDialog(IWorkingEnvironment environment)
		{
			Verify.Argument.IsNotNull(environment, "environment");

			_environment = environment;

			InitializeComponent();

			Text = Resources.StrOptions;

			_propertyPages = new Dictionary<Guid, PropertyPage>();
		}

		public override DialogButtons OptimalButtons
		{
			get { return DialogButtons.All; }
		}

		protected override void OnShown()
		{
			if(_lstOptions.Items.Count != 0)
			{
				var item = _lstOptions.Items[0];
				item.IsSelected = true;
				item.Activate();
			}
		}

		private void OnItemActivated(object sender, ItemEventArgs e)
		{
			var desc = (e.Item as PropertyPageItem).DataContext;
			PropertyPage page;
			if(_activePage != null)
			{
				if(_activePage.Guid == desc.Guid) return;
			}
			if(!_propertyPages.TryGetValue(desc.Guid, out page))
			{
				page = desc.CreatePropertyPage(_environment);
				bool raiseElevatedChanged = false;
				if(page != null)
				{
					var elevated = page as IElevatedExecutableDialog;
					if(elevated != null)
					{
						elevated.RequireElevationChanged += OnRequireElevationChanged;
						if(!RequireElevation && elevated.RequireElevation)
						{
							raiseElevatedChanged = true;
						}
					}
				}
				_propertyPages.Add(desc.Guid, page);
				if(raiseElevatedChanged)
				{
					RequireElevationChanged.Raise(this);
				}
			}
			if(page != null)
			{
				page.Dock = DockStyle.Fill;
				page.Parent = _pnlPageContainer;
				page.InvokeOnShown();
			}
			if(_activePage != null)
			{
				_activePage.Parent = null;
			}
			_activePage = page;
		}

		private void OnRequireElevationChanged(object sender, EventArgs e)
		{
			bool require = false;
			foreach(var page in _propertyPages.Values)
			{
				if(page != null && page != sender)
				{
					var elevated = page as IElevatedExecutableDialog;
					if(elevated != null)
					{
						if(elevated.RequireElevation)
						{
							require = true;
							break;
						}
					}
				}
			}
			if(require) return;
			RequireElevationChanged.Raise(this);
		}

		#region IExecutableDialog Members

		public bool Execute()
		{
			bool res = true;
			foreach(var page in _propertyPages.Values)
			{
				if(page != null)
				{
					var executable = page as IExecutableDialog;
					if(executable != null)
					{
						if(!executable.Execute())
						{
							res = false;
						}
					}
				}
			}
			return res;
		}

		#endregion

		#region IElevatedExecutableDialog Members

		public event EventHandler RequireElevationChanged;

		public bool RequireElevation
		{
			get
			{
				foreach(var page in _propertyPages.Values)
				{
					if(page != null)
					{
						var elevated = page as IElevatedExecutableDialog;
						if(elevated != null)
							if(elevated.RequireElevation) return true;
					}
				}
				return false;
			}
		}

		public Action ElevatedExecutionAction
		{
			get
			{
				var list = new List<Action>(_propertyPages.Count);
				foreach(var page in _propertyPages.Values)
				{
					if(page != null)
					{
						var elevated = page as IElevatedExecutableDialog;
						if(elevated != null && elevated.RequireElevation)
						{
							var action = elevated.ElevatedExecutionAction;
							if(action != null) list.Add(action);
						}
					}
				}
				if(list.Count == 0) return null;
				if(list.Count == 1) return list[0];
				return (Action)MulticastDelegate.Combine(list.ToArray());
			}
		}

		#endregion
	}
}
