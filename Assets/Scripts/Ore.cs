using UnityEngine;

public class Ore : MonoBehaviour
{
    public OreSpawner spawner;
    public GameObject orePrefab; // prefab chính của quặng này
    public float respawnDelay = 10f;

    private bool isDestroyed = false;
    public void MineOre()
    {
        if(isDestroyed) return;
        isDestroyed = true;
        gameObject.SetActive(false); // ẩn quặng (có thể Destroy nếu mún)

        //Gọi hàm spawn lại quặng mới
        if(spawner != null )
        {
            spawner.StartCoroutine(spawner.RespawnOre(orePrefab));
        }    

    }    

    
    
}
