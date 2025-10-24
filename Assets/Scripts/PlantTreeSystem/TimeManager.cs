using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static event Action OnNewDay;

    public float secondsperDay = 10f;

    private float dayTimer;


    void Update()
    {
        dayTimer += Time.deltaTime;
        if (dayTimer >= secondsperDay)
        {
            dayTimer = 0f;
            OnNewDay?.Invoke();
            Debug.Log("A new day has started!");
        }
    }
}
