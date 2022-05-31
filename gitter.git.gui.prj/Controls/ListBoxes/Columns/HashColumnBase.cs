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

namespace gitter.Git.Gui.Controls;

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

	public static readonly IDpiBoundValue<Font> Font = DpiBoundValue.Font(new("Consolas", 9.0f, FontStyle.Regular, GraphicsUnit.Point));

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
		SetContentFont(Font);

		_abbreviate = DefaultAbbreviate;
	}

	/// <inheritdoc/>
	protected override void OnListBoxAttached()
	{
		base.OnListBoxAttached();
		_extender = new HashColumnExtender(this);
		Extender  = new Popup(_extender);
	}

	/// <inheritdoc/>
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
				if(Width != w)
				{
					ListBox?.Refresh();
				}
				AbbreviateChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	protected virtual string GetHash(Revision revision) => revision.HashString;

	protected virtual string GetHash(CustomListBoxItem item)
	{
		switch(item)
		{
			case IDataContextProvider<Revision> revItem:
				{
					var revision = revItem.DataContext;
					return revision is not null ? GetHash(revision) : NoHash;
				}
			case IDataContextProvider<IRevisionPointer> revPtrItem:
				return GetHash(revPtrItem.DataContext.Dereference());
			case IDataContextProvider<IRemoteReference> remoteRefItem:
				return remoteRefItem.DataContext.Hash.ToString();
			default:
				return NoHash;
		}
	}

	/// <inheritdoc/>
	protected override void OnPaintSubItem(SubItemPaintEventArgs subItemPaintEventArgs)
	{
		Assert.IsNotNull(subItemPaintEventArgs);

		var hash = GetHash(subItemPaintEventArgs.Item);
		if(ReferenceEquals(hash, NoHash))
		{
			var style = Style;
			if((subItemPaintEventArgs.State & ItemState.Selected) == ItemState.Selected && style.Type == GitterStyleType.DarkBackground)
			{
				if(Abbreviate)
				{
#if NET5_0_OR_GREATER
					subItemPaintEventArgs.PaintText(NoHash.AsSpan(0, DefaultAbbrevLength));
#else
					subItemPaintEventArgs.PaintText(NoHash.Substring(0, DefaultAbbrevLength));
#endif
				}
				else
				{
					subItemPaintEventArgs.PaintText(NoHash);
				}
			}
			else
			{
				using var textBrush = new SolidBrush(style.Colors.GrayText);
				if(Abbreviate)
				{
#if NET5_0_OR_GREATER
					subItemPaintEventArgs.PaintText(NoHash.AsSpan(0, DefaultAbbrevLength), textBrush);
#else
					subItemPaintEventArgs.PaintText(NoHash.Substring(0, DefaultAbbrevLength), textBrush);
#endif
				}
				else
				{
					subItemPaintEventArgs.PaintText(NoHash, textBrush);
				}
			}
		}
		else
		{
			if(Abbreviate)
			{
#if NET5_0_OR_GREATER
				subItemPaintEventArgs.PaintText(hash.AsSpan(0, DefaultAbbrevLength));
#else
				subItemPaintEventArgs.PaintText(hash.Substring(0, DefaultAbbrevLength));
#endif
			}
			else
			{
				subItemPaintEventArgs.PaintText(hash);
			}
		}
	}

	/// <inheritdoc/>
	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		Assert.IsNotNull(measureEventArgs);

		var hash = GetHash(measureEventArgs.Item);
		if(Abbreviate)
		{
#if NET5_0_OR_GREATER
			return measureEventArgs.MeasureText(hash.AsSpan(0, DefaultAbbrevLength), ContentFont);
#else
			return measureEventArgs.MeasureText(hash.Substring(0, DefaultAbbrevLength), ContentFont);
#endif
		}
		return measureEventArgs.MeasureText(hash, ContentFont);
	}

	/// <inheritdoc/>
	protected override void SaveMoreTo(Section section)
	{
		base.SaveMoreTo(section);
		section.SetValue("Abbreviate", Abbreviate);
	}

	/// <inheritdoc/>
	protected override void LoadMoreFrom(Section section)
	{
		base.LoadMoreFrom(section);
		Abbreviate = section.GetValue("Abbreviate", Abbreviate);
	}
}
