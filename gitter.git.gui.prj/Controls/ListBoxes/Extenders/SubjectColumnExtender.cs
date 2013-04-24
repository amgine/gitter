namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Drawing;

	using Resources = gitter.Git.Gui.Properties.Resources;

	using gitter.Framework.Controls;

	[ToolboxItem(false)]
	partial class SubjectColumnExtender : ExtenderBase
	{
		#region Data

		private bool _disableEvents;

		private GroupSeparator _grpVisibleReferences;
		private ICheckBoxWidget _chkAlignToGraph;
		private ICheckBoxWidget _chkLocalBranches;
		private ICheckBoxWidget _chkRemoteBranches;
		private ICheckBoxWidget _chkTags;
		private ICheckBoxWidget _chkStash;

		#endregion

		#region .ctor

		public SubjectColumnExtender(SubjectColumn column)
			: base(column)
		{
			InitializeComponent();
			CreateControls();
			SubscribeToColumnEvents();
		}

		#endregion

		#region Methods

		private void CreateControls()
		{
			const int height = 27;
			const int hmargin = 6;
			const int vspacing = -4;
			int width = Width - hmargin * 2;
			int yoffset = 0;

			if(_chkAlignToGraph != null) _chkAlignToGraph.Dispose();
			_chkAlignToGraph = Style.CreateCheckBox();
			_chkAlignToGraph.Text = Resources.StrAlignToGraph;
			_chkAlignToGraph.Image = CachedResources.Bitmaps["ImgAlignToGraph"];
			_chkAlignToGraph.Control.Bounds = new Rectangle(hmargin, yoffset, width, height);
			_chkAlignToGraph.Control.Parent = this;
			_chkAlignToGraph.IsChecked = Column.AlignToGraph;
			_chkAlignToGraph.IsCheckedChanged += OnAlignToGraphCheckedChanged;
			yoffset += height + vspacing;

			if(_grpVisibleReferences == null)
			{
				_grpVisibleReferences = new GroupSeparator()
				{
					Text = Resources.StrVisibleReferences,
					Bounds = new Rectangle(0, yoffset + 2, Width - hmargin, height),
					Parent = this,
				};
			}
			yoffset += height + vspacing;

			if(_chkLocalBranches != null) _chkLocalBranches.Dispose();
			_chkLocalBranches = Style.CreateCheckBox();
			_chkLocalBranches.Text = Resources.StrLocalBranches;
			_chkLocalBranches.Image = CachedResources.Bitmaps["ImgBranch"];
			_chkLocalBranches.Control.Bounds = new Rectangle(hmargin, yoffset, width, height);
			_chkLocalBranches.Control.Parent = this;
			_chkLocalBranches.IsChecked = Column.ShowLocalBranches;
			_chkLocalBranches.IsCheckedChanged += OnLocalBranchesCheckedChanged;
			yoffset += height + vspacing;

			if(_chkRemoteBranches != null) _chkRemoteBranches.Dispose();
			_chkRemoteBranches = Style.CreateCheckBox();
			_chkRemoteBranches.Text = Resources.StrRemoteBranches;
			_chkRemoteBranches.Image = CachedResources.Bitmaps["ImgBranchRemote"];
			_chkRemoteBranches.Control.Bounds = new Rectangle(hmargin, yoffset, width, height);
			_chkRemoteBranches.Control.Parent = this;
			_chkRemoteBranches.IsChecked = Column.ShowRemoteBranches;
			_chkRemoteBranches.IsCheckedChanged += OnRemoteBranchesCheckedChanged;
			yoffset += height + vspacing;

			if(_chkTags != null) _chkTags.Dispose();
			_chkTags = Style.CreateCheckBox();
			_chkTags.Text = Resources.StrTags;
			_chkTags.Image = CachedResources.Bitmaps["ImgTag"];
			_chkTags.Control.Bounds = new Rectangle(hmargin, yoffset, width, height);
			_chkTags.Control.Parent = this;
			_chkTags.IsChecked = Column.ShowTags;
			_chkTags.IsCheckedChanged += OnTagsCheckedChanged;
			yoffset += height + vspacing;

			if(_chkStash != null) _chkStash.Dispose();
			_chkStash = Style.CreateCheckBox();
			_chkStash.Text = Resources.StrStash;
			_chkStash.Image = CachedResources.Bitmaps["ImgStash"];
			_chkStash.Control.Bounds = new Rectangle(hmargin, yoffset, width, height);
			_chkStash.Control.Parent = this;
			_chkStash.IsChecked = Column.ShowStash;
			_chkStash.IsCheckedChanged += OnStashCheckedChanged;
		}

		private void SubscribeToColumnEvents()
		{
			Column.AlignToGraphChanged += OnColumnAlignToGraphChanged;
			Column.ShowLocalBranchesChanged += OnColumnShowLocalBranchesChanged;
			Column.ShowRemoteBranchesChanged += OnColumnShowRemoteBranchesChanged;
			Column.ShowTagsChanged += OnColumnShowTagsChanged;
			Column.ShowStashChanged += OnColumnShowStashChanged;
		}

		private void UnsubscribeFromColumnEvents()
		{
			Column.AlignToGraphChanged -= OnColumnAlignToGraphChanged;
			Column.ShowLocalBranchesChanged -= OnColumnShowLocalBranchesChanged;
			Column.ShowRemoteBranchesChanged -= OnColumnShowRemoteBranchesChanged;
			Column.ShowTagsChanged -= OnColumnShowTagsChanged;
			Column.ShowStashChanged -= OnColumnShowStashChanged;
		}

		public void UpdateStates()
		{
			_disableEvents = true;
			_chkAlignToGraph.IsChecked = Column.AlignToGraph;
			_chkLocalBranches.IsChecked = Column.ShowLocalBranches;
			_chkRemoteBranches.IsChecked = Column.ShowRemoteBranches;
			_chkTags.IsChecked = Column.ShowTags;
			_chkStash.IsChecked = Column.ShowStash;
			_disableEvents = false;
		}

		protected override void OnStyleChanged()
		{
			base.OnStyleChanged();
			CreateControls();
		}

		#endregion

		#region EVent Handlers

		private void OnColumnAlignToGraphChanged(object sender, EventArgs e)
		{
			AlignToGraph = Column.AlignToGraph;
		}

		private void OnColumnShowLocalBranchesChanged(object sender, EventArgs e)
		{
			ShowLocalBranches = Column.ShowLocalBranches;
		}

		private void OnColumnShowRemoteBranchesChanged(object sender, EventArgs e)
		{
			ShowRemoteBranches = Column.ShowRemoteBranches;
		}

		private void OnColumnShowTagsChanged(object sender, EventArgs e)
		{
			ShowTags = Column.ShowTags;
		}

		private void OnColumnShowStashChanged(object sender, EventArgs e)
		{
			ShowStash = Column.ShowStash;
		}

		private void OnAlignToGraphCheckedChanged(object sender, EventArgs e)
		{
			if(!_disableEvents)
			{
				_disableEvents = true;
				Column.AlignToGraph = ((ICheckBoxWidget)sender).IsChecked;
				_disableEvents = false;
			}
		}

		private void OnLocalBranchesCheckedChanged(object sender, EventArgs e)
		{
			if(!_disableEvents)
			{
				_disableEvents = true;
				Column.ShowLocalBranches = ((ICheckBoxWidget)sender).IsChecked;
				_disableEvents = false;
			}
		}

		private void OnRemoteBranchesCheckedChanged(object sender, EventArgs e)
		{
			if(!_disableEvents)
			{
				_disableEvents = true;
				Column.ShowRemoteBranches = ((ICheckBoxWidget)sender).IsChecked;
				_disableEvents = false;
			}
		}

		private void OnTagsCheckedChanged(object sender, EventArgs e)
		{
			if(!_disableEvents)
			{
				_disableEvents = true;
				Column.ShowTags = ((ICheckBoxWidget)sender).IsChecked;
				_disableEvents = false;
			}
		}

		private void OnStashCheckedChanged(object sender, EventArgs e)
		{
			if(!_disableEvents)
			{
				_disableEvents = true;
				Column.ShowStash = ((ICheckBoxWidget)sender).IsChecked;
				_disableEvents = false;
			}
		}

		#endregion

		#region Properties

		public new SubjectColumn Column
		{
			get { return (SubjectColumn)base.Column; }
		}

		public bool AlignToGraph
		{
			get { return _chkAlignToGraph != null ? _chkAlignToGraph.IsChecked : Column.AlignToGraph; }
			private set
			{
				if(_chkAlignToGraph != null)
				{
					_disableEvents = true;
					_chkAlignToGraph.IsChecked = value;
					_disableEvents = false;
				}
			}
		}

		public bool ShowLocalBranches
		{
			get { return _chkLocalBranches != null ? _chkLocalBranches.IsChecked : Column.ShowLocalBranches; }
			private set
			{
				if(_chkLocalBranches != null)
				{
					_disableEvents = true;
					_chkLocalBranches.IsChecked = value;
					_disableEvents = false;
				}
			}
		}

		public bool ShowRemoteBranches
		{
			get { return _chkRemoteBranches != null ? _chkRemoteBranches.IsChecked : Column.ShowRemoteBranches; }
			private set
			{
				if(_chkRemoteBranches != null)
				{
					_disableEvents = true;
					_chkRemoteBranches.IsChecked = value;
					_disableEvents = false;
				}
			}
		}

		public bool ShowTags
		{
			get { return _chkTags != null ? _chkTags.IsChecked : Column.ShowTags; }
			private set
			{
				if(_chkTags != null)
				{
					_disableEvents = true;
					_chkTags.IsChecked = value;
					_disableEvents = false;
				}
			}
		}

		public bool ShowStash
		{
			get { return _chkStash != null ? _chkStash.IsChecked : Column.ShowStash; }
			private set
			{
				if(_chkStash != null)
				{
					_disableEvents = true;
					_chkStash.IsChecked = value;
					_disableEvents = false;
				}
			}
		}

		#endregion
	}
}
