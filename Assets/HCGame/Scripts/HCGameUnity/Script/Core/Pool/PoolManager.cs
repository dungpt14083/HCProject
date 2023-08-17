using System;
using System.Collections.Generic;
using MonsterLove.Collections;
using UnityEngine;

public class PoolManager : SingletonMono<PoolManager>
{
	public bool logStatus;
	public Transform root;

	private Dictionary<GameObject, ObjectPool<GameObject>> prefabLookup;
	private Dictionary<GameObject, ObjectPool<GameObject>> instanceLookup; 
	
	private bool dirty = false;
	
	void Awake () 
	{
		prefabLookup = new Dictionary<GameObject, ObjectPool<GameObject>>();
		instanceLookup = new Dictionary<GameObject, ObjectPool<GameObject>>();
		if(null == root)
		{
			root = transform;
		}
	}

	void Update()
	{
		if(logStatus && dirty)
		{
			PrintStatus();
			dirty = false;
		}
	}

	private void warmPool(GameObject prefab, int size)
	{
		if(prefabLookup.ContainsKey(prefab))
		{
			throw new Exception("Pool for prefab " + prefab.name + " has already been created");
		}
		var pool = new ObjectPool<GameObject>(() => { return InstantiatePrefab(prefab); }, size);
		prefabLookup[prefab] = pool;

		dirty = true;
	}

	public bool IsContainsKeyWarmPool(GameObject prefab)
    {
		if (prefabLookup.ContainsKey(prefab))
			return true;
		return false;
	}

	public void WarmPools(GameObject prefab, int size, bool activeWarm)
    {
		if (prefabLookup.ContainsKey(prefab))
		{
			throw new Exception("Pool for prefab " + prefab.name + " has already been created");
		}
		var pool = new ObjectPool<GameObject>(() => 
		{
			GameObject objWarm = InstantiatePrefab(prefab);
			objWarm.SetActive(activeWarm);
			return objWarm;
		}, size);
		prefabLookup[prefab] = pool;

		dirty = true;
	}

	public GameObject spawnObject(GameObject prefab)
	{
		return spawnObject(prefab, prefab.transform.position, prefab.transform.rotation);
	}
	
	public GameObject spawnObject(GameObject prefab, Transform parent)
	{
		var result =  spawnObject(prefab);
		result.transform.SetParent(parent);
		return result;
	}



	public GameObject spawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		if (!prefabLookup.ContainsKey(prefab))
		{
			WarmPool(prefab, 1);
		}

		var pool = prefabLookup[prefab];

		var clone = pool.GetItem();
		clone.transform.position = position;
		clone.transform.rotation = rotation;
		clone.SetActive(true);

		instanceLookup.Add(clone, pool);
		dirty = true;
		return clone;
	}

	public void releaseObject(GameObject clone)
	{
		clone.SetActive(false);

		if(instanceLookup.ContainsKey(clone))
		{
			instanceLookup[clone].ReleaseItem(clone);
			instanceLookup.Remove(clone);
			dirty = true;
		}
		else
		{
			Debug.LogWarning("No pool contains the object: " + clone.name);
		}
	}

	private GameObject InstantiatePrefab(GameObject prefab)
	{
		var go = GameObject.Instantiate(prefab) as GameObject;
		if (root != null) go.transform.SetParent(root);
		return go;
	}

	public void PrintStatus()
	{
		foreach (KeyValuePair<GameObject, ObjectPool<GameObject>> keyVal in prefabLookup)
		{
			Debug.Log(string.Format("Object Pool for Prefab: {0} In Use: {1} Total {2}", keyVal.Key.name, keyVal.Value.CountUsedItems, keyVal.Value.Count));
		}
	}

	#region Static API

	public static void WarmPool(GameObject prefab, int size)
	{
		Instance.warmPool(prefab, size);
	}

	public static GameObject SpawnObject(GameObject prefab)
	{
		return Instance.spawnObject(prefab);
	}

	public static GameObject SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		return Instance.spawnObject(prefab, position, rotation);
	}

	public static void ReleaseObject(GameObject clone)
	{
		Instance.releaseObject(clone);
	}

	#endregion
}


