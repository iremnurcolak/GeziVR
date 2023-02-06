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
        infoText.text = "Hos geldin! " + PlayerPrefs.GetString("name");
        StartCoroutine(setImage(PlayerPrefs.GetString("photoUrl")));
        GameObject DBManager = GameObject.Find("DatabaseManager");
        DBManager.GetComponent<DatabaseManager>().SaveNewPlayer(PlayerPrefs.GetString("name"), PlayerPrefs.GetString("email"), PlayerPrefs.GetString("token"));
    }

    // Update is called once per frame
    void Update()
    {
        
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
