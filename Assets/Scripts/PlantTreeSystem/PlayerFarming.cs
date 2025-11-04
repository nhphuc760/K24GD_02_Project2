using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class PlayerFarming : MonoBehaviour
{
    public Tilemap plowableLayer; // Tilemap for plowable ground
    public TileBase farmPlotTile;
    public LayerMask cropsLayerMask;//quét tìm cropsLayerMask
    public LayerMask interactableLayerMask;//quét tìm interactableLayerMask
    public float interactionRadius = 0.8f; // Bán kính tương tác với các đối tượng xung quanh

    private void Start()
    {
        GameEventManager.Ins.gameInput.interacPressed += HandleInput;
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
            this.enabled = true;
        }
    }

    private void OnDisable()
    {
        GameEventManager.Ins.gameInput.interacPressed -= HandleInput;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }
    //void HandleSeedSelection()
    //{
    //    if (Input.GetKeyDown(KeyCode.Alpha1) && seedHotbar.Count >= 1) { selectedSeed = seedHotbar[0]; Debug.Log("Đã chọn: " + selectedSeed.cropName); }
    //    else if (Input.GetKeyDown(KeyCode.Alpha2) && seedHotbar.Count >= 2) { selectedSeed = seedHotbar[1]; Debug.Log("Đã chọn: " + selectedSeed.cropName); }
    //}
    void HandleInput()
    {
        //HandleSeedSelection();
        InventorySlot curSelected = GameEventManager.Ins.toolKitEvent.GetCurDataChoose();
        var seedData = curSelected.ItemData as SeedData;
        if(curSelected == null || seedData == null)
        {
            GameEventManager.Ins.TriggerDialog("Bạn chưa chọn hạt giống");
            return;
        } 
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactionRadius, interactableLayerMask);
        if (hit != null && hit.GetComponent<IInteractable>() != null)
        {
            Debug.Log("Harvest");
            hit.GetComponent<IInteractable>().Interact();
        }
        else if (seedData!= null)
        {
            Plant(seedData);
        }
    }
    //void TryInteract()
    //{
    //    Collider2D hit  = Physics2D.OverlapCircle(transform.position, interactionRadius);
    //    if(hit != null)
    //    {
    //        //Thữ lấy IInteraactable từ đối tượng bị va chạm
    //        IInteractable interactableObject = hit.GetComponent<IInteractable>();
    //        if(interactableObject != null)
    //        {
    //            //Néu có thì gọi hàm Interact
    //            interactableObject.Interact();
    //            return;
    //            Debug.Log("Đã tương tác với " + hit.name);
    //        }
    //    }
    //    if(selectedSeed != null)
    //    {
    //        Plant(selectedSeed);
    //    }
    //}


    //Trồng, thu hoạch cây.
    void Plant(SeedData cropToPlant)
    {
        Vector3Int cellPosition = plowableLayer.WorldToCell(transform.position);
        Vector3 cellCenterPosition = plowableLayer.GetCellCenterWorld(cellPosition);
        TileBase currentTile = plowableLayer.GetTile(cellPosition);
        if (currentTile == farmPlotTile)
        {
            Collider2D existingCrop = Physics2D.OverlapCircle(cellCenterPosition, 0.1f, cropsLayerMask);//kiêm tra xem đã có cây trồng chưa
            if (existingCrop == null)
            {
                // Thì mới tiến hành trồng cây
                GameObject cropInstance = Instantiate(cropToPlant.cropData.prefab, cellCenterPosition, Quaternion.identity);
                cropInstance.GetComponent<Seed>().Plant(cropToPlant);
                Debug.Log($"Đã trồng {cropToPlant._itemName} tại ô {cellPosition}");
            }
            else
            {
                GameEventManager.Ins.TriggerDialog("Ô này đã được trồng rồi");
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // Sửa ở đây: dùng biến interactionRadius để hình vẽ luôn khớp
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
