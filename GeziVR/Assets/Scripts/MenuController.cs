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
        balanceText.text = playerScriptable.balance.ToString();
        StartCoroutine(setImage(playerScriptable.profileImageUrl));
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

    public void EnterMuseum()
    {
        SceneManager.LoadScene("GeziVr");
        
    }
}
