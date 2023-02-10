using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;


public class DatabaseManager : MonoBehaviour
{
    private DatabaseReference reference;
    public PlayerScriptable playerScriptable;

    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;        
    }   
}

