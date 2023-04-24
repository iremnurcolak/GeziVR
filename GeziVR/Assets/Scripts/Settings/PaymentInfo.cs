using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class PaymentInfo : MonoBehaviour
{
    public PlayerScriptable playerScriptable;

    [SerializeField] private TMP_InputField privateKey;
    [SerializeField] private  TMP_InputField accountAddress;
    [SerializeField] private TMP_Text balance;

    private bool isBalanceSet = false;
    private bool isPrivateKeySet = false;
    private bool isAccountAddressSet = false;

    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject paymentInfoPanel;
    
    
    void Start()
    {
        paymentInfoPanel.SetActive(false);
        loadingPanel.SetActive(true);
        
        StartCoroutine(GetAccountAddress("https://gezivr.onrender.com/getAccountAddress/" + playerScriptable.token));
        StartCoroutine(GetPrivateKey("https://gezivr.onrender.com/getPrivateKey/" + playerScriptable.token));
    }

    void Update()
    {
        if (isBalanceSet && isPrivateKeySet && isAccountAddressSet)
        {
            loadingPanel.SetActive(false);
            paymentInfoPanel.SetActive(true);
        }
    }

    public void UpdatePaymentInfo()
    {
        string encrypted_key = "";
        
        if(privateKey.text != "")
            encrypted_key = EncryptAES(privateKey.text);
        int len = privateKey.text.Length;
        StartCoroutine(UpdatePaymentInfo("https://gezivr-web3.onrender.com/setPaymentInfo" ,encrypted_key , playerScriptable.token,accountAddress.text , len));
    
    }

    IEnumerator GetBalance(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
            playerScriptable.balance = float.Parse(www.text.Replace(".", ","));
            balance.text = www.text;
            isBalanceSet = true;
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }

    IEnumerator GetAccountAddress(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        if (www.error == null)
        {
            if(www.text != "No account address"){
                Debug.Log("WWW Ok!: " + www.text);
                playerScriptable.accountAddress = www.text;
                accountAddress.text = www.text;
                
                StartCoroutine(GetBalance("https://gezivr-web3.onrender.com/getBalance/" + www.text));
            }
            else
            {
                isBalanceSet = true;
            }
            isAccountAddressSet = true;
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
            Debug.Log("AAAA"  + json.privateKey);
            if(json.privateKey != null && json.privateKey != "")
            {
                string private_key = DecryptAES(json.privateKey);
                private_key = private_key.Substring(0, json.length);
                playerScriptable.privateKey = private_key;
                privateKey.text = private_key;
            }
            isPrivateKeySet = true;
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }

    IEnumerator UpdatePaymentInfo(string url, string en_privateKey, string token, string accountAddress, int len)
    {

        JsonPaymentInfo jsonPaymentInfo = new JsonPaymentInfo(token, en_privateKey, accountAddress, len);
        var jsonDataToSend = JsonConvert.SerializeObject(jsonPaymentInfo);
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            byte [] bodyRaw = Encoding.UTF8.GetBytes(jsonDataToSend);
            www.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Update Payment Ok!");
                playerScriptable.privateKey = privateKey.text;
                playerScriptable.accountAddress = accountAddress;
                if(accountAddress != "")
                    StartCoroutine(GetBalance("https://gezivr-web3.onrender.com/getBalance/" + accountAddress));
                else
                    playerScriptable.balance = 0;
                    balance.text = "0";
            }
        }
        
    }
    
    public void GoBackMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScreen");
    }

    private string EncryptAES(string input)
    {
        string key = "1c133ce01ec9718fbb2fd514527956694540e1c52110eacd3ea7d9a269b4606e";
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] salt = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF };
        byte[] derivedKey;

        using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(keyBytes, salt, 10000))
        {
            derivedKey = pbkdf2.GetBytes(32); // 256-bit key
        }

        byte[] inputBytes = Encoding.UTF8.GetBytes(input);

        using (RijndaelManaged rijndael = new RijndaelManaged())
        {
            rijndael.KeySize = 256;
            rijndael.BlockSize = 128;
            rijndael.Key = derivedKey;
            rijndael.IV = new byte[16];
            rijndael.Mode = CipherMode.CBC;

            ICryptoTransform encryptor = rijndael.CreateEncryptor(rijndael.Key, rijndael.IV);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(inputBytes, 0, inputBytes.Length);
                }
                byte[] encryptedBytes = ms.ToArray();
                return Convert.ToBase64String(encryptedBytes);
            }
        }
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


public class JsonPaymentInfo
{
    public string userId;
    public string en_privateKey;
    public string accountAddress;
    public int length;
    public JsonPaymentInfo(string userId, string en_privateKey, string accountAddress, int length)
    {
        this.userId = userId;
        this.en_privateKey = en_privateKey;
        this.accountAddress = accountAddress;
        this.length = length;
    }
}

public class JsonPrivateKey
{
    public string privateKey;
    public int length;
    public JsonPrivateKey(string privateKey, int length)
    {
        this.privateKey = privateKey;
        this.length = length;
    }
}