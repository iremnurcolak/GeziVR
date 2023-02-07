using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;


public class DatabaseManager : MonoBehaviour
{
    private DatabaseReference reference;
    public PlayerScriptable playerScriptable;
    // Start is called before the first frame update
    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;        
    }

    public void WriteNewUser()
    {
        User user = new User(playerScriptable.name, playerScriptable.email, playerScriptable.profileImageUrl, playerScriptable.token);
        string json = JsonUtility.ToJson(user);
        reference.Child("users").Child(playerScriptable.token.GetHashCode().ToString()).SetRawJsonValueAsync(json).ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.Log("Error");
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Success");
            }
        });
    }
}
