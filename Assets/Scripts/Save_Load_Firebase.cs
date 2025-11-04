using System;
using System.IO;
using System.Threading.Tasks;
using Firebase.Database;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
public static class Save_Load_Firebase 
{
    static DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
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
    
    public static async Task SaveData(string path, object value)
    {
       
        await reference.Child(GetUserID()).Child(path).SetValueAsync(value);
       
    }

    public static async Task<DataSnapshot> LoadData(string path)
    {
        
        DataSnapshot snapshot = await reference.Child(GetUserID()).Child(path).GetValueAsync();
        return snapshot;

    }
    public static async Task<DateTime?> GetSeverDateTime()
    {

        try
        {
            await reference.Child("SeverTime").SetValueAsync(Firebase.Database.ServerValue.Timestamp);
            var task = reference.Child("SeverTime").GetValueAsync();
            await task;
            if (task.IsCompleted)
            {
                DataSnapshot dataSnapshot = task.Result;
                long severMiliseconds = (long)dataSnapshot.Value;
                return DateTimeOffset.FromUnixTimeMilliseconds(severMiliseconds).UtcDateTime;
            }
        }catch(Exception e)
        {
            Debug.LogError($"GetServerDateTime error: {e.Message}");
        }
        return null;
    }

    public static async Task<GameData> LoadGame()
    {
        try
        {
           var task = await LoadData("GameData");
            if (task.Exists)
            {
                GameData data = JsonConvert.DeserializeObject<GameData>(task.Value.ToString());
                return data;
            }
            else
            {
                return new GameData();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return new GameData();
        }
    }

    public static async Task SaveGame(GameData data)
    {
      
        string json = JsonConvert.SerializeObject(data);
        try
        {
            await SaveData("GameData", json);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message + "\n" + "From SaveGame() - Save_Load_Firebase");
        }
    }

}
