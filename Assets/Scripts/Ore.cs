using UnityEngine;

public class Ore : MonoBehaviour
{
    [Header("Ore Settings")]
    public OreSpawner spawner;
    public GameObject orePrefab; // prefab chính của quặng này
    public float respawnDelay = 10f;

    [Header("Hit Settings")]
    [SerializeField] int maxHitPoints = 3; // số lần đập để bể
    int currentHitPoints;
    bool isDestroyed = false;

    [Header("Drop Setting")]
    public GameObject dropPrefab;   //prefab vật phẩm rớt ra 
    public int dropCount = 2;       //số lượng vật phẩm rớt ra
    public float dropForce = 3f;    //lực bắn khi rớt

    private void Start()
    {
        currentHitPoints = maxHitPoints;
    }
    public void MineOre()
    {
        if (isDestroyed) return;

        currentHitPoints--;

        Debug.Log($"Ore hit! Remaining HP: {currentHitPoints}");

        // Nếu vẫn còn HP thì chỉ rung nhẹ hoặc hiệu ứng nứt
        if (currentHitPoints > 0)
        {
            // bạn có thể thêm hiệu ứng rung hoặc âm thanh ở đây
            return;
        }

        // Hết HP thì phá quặng
        BreakOre();
    }
    void BreakOre()
    {
        if (isDestroyed) return;
        isDestroyed = true;       
        Debug.Log("Ore destroyed!");

        //Gọi hàm spawn vật phẩm rớt ra
        DropItem();

        // Gọi spawn lại quặng sau delay
        if (spawner != null)
        {
            spawner.StartCoroutine(spawner.RespawnOre(orePrefab));
        }
        Destroy(gameObject);
    }
    void DropItem()
    {
        if (dropPrefab == null) return;
        for (int i = 0; i < dropCount; i++)
        {
            GameObject drop = Instantiate(dropPrefab, transform.position, Quaternion.identity);

            //Thêm lực ngẫu nhiên để vật phẩm bay ra tự nhiên hơn
            Rigidbody2D rb = drop.GetComponent<Rigidbody2D>();
            if(rb != null)
            {
                Vector2 randomDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-0.5f, 1f)).normalized;
                rb.AddForce(randomDir * dropForce, ForceMode2D.Impulse);
            }    

        }
    }
}
