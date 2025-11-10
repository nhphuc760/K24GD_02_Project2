using System;
using System.Threading.Tasks;
using Firebase.Database;
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
        Debug.Log("Lưu dữ liệu: " + path);
    }

    public static async Task<DataSnapshot> LoadData(string path)
    {
        
        DataSnapshot snapshot = await reference.Child(GetUserID()).Child(path).GetValueAsync();
        Debug.Log("Load dữ liệu từ: " + path);
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

    public static async Task RemoveAllDataBase()
    {
        await reference.Child(GetUserID()).RemoveValueAsync();
        Debug.Log("Xóa dữ liệu thành công");
    }

    public static async Task RemoveAsync(string path)
    {
        await reference.Child(GetUserID()).Child(path).RemoveValueAsync();
    }
}
