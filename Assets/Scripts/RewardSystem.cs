using UnityEngine;

public class RewardSystem : MonoBehaviour
{
    public static RewardSystem Ins;
    private void Awake()
    {
       if(Ins!= null && Ins != this)
       {
        Destroy(this.gameObject);
       }
       else
       {
        Ins = this;
        DontDestroyOnLoad(this.gameObject);
        }
    }

    public void GiveReward(RewardData rewardData)
    {
        if (rewardData == null) return;
        if(rewardData.experience > 0)
        {
            Debug.Log($"Player gained {rewardData.experience} experience points.");
            // Add experience to player
        }
        if(rewardData.gold > 0)
        {
            Debug.Log($"Player gained {rewardData.gold} gold.");
            // Add gold to player
        }
        if(rewardData.items != null && rewardData.items.Count > 0)
        {
            foreach(var itemReward in rewardData.items)
            {
               // InventoryManager.Instance.inventory.AddItem(itemReward.item, itemReward.quantity);
            }
        }
    }
}
