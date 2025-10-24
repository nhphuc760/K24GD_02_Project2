using UnityEngine;

/// <summary>
/// Small marker that remembers which prefab this pooled instance came from.
/// </summary>
public class PooledObject : MonoBehaviour
{
    public GameObject prefab;
}
