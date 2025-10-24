using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple object pool for fruit prefabs. Creates a GameObject in the scene if none exists.
/// Usage: FruitPool.Instance.Spawn(prefab, pos, rot) and FruitPool.Instance.ReturnToPool(obj)
/// </summary>
public class FruitPool : MonoBehaviour
{
    private static FruitPool _instance;
    public static FruitPool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<FruitPool>();
                if (_instance == null)
                {
                    var go = new GameObject("FruitPool");
                    _instance = go.AddComponent<FruitPool>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
            }
            return _instance;
        }
    }

    // mapping from prefab -> queue of inactive instances
    private Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();

    [Tooltip("Initial number of instances to create per prefab when first requested")]
    public int defaultSize = 10;

    public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        if (prefab == null) return null;

        if (!pools.ContainsKey(prefab))
            CreatePool(prefab, defaultSize);

        var q = pools[prefab];

        GameObject obj = null;
        // try to find a non-null object in the queue
        while (q.Count > 0)
        {
            obj = q.Dequeue();
            if (obj != null)
                break;
            obj = null;
        }

        if (obj == null)
        {
            obj = Instantiate(prefab, pos, rot);
            var po = obj.GetComponent<PooledObject>() ?? obj.AddComponent<PooledObject>();
            po.prefab = prefab;
        }

        obj.transform.position = pos;
        obj.transform.rotation = rot;
        obj.SetActive(true);

        return obj;
    }

    public void ReturnToPool(GameObject go)
    {
        if (go == null) return;

        var po = go.GetComponent<PooledObject>();
        if (po == null || po.prefab == null)
        {
            // Not a pooled object, destroy it
            Destroy(go);
            return;
        }

        go.SetActive(false);

        if (!pools.ContainsKey(po.prefab))
            pools[po.prefab] = new Queue<GameObject>();

        pools[po.prefab].Enqueue(go);
    }

    private void CreatePool(GameObject prefab, int size)
    {
        var q = new Queue<GameObject>();
        for (int i = 0; i < size; i++)
        {
            var obj = Instantiate(prefab);
            obj.SetActive(false);
            var po = obj.GetComponent<PooledObject>() ?? obj.AddComponent<PooledObject>();
            po.prefab = prefab;
            q.Enqueue(obj);
        }
        pools[prefab] = q;
    }
}
