#region Copyright Notice
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

namespace gitter.Git;

using System;

public sealed class HashStringCache
{
	private readonly Func<Sha1Hash> _getHash;
	private readonly WeakReference<string> _ref;
	private int _length;

	public HashStringCache(Func<Sha1Hash> getHash)
	{
		Verify.Argument.IsNotNull(getHash);

		_getHash = getHash;
		_ref     = new(null!);
		_length  = -1;
	}

	private string CreateString(int length)
	{
		var hash = _getHash();
		var value = length == 40 ? hash.ToString() : hash.ToString(length);
		_ref.SetTarget(value);
		return value;
	}

	public string GetValue(int length)
	{
		if(length == 0)
		{
			return string.Empty;
		}
		if(_length != length)
		{
			_length = length;
			return CreateString(length);
		}
		return _ref.TryGetTarget(out var value)
			? value
			: CreateString(length);
	}
}
