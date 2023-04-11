using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GetETHPrice : MonoBehaviour
{
    public string url = "https://api.coingecko.com/api/v3/coins/ethereum";

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
            ETHData data = JsonUtility.FromJson<ETHData>(json);
            float ethPrice = data.market_data.current_price.usd;
            Debug.Log("Current ETH Price: $" + ethPrice);
        }
    }

    [System.Serializable]
    public class ETHData
    {
        public MarketData market_data;
    }

    [System.Serializable]
    public class MarketData
    {
        public PriceData current_price;
    }

    [System.Serializable]
    public class PriceData
    {
        public float usd;
    }
}

