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

namespace gitter.Framework
{
	using Color = System.Drawing.Color;

	public interface IGitterStyleColors
	{
		Color WorkArea						{ get; }
		Color Window						{ get; }
		Color ScrollBarSpacing				{ get; }
		Color Separator						{ get; }
		Color Alternate						{ get; }
		Color WindowText					{ get; }
		Color GrayText						{ get; }
		Color HyperlinkText					{ get; }
		Color HyperlinkTextHotTrack			{ get; }

		Color FileHeaderColor1				{ get; }
		Color FileHeaderColor2				{ get; }
		Color FilePanelBorder				{ get; }
		Color LineContextForeground			{ get; }
		Color LineContextBackground			{ get; }
		Color LineAddedForeground			{ get; }
		Color LineAddedBackground			{ get; }
		Color LineRemovedForeground			{ get; }
		Color LineRemovedBackground			{ get; }
		Color LineNumberForeground			{ get; }
		Color LineNumberBackground			{ get; }
		Color LineNumberBackgroundHover		{ get; }
		Color LineHeaderForeground			{ get; }
		Color LineHeaderBackground			{ get; }
		Color LineSelectedBackground		{ get; }
		Color LineSelectedBackgroundHover	{ get; }
		Color LineBackgroundHover			{ get; }
	}
}
