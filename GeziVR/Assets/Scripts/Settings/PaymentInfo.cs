using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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
        StartCoroutine(UpdatePaymentInfo("https://gezivr.onrender.com/setPaymentInfo/" + playerScriptable.token + "/" + privateKey.text + "/" + accountAddress.text));
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
            Debug.Log("WWW Ok!: " + www.text);
            playerScriptable.accountAddress = www.text;
            accountAddress.text = www.text;
            isAccountAddressSet = true;
            StartCoroutine(GetBalance("https://gezivr-web3.onrender.com/getBalance/" + www.text));
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
            playerScriptable.privateKey = www.text;
            privateKey.text = www.text;
            isPrivateKeySet = true;
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }

    IEnumerator UpdatePaymentInfo(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        if (www.error == null)
        {
            Debug.Log("Update Payment Ok!: " + www.text);
            playerScriptable.privateKey = privateKey.text;
            playerScriptable.accountAddress = accountAddress.text;
            StartCoroutine(GetBalance("https://gezivr-web3.onrender.com/getBalance/" + accountAddress.text));
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }
    
    public void GoBackMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScreen");
    }


}
