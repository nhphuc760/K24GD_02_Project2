using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class PlayerFarming : MonoBehaviour
{
    Tilemap plowableLayer; // Tilemap for plowable ground
    public TileBase farmPlotTile;
    public LayerMask cropsLayerMask;//quét tìm cropsLayerMask
    public LayerMask interactableLayerMask;//quét tìm interactableLayerMask
    public float interactionRadius = 0.8f; // Bán kính tương tác với các đối tượng xung quanh

    private void Start()
    {
      
        SceneManager.sceneLoaded += OnSceneLoad; //Tắt script nếu không cần thiết
    }



    private void OnSceneLoad(Scene arg0, LoadSceneMode arg1)
    {
        if (!arg0.name.Equals("Farm"))
        {
            this.enabled = false;
        }
        else
        {
            if(plowableLayer == null)
            {
                plowableLayer = GameObject.FindGameObjectWithTag("FarmLand").GetComponent<Tilemap>();
            }
            this.enabled = true;
        }
    }
    private void OnEnable()
    {
        GameEventManager.Ins.gameInput.interacPressed += HandleInput;
    }
  

    private void OnDisable()
    {
        GameEventManager.Ins.gameInput.interacPressed -= HandleInput;             
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }
    void HandleInput()
    {
        InventorySlot curSelected = GameEventManager.Ins.toolKitEvent.GetCurDataChoose();
        if(curSelected == null)
        {
            GameEventManager.Ins.TriggerDialog("<color=red>Bạn chưa chọn hạt giống</color>");
            return;
        }
        var seedData = curSelected.ItemData as SeedDataSO;
        
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactionRadius, interactableLayerMask);
        if (hit != null && hit.GetComponent<IInteractable>() != null)
        {
            Debug.Log("Harvest");
            hit.GetComponent<IInteractable>().Interact();
            return;
        }

        if (seedData == null)
        {
            GameEventManager.Ins.TriggerDialog("<color=red>Bạn chưa chọn hạt giống</color>");
            return;
        }
        Plant(seedData);
        
    }


    //Trồng, thu hoạch cây.
    void Plant(SeedDataSO cropToPlant)
    {
        Vector3Int cellPosition = plowableLayer.WorldToCell(transform.position);
        Vector3 cellCenterPosition = plowableLayer.GetCellCenterWorld(cellPosition);
        TileBase currentTile = plowableLayer.GetTile(cellPosition);
        if (currentTile != null)
        {
            Collider2D existingCrop = Physics2D.OverlapPoint(cellCenterPosition, cropsLayerMask);//kiêm tra xem đã có cây trồng chưa
            if (existingCrop == null)
            {
                // Thì mới tiến hành trồng cây
                //trừ hạt giống trong inventory
                int curIndex = GameEventManager.Ins.toolKitEvent.GetCurrentIndex();
                if (curIndex < 0) return;
                GameEventManager.Ins.inventoryEvent.RemoveItem(curIndex, 1);
                GameObject cropInstance = Instantiate(cropToPlant.cropData.prefab, cellCenterPosition, Quaternion.identity);
                cropInstance.GetComponent<Seed>().Plant(cropToPlant);
            }
            else
            {
                GameEventManager.Ins.TriggerDialog("Ô này đã được trồng rồi");
            }
        }
    }

   
}
