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

namespace gitter.Framework.Configuration;

using System;

static class TypeHelpers
{
	public static Type GetType<T>(T value)
	{
		if(value is not null) return value.GetType();
		return typeof(T);
	}

	public static Type GetType(Type type, object value)
	{
		if(value is not null) return value.GetType();
		return type;
	}

	public static T UnpackValue<T>(object value)
		=> value is T typed ? typed : default;

	public static bool TryUnpackValue<T>(object value, out T unpacked)
	{
		if(value is T typed)
		{
			unpacked = typed;
			return true;
		}
		unpacked = default;
		return false;
	}

	public static object PackValue<T>(T value)
	{
		if(value is null) return null;
		return value;
	}
}
