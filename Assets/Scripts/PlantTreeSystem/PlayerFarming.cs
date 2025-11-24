using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class PlayerFarming : MonoBehaviour
{
    [SerializeField] Transform groundCheck; // Vị trí kiểm tra đất
    Tilemap plowableLayer; // Tilemap for plowable ground
    public TileBase farmPlotTile;
    public LayerMask cropsLayerMask;//quét tìm cropsLayerMask
    public LayerMask interactableLayerMask;//quét tìm interactableLayerMask
                                           //  public float interactionRadius = 0.8f; // Bán kính tương tác với các đối tượng xung quanh

    //audio
    public AudioClip PlantSound;
    public AudioClip HarvestSound;
    private void Start()
    {
      
        SceneManager.sceneLoaded += OnSceneLoad; //Tắt script nếu không cần thiết
        if (groundCheck == null)
        {
            groundCheck = transform.GetChild(1);
        }
        
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
        if (curSelected == null) return;
        var toolDataSO = curSelected.ItemData as ToolDataSO;
        Collider2D hit = Physics2D.OverlapPoint(groundCheck.position, interactableLayerMask);

        if(hit != null)
        {
            if(toolDataSO != null && hit.TryGetComponent<IToolTarget>(out IToolTarget toolTarget))
            {
                if(toolDataSO.toolType.Equals(toolTarget.RequireTool))
                {
                    // Ensure runtime data exists and is of correct type before calling ToolUse
                    ToolRunTimeData runtime = curSelected.dataRuntime as ToolRunTimeData;
                    if (runtime == null)
                    {
                        runtime = new ToolRunTimeData();
                        runtime.Init(toolDataSO);
                        curSelected.dataRuntime = runtime; // persist to slot so durability updates survive
                    }

                    GameEventManager.Ins.animationEvent.ToolUse(hit.GetComponent<IToolTarget>(), toolDataSO, runtime);
                    if (AudioManager.instance != null)
                    {
                        Debug.Log("Play harvest sound");
                        AudioManager.instance.PlayFX(HarvestSound);
                    }
                }
                else
                    GameEventManager.Ins.TriggerDialog("<color=red>Công cụ không phù hợp</color>");
                return;
            }
          
        }

        var seedData = curSelected.ItemData as SeedDataSO;

        if (seedData == null)
        {
            GameEventManager.Ins.TriggerDialog("<color=red>Bạn chưa chọn hạt giống</color>");
            return;
        }
        Plant(seedData);


    }


    //Trồng cây.
    void Plant(SeedDataSO cropToPlant)
    {
        Vector3Int cellPosition = plowableLayer.WorldToCell(groundCheck.position);
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
                if (AudioManager.instance != null)
                {
                    Debug.Log("Play plant sound");
                    AudioManager.instance.PlayFX(PlantSound);
                }
                else
                {
                    Debug.LogWarning("AudioManager instance is null. Cannot play sound.");
                }
            }
            else
            {
                GameEventManager.Ins.TriggerDialog("<color=red>Ô này đã được trồng rồi</color>");
            }
        }
    }

   
}
