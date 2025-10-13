using UnityEngine;
using System.Collections;
public class OreSpawner : MonoBehaviour
{
    [Header("Ore Prefabs")]
    public GameObject copperOrePrefab;
    public GameObject ironOrePrefab;
    public GameObject goldOrePrefab;

    [Header("Ore Setting")]
    public int copperOreCount = 10;
    public int ironOreCount = 5;
    public int goldOreCount = 3;
    public float respawnDelay = 10f; //Thời gian spawn lại sau khi đập
    public float spawnRadius = 20f; //Bán kính khu vực spawn quanh vị trí gốc \

    [Header("Spawn Area")]
    public LayerMask obstacleLayer; //Tránh spawm đè lên tường và vật cản
    public float checkRadius = 0.3f;//bán kính kiểm tra va chạm khi spawn 
    void Start()
    {
        for(int i = 0; i < copperOreCount; i++)
        {
            SpawnOre(copperOrePrefab);
        }
        for (int i = 0; i < ironOreCount; i++)
        {
            SpawnOre(ironOrePrefab);
        }
        for (int i = 0; i < goldOreCount; i++)
        {
            SpawnOre(goldOrePrefab);
        }
    }
    //Hàm spawn 1 quặng được gắn Prefab
    public void SpawnOre(GameObject orePrefab)
    {
        Vector2 spawnPos = GetValidSpawnPosition();
        GameObject ore = Instantiate(orePrefab, spawnPos, Quaternion.identity);
        Ore oreScript = ore.GetComponent<Ore>();
        if (oreScript != null)
        {
            oreScript.spawner = this;
            oreScript.respawnDelay = respawnDelay;
        }
    }    
    //Lấy vị trí spawn (ngẫu nhiên và không bị đè lên vật cản)
    private Vector2 GetValidSpawnPosition()
    {
        int maxAttempts = 50; //tránh vòng lặp vô hạn
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
            Collider2D hit = Physics2D.OverlapCircle(randomPos, checkRadius, obstacleLayer);
            if (hit == null || (!hit.CompareTag("Building_Lower") && !hit.CompareTag("Decor_Lower")))
                return randomPos;
        }
        //Nếu ko tìm thấy vị trí hợp lệ sau nhìu lần => spawn ra vị trí trung tâm
        return transform.position;

    } 
    //Hàm gọi lại sau khi quặng bị phá
    public IEnumerator RespawnOre(GameObject orePreFab)
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnOre(orePreFab);
    }    
}
