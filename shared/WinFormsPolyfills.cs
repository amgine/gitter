#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

#if !NET10_0_OR_GREATER
static class DataObjectExtensions
{
	extension(System.Windows.Forms.IDataObject dataObject)
	{
		public bool TryGetData<T>(
			[System.Diagnostics.CodeAnalysis.MaybeNullWhen(returnValue: false)]
			out T data)
		{
			if(!dataObject.GetDataPresent(typeof(T)))
			{
				data = default;
				return false;
			}
			data = (T?)dataObject.GetData(typeof(T));
			return data is not null;
		}
	}
}
#endif
