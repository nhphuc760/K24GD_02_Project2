using System;
using System.Collections;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class Kitchen : MonoBehaviour
{

    [SerializeField] TextMeshPro timerText;
    [SerializeField] SpriteRenderer _iconItemCooking;
    DateTime coolDownStart;
    DateTime coolDownEnd;
    RecipeSO curRecipeSO;
    bool isCooking;
    [SerializeField] RecipeListSO recipeSOlist;
    bool isChangeItem;
    private async void Start()
    {
        //Load
        timerText.enabled = false;
        _iconItemCooking.enabled = false;
        var task = await Save_Load_Firebase.LoadData("Kitchen/KitchenDatabase");
        var task2 = await Save_Load_Firebase.GetSeverDateTime();
        if (task.Exists)
        {
            var kitchenData = JsonConvert.DeserializeObject<KitchenData>(task.Value.ToString());
            if(task2 != null)
            {
                coolDownStart = task2.Value;
            }
            else
            {
                coolDownStart = kitchenData.coolDownStart;
            }
                coolDownEnd = kitchenData.coolDownEnd;
            var timeSpanStart = kitchenData.coolDownEnd - coolDownStart;
            curRecipeSO = recipeSOlist.GetRecipeSOByIdResult(kitchenData._idItemKitchen);
            if (timeSpanStart < TimeSpan.Zero)
            {
                isCooking = false;
            }
            else
            {
                isCooking = kitchenData.isCooking;
            }
            
        }
        if (isCooking)
        {
            StartCoroutine(CoolDownTimer());
        }
        else
        {
            if (curRecipeSO != null)
            {
                //món đã nấu xong. chờ nhận item
                if(_iconItemCooking != null){
                _iconItemCooking.enabled = true;
                    _iconItemCooking.sprite = curRecipeSO.result._icon;
                }
            }
        }
        GameEventManager.Ins.cookingEvent.onCookClick += CookingEvent_onCookClick;
        GameEventManager.Ins.cookingEvent.confirmCookingChange += CookingEvent_confirmCookingChange;
    }

    private async void CookingEvent_onCookClick(RecipeSO recipe)
    {
        await Cook(recipe);
    }

    private async void CookingEvent_confirmCookingChange(RecipeSO recipe)
    {
        isCooking = false;
        curRecipeSO = null;
        isChangeItem = true;
        await Cook(recipe);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

           GameEventManager.Ins.cookingEvent.PlayerEnterKitchen();
            if(!isCooking && curRecipeSO != null)
            {
                GetItem();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
          GameEventManager.Ins.cookingEvent.PlayerLeaveKitchen();
        }
    }

    public async Task Cook(RecipeSO recipeSO)
    {

        //kiểm tra số lượng, item cần có trong inventory
        if(curRecipeSO != null && isCooking)
        {
            //Trigger Dialog change cooking
            GameEventManager.Ins.cookingEvent.EnableDialogCookingChange(recipeSO);
            return;
        }
        foreach (var item in recipeSO.ingredients)
        {
            if (!GameEventManager.Ins.inventoryEvent.CheckHasItem(item.itemSO, item.quantity))
            {
                GameEventManager.Ins.TriggerDialog("<color=red>Vui lòng thu thập đủ nguyên liệu trước khi bắt đầu</color>");
                return;
            }
        }

        isCooking = true;
        curRecipeSO = recipeSO;
        foreach (var item in recipeSO.ingredients)
        {
            GameEventManager.Ins.inventoryEvent.RemoveItemByData(item.itemSO, item.quantity);
        }

        var severTime = await Save_Load_Firebase.GetSeverDateTime();

        if(severTime != null)
        {
            coolDownStart = severTime.Value;
           coolDownEnd = coolDownStart.AddSeconds(curRecipeSO.timeCooldown);
            await Save();
            StartCoroutine(CoolDownTimer());
        }
        GameEventManager.Ins.cookingEvent.StartCooking();
    }

    IEnumerator CoolDownTimer()
    {
        _iconItemCooking.enabled = true;
        _iconItemCooking.sprite = curRecipeSO.result._icon;
        timerText.enabled = true;
        while (coolDownStart < coolDownEnd)
        {


            if (isChangeItem)
            {
                isChangeItem = false;
                break;
            }
            coolDownStart = coolDownStart.AddSeconds(1);
            timerText.text = (coolDownEnd - coolDownStart).ToString(@"hh\:mm\:ss");
            if(coolDownStart >= coolDownEnd)
            {
                //Trigger event finishCook
                GameEventManager.Ins.TriggerDialog($"<color=green>{curRecipeSO.result._itemName} của bạn đã hoàn thành</color>");
                isCooking = false;
                timerText.enabled = false;
                break;
            }
            yield return new WaitForSeconds(1);
        }
    }

    private async void OnDestroy()
    {
        GameEventManager.Ins.cookingEvent.onCookClick -= CookingEvent_onCookClick;
        GameEventManager.Ins.cookingEvent.confirmCookingChange -= CookingEvent_confirmCookingChange;
        if (curRecipeSO == null) return;
        await Save();
      
    }
    public void GetItem()
    {
       
        timerText.enabled = false;
        _iconItemCooking.enabled = false;
        isCooking = false;    
        if (GameEventManager.Ins.inventoryEvent.AddItem(curRecipeSO.result, 1))
        {
            Debug.Log("Thêm thành công");
        }
        else
        {
            Debug.Log("Oops");
        }
        curRecipeSO = null;
    }

    async Task Save()
    {
        KitchenData kitchenData = new KitchenData
        {
            coolDownStart = this.coolDownStart,
            coolDownEnd = this.coolDownEnd,
            isCooking = this.isCooking,
            _idItemKitchen = curRecipeSO.result._id
        };

        await Save_Load_Firebase.SaveData("Kitchen/KitchenDatabase", kitchenData.ToString());
    }
}
