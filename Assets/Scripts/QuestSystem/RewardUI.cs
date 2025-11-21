using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI buffReward;
    [SerializeField] Transform containerItem;
    public void UpdateUI(RewardData rewardData, Sprite background, SlotRewardUI prefabSlot)
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
                InstantiateItemReward(item.itemSO._icon, item.quantity, background, prefabSlot);
        }
    }


    void InstantiateItemReward(Sprite icon, int _quantity, Sprite background, SlotRewardUI prefabSlot)
    {
        SlotRewardUI itemReward = Instantiate(prefabSlot, containerItem);
        itemReward.UpdateUI(icon, _quantity, background);
    }

    

}
