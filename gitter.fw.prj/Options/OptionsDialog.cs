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
			Verify.Argument.IsNotNull(environment, nameof(environment));

			_environment = environment;

			InitializeComponent();

			Text = Resources.StrOptions;

			_lstOptions.Style = GitterApplication.DefaultStyle;
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
					RequireElevationChanged?.Invoke(this, EventArgs.Empty);
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
			RequireElevationChanged?.Invoke(this, EventArgs.Empty);
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
						if(page is IElevatedExecutableDialog elevated)
						{
							if(elevated.RequireElevation) return true;
						}
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
						if(page is IElevatedExecutableDialog elevated && elevated.RequireElevation)
						{
							var action = elevated.ElevatedExecutionAction;
							if(action != null) list.Add(action);
						}
					}
				}
				if(list.Count == 0) return null;
				if(list.Count == 1) return list[0];
				return (Action)Delegate.Combine(list.ToArray());
			}
		}

		#endregion
	}
}
