using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BonusPoolers : SingletonNotAbleNull<BonusPoolers>
{
    private readonly Dictionary<GameObject, List<GameObject>> _gameObjectPools = new Dictionary<GameObject, List<GameObject>>();
    private readonly Dictionary<GameObject, Transform> _parentPools = new Dictionary<GameObject, Transform>();
    private const string _SUFFIX = "_Pool";

    private Transform _cacheTrs;

    protected override void Awake()
    {
        base.Awake();
        _cacheTrs = transform;
    }

    public T Spawn<T>(T go, Transform parent = null, bool worldPositionStays = false) where T : Component
    {
        if (_gameObjectPools.ContainsKey(go.gameObject))
        {
            foreach (var o in _gameObjectPools[go.gameObject])
            {
                if (o && !o.activeSelf)
                {
                    o.transform.SetParent(parent);
                    if (parent)
                    {
                        o.transform.localPosition = Vector3.zero;
                        o.transform.localScale = go.transform.localScale;
                    }
                    o.SetActive(true);
                    return o.GetComponent<T>();
                }
            }

            var item = Instantiate(go, parent ? parent : _parentPools[go.gameObject], worldPositionStays);
            if (parent)
            {
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = go.transform.localScale;
            }
            _gameObjectPools[go.gameObject].Add(item.gameObject);
            return item;
        }
        else
        {
            var holder = new GameObject($"{go.name}{_SUFFIX}").transform;
            holder.SetParent(_cacheTrs);
            _parentPools.Add(go.gameObject, holder);
            var item = Instantiate(go, parent ? parent : _parentPools[go.gameObject], worldPositionStays);
            _gameObjectPools.Add(go.gameObject, new List<GameObject> { item.gameObject });
            if (parent)
            {
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = go.transform.localScale;
            }
            return item;
        }
    }

    public T Spawn<T>(T go, Vector3 position, Quaternion rotation, Transform parent = null, bool worldPositionStays = false)
        where T : Component
    {
        if (_gameObjectPools.ContainsKey(go.gameObject))
        {
            foreach (var o in _gameObjectPools[go.gameObject])
            {
                if (!o.activeSelf)
                {
                    o.transform.SetParent(parent ? parent : _parentPools[o]);
                    o.transform.position = position;
                    o.transform.rotation = rotation;
                    o.SetActive(true);
                    return o.GetComponent<T>();
                }
            }

            var item = Instantiate(go, position, rotation, parent ? parent : _parentPools[go.gameObject]);
            _gameObjectPools[go.gameObject].Add(item.gameObject);
            return item;
        }
        else
        {
            var holder = new GameObject($"{go.name}{_SUFFIX}").transform;
            holder.SetParent(_cacheTrs);
            _parentPools.Add(go.gameObject, holder);

            var item = Instantiate(go, position, rotation, parent ? parent : _parentPools[go.gameObject]);
            _gameObjectPools.Add(go.gameObject, new List<GameObject> { item.gameObject });
            return item;
        }
    }

    public GameObject Spawn(GameObject go, Transform parent = null, bool worldPositionStays = false)
    {
        if (_gameObjectPools.ContainsKey(go))
        {
            foreach (var o in _gameObjectPools[go])
            {
                if (o && !o.activeSelf)
                {
                    o.transform.SetParent(parent ? parent : _parentPools[go.gameObject]);
                    if (parent)
                    {
                        o.transform.localPosition = Vector3.zero;
                        o.transform.localScale = go.transform.localScale;
                    }
                    o.SetActive(true);
                    return o;
                }
            }
    
            var item = Instantiate(go, parent ? parent : _parentPools[go.gameObject], worldPositionStays);
            if (parent)
            {
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = go.transform.localScale;
            }

            _gameObjectPools[go].Add(item);
            
            return item;
        }
        else
        {
            var holder = new GameObject($"{go.name}{_SUFFIX}").transform;
            holder.SetParent(_cacheTrs);
            _parentPools.Add(go.gameObject, holder);
            
            var item = Instantiate(go, parent? parent : _parentPools[go.gameObject], worldPositionStays);
            _gameObjectPools.Add(go, new List<GameObject> { item });
            if (parent)
            {
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = go.transform.localScale;
            }
            return item;
        }
    }
    
    public GameObject Spawn(GameObject go, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (_gameObjectPools.ContainsKey(go))
        {
            foreach (var o in _gameObjectPools[go])
            {
                if (o && !o.activeSelf)
                {
                    o.transform.SetParent(parent ? parent : _parentPools[go]);
                    o.transform.position = position;
                    o.transform.rotation = rotation;
                    o.SetActive(true);
                    return o;
                }
            }
    
            var item = Instantiate(go, position, rotation, parent ? parent : _parentPools[go]);
            _gameObjectPools[go].Add(item);
            return item;
        }
        else
        {
            var holder = new GameObject($"{go.name}{_SUFFIX}").transform;
            holder.SetParent(_cacheTrs);
            _parentPools.Add(go.gameObject, holder);
            var item = Instantiate(go, position, rotation, parent ? parent : _parentPools[go.gameObject]);
            _gameObjectPools.Add(go, new List<GameObject> { item });
            return item;
        }
    }

    public void DeSpawn<T>(T go) where T : Component
    {
        var key = GetKeyFromClone(go.gameObject);
        if (key)
        {
            go.gameObject.SetActive(false);
            go.transform.SetParent(_parentPools[key]);
        }
        else
        {
            go.gameObject.SetActive(false);
        }
    }

    public void DeSpawn(GameObject go)
    {
        var key = GetKeyFromClone(go);
        if (key)
        {
            go.gameObject.SetActive(false);
            go.transform.SetParent(_parentPools[key]);
        }
        else
        {
            go.gameObject.SetActive(false);
        }
    }

    public void DeSpawn<T>(T go, float timeDelay) where T : Component
    {
        DOVirtual.DelayedCall(timeDelay, () => { DeSpawn(go); });
    }

    public void DeSpawn(GameObject go, float timeDelay)
    {
        if (timeDelay > 0f)
        {
            DOVirtual.DelayedCall(timeDelay, () => { DeSpawn(go); }).SetUpdate(false);
        }
        else
        {
            DeSpawn(go);
        }
    }

    public void DeSpawnAll<T>(T go) where T : Component
    {
        if (_gameObjectPools.ContainsKey(go.gameObject))
        {
            foreach (var component in _gameObjectPools[go.gameObject])
            {
                component.gameObject.SetActive(false);
                component.transform.SetParent(_cacheTrs);
            }
        }
    }

    public void DeSpawnAll(GameObject go)
    {
        if (_gameObjectPools.ContainsKey(go))
        {
            foreach (var item in _gameObjectPools[go])
            {
                item.gameObject.SetActive(false);
                item.transform.SetParent(_parentPools[go]);
            }
        }
    }

    public void DestroyObject(GameObject go)
    {
        var key = GetKeyFromClone(go);
        if (key)
        {
            _gameObjectPools[key].Remove(go);
            Destroy(go);
        }
    }
    
    public void DestroyObject<T>(T go) where T : Component
    {
        var key = GetKeyFromClone(go.gameObject);
        if (key)
        {
            _gameObjectPools[key].Remove(go.gameObject);
            Destroy(go);
        }
    }

    private GameObject GetKeyFromClone(GameObject clone)
    {
        foreach (var pool in _gameObjectPools)
        {
            if (pool.Value.Contains(clone.gameObject))
            {
                return pool.Key;
            }
        }

        return null;
    }
}
