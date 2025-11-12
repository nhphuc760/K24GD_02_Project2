using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HighLightFarm : MonoBehaviour
{
    [SerializeField] Tilemap farmLand;
    [SerializeField] Tilemap highlightMap;
    [SerializeField] TileBase greenHightLight;
    [SerializeField] TileBase redHightLight;
    public LayerMask cropLayerMask;
    ItemDataSO curToolKit;
    Transform player;
    bool isInFarmLand = false;
    Vector3Int curPos;
    Vector3Int lastHighLightPos;

    private void Awake()
    {
        player = FindAnyObjectByType<Player>().transform;
    }
    private void OnEnable()
    {
       GameEventManager.Ins.toolKitEvent.onCurSelectedChange += CurSelectedChange;
        StartCoroutine(CheckPlayerInFarm());
    }
    private void Update()
    {
        if (curPos == lastHighLightPos) return;
        highlightMap.SetTile(lastHighLightPos, null);
        if (curToolKit != null && curToolKit is SeedDataSO && isInFarmLand)
        {
            var cellCenterWorld = farmLand.GetCellCenterWorld(curPos);
            var hit = Physics2D.OverlapPoint(cellCenterWorld, cropLayerMask);
            if(hit != null)
            {
                highlightMap.SetTile(curPos, redHightLight);
            }
            else
                highlightMap.SetTile(curPos, greenHightLight);

        }
        lastHighLightPos = curPos;

    }

    private void OnDisable()
    {
        StopCoroutine(CheckPlayerInFarm());
        GameEventManager.Ins.toolKitEvent.onCurSelectedChange += CurSelectedChange;
    }

    IEnumerator CheckPlayerInFarm()
    {
        while (true)
        {
            Vector3Int cellPosition = farmLand.WorldToCell(player.position);
            TileBase currentTile = farmLand.GetTile(cellPosition);
            curPos = cellPosition;
            isInFarmLand = currentTile != null;
            yield return new WaitForSeconds(0.1f);
        }
    }
    private void CurSelectedChange(ItemDataSO sO)
    {
        curToolKit = sO;
    }

}
