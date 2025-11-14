using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FarmLandManager : MonoBehaviour
{
    [SerializeField] Tilemap farmLand;
    [SerializeField] Tilemap highlightMap;
    [SerializeField] TileBase greenHightLight;
    [SerializeField] TileBase redHightLight;
    [SerializeField] TileBase normalHighLight;
    public LayerMask cropLayerMask;
    ItemDataSO curToolKit;
    Transform groundCheck;
    bool isInFarmLand = false;
    Vector3Int curPos;
    Vector3Int lastHighLightPos;
    private void Awake()
    {
        Transform player = FindAnyObjectByType<Player>().transform;
        groundCheck = player.GetChild(1);
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
        if(!isInFarmLand)return;


        var cellCenterWorld = farmLand.GetCellCenterWorld(curPos);
        var hit = Physics2D.OverlapPoint(cellCenterWorld, cropLayerMask);
        if(hit != null && hit.TryGetComponent<Seed>(out Seed seed))
        {
            seed.ShowCoolDown();
        }
        var cellCenterLasPos = farmLand.GetCellCenterWorld(lastHighLightPos);
        var hitLast = Physics2D.OverlapPoint(cellCenterLasPos, cropLayerMask);
        if (hitLast != null && hitLast.TryGetComponent<Seed>(out Seed lastSeed))
        {
            lastSeed.HideCoolDown();
        }

        if (curToolKit != null && curToolKit is SeedDataSO)
        {  
            var tilebase = hit != null ? redHightLight : greenHightLight;
            highlightMap.SetTile(curPos, tilebase);

        }else
        {
            highlightMap.SetTile(curPos, normalHighLight);
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
            Vector3Int cellPosition = farmLand.WorldToCell(groundCheck.position);
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
