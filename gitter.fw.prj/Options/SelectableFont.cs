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

namespace gitter.Framework.Options;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework.Configuration;

using Resources = gitter.Framework.Properties.Resources;

#if NET6_0_OR_GREATER
[System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
public sealed class SelectableFont : IDpiBoundValue<Font>
{
	#region Data

	private Font _font;
	private IDpiBoundValue<Font> _scalable;

	#endregion

	#region Events

	public event EventHandler Changed;

	#endregion

	#region .ctor

	public SelectableFont(string id, string name, Font font)
	{
		Verify.Argument.IsNeitherNullNorWhitespace(name);
		Verify.Argument.IsNotNull(font);

		Id = id;
		Name = name;
		_font = font;
	}

	public SelectableFont(string id, string name, Section section)
	{
		Verify.Argument.IsNeitherNullNorWhitespace(name);
		Verify.Argument.IsNotNull(section);

		var fontName = section.GetValue("Name", default(string));
		var size     = section.GetValue("Size", 0.0f);
		var style    = section.GetValue("Style", FontStyle.Regular);
		var unit     = section.GetValue("Unit", GraphicsUnit.Point);

		Verify.Argument.IsTrue(fontName is not null, nameof(section), "Section does not contain a valid font name.");
		Verify.Argument.IsTrue(size > 0, nameof(section), "Section contains invalid font size.");

		_font = new Font(fontName, size, style, unit);
		Id    = id;
		Name  = name;
	}

	#endregion

	#region Properties

	public string Id { get; }

	public string Name { get; }

	public IDpiBoundValue<Font> ScalableFont => _scalable ??= DpiBoundValue.Font(Font);

	public Font Font
	{
		get => _font;
		set
		{
			Verify.Argument.IsNotNull(value);

			if(_font != value)
			{
				if(_scalable is not null)
				{
					_scalable = default;
					(_scalable as IDisposable)?.Dispose();
				}
				_font = value;
				Changed?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	#endregion

	public void Apply(params Control[] controls)
	{
		Verify.Argument.IsNotNull(controls);
		Verify.Argument.HasNoNullItems(controls);

		ApplyCore(controls);
	}

	public void Apply(IEnumerable<Control> controls)
	{
		Verify.Argument.IsNotNull(controls);
		Verify.Argument.HasNoNullItems(controls);

		ApplyCore(controls);
	}

	private void ApplyCore(IEnumerable<Control> controls)
	{
		Assert.IsNotNull(controls);

		foreach(var control in controls)
		{
			control.Font = ScalableFont.GetValue(Dpi.FromControl(control));
		}
	}

	public static implicit operator Font(SelectableFont font) => font._font;

	public void SaveTo(Section section)
	{
		Verify.Argument.IsNotNull(section);

		section.SetValue("Name",  _font.Name);
		section.SetValue("Size",  _font.SizeInPoints);
		section.SetValue("Style", _font.Style);
		section.SetValue("Unit",  _font.Unit);
	}

	public void LoadFrom(Section section)
	{
		Verify.Argument.IsNotNull(section);

		var name  = section.GetValue("Name", default(string));
		var size  = section.GetValue("Size", 0.0f);
		var style = section.GetValue("Name", FontStyle.Regular);
		var unit  = section.GetValue("Unit", GraphicsUnit.Point);

		Assert.IsNeitherNullNorWhitespace(name);
		Assert.BoundedDoubleInc(0, size, 100);

		if(_font.Name != name || _font.Size != size || _font.Style != style)
		{
			Font font = null;
			try
			{
				font = new Font(name, size, style, unit);
			}
			catch(Exception exc) when(!exc.IsCritical())
			{
			}
			if(font is not null)
			{
				_font = font;
			}
			Changed?.Invoke(this, EventArgs.Empty);
		}
	}

	/// <inheritdoc/>
	public override string ToString() => Name;

	/// <inheritdoc/>
	Font IDpiBoundValue<Font>.GetValue(Dpi dpi) => ScalableFont.GetValue(dpi);
}
