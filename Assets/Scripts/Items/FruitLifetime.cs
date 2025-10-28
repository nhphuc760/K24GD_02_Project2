using System.Collections;
using UnityEngine;

/// <summary>
/// Automatically returns the fruit to the pool (or destroys it) after a lifetime.
/// Works cleanly with FruitPool: when returned the GameObject is deactivated.
/// </summary>
public class FruitLifetime : MonoBehaviour
{
    [Tooltip("Life time in seconds before the fruit is removed (returned to pool or destroyed)")]
    public float lifetime = 5f;

    private Coroutine lifeCoroutine;

    void OnEnable()
    {
        // start lifetime countdown when object becomes active
        if (lifetime > 0f)
            lifeCoroutine = StartCoroutine(LifeTick());
    }

    void OnDisable()
    {
        // stop coroutine when object is deactivated (e.g., returned to pool)
        if (lifeCoroutine != null)
        {
            StopCoroutine(lifeCoroutine);
            lifeCoroutine = null;
        }
    }

    IEnumerator LifeTick()
    {
        yield return new WaitForSeconds(lifetime);

        // If pool exists, return to pool; otherwise destroy
        if (FruitPool.Instance != null)
        {
            FruitPool.Instance.ReturnToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
