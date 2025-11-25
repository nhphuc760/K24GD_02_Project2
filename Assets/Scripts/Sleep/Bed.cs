using UnityEngine;

public class Bed : MonoBehaviour, IInteractable
{
    public int sleepStartHour = 20; // 20 giờ (8h tối) mới được ngủ
    public int wakeupHour = 6;
    public bool CanInteract()
    {
        int currentHour = TimeManager.instance.GetCurrentHour();
        if (currentHour >= sleepStartHour || currentHour < wakeupHour)
            return true;
        else
            return false;
    }

    public void Interact()
    {
        if (TimeManager.instance == null) return;
        int currentHour = TimeManager.instance.GetCurrentHour();

        if (currentHour >= sleepStartHour || currentHour < wakeupHour)
        {
            Debug.Log("Zzz... Đi ngủ thôi!");
            GameManager.Ins.StartSleepSequence();
        }
        else
        {
            // --- CHƯA ĐỦ ĐIỀU KIỆN -> BÁO LỖI ---
            Debug.Log("Còn sớm quá, chưa ngủ được!");

            //gọi hệ thống dialog
            GameEventManager.Ins.TriggerDialog($"<color=brown>Chưa đến giờ ngủ! (Hãy quay lại sau {sleepStartHour}:00)</color>");
        }
    }
}
