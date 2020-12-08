#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2020  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;

	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Framework.Configuration;

	/// <summary>"Hash" column.</summary>
	public abstract class HashColumnBase : CustomListBoxColumn
	{
		public const bool DefaultAbbreviate = true;
		public const int  DefaultAbbrevLength = 7;

		public static readonly Font Font = new Font("Consolas", 9.0f, FontStyle.Regular, GraphicsUnit.Point);

		private const string NoHash = "----------------------------------------";

		#region Data

		private bool _abbreviate;
		private HashColumnExtender _extender;

		#endregion

		#region Events

		public event EventHandler AbbreviateChanged;

		#endregion

		protected HashColumnBase(int id, string name, bool visible)
			: base(id, name, visible)
		{
			Width = 56;
			ContentFont = Font;

			_abbreviate = DefaultAbbreviate;
		}

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			_extender = new HashColumnExtender(this);
			Extender  = new Popup(_extender);
		}

		protected override void OnListBoxDetached()
		{
			Extender.Dispose();
			Extender = null;
			_extender.Dispose();
			_extender = null;
			base.OnListBoxDetached();
		}

		public bool Abbreviate
		{
			get => _abbreviate;
			set
			{
				if(_abbreviate != value)
				{
					_abbreviate = value;
					var w = Width;
					AutoSize();
					if(Width != w && ListBox != null)
					{
						ListBox.Refresh();
					}
					AbbreviateChanged?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		protected abstract string GetHash(Revision revision);

		private string GetHash(CustomListBoxItem item)
		{
			switch(item)
			{
				case IRevisionPointerListItem rev:
					var ptr = rev.RevisionPointer?.Dereference();
					if(ptr != null) return GetHash(ptr) ?? NoHash;
					break;
				case RemoteReferenceListItem remoteRef:
					return remoteRef.DataContext.Hash.ToString();
			}
			return NoHash;
		}

		protected override void OnPainSubItem(SubItemPaintEventArgs subItemPaintEventArgs)
		{
			Assert.IsNotNull(subItemPaintEventArgs);

			var hash = GetHash(subItemPaintEventArgs.Item);
			if(ReferenceEquals(hash, NoHash))
			{
				if(Abbreviate) hash = hash.Substring(0, HashColumn.DefaultAbbrevLength);
				var style = Style;
				if((subItemPaintEventArgs.State & ItemState.Selected) == ItemState.Selected && style.Type == GitterStyleType.DarkBackground)
				{
					subItemPaintEventArgs.PaintText(hash);
				}
				else
				{
					using var textBrush = new SolidBrush(style.Colors.GrayText);
					subItemPaintEventArgs.PaintText(hash, textBrush);
				}
			}
			else
			{
				if(Abbreviate) hash = hash.Substring(0, HashColumn.DefaultAbbrevLength);
				subItemPaintEventArgs.PaintText(hash);
			}
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			Assert.IsNotNull(measureEventArgs);

			var hash = GetHash(measureEventArgs.Item);
			if(Abbreviate) hash = hash.Substring(0, HashColumn.DefaultAbbrevLength);
			return measureEventArgs.MeasureText(hash, ContentFont);
		}

		protected override void SaveMoreTo(Section section)
		{
			base.SaveMoreTo(section);
			section.SetValue("Abbreviate", Abbreviate);
		}

		protected override void LoadMoreFrom(Section section)
		{
			base.LoadMoreFrom(section);
			Abbreviate = section.GetValue("Abbreviate", Abbreviate);
		}
	}
}
