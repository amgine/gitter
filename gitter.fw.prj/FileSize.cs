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
	public struct FileSize
	{
		private readonly long _size;
		private readonly string _units;
		private readonly string _shortSize;

		private static readonly string[] KnownUnits = new[]
		{
			"B", "KB", "MB", "GB", "TB", "PB"
		};

		public FileSize(long size)
		{
			_size = size;
			double s = size;
			var sizeId = 0;
			while(s > 1024)
			{
				s /= 1024;
				++sizeId;
			}
			if(sizeId >= KnownUnits.Length)
			{
				_shortSize = size.ToString();
				_units = "";
			}
			else
			{
				_shortSize = ((int)(s + .5)).ToString();
				_units = KnownUnits[sizeId];
			}
		}

		public long Size
		{
			get { return _size; }
		}

		public string ShortSize
		{
			get { return _shortSize; }
		}

		public string ShortSizeUnits
		{
			get { return _units; }
		}

		public override string ToString()
		{
			return ShortSize + " " + ShortSizeUnits;
		}
	}
}
