using System;
using System.Collections.Generic;
using NUnit.Framework.Internal.Execution;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "SeedDataSO", menuName = "Scriptable Objects/SeedDataSO")]
public class SeedDataSO : ItemShopDataSO 
{
    [Header("Crop Info")]
    [Tooltip("Khoảng thời gian từ lúc trồng đến lúc thu hoạch, tính bằng giây")]
    public float timeSpandHarvest;// khoảng thời gian phát triển
    public CropDataSO cropData; //Dữ liệu cây thu hoạch
    [Header("Crop Stages Sprites adn prefab")]
    public List<Sprite> growhtSprites; // Sprite for the mature stage
    public Sprite harvestIndicator;
    [Header("Crop Economic Value")]
    public int yield;// sản lượng thu được mỗi khi thu hoạch



    private void Reset()
    {
        isCanSell = true;
    }


    /// <summary>
    /// Lấy khoảng thời gian mỗi giai đoạn của cây
    /// </summary>
    /// <returns></returns>
    public float GetTimeSpanPerProgress()
    {
        return timeSpandHarvest / (growhtSprites.Count - 1);
    }
    public override void OnValidate()
    {
        _description = $"Trồng và thu hoạch\nThời gian: {timeSpandHarvest}\nSản lượng: {yield}\nLợi nhuận: {cropData.sell}$/1";
        
    }
}


