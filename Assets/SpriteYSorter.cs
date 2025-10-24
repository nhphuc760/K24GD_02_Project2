using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteYSorter : MonoBehaviour
{
    private SpriteRenderer sr;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    private void LateUpdate()
    {
        sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }
}
