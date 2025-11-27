using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
public class AnimalManager : MonoBehaviour
{
    int quantityAnimal;
    List<AnimalSaveData> saveDatas = new List<AnimalSaveData> ();
    [SerializeField]
    AnimalDatabase database;
    DateTime curTime;
    [SerializeField] int maxAnimal;
    public static AnimalManager Ins;
    private async void Awake()
    {
        Ins = this; 
        if(database == null)
        {
            database = Resources.Load<AnimalDatabase>("Items/Animal/AnimalData/AnimalDatabase");
        }
        try
        {
            var task = await Save_Load_Firebase.LoadData("AnimalData");
            if (task.Exists)
            {
                var timeSever = await Save_Load_Firebase.GetSeverDateTime();
                if (timeSever.HasValue)
                {
                    curTime = timeSever.Value;
                }
                else
                {
                    curTime = DateTime.UtcNow;
                }
                
                if (task.Value == null) return;
                var task2 = await Save_Load_Firebase.LoadData("AnimalMaxSlot");
                if (task2.Exists)
                {
                    maxAnimal = Convert.ToInt32(task2.Value);
                }
                else
                {
                    maxAnimal = 10;
                }
    
                List<AnimalSaveData> datas = JsonConvert.DeserializeObject<List<AnimalSaveData>>(task.Value.ToString());
                if (datas == null || datas.Count == 0) return;
                foreach (var child in datas)
                {
                    AnimalDataSO dataSO = database.GetAnimalDataByID(child._idSO);
                    if (dataSO == null) continue;
                    GameObject obj = Instantiate(dataSO.animalPrefab, child.worldPosition.ToVector3(), Quaternion.identity);
                    obj.GetComponent<FarmAnimal>().LoadAnimalState(dataSO, child.timeProductReady, curTime);
                    quantityAnimal++;
                }
            }
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    private void OnEnable()
    {
        GameEventManager.Ins.animalEvent.onBuyAnimal += OnBuyAnimal;
        GameEventManager.Ins.onCheckConDition += CheckConditionBuy;
        GameEventManager.Ins.onSeparate += Separate;
    }

    private void Separate(AnimalDataSO sO, int arg2)
    {
        for (int i = 0; i < arg2; i++)
        {
            GameManager.Ins.BuyAnimal(sO);
       
        }
    }

    private bool CheckConditionBuy(AnimalDataSO sO, int quantity)
    {
        return quantityAnimal + quantity <= maxAnimal;
    }

    private void OnBuyAnimal(FarmAnimal animal)
    {
        quantityAnimal++;
    }

    private void OnDisable()
    {
        if (GameEventManager.Ins != null)
        {
            GameEventManager.Ins.animalEvent.onBuyAnimal -= OnBuyAnimal;
            GameEventManager.Ins.onCheckConDition -= CheckConditionBuy;
            GameEventManager.Ins.onSeparate -= Separate;
        }
    }

    private async void OnDestroy()
    {
        await Save_Load_Firebase.SaveData("AnimalData", JsonConvert.SerializeObject(saveDatas));
        await Save_Load_Firebase.SaveData("AnimalMaxSlot", maxAnimal);
    }

    private void OnValidate()
    {
        database = Resources.Load<AnimalDatabase>("Items/Animal/AnimalData/AnimalDatabase");
    }

    public void AddDataAnimal(AnimalSaveData data)
    {
       saveDatas.Add(data);
    }
}
