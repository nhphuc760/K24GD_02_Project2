using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "FishDatabase", menuName = "Scriptable Objects/FishDatabaseSO")]
public class FishDataBase : ScriptableObject
{
    public List<FishDataSO> fishDatabase;
    private void OnValidate()
    {
        fishDatabase = Resources.LoadAll<FishDataSO>("Items/Fish_7").ToList();
    }
    public FishDataSO GetFishDataById(int id)
    {
        return fishDatabase.Find(x => x._id == id);
    }
    public FishDataSO GetRandomFish()
    {
        return fishDatabase[Random.Range(0, fishDatabase.Count - 1)];
    }
}
