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

namespace gitter.Framework;

using System;
using System.Collections.Generic;

public abstract class CacheUpdater<TKey, TObject, TData>(
	Dictionary<TKey, TObject> dictionary,
	LockType                  syncRoot)
	where TKey : notnull
{
	protected abstract TKey GetKey(TObject @object);

	protected abstract TKey GetKey(TData data);

	protected abstract void UpdateObject(TObject @object, TData data);

	protected abstract TObject CreateObject(TData data);

	protected virtual void OnObjectAdded(TObject @object) { }

	protected virtual void OnObjectRemoved(TObject @object) { }

	protected virtual bool Filter(TObject @object) => true;

	protected virtual bool Filter(TData data) => true;

	public void Update(IEnumerable<TData> dataList)
	{
		lock(syncRoot)
		{
			var hset = default(HashSet<TKey>);
			if(dictionary.Count != 0)
			{
				hset = new HashSet<TKey>(capacity: dictionary.Count);
				foreach(var kvp in dictionary)
				{
					if(!Filter(kvp.Value)) continue;
					hset.Add(kvp.Key);
				}
			}

			foreach(var data in dataList)
			{
				if(!Filter(data)) continue;

				var key = GetKey(data);
				if(!dictionary.TryGetValue(key, out var obj))
				{
					dictionary.Add(key, obj = CreateObject(data));
					OnObjectAdded(obj);
				}
				else
				{
					UpdateObject(obj, data);
					hset?.Remove(key);
				}
			}

			if(hset is not { Count: not 0 }) return;

			foreach(var key in hset)
			{
#if NETCOREAPP
				if(dictionary.Remove(key, out var removed))
				{
					OnObjectRemoved(removed);
				}
#else
				if(dictionary.TryGetValue(key, out var removed))
				{
					dictionary.Remove(key);
					OnObjectRemoved(removed);
				}
#endif
			}
		}
	}
}

/// <summary>Helper class to update data cached in dictionary or list.</summary>
public static class CacheUpdater
{
	public static void UpdateObjectDictionary<TObject, TInfo>(
		IDictionary<string, TObject> dictionary,
		Predicate<TObject>? validateObject,
		Predicate<TInfo>? validateInfo,
		IEnumerable<TInfo> actualList,
		Func<TInfo, TObject> factory,
		Action<TObject, TInfo> updater,
		Action<TObject>? objectCreated,
		Action<TObject>? objectDeleted,
		bool callUpdate)
		where TObject : INamedObject
		where TInfo : INamedObject
	{
		var hset = default(HashSet<TObject>);
		if(dictionary.Count != 0)
		{
			hset = [];
			foreach(var kvp in dictionary)
			{
				if(validateObject is null || validateObject(kvp.Value))
				{
					hset.Add(kvp.Value);
				}
			}
		}

		foreach(var info in actualList)
		{
			if(validateInfo is null || validateInfo(info))
			{
				if(!dictionary.TryGetValue(info.Name, out var obj))
				{
					obj = factory(info);
					dictionary.Add(obj.Name, obj);
					objectCreated?.Invoke(obj);
				}
				else
				{
					if(callUpdate)
					{
						updater(obj, info);
					}
					hset?.Remove(obj);
				}
			}
		}

		if(hset is { Count: not 0 })
		{
			foreach(var obj in hset)
			{
				dictionary.Remove(obj.Name);
				objectDeleted?.Invoke(obj);
			}
		}
	}

	public static void UpdateObjectDictionary<TObject, TInfo>(
		IDictionary<string, TObject> dictionary,
		Predicate<TObject>? validateObject,
		Predicate<TInfo>? validateInfo,
		IDictionary<string, TInfo> actualDictionary,
		Func<TInfo, TObject> factory,
		Action<TObject, TInfo> updater,
		Action<TObject>? objectCreated,
		Action<TObject>? objectDeleted,
		bool callUpdate)
		where TObject : INamedObject
		where TInfo : INamedObject
	{
		var hset = default(HashSet<TObject>);
		if(dictionary.Count != 0)
		{
			hset = new HashSet<TObject>();
			foreach(var kvp in dictionary)
			{
				if(validateObject is null || validateObject(kvp.Value))
				{
					hset.Add(kvp.Value);
				}
			}
		}

		foreach(var info in actualDictionary.Values)
		{
			if(validateInfo is null || validateInfo(info))
			{
				if(!dictionary.TryGetValue(info.Name, out var obj))
				{
					obj = factory(info);
					dictionary.Add(obj.Name, obj);
					objectCreated?.Invoke(obj);
				}
				else
				{
					if(callUpdate)
					{
						updater(obj, info);
					}
					hset?.Remove(obj);
				}
			}
		}

		if(hset is { Count: not 0 })
		{
			foreach(var obj in hset)
			{
				dictionary.Remove(obj.Name);
				objectDeleted?.Invoke(obj);
			}
		}
	}

