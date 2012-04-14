namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	using Resources = gitter.Git.Properties.Resources;

	using gitter.Framework.Controls;

	[ToolboxItem(false)]
	public partial class SubjectColumnExtender : BaseExtender
	{
		private readonly SubjectColumn _column;
		private bool _disableEvents;

		public SubjectColumnExtender(SubjectColumn column)
		{
			if(column == null) throw new ArgumentNullException("column");
			_column = column;

			InitializeComponent();

			_chkAlignToGraph.Text		= Resources.StrAlignToGraph;
			_grpVisibleReferences.Text	= Resources.StrVisibleReferences;
			_chkLocalBranches.Text		= Resources.StrLocalBranches;
			_chkRemoteBranches.Text		= Resources.StrRemoteBranches;
			_chkTags.Text				= Resources.StrTags;
			_chkStash.Text				= Resources.StrStash;

			_chkAlignToGraph.Image		= CachedResources.Bitmaps["ImgAlignToGraph"];
			_chkLocalBranches.Image		= CachedResources.Bitmaps["ImgBranch"];
			_chkRemoteBranches.Image	= CachedResources.Bitmaps["ImgBranchRemote"];
			_chkTags.Image				= CachedResources.Bitmaps["ImgTag"];
			_chkStash.Image				= CachedResources.Bitmaps["ImgStash"];

			UpdateStates();
			SubscribeToColumnEvents();
		}

		private void SubscribeToColumnEvents()
		{
			_column.AlignToGraphChanged += OnColumnAlignToGraphChanged;
			_column.ShowLocalBranchesChanged += OnColumnShowLocalBranchesChanged;
			_column.ShowRemoteBranchesChanged += OnColumnShowRemoteBranchesChanged;
			_column.ShowTagsChanged += OnColumnShowTagsChanged;
			_column.ShowStashChanged += OnColumnShowStashChanged;
		}

		private void UnsubscribeFromColumnEvents()
		{
			_column.AlignToGraphChanged -= OnColumnAlignToGraphChanged;
			_column.ShowLocalBranchesChanged -= OnColumnShowLocalBranchesChanged;
			_column.ShowRemoteBranchesChanged -= OnColumnShowRemoteBranchesChanged;
			_column.ShowTagsChanged -= OnColumnShowTagsChanged;
			_column.ShowStashChanged -= OnColumnShowStashChanged;
		}

		private void OnColumnAlignToGraphChanged(object sender, EventArgs e)
		{
			AlignToGraph = _column.AlignToGraph;
		}

		private void OnColumnShowLocalBranchesChanged(object sender, EventArgs e)
		{
			ShowLocalBranches = _column.ShowLocalBranches;
		}

		private void OnColumnShowRemoteBranchesChanged(object sender, EventArgs e)
		{
			ShowRemoteBranches = _column.ShowRemoteBranches;
		}

		private void OnColumnShowTagsChanged(object sender, EventArgs e)
		{
			ShowTags = _column.ShowTags;
		}

		private void OnColumnShowStashChanged(object sender, EventArgs e)
		{
			ShowStash = _column.ShowStash;
		}

		public void UpdateStates()
		{
			_disableEvents = true;
			_chkAlignToGraph.Checked = _column.AlignToGraph;
			_chkLocalBranches.Checked = _column.ShowLocalBranches;
			_chkRemoteBranches.Checked = _column.ShowRemoteBranches;
			_chkTags.Checked = _column.ShowTags;
			_chkStash.Checked = _column.ShowStash;
			_disableEvents = false;
		}

		public bool AlignToGraph
		{
			get { return _chkAlignToGraph.Checked; }
			set
			{
				_disableEvents = true;
				_chkAlignToGraph.Checked = value;
				_disableEvents = false;
			}
		}

		public bool ShowLocalBranches
		{
			get { return _chkLocalBranches.Checked; }
			set
			{
				_disableEvents = true;
				_chkLocalBranches.Checked = value;
				_disableEvents = false;
			}
		}

		public bool ShowRemoteBranches
		{
			get { return _chkRemoteBranches.Checked; }
			set
			{
				_disableEvents = true;
				_chkRemoteBranches.Checked = value;
				_disableEvents = false;
			}
		}

		public bool ShowTags
		{
			get { return _chkTags.Checked; }
			set
			{
				_disableEvents = true;
				_chkTags.Checked = value;
				_disableEvents = false;
			}
		}

		public bool ShowStash
		{
			get { return _chkStash.Checked; }
			set
			{
				_disableEvents = true;
				_chkStash.Checked = value;
				_disableEvents = false;
			}
		}

		private void OnAlignToGraphCheckedChanged(object sender, EventArgs e)
		{
			if(!_disableEvents)
			{
				_column.AlignToGraph = ((CheckBox)sender).Checked;
			}
		}

		private void OnLocalBranchesCheckedChanged(object sender, EventArgs e)
		{
			if(!_disableEvents)
			{
				_column.ShowLocalBranches = ((CheckBox)sender).Checked;
			}
		}

		private void OnRemoteBranchesCheckedChanged(object sender, EventArgs e)
		{
			if(!_disableEvents)
			{
				_column.ShowRemoteBranches = ((CheckBox)sender).Checked;
			}
		}

		private void OnTagsCheckedChanged(object sender, EventArgs e)
		{
			if(!_disableEvents)
			{
				_column.ShowTags = ((CheckBox)sender).Checked;
			}
		}

		private void OnStashCheckedChanged(object sender, EventArgs e)
		{
			if(!_disableEvents)
			{
				_column.ShowStash = ((CheckBox)sender).Checked;
			}
		}
	}
}
