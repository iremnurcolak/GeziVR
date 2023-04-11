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
    public TMPro.TextMeshProUGUI balanceText;
    public Image profilePicture;

    public PlayerScriptable playerScriptable;
    
    void Start()
    {
        infoText.text = "Hos geldin " + playerScriptable.name + "!";
        
        StartCoroutine(setImage(playerScriptable.profileImageUrl));
        if(playerScriptable.accountAddress != "")
            StartCoroutine(GetBalance("https://gezivr-web3.onrender.com/getBalance/" + playerScriptable.accountAddress));

    }
    
    void Update()
    {
        infoText.text = "Hos geldin " + playerScriptable.name + "!";
        balanceText.text = playerScriptable.balance.ToString();
        StartCoroutine(setImage(playerScriptable.profileImageUrl));
    }


    IEnumerator setImage(string url) {
        WWW www = new WWW(url);
        yield return www;

        profilePicture.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
    }

    IEnumerator GetBalance(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
            playerScriptable.balance = float.Parse(www.text.Replace(".", ","));
            balanceText.text = playerScriptable.balance.ToString();
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }

    public void EnterMuseum()
    {
        SceneManager.LoadScene("GeziVr");
        
    }

    public void GoToGallery()
    {
        SceneManager.LoadScene("GalleryScreen");
    }
    
    public void GoToWiki()
    {
        SceneManager.LoadScene("Wikipedia");
    }

    public void GoToPaymentSettings()
    {
        SceneManager.LoadScene("Settings");
    }
}
