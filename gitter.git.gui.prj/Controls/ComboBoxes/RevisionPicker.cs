namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Framework.Options;

	public class RevisionPicker : CustomPopupComboBox
	{
		private ReferencesListBox _lstReferences;
		private RevisionToolTip _revisionToolTip;

		public RevisionPicker()
		{
			_lstReferences = new ReferencesListBox()
			{
				HeaderStyle = HeaderStyle.Hidden,
				BorderStyle = BorderStyle.FixedSingle,
				ItemActivation = gitter.Framework.Controls.ItemActivation.SingleClick,
				Size = new Size(Width, 2 + 2 + 21 * 10),
				DisableContextMenus = true,
				Font = LicenseManager.UsageMode == LicenseUsageMode.Runtime ?
					GitterApplication.FontManager.UIFont.Font :
					SystemFonts.MessageBoxFont,
			};
			_lstReferences.ItemActivated += OnItemActivated;
			_revisionToolTip = new RevisionToolTip();

			DropDownControl = _lstReferences;
		}

		public ReferencesListBox References
		{
			get { return _lstReferences; }
		}

		private void OnItemActivated(object sender, ItemEventArgs e)
		{
			if(e.Item is BranchListItem)
			{
				var branch = ((BranchListItem)e.Item).DataContext;
				Text = branch.Name;
				HideDropDown();
			}
			if(e.Item is RemoteBranchListItem)
			{
				var branch = ((RemoteBranchListItem)e.Item).DataContext;
				Text = branch.Name;
				HideDropDown();
			}
			else if(e.Item is TagListItem)
			{
				var tag = ((TagListItem)e.Item).DataContext;
				Text = tag.Name;
				HideDropDown();
			}
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			var repository = References.Repository;
			if(repository != null)
			{
				try
				{
					var p = repository.GetRevisionPointer(Text.Trim());
					var revision = p.Dereference();
					if(!revision.IsLoaded)
					{
						revision.Load();
					}
					_revisionToolTip.Revision = revision;
					_revisionToolTip.Show(this, new Point(0, Height + 1));
				}
				catch
				{
					_revisionToolTip.Revision = null;
				}
			}
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			_revisionToolTip.Hide(this);
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_lstReferences != null)
				{
					_lstReferences.LoadData(null);
					_lstReferences.ItemActivated -= OnItemActivated;
					_lstReferences.Dispose();
					_lstReferences = null;
				}
				if(_revisionToolTip != null)
				{
					_revisionToolTip.Dispose();
					_revisionToolTip = null;
				}
			}
			base.Dispose(disposing);
		}

		public override int DropDownHeight
		{
			get { return _lstReferences.Height; }
			set { _lstReferences.Height = value; }
		}
	}
}
