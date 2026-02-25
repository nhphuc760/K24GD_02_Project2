using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;
    public static event Action OnNewDay;

    public float secondsperDay = 900f;//ví dụ 1 ngày = 2phut;

    [Header("Internal Tracking")]
    [Tooltip("Tổng số giây đã trôi qua trong toàn bộ game")]
    public double totalTimeElapsed = 0;


    private int currentHour = 6; //bắt đầu ngày mới từ 6 giờ sáng
    private int currentMinute = 0;
    private float timeSinceLastMinute;
    private float secondsPerMinute;


    //sleep
    public event Action<float> OnTimeSkipped; //báo cho crop biết đã bao lâu trôi qua khi ngủ
    public float SecondsPerMinute { get => secondsPerMinute; }

    private void Start()
    {
        //Thiết lập singleton
       instance = this;
        secondsPerMinute = secondsperDay / 1440f; //1440 phút trong một ngày
        //Cập nhật UI lần đầu tiên khi game bắt đầu

        UIManager.instance.UpdateClock(currentHour, currentMinute);
    }
    void Update()
    {
        //cập nhật tổng hời giand đã trôi qua
        totalTimeElapsed += Time.deltaTime;
        timeSinceLastMinute += Time.deltaTime;

        // Nếu đã trôi qua đủ thời gian cho 1 phút trong game
        if (timeSinceLastMinute >= secondsPerMinute)
        {
            timeSinceLastMinute = 0;
            currentMinute++;

            // Nếu hết 60 phút, tăng giờ lên
            if (currentMinute >= 60)
            {
                currentMinute = 0;
                currentHour++;

                // Nếu hết 24 giờ, reset về 0 và báo ngày mới
                if (currentHour >= 24)
                {
                    currentHour = 0;
                    OnNewDay?.Invoke(); // Rung chuông báo ngày mới!
                    Debug.Log("Một ngày mới đã bắt đầu!");
                }
            }

            // Gọi UIManager để cập nhật hiển thị đồng hồ
            UIManager.instance.UpdateClock(currentHour, currentMinute);
        }
    }
    public int GetCurrentHour()
    {
        return currentHour;
    }
    public int GetCurrentMinute()
    {
        return currentMinute;
    }


    //hàm được gọi khi ngủ
    public void SkipToNextDay()
    {
        //Tính toán thời gian chênh lệch (Game Time)
        int hoursToSkip = (24 - currentHour) + 6; // Từ giờ hiện tại đến 24h đêm + 6h sáng hôm sau
        int minutesToSkip = (hoursToSkip * 60) - currentMinute;

        float realSecondsSkipped = minutesToSkip * secondsPerMinute;
        Debug.Log($"Đi ngủ: Bỏ qua {minutesToSkip} phút game (~{realSecondsSkipped} giây thực).");

        OnTimeSkipped?.Invoke(realSecondsSkipped);
        currentHour = 6;
        currentMinute = 0;

        //cập nhật ui
        if (UIManager.instance != null)
            UIManager.instance.UpdateClock(currentHour, currentMinute);

        Debug.Log("Ngủ dậy vào ngày mới lúc 6 giờ sáng!");
    }
}
