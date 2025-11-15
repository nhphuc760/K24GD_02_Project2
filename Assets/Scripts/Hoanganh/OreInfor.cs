using UnityEngine;
using System.Collections;

public class OreInfor : MonoBehaviour, IToolTarget
{
    [Header("Ore Settings")]
    [HideInInspector]
    public OreSpawner spawner;
    public GameObject orePrefab; // prefab chính của quặng này
    public float respawnDelay = 10f;
    [SerializeField] Animator animator;
    [Header("Hit Settings")]
    [SerializeField] int maxHitPoints = 3; // số lần đập để bể
    public ToolDataSO.ToolType requireTool = ToolDataSO.ToolType.PickAxe;
    int currentHitPoints;
    bool isDestroyed = false;
    [Header("Drop Setting")]
    public GameObject dropPrefab;        //prefab vật phẩm rớt ra
    public int dropCount = 2;            //số lượng vật phẩm rớt ra
    public float dropForce = 3f;         //lực bắn khi rớt

    [Header("Knockback Settings")]
    public float knockbackForce = 2f;   //Lực văng của quặng khi bị phá
    private Rigidbody2D rb;

    int takeDamage = Animator.StringToHash("TakeDamage");

    public ToolDataSO.ToolType RequireTool => requireTool;

    private void Awake()
    {
       if(animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }
    private void Start()
    {
        currentHitPoints = maxHitPoints;
        rb = GetComponent<Rigidbody2D>();
    }
   
    void BreakOre()
    {
        if (isDestroyed) return;
        isDestroyed = true;       

        Debug.Log("Ore destroyed!");       

        // Văng cục quặng ra nhẹ trước khi phá
        StartCoroutine(KnockbackAndDestroy());

        // Gọi spawn lại quặng sau delay
        if (spawner != null)
        {
            spawner.StartCoroutine(spawner.RespawnOre(orePrefab));
        }       
    }
    IEnumerator KnockbackAndDestroy()
    {
       
        //Tạo hướng văng nhẹ ngẫu nhiên 
        Vector2 randomDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-0.3f, 1f)).normalized;

        if(rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            yield return null;
            rb.AddForce(randomDir * knockbackForce, ForceMode2D.Impulse);
        }
        // Chờ 0.1s để cục quặng văng xong
        yield return new WaitForSeconds(0.1f);

        DropItem();
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
                rb.AddTorque(Random.Range(-5f, 5f)); //xoay nhẹ
            }    
        }
    }

    public void InteractWithTool(ToolDataSO toolDataSO, ToolRunTimeData tool)
    {
        if (isDestroyed) return;

        currentHitPoints--;
        tool.currentDurability -= toolDataSO.durabilityLossPerUse;

        // Nếu vẫn còn HP thì chỉ rung nhẹ hoặc hiệu ứng nứt
        if (currentHitPoints > 0)
        {
            //triger damage
            animator.SetTrigger(takeDamage);
            return;
        }
        // Hết HP thì phá quặng
        BreakOre();
    }
}
