﻿#region Copyright Notice
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

namespace gitter.Git.Gui.Views;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Mvc;
using gitter.Framework.Mvc.WinForms;
using gitter.Git.Gui.Controls;

[DesignerCategory("")]
partial class HistoryFilterDropDown : UserControl
{
	private Repository _repository;
	private LogOptions _logOptions;

	public IUserInputSource<string> Search { get; }

	public HistoryFilterDropDown()
	{
		InitializeComponent();

#if NETCOREAPP
		_txtSearch.PlaceholderText = "Filter";
#endif

		if(LicenseManager.UsageMode == LicenseUsageMode.Runtime)
		{
			Font = GitterApplication.FontManager.UIFont;
			BackColor = GitterApplication.Style.Colors.Window;
			ForeColor = GitterApplication.Style.Colors.WindowText;
		}
		else
		{
			Font = SystemFonts.MessageBoxFont;
		}
		_txtSearch.BackColor = BackColor;
		_txtSearch.ForeColor = ForeColor;
		_txtSearch.TextChanged += _txtSearch_TextChanged;
		Search = new TextBoxInputSource(_txtSearch);
	}

	public LogOptions LogOptions
	{
		get => _logOptions;
		set
		{
			if(_logOptions != value)
			{
				_logOptions = value;
				if(value is not null)
				{
					switch(value.Filter)
					{
						case LogReferenceFilter.All:
							radioButton1.Checked = true;
							break;
						case LogReferenceFilter.HEAD:
							radioButton2.Checked = true;
							break;
						case LogReferenceFilter.Allowed:
							radioButton3.Checked = true;
							break;
					}
				}
				_lstReferences.ItemCheckedChanged -= OnItemCheckedChanged;
				UpdateCheckStatuses();
				_lstReferences.ItemCheckedChanged += OnItemCheckedChanged;
			}
		}
	}

	private void OnItemCheckedChanged(object sender, ItemEventArgs e)
	{
		if(e.Item is IRevisionPointerListItem item && item.RevisionPointer is Reference reference)
		{
			if(e.Item.IsChecked)
			{
				_logOptions.AllowReference(reference);
			}
			else
			{
				_logOptions.DisallowReference(reference);
			}
		}
	}

	private void UpdateCheckStatuses()
	{
		if(_logOptions is not null)
		{
			UpdateCheckStatuses(_lstReferences.Items);
		}
	}

	private void UpdateCheckStatuses(CustomListBoxItemsCollection items)
	{
		foreach(var item in items)
		{
			if(item is IRevisionPointerListItem { RevisionPointer: Reference reference })
			{
				item.IsChecked = _logOptions.AllowedReferences.Contains(reference);
			}
			UpdateCheckStatuses(item.Items);
		}
	}

	public Repository Repository
	{
		get => _repository;
		set
		{
			if(_repository != value)
			{
				_repository = value;
				LoadReferences(value, null);
			}
		}
	}

	private void OnFilterTypeCheckedChanged(object sender, EventArgs e)
	{
		if(((RadioButton)sender).Checked && _logOptions != null)
		{
			if(radioButton1.Checked) _logOptions.Filter = LogReferenceFilter.All;
			if(radioButton2.Checked) _logOptions.Filter = LogReferenceFilter.HEAD;
			if(radioButton3.Checked) _logOptions.Filter = LogReferenceFilter.Allowed;
		}
	}

	private void LoadReferences(Repository repository, string filter)
	{
		_lstReferences.ItemCheckedChanged -= OnItemCheckedChanged;
		if(string.IsNullOrEmpty(filter))
		{
			_lstReferences.LoadData(repository);
		}
		else
		{
			_lstReferences.LoadData(repository, ReferenceType.Reference, true, true, x =>
			{
				if(x is Reference reference) return reference.Name.Contains(filter);
				return x.FullName.Contains(filter);
			});
		}
		_lstReferences.EnableCheckboxes();
		_lstReferences.ExpandAll();
		UpdateCheckStatuses();
		_lstReferences.ItemCheckedChanged += OnItemCheckedChanged;
	}

	private void _txtSearch_TextChanged(object sender, EventArgs e)
	{
		if(_lstReferences.Items.Count == 0) return;
		LoadReferences(Repository, Search.Value);
	}

	private void HistoryFilterDropDown_VisibleChanged(object sender, EventArgs e)
	{
		Search.Value = string.Empty;
	}
}
