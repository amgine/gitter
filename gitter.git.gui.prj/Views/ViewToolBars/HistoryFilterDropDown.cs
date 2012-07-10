namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using gitter.Git.Gui.Controls;

	public partial class HistoryFilterDropDown : UserControl
	{
		private Repository _repository;
		private LogOptions _logOptions;

		public HistoryFilterDropDown()
		{
			InitializeComponent();


			Font = GitterApplication.FontManager.UIFont;
		}

		public LogOptions LogOptions
		{
			get { return _logOptions; }
			set
			{
				if(_logOptions != value)
				{
					_logOptions = value;
					if(value != null)
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
			var item = e.Item as IRevisionPointerListItem;
			if(item != null)
			{
				var reference = item.RevisionPointer as Reference;
				if(reference != null)
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
		}

		private void UpdateCheckStatuses()
		{
			if(_logOptions != null)
			{
				UpdateCheckStatuses(_lstReferences.Items);
			}
		}

		private void UpdateCheckStatuses(CustomListBoxItemsCollection items)
		{
			foreach(var item in items)
			{
				var refItem = item as IRevisionPointerListItem;
				if(refItem != null)
				{
					var reference = refItem.RevisionPointer as Reference;
					if(reference != null)
					{
						item.IsChecked = _logOptions.AllowedReferences.Contains(reference);
					}
				}
				UpdateCheckStatuses(item.Items);
			}
		}

		public Repository Repository
		{
			get { return _repository; }
			set
			{
				if(_repository != value)
				{
					_repository = value;
					_lstReferences.ItemCheckedChanged -= OnItemCheckedChanged;
					_lstReferences.LoadData(value);
					_lstReferences.EnableCheckboxes();
					_lstReferences.ExpandAll();
					UpdateCheckStatuses();
					_lstReferences.ItemCheckedChanged += OnItemCheckedChanged;
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
	}
}
