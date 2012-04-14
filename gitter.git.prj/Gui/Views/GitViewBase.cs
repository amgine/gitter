namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;

	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Framework.Configuration;

	[ToolboxItem(false)]
	partial class GitViewBase : ViewBase
	{
		private readonly GuiProvider _gui;
		private Repository _repository;

		public event EventHandler RepositoryChanged;

		public GitViewBase()
		{
		}

		public GitViewBase(Guid guid, IDictionary<string, object> parameters, GuiProvider gui)
			: base(guid, parameters)
		{
			if(gui == null) throw new ArgumentNullException("gui");
			_gui = gui;
			_repository = gui.Repository;
		}

		protected void ShowDiffTool(IDiffSource diffSource)
		{
			Gui.Environment.ViewDockService.ShowView(Guids.DiffViewGuid,
				new Dictionary<string, object>()
				{
					{ "source", diffSource }
				});
		}

		protected void ShowContextualDiffTool(IDiffSource diffSource)
		{
			Gui.Environment.ViewDockService.ShowView(Guids.ContextualDiffViewGuid,
				new Dictionary<string, object>()
				{
					{ "source", diffSource }
				}, false);
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			if(_repository != null)
			{
				AttachToRepository(_repository);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				Repository = null;
			}
			base.Dispose(disposing);
		}

		protected override void OnClosing()
		{
			Repository = null;
			base.OnClosing();
		}

		protected virtual void AttachToRepository(Repository repository)
		{
			LoadRepositoryConfig(repository.ConfigSection);
		}

		protected virtual void DetachFromRepository(Repository repository)
		{
			SaveRepositoryConfig(repository.ConfigSection);
		}

		public Repository Repository
		{
			get { return _repository; }
			set
			{
				if(value != _repository)
				{
					if(_repository != null)
					{
						DetachFromRepository(_repository);
					}
					_repository = value;
					if(_repository != null)
					{
						AttachToRepository(_repository);
					}
					RepositoryChanged.Raise(this);
				}
			}
		}

		protected virtual void LoadRepositoryConfig(Section section)
		{
		}

		protected virtual void SaveRepositoryConfig(Section section)
		{
		}

		public GuiProvider Gui
		{
			get { return _gui; }
		}

		public virtual void RefreshContent()
		{
		}
	}
}
