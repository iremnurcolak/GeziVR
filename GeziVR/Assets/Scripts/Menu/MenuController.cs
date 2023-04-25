using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using OpenAI_handler;

public class MenuController : MonoBehaviour
{
    public TMPro.TextMeshProUGUI infoText;
    public TMPro.TextMeshProUGUI balanceText;
    public Image profilePicture;

    public PlayerScriptable playerScriptable;
    
    void Start()
    {
        infoText.text = "Hos geldin " + playerScriptable.name + "!";
        // Sunum sırasında alt satir yorumdan cikarilmali
        // gameObject.AddComponent<OpenAIRequestHandler>().generate();

        StartCoroutine(setImage(playerScriptable.profileImageUrl));
        if(playerScriptable.accountAddress == "")
            StartCoroutine(GetAccountAddress("https://gezivr.onrender.com/getAccountAddress/" + playerScriptable.token));
        else if( playerScriptable.accountAddress != "No account address")
            StartCoroutine(GetBalance("https://gezivr-web3.onrender.com/getBalance/" + playerScriptable.accountAddress));
        if(playerScriptable.privateKey == "")
            StartCoroutine(GetPrivateKey("https://gezivr.onrender.com/getPrivateKey/" + playerScriptable.token));

    }
    
    void Update()
    {
        //infoText.text = "Hos geldin " + playerScriptable.name + "!";
        //balanceText.text = playerScriptable.balance.ToString();
        //StartCoroutine(setImage(playerScriptable.profileImageUrl));
    }

    IEnumerator GetAccountAddress(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
            playerScriptable.accountAddress = www.text;
            Debug.Log("Account address: " + playerScriptable.accountAddress);
            if(playerScriptable.accountAddress != "No account address")
            {
                StartCoroutine(GetBalance("https://gezivr-web3.onrender.com/getBalance/" + playerScriptable.accountAddress));
            }            
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }

    IEnumerator GetPrivateKey(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
            var json = JsonUtility.FromJson<JsonPrivateKey>(www.text);
            if(json.privateKey != null && json.privateKey != "")
            {
                string private_key = DecryptAES(json.privateKey);
                private_key = private_key.Substring(0, json.length);
                playerScriptable.privateKey = private_key;
            }
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
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
        SceneManager.LoadScene("MainArea");
        
    }

    public void GoToGallery()
    {
        SceneManager.LoadScene("GalleryScreen");
    }

    public void GoToPaymentSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    private string DecryptAES(string input)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes("1c133ce01ec9718fbb2fd514527956694540e1c52110eacd3ea7d9a269b4606e");
        byte[] salt = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF };
        byte[] derivedKey;

        using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(keyBytes, salt, 10000))
        {
            derivedKey = pbkdf2.GetBytes(32); // 256-bit key
        }

        byte[] inputBytes = Convert.FromBase64String(input);

        using (RijndaelManaged rijndael = new RijndaelManaged())
        {
            rijndael.KeySize = 256;
            rijndael.BlockSize = 128;
            rijndael.Key = derivedKey;
            rijndael.IV = new byte[16];
            rijndael.Mode = CipherMode.CBC;

            ICryptoTransform decryptor = rijndael.CreateDecryptor(rijndael.Key, rijndael.IV);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                {
                    cs.Write(inputBytes, 0, inputBytes.Length);
                }
                byte[] decryptedBytes = ms.ToArray();
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
    }
}
