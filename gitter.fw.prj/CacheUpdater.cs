namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;

	/// <summary>Helper class to update data cached in dictionary or list.</summary>
	public static class CacheUpdater
	{
		public static void UpdateObjectDictionary<TObject, TInfo>(
			IRepository repository,
			IDictionary<string, TObject> dictionary,
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
			HashSet<TObject> hset = null;
			if(dictionary.Count != 0)
			{
				hset = new HashSet<TObject>();
				foreach(var kvp in dictionary)
				{
					if(validateObject == null || validateObject(kvp.Value))
					{
						hset.Add(kvp.Value);
					}
				}
			}

			foreach(var info in actualList)
			{
				if(validateInfo == null || validateInfo(info))
				{
					TObject obj;
					if(!dictionary.TryGetValue(info.Name, out obj))
					{
						obj = info.Construct(repository);
						dictionary.Add(obj.Name, obj);
						if(objectCreated != null)
							objectCreated(obj);
					}
					else
					{
						if(callUpdate)
							info.Update(obj);
						if(hset != null)
							hset.Remove(obj);
					}
				}
			}

			if(hset != null && hset.Count != 0)
			{
				foreach(var obj in hset)
				{
					dictionary.Remove(obj.Name);
					if(objectDeleted != null)
						objectDeleted(obj);
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
			HashSet<TObject> hset = null;
			if(dictionary.Count != 0)
			{
				hset = new HashSet<TObject>();
				foreach(var kvp in dictionary)
				{
					if(validateObject == null || validateObject(kvp.Value))
					{
						hset.Add(kvp.Value);
					}
				}
			}

			foreach(var info in actualDictinary.Values)
			{
				if(validateInfo == null || validateInfo(info))
				{
					TObject obj;
					if(!dictionary.TryGetValue(info.Name, out obj))
					{
						obj = info.Construct(repository);
						dictionary.Add(obj.Name, obj);
						if(objectCreated != null)
							objectCreated(obj);
					}
					else
					{
						if(callUpdate)
							info.Update(obj);
						if(hset != null)
							hset.Remove(obj);
					}
				}
			}

			if(hset != null && hset.Count != 0)
			{
				foreach(var obj in hset)
				{
					dictionary.Remove(obj.Name);
					if(objectDeleted != null)
						objectDeleted(obj);
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
				if(validateInfo == null || validateInfo(info))
				{
					TObject obj;
					if(!dictionary.TryGetValue(info.Name, out obj))
					{
						obj = info.Construct(repository);
						dictionary[obj.Name] = obj;
						if(objectCreated != null)
							objectCreated(obj);
					}
					else
					{
						if(callUpdate)
							info.Update(obj);
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
			HashSet<TObject> hset = null;
			if(list.Count != 0)
			{
				hset = new HashSet<TObject>();
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
				if(validateInfo == null || validateInfo(info))
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
								info.Update(list[id]);
							if(hset != null)
								hset.Remove(list[id]);
							found = true;
							break;
						}
					}
					if(!found)
					{
						var obj = info.Construct(repository);
						list.Insert(id, obj);
						if(objectCreated != null)
							objectCreated(obj);
					}
					++id;
				}
			}

			if(hset != null && hset.Count != 0)
			{
				foreach(var obj in hset)
				{
					list.Remove(obj);
					if(objectDeleted != null)
						objectDeleted(obj);
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
				if(validateInfo == null || validateInfo(info))
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
								info.Update(list[id]);
							found = true;
							break;
						}
					}
					if(!found)
					{
						var obj = info.Construct(repository);
						list.Insert(id, obj);
						if(objectCreated != null)
							objectCreated(obj);
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
}
