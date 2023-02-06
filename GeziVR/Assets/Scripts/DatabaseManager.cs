using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using TMPro;

public class DatabaseManager : MonoBehaviour
{
    private DatabaseReference reference;
    
    public void SaveNewPlayer(string name, string email, string token)
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        Player player = new Player(name, email, token);
        string json = JsonUtility.ToJson(player);
        reference.Child("visitors").Child(email).SetRawJsonValueAsync(json);
    }
}
