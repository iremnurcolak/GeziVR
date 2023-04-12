using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
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
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
 
            string[] pages = url.Split('/');
            int page = pages.Length - 1;
 
            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                playerScriptable.balance = float.Parse(webRequest.downloadHandler.text.Replace(".", ","));
                balanceText.text = playerScriptable.balance.ToString();
            }
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
