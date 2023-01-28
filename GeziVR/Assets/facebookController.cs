using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;

public class facebookController : MonoBehaviour
{
    

    private void Awake()
    {    
        FB.Init(SetInit, OnHideUnity);
    }
    void SetInit()
    {
        if (FB.IsLoggedIn)
        {
            Debug.Log("Logged in Successfully");
        }
        else
        {
            Debug.Log("FB is not logged in");
        }
    }
    void OnHideUnity(bool isGameShown)
    {
        if (isGameShown)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
    }
    public void FBLogin()
    {
        List<string> permissions = new List<string>();
        permissions.Add("public profile");
        FB.LogInWithReadPermissions(permissions, AuthCallResult);
    }
    void AuthCallResult(ILoginResult result)
    {
        if(result.Error != null)
        {
            Debug.Log(result.Error);
        }
        else
        {
            if (FB.IsLoggedIn)
            {
                Debug.Log("FB logged in");
                Debug.Log(result.RawResult);
            }
            else
            {
                Debug.Log("Login Failed!");
            }
        }
    }
   
}
