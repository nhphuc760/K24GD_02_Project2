using Unity.VisualScripting;
using UnityEngine;

public class PlayerTestMining : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float interactRange = 1.5f; // khoảng cách có thể đập quặng
    [SerializeField] LayerMask oreLayer; //Layer quặng
    [SerializeField] Transform groundCheck;

    private void Update()
    {
        HandleMining();
    }
    void HandleMining()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryMineOre();
        }
    }
   
    void TryMineOre()
    {

        // Lấy vị trí con trỏ chuột trong thế giới (world space)
        Vector3 mouseScreen = Input.mousePosition;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        Vector2 mousePos2D = new Vector2(mouseWorld.x, mouseWorld.y);
        Collider2D hit = Physics2D.OverlapPoint(mousePos2D, oreLayer);

        if (hit != null)
        {
            float dist = Vector2.Distance(groundCheck.position, hit.transform.position);
            if (dist <= interactRange)
            {
                // Gọi script quặng để xử lý đào
                var ore = hit.GetComponent<OreInfor>(); // class quặng của bạn
                if (ore != null)
                {
                    Debug.Log("Hit is ore");
                    ore.MineOre();
                }
                var tree = hit.GetComponent<TreeInfor>();
                if(tree != null)
                {
                    Debug.Log("hit is tree");
                    tree.OnChop();
                }    
            }
        }
        else
        {
            Debug.Log("hit is null");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, interactRange);
    }
}
