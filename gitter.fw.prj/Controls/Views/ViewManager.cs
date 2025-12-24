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

namespace gitter.Framework.Controls;

using System;

public static class ViewManager
{
	private static ViewRenderer? _msvs2010StyleRender;
	private static ViewRenderer? _msvs2012DarkStyleRender;
	private static ViewRenderer? _msvs2012LightStyleRender;
	private static ViewRenderer? _viewRenderer;

	public static event EventHandler? RendererChanged;

	private static void OnRendererChanged()
		=> RendererChanged?.Invoke(null, EventArgs.Empty);

	public static ViewRenderer Renderer
	{
		get => _viewRenderer ??= MSVS2010StyleRenderer;
		set
		{
			Verify.Argument.IsNotNull(value);

			if(_viewRenderer != value)
			{
				_viewRenderer = value;
				OnRendererChanged();
			}
		}
	}

	public static ViewRenderer MSVS2010StyleRenderer
		=> _msvs2010StyleRender ??= new MSVS2010StyleViewRenderer();

	public static ViewRenderer MSVS2012DarkStyleRenderer
		=> _msvs2012DarkStyleRender ??= new MSVS2012StyleViewRenderer(MSVS2012StyleViewRenderer.DarkColors);

	public static ViewRenderer MSVS2012LightStyleRenderer
		=> _msvs2012LightStyleRender ??= new MSVS2012StyleViewRenderer(MSVS2012StyleViewRenderer.LightColors);
}
