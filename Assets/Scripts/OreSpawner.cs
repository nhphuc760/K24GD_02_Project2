using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class OreInfo
{
    public GameObject orePrefab;    //Prefab của quặng
    public int oreCount;            //Số lượng quặng spawn
}
public class OreSpawner : MonoBehaviour
{
    

    [Header("Ore Setting")]
    public List<OreInfo> oreList = new List<OreInfo>(); // Danh sách các loại quặng
    public float respawnDelay = 10f; //Thời gian spawn lại sau khi đập
    public float spawnRadius = 20f; //Bán kính khu vực spawn quanh vị trí gốc \

    [Header("Spawn Area")]
    public LayerMask obstacleLayer; //Tránh spawm đè lên tường và vật cản
    public float checkRadius = 0.3f;//bán kính kiểm tra va chạm khi spawn 
    void Start()
    {
        foreach (OreInfo oreInfo in oreList)
        {
            for (int i = 0; i < oreInfo.oreCount; i++)
            {
                SpawnOre(oreInfo.orePrefab);
            }
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
            if (hit == null)
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
