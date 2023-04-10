using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerScriptable : ScriptableObject
{
    public string name;
    public string email;
    public string profileImageUrl;
    public string token;
    public float balance;

    public void Reset()
    {
        this.name = "";
        this.email = "";
        this.profileImageUrl = "";
        this.token = "";
        this.balance = 0;
    }
}
