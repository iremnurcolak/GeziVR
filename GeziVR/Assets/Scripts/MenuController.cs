using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public TMPro.TextMeshProUGUI infoText;
    public Image profilePicture;
    
    void Start()
    {
        infoText.text = "Hos geldin! " + GoogleSignInFirebase.userEmail;
        StartCoroutine(setImage(GoogleSignInFirebase.userPhotoUrl.ToString()));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SignOut()
    {
        
        GoogleSignInFirebase.SignOutFromGoogle();
        SceneManager.LoadScene("LoginScreen");
    }

    IEnumerator setImage(string url) {
        WWW www = new WWW(url);
        yield return www;

        profilePicture.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
    }
}
