using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerFarming : MonoBehaviour
{
    public Tilemap plowableLayer; // Tilemap for plowable ground

    public List<CropData> seedHotbar;
    public CropData selectedSeed;
    public TileBase farmPlotTile;
    public LayerMask cropsLayerMask;//quét tìm cropsLayerMask
    public LayerMask interactableLayerMask;//quét tìm interactableLayerMask
    public float interactionRadius = 0.8f; // Bán kính tương tác với các đối tượng xung quanh

    private void Start()
    {
        if(seedHotbar.Count > 0)
        {
            selectedSeed = seedHotbar[0]; // Mặc định chọn loại hạt giống đầu tiên trong danh sách
        }
    }

    private void Update()
    {
        HandleInput();
    }
    void HandleSeedSelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && seedHotbar.Count >= 1) { selectedSeed = seedHotbar[0]; Debug.Log("Đã chọn: " + selectedSeed.cropName); }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && seedHotbar.Count >= 2) { selectedSeed = seedHotbar[1]; Debug.Log("Đã chọn: " + selectedSeed.cropName); }
    }
    void HandleInput()
    {
        HandleSeedSelection();
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, interactionRadius, interactableLayerMask);
            if(hit != null && hit.GetComponent<IInteractable>() != null)
            {
                hit.GetComponent<IInteractable>().Interact();
            }
            else if(selectedSeed != null)
            {
                Plant(selectedSeed);
            }
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

    void Plant(CropData cropToPlant)
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
                GameObject cropInstance = Instantiate(cropToPlant.cropPrefab, cellCenterPosition, Quaternion.identity);
                cropInstance.GetComponent<Crop>().Plant(cropToPlant);
                Debug.Log($"Đã trồng {cropToPlant.cropName} tại ô {cellPosition}");
            }
            else
            {
                // Nếu đã có cây, báo cho chúng ta biết
                Debug.Log("Ô này đã được trồng rồi!");
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
