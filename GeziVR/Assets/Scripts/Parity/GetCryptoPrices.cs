using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GetCryptoPrices : MonoBehaviour
{
    public string url = "https://api.coingecko.com/api/v3/simple/price?ids=bitcoin,ethereum,tether&vs_currencies=try";

    public GameObject ethPrice;
    public GameObject usdtPrice;
    public GameObject tetherPrice;

    void Start()
    {
        StartCoroutine(GetRequest(url));
    }

    IEnumerator GetRequest(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            string json = www.downloadHandler.text;
            Dictionary<string, Dictionary<string, float>> prices = JsonUtility.FromJson<Dictionary<string, Dictionary<string, float>>>(json);

            float btcPrice = prices["bitcoin"]["try"];
            float ethPrice = prices["ethereum"]["try"];
            float usdtPrice = prices["tether"]["try"];

            Debug.Log("BTC Price: " + btcPrice + " TRY");
            Debug.Log("ETH Price: " + ethPrice + " TRY");
            Debug.Log("USDT Price: " + usdtPrice + " TRY");
        }
    }
}
