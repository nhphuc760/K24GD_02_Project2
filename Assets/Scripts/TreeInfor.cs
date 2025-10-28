using UnityEngine;
using System.Collections;

public class TreeInfor : MonoBehaviour
{
    [Header("Tree Setting")]
    [HideInInspector] public TreeSpawner spawner;
    [HideInInspector] public GameObject treePrefab;
    [HideInInspector] public float respawnDelay = 10f;
    [HideInInspector] public BoxCollider2D spawnArea;

    [Header("Hit Setting")]
    [SerializeField] public int maxHitPoints = 3;             // Số lần chặt để đốn hạ cây
    int currentHitPoints;             // Số lần đã chặt
    bool isChopped = false;         // Đánh dấu cây đã đổ

    [Header("Drop Setting")]
    public GameObject woodPrefab;
    public int dropCount = 2;
    public float dropForce = 2f;
    [Header("Shake Setting")]
    public float shakeDuration = 0.1f;
    public float shakeAmount = 0.08f;

    private Vector3 originalPos;
    private void Start()
    {
        currentHitPoints = maxHitPoints;
        originalPos = transform.position;
    }
    //Gọi người chơi khi chặt cây
    public void OnChop()
    {
        if (isChopped) return;

        currentHitPoints--;

        //Gọi hiệu ứng rung khi chặt cây
        StartCoroutine(ShakeTree());
       
        if (currentHitPoints > 0) return;
        //Khi chặt cây
        ChopDown();
    }
   
    private void ChopDown()
    {
        if(isChopped) return;
        isChopped = true;
        Debug.Log("Tree Chopped down");
        DropWood();
        Destroy(gameObject);
        //Gọi Respawn trong treeSpawn
        if(spawner != null)
        {
            spawner.StartCoroutine(spawner.RespawnTree(treePrefab, spawnArea));
        }    
    }
    void DropWood()
    {
        if(woodPrefab == null) return;
        for (int i = 0; i < dropCount; i++)
        {
            GameObject drop = Instantiate(woodPrefab, transform.position, Quaternion.identity);

            Rigidbody2D rb = drop.GetComponent<Rigidbody2D>();
            if(rb != null)
            {
                Vector2 randomDir = new Vector2(Random.Range(-1f, 1f), Random.Range( 0f, 1f)).normalized;
                rb.AddForce(randomDir * dropForce, ForceMode2D.Impulse);
                rb.AddTorque(Random.Range(-5f, 5f));
            }
        }
    }
    IEnumerator ShakeTree()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            Vector3 randomPoint = originalPos + (Vector3)Random.insideUnitCircle * shakeAmount;
            transform.localPosition = randomPoint;
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos; // Reset vị trí
        
    }
}