	public static void UpdateObjectDictionary<TObject, TInfo>(
		IRepository repository,
		IDictionary<string, TObject> dictionary,
		Predicate<TObject> validateObject,
		Predicate<TInfo> validateInfo,
		IDictionary<string, TInfo> actualDictinary,
		Action<TObject> objectCreated,
		Action<TObject> objectDeleted,
		bool callUpdate
		)
		where TObject : INamedObject
		where TInfo : INamedObject, IObjectData<TObject>
	{
		var hset = default(HashSet<TObject>);
		if(dictionary.Count != 0)
		{
			hset = [];
			foreach(var kvp in dictionary)
			{
				if(validateObject is null || validateObject(kvp.Value))
				{
					hset.Add(kvp.Value);
				}
			}
		}

		foreach(var info in actualDictinary.Values)
		{
			if(validateInfo is null || validateInfo(info))
			{
				if(!dictionary.TryGetValue(info.Name, out var obj))
				{
					obj = info.Construct(repository);
					dictionary.Add(obj.Name, obj);
					objectCreated?.Invoke(obj);
				}
				else
				{
					if(callUpdate)
					{
						info.Update(obj);
					}
					hset?.Remove(obj);
				}
			}
		}

		if(hset is { Count: not 0 })
		{
			foreach(var obj in hset)
			{
				dictionary.Remove(obj.Name);
				objectDeleted?.Invoke(obj);
			}
		}
	}

	public static void UpdateObjectDictionaryNoRemove<TObject, TInfo>(
		IRepository repository,
		IDictionary<string, TObject> dictionary,
		Predicate<TObject> validateObject,
		Predicate<TInfo> validateInfo,
		IEnumerable<TInfo> actualList,
		Action<TObject> objectCreated,
		bool callUpdate
		)
		where TObject : INamedObject
		where TInfo : INamedObject, IObjectData<TObject>
	{
		foreach(var info in actualList)
		{
			if(validateInfo is null || validateInfo(info))
			{
				if(!dictionary.TryGetValue(info.Name, out var obj))
				{
					obj = info.Construct(repository);
					dictionary[obj.Name] = obj;
					objectCreated?.Invoke(obj);
				}
				else
				{
					if(callUpdate)
					{
						info.Update(obj);
					}
				}
			}
		}
	}

	public static void UpdateObjectList<TObject, TInfo>(
		IRepository repository,
		IList<TObject> list,
		Predicate<TObject> validateObject,
		Predicate<TInfo> validateInfo,
		IEnumerable<TInfo> actualList,
		Action<TObject> objectCreated,
		Action<TObject> objectDeleted,
		bool callUpdate
		)
		where TObject : INamedObject
		where TInfo : INamedObject, IObjectData<TObject>
	{
		var hset = default(HashSet<TObject>);
		if(list.Count != 0)
		{
			hset = [];
			foreach(var obj in list)
			{
				if(validateObject == null || validateObject(obj))
				{
					hset.Add(obj);
				}
			}
		}

		int id = 0;
		foreach(var info in actualList)
		{
			if(validateInfo is null || validateInfo(info))
			{
				bool found = false;
				for(int i = id; i < list.Count; ++i)
				{
					if(list[i].Name == info.Name)
					{
						if(i != id)
						{
							var temp = list[i];
							list[i] = list[id];
							list[id] = temp;
						}
						if(callUpdate)
						{
							info.Update(list[id]);
						}
						hset?.Remove(list[id]);
						found = true;
						break;
					}
				}
				if(!found)
				{
					var obj = info.Construct(repository);
					list.Insert(id, obj);
					objectCreated?.Invoke(obj);
				}
				++id;
			}
		}

		if(hset is { Count: not 0 })
		{
			foreach(var obj in hset)
			{
				list.Remove(obj);
				objectDeleted?.Invoke(obj);
			}
		}
	}

	public static void UpdateObjectListNoRemove<TObject, TInfo>(
		IRepository repository,
		IList<TObject> list,
		Predicate<TObject> validateObject,
		Predicate<TInfo> validateInfo,
		IEnumerable<TInfo> actualList,
		Action<TObject> objectCreated,
		bool callUpdate
		)
		where TObject : INamedObject
		where TInfo : INamedObject, IObjectData<TObject>
	{
		int id = 0;
		foreach(var info in actualList)
		{
			if(validateInfo is null || validateInfo(info))
			{
				bool found = false;
				for(int i = id; i < list.Count; ++i)
				{
					if(list[i].Name == info.Name)
					{
						if(i != id)
						{
							var temp = list[i];
							list[i] = list[id];
							list[id] = temp;
						}
						if(callUpdate)
						{
							info.Update(list[id]);
						}
						found = true;
						break;
					}
				}
				if(!found)
				{
					var obj = info.Construct(repository);
					list.Insert(id, obj);
					objectCreated?.Invoke(obj);
				}
				++id;
			}
		}
	}

	public static TObject[] TransformToArray<TObject, TInfo>(IRepository repository, IList<TInfo> list)
		where TInfo : IObjectData<TObject>
	{
		var res = new TObject[list.Count];
		for(int i = 0; i < list.Count; ++i)
		{
			res[i] = list[i].Construct(repository);
		}
		return res;
	}

	public static Dictionary<string, TObject> TransformToDictionary<TObject, TInfo>(IRepository repository, IList<TInfo> list)
		where TInfo : INamedObject, IObjectData<TObject>
	{
		var res = new Dictionary<string, TObject>();
		for(int i = 0; i < list.Count; ++i)
		{
			res.Add(list[i].Name, list[i].Construct(repository));
		}
		return res;
	}
}
