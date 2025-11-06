using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class TreeInfo
{
    public GameObject treePrefab;   //Prefab tree
    public int treeCount;           //Số lượng cây
    public BoxCollider2D spawnArea; //Vùng Spawn riêng cho từng loại cây
}
public class TreeSpawner : MonoBehaviour
{
    [Header("Tree Setting")]
    public List<TreeInfo> treeList = new List<TreeInfo>();  //Danh sách Tree
    public float respawnDelay = 10f;                        //Thời gian sau mỗi lần spawn cây

    [Header("Spawn Area")]  
    public LayerMask obstacleLayer;
    public float checkRadius = 0.3f;    
    
    private List<Vector2> spawnedTreePositions = new List<Vector2>();
    public float minTreeDistance = 1.5f;

    void Start()
    {
        SpawnAllTree();
    }
   
    void SpawnAllTree()
    {
        foreach (TreeInfo treeInfo in treeList)
        {
            if(treeInfo.spawnArea == null)
            {
                Debug.Log("Chưa gán vùng spawn");
                continue;
            }    
            for (int i = 0; i < treeInfo.treeCount; i++)
            {
                SpawnTree(treeInfo.treePrefab, treeInfo.spawnArea);
            }    
        }
    }
    public void SpawnTree(GameObject treePrefab, BoxCollider2D spawnArea)
    {
        Vector2 spawnPos = GetValidSpawnPosition(spawnArea);
        GameObject tree = Instantiate(treePrefab, spawnPos, Quaternion.identity);

        TreeInfor treeScript = tree.GetComponent<TreeInfor>();
        if (treeScript != null)
        {
            treeScript.spawner = this;
            treeScript.treePrefab = treePrefab;
            treeScript.respawnDelay = respawnDelay;
            treeScript.spawnArea = spawnArea;
        }
    } 
    private Vector2 GetValidSpawnPosition(BoxCollider2D spawnArea)
    {
        int maxAttempts = 50; //tránh vòng lặp vô hạn
        for (int i = 0; i < maxAttempts; i++)
        {          
            Vector2 areaCenter = spawnArea.transform.position;
            Vector2 areaSize = spawnArea.size;

            Vector2 randomPos = new Vector2(
                Random.Range(areaCenter.x - areaSize.x / 2, areaCenter.x + areaSize.x / 2),
                Random.Range(areaCenter.y - areaSize.y / 2, areaCenter.y + areaSize.y / 2)
                );

            Collider2D hit = Physics2D.OverlapCircle(randomPos, checkRadius, obstacleLayer);
            if (hit != null)
                continue;

            bool tooClose = false;
            foreach(Vector2 pos in spawnedTreePositions)
            {
                if(Vector2.Distance(pos, randomPos) < minTreeDistance)
                {
                    tooClose = true;
                    break;
                }    
            } 
            if(!tooClose)
            {
                spawnedTreePositions.Add(randomPos);
                return randomPos;
            }    
        }
        //Nếu ko tìm thấy vị trí hợp lệ sau nhìu lần => spawn ra vị trí trung tâm
        return spawnArea.transform.position;
    }
    public IEnumerator RespawnTree(GameObject treePreFab, BoxCollider2D spawnArea)
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnTree(treePreFab, spawnArea);
    }
}
