using UnityEngine;

public static class BonusPool
{
    public static T Spawn<T>(T prefab, Transform parent = null, bool worldPositionStays = false)
        where T : Component
    {
        var clone = BonusPoolers.Instance.Spawn(prefab, parent, worldPositionStays);
        return clone;
    }

    /// <summary>This allows you to spawn a prefab via Component.</summary>
    public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null,
        bool worldPositionStays = true)
        where T : Component
    {
        // Clone this component's GameObject
        var clone = BonusPoolers.Instance.Spawn(prefab, position, rotation, parent, worldPositionStays);
        return clone;
    }


    /// <summary>This allows you to spawn a prefab via GameObject.</summary>
    public static GameObject Spawn(GameObject prefab, Transform parent = null, bool worldPositionStays = false)
    {
        if (prefab)
        {
            return BonusPoolers.Instance.Spawn(prefab, parent, worldPositionStays);
        }
        else
        {
            Debug.LogError("Attempting to spawn a null prefab.");
        }

        return null;
    }

    /// <summary>This allows you to spawn a prefab via GameObject.</summary>
    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null,
        bool worldPositionStays = true)
    {
        if (prefab)
        {
            return BonusPoolers.Instance.Spawn(prefab, position, rotation, parent);
        }
        else
        {
            Debug.LogError("Attempting to spawn a null prefab.");
        }

        return null;
    }

    /// <summary>This allows you to despawn a clone via Component, with optional delay.</summary>
    public static void DeSpawn(Component clone, float delay = 0.0f)
    {
        if (clone) DeSpawn(clone.gameObject, delay);
    }

    /// <summary>This allows you to despawn a clone via GameObject, with optional delay.</summary>
    public static void DeSpawn(GameObject clone, float delay)
    {
        if (clone)
        {
            BonusPoolers.Instance.DeSpawn(clone, delay);
        }
    }

    /// <summary>This allows you to despawn a clone via GameObject, with optional delay.</summary>
    public static void DeSpawn(GameObject clone)
    {
        if (clone)
        {
            BonusPoolers.Instance.DeSpawn(clone);
        }
    }
}