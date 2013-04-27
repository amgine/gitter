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

namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Configuration;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	partial class DiffView : GitViewBase
	{
		#region Data

		private IDiffSource _source;
		private DiffOptions _options;
		private bool _loaded;

		#endregion

		public DiffView(Guid guid, IDictionary<string, object> parameters, GuiProvider gui)
			: base(guid, gui, parameters)
		{
			InitializeComponent();

			_diffViewer.PreviewKeyDown += OnKeyDown;
			_diffViewer.DiffFileContextMenuRequested += OnDiffFileContextMenuRequested;
			_diffViewer.UntrackedFileContextMenuRequested += OnUntrackedFileContextMenuRequested;

			ApplyParameters(parameters);

			AddTopToolStrip(new DiffToolbar(this));
		}

		public override bool IsDocument
		{
			get { return true; }
		}

		protected override void AttachToRepository(Repository repository)
		{
			_diffViewer.Repository = repository;
			base.AttachToRepository(repository);
		}

		protected override void DetachFromRepository(Repository repository)
		{
			base.DetachFromRepository(repository);
			_diffViewer.Repository = null;
			_diffViewer.Clear();
		}

		protected override void SaveRepositoryConfig(Section section)
		{
			if(Guid == Guids.ContextualDiffViewGuid)
			{
				var node = section.GetCreateSection("ContextualDiffOptions");
				node.SetValue<int>("Context", _options.Context);
				node.SetValue<bool>("IgnoreWhitespace", _options.IgnoreWhitespace);
				node.SetValue<bool>("UsePatienceAlgorithm", _options.UsePatienceAlgorithm);
			}
			base.SaveRepositoryConfig(section);
		}

		protected override void LoadRepositoryConfig(Section section)
		{
			//if(Guid == Guids.ContextualDiffViewGuid)
			//{
			//    var node = section.TryGetSection("ContextualDiffOptions");
			//    if(node != null)
			//    {
			//        _options.Context = node.GetValue<int>("Context", _options.Context);
			//        _options.IgnoreWhitespace = node.GetValue<bool>("IgnoreWhitespace", _options.IgnoreWhitespace);
			//        _options.UsePatienceAlgorithm = node.GetValue<bool>("UsePatienceAlgorithm", _options.UsePatienceAlgorithm);
			//    }
			//}
			base.LoadRepositoryConfig(section);
		}

		public override void ApplyParameters(IDictionary<string, object> parameters)
		{
			if(parameters != null)
			{
				var source = (IDiffSource)parameters["source"];
				if(_source != source)
				{
					if(_source != null)
					{
						_source.Updated -= OnSourceUpdated;
						_source.Dispose();
					}
					_source = source;
					if(_source != null)
					{
						_source.Updated += OnSourceUpdated;
					}
				}
				object options;
				if(parameters.TryGetValue("options", out options))
				{
					_options = options as DiffOptions;
				}
				if(_options == null)
				{
					_options = new DiffOptions();
				}
				UpdateText();
				Reload();
			}
			else
			{
				UpdateText();
				_diffViewer.Clear();
				if(_source != null)
				{
					_source.Updated -= OnSourceUpdated;
					_source.Dispose();
				}
			}
			base.ApplyParameters(parameters);
		}

		private void OnSourceUpdated(object sender, EventArgs e)
		{
			if(InvokeRequired)
			{
				BeginInvoke(new EventHandler(OnSourceUpdated), sender, e);
			}
			else
			{
				Reload();
			}
		}

		private void OnDiffFileContextMenuRequested(object sender, DiffFileContextMenuRequestedEventArgs e)
		{
			var menu = new DiffFileMenu(_source, e.File);
			if(menu.Items.Count == 0)
			{
				menu.Dispose();
			}
			else
			{
				Utility.MarkDropDownForAutoDispose(menu);
				e.ContextMenu = menu;
			}
		}

		private void OnUntrackedFileContextMenuRequested(object sender, UntrackedFileContextMenuRequestedEventArgs e)
		{
			var menu = new UnstagedItemMenu(e.File);
			Utility.MarkDropDownForAutoDispose(menu);
			e.ContextMenu = menu;
		}

		public void Reload()
		{
			if(_loaded)
			{
				if(_source != null)
				{
					_diffViewer.LoadAsync(_source, _options);
				}
				else
				{
					_diffViewer.Clear();
				}
			}
		}

		public override void RefreshContent()
		{
			Reload();
		}

		public DiffOptions DiffOptions
		{
			get
			{
				if(_options == null)
				{
					_options = new DiffOptions();
				}
				return _options;
			}
		}

		private void UpdateText()
		{
			if(Guid != Guids.ContextualDiffViewGuid)
			{
				if(_source != null)
				{
					Text = _source.ToString();
				}
				else
				{
					Text = Resources.StrDiff;
				}
			}
			else
			{
				Text = Resources.StrContextualDiff;
			}
		}

		public void SetSource(IDiffSource diffSource)
		{
			if(diffSource != null)
			{
				_diffViewer.LoadAsync(diffSource, _options);
			}
			else
			{
				_diffViewer.Clear();
			}
		}

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgDiff"]; }
		}

		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			OnKeyDown(this, e);
			base.OnPreviewKeyDown(e);
		}

		private void OnKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.F5:
					RefreshContent();
					e.IsInputKey = true;
					break;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if(!_loaded)
			{
				_loaded = true;
				Reload();
			}
		}
	}
}
