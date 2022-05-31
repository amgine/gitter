﻿#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git.Gui.Views;

using System;
using System.ComponentModel;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Configuration;

[ToolboxItem(false)]
[DesignerCategory("")]
partial class GitViewBase : ViewBase
{
	private Repository _repository;

	public event EventHandler RepositoryChanged;

	public GitViewBase()
	{
	}

	public GitViewBase(Guid guid, GuiProvider guiProvider)
		: base(guid, guiProvider.Environment)
	{
		Verify.Argument.IsNotNull(guiProvider);

		Gui = guiProvider;
		_repository = guiProvider.Repository;
	}

	protected void ShowDiffView(IDiffSource diffSource)
	{
		WorkingEnvironment.ViewDockService.ShowView(Guids.DiffViewGuid, new DiffViewModel(diffSource, null));
	}

	protected void ShowContextualDiffView(IDiffSource diffSource)
	{
		WorkingEnvironment.ViewDockService.ShowView(Guids.ContextualDiffViewGuid, new DiffViewModel(diffSource, null), false);
	}

	protected override void OnCreateControl()
	{
		base.OnCreateControl();
		if(_repository is not null)
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
		get => _repository;
		set
		{
			if(value != _repository)
			{
				DetachRepository();
				AttachRepository(value);
				RepositoryChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	private void DetachRepository()
	{
		var repository = _repository;
		_repository = null;
		if(repository is not null)
		{
			DetachFromRepository(repository);
		}
	}

	private void AttachRepository(Repository repository)
	{
		_repository = repository;
		if(repository is not null)
		{
			AttachToRepository(repository);
		}
	}

	protected virtual void LoadRepositoryConfig(Section section)
	{
	}

	protected virtual void SaveRepositoryConfig(Section section)
	{
	}

	public GuiProvider Gui { get; }

	public virtual void RefreshContent()
	{
	}
}
