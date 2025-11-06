using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest Infor", menuName = "Scriptable Objects/QuestInforSO")]
public class QuestInforSO : ScriptableObject
{
    [field:SerializeField] public string _id { get; private set; }
    [Header("General")]
    public string displayName;
    [Header("Requirements")]
    public int requiredLevel;
    public QuestInforSO[] prerequisiteQuests;
    [Header("Steps")]
    public GameObject questStepPrefab;
    [Header("Rewards")]
    public RewardData reward;
    private void OnValidate()
    {
#if UNITY_EDITOR
        this._id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }



}

[Serializable]
public class RewardData
{
    public int experience;
    public int gold;
    public List<ItemReward> items;
}

[Serializable]
public class ItemReward
{
    public ItemDataSO itemSO;
    public int quantity;
}
