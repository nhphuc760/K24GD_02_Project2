using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI buffReward;
    [SerializeField] Transform containerItem;
    public void UpdateUI(RewardData rewardData, Sprite background, SlotRewardUI prefabSlot, RectTransform rootParent)
    {
        if (buffReward != null)
        {
            buffReward.fontStyle = FontStyles.Italic;
            buffReward.color = Color.black;
            buffReward.text = $"Exp: {rewardData.experience}, Gold: {rewardData.gold}";
        }

        if (containerItem == null)
        {
            Debug.LogError("[RewardUI] containerItem is not assigned in the Inspector.");
            return;
        }

        if (rewardData.items == null) return;
        foreach (var item in rewardData.items)
        {
            if (item.itemSO != null) 
                InstantiateItemReward(item.itemSO._icon, item.quantity, background, prefabSlot, item.itemSO._itemName, rootParent);
        }
    }


    void InstantiateItemReward(Sprite icon, int _quantity, Sprite background, SlotRewardUI prefabSlot, string _nameItem ,RectTransform rootParent)
    {
        SlotRewardUI itemReward = Instantiate(prefabSlot, containerItem);
        itemReward.InitForToolTip(_nameItem, rootParent);
        itemReward.UpdateUI(icon, _quantity, background);
    }

    

}
