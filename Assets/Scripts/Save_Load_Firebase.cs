using System;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using UnityEngine;
public static class Save_Load_Firebase 
{
    public static string GetUserID()
    {
        var user = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null)
        {
            return user.UserId;
        }
        else
        {
            Debug.LogError("No user is signed in.");
            return null;
        }
    }  
    
    public static void SaveData<T>(string path, T value, Action actionContinueWithMain)
    {
        var reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child(GetUserID()).Child(path).SetValueAsync(value).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Data could not be saved: " + task.Exception);
            }
            else
            {
                Debug.Log("Data saved successfully.");
                actionContinueWithMain?.Invoke();
            }
        });
    }

    public static void LoadData<T>(string path, Action<T> actionContinueWithMain)
    {
        var reference = Firebase.Database.FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child(GetUserID()).Child(path).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Data could not be loaded: " + task.Exception);
              
            }
            else if (task.IsCompleted)
            {
               
            }
        });
    }
}
