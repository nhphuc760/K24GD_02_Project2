using UnityEngine;
using System.Collections;

public class TreeInfor : MonoBehaviour, IToolTarget
{
    [Header("Tree Setting")]
    [HideInInspector] public TreeSpawner spawner;
    [HideInInspector] public GameObject treePrefab;
    [HideInInspector] public float respawnDelay = 10f;
    [HideInInspector] public BoxCollider2D spawnArea;
    [SerializeField] Animator bodyAnim;
    [SerializeField] Animator rootAnim;
    [Header("Hit Setting")]
    [SerializeField] public int maxHitPoints = 3;             // Số lần chặt để đốn hạ cây
    public ToolDataSO.ToolType requiredTool = ToolDataSO.ToolType.Axe;
    int currentHitPoints;             // Số lần đã chặt
    bool isChopped = false;         // Đánh dấu cây đã đổ

    [Header("Drop Setting")]
    [Tooltip("Dữ liệu sản phẩm")]
    public ResourceSO dropDataSO;
    public int dropCount = 2;
    public float dropForce = 2f;
    int rootHit;

    public ToolDataSO.ToolType RequireTool => requiredTool;

    //sound chat cay
    public AudioClip chopSound;

    private void Awake()
    {
        if(bodyAnim == null)
        {
            bodyAnim = transform.Find("Body").GetComponent<Animator>();
        }
        if(rootAnim == null)
        {
            rootAnim = transform.Find("Root").GetComponent<Animator>();
        }
    }
    private void Start()
    {
        currentHitPoints = maxHitPoints;
        //Tính số lần chặt (70% body và 30% root)
        int bodyHit = Mathf.CeilToInt(.7f * maxHitPoints);
        rootHit = maxHitPoints - bodyHit;
    }
    //Gọi người chơi khi chặt cây
   
    private void ChopDown()
    {
        if(isChopped) return;
        isChopped = true;
        if (chopSound != null)
        {
            AudioManager.instance.PlayFX(chopSound);
        }
        Debug.Log("Tree Chopped down");
        DropWood();
        Destroy(gameObject);
        GameEventManager.Ins.CutDownTree(dropDataSO);
        //Gọi Respawn trong treeSpawn
        if(spawner != null)
        {
            spawner.StartCoroutine(spawner.RespawnTree(treePrefab, spawnArea));
        }    
    }
    void DropWood()
    {
        
        if(dropDataSO == null) return;
        for (int i = 0; i < dropCount; i++)
        {
            GameObject drop = Instantiate(dropDataSO.prefabObj, transform.position, Quaternion.identity);

            Rigidbody2D rb = drop.GetComponent<Rigidbody2D>();
            if(rb != null)
            {
                Vector2 randomDir = new Vector2(Random.Range(-1f, 1f), Random.Range( 0f, 1f)).normalized;
                rb.AddForce(randomDir * dropForce, ForceMode2D.Impulse);
                rb.AddTorque(Random.Range(-5f, 5f));
            }
        }
    }


    void ShowRoot()
    {

        rootAnim.gameObject.SetActive(true);
    }
    void HideBody()
    {
        bodyAnim.gameObject.SetActive(false);
    }

    public void InteractWithTool(ToolDataSO toolDataSO, ToolRunTimeData tool)
    {
        if (isChopped) return;

        currentHitPoints--;
        tool.currentDurability -= toolDataSO.durabilityLossPerUse;
        ////Gọi hiệu ứng rung khi chặt cây
        //StartCoroutine(ShakeTree());
        if (currentHitPoints > 0)
        {
            if (currentHitPoints > rootHit)
            {
                bodyAnim.SetTrigger("Interact");
                if (chopSound != null)
                {
                    AudioManager.instance.PlayFX(chopSound);
                }
                return;
            }
            else
            {
                HideBody();
                ShowRoot();
                rootAnim.SetTrigger("Interact");
                return;
            }
        }

        //Khi chặt cây
        ChopDown();
    }
}
