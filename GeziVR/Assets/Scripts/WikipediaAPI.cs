using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System;
using Newtonsoft.Json;

public class WikipediaAPI : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMPro.TextMeshProUGUI infoText;
    [SerializeField] private Image image;
    [SerializeField] private Image image2;
    [SerializeField] private Image image3;
    [SerializeField] private Image imageArtist;
    private WikiArtArtist artist;
    private string search;

    public void Search()
    {
        artist = new WikiArtArtist();
        search = inputField.text;
        //FindNearest();
        StartCoroutine(GetArtistInfo("https://api.wikimedia.org/core/v1/wikipedia/en/search/page?q=" + search +"&limit=1"));

    }

    IEnumerator GetArtistInfo(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    artist.key = JsonUtility.FromJson<Page>(webRequest.downloadHandler.text).pages[0].key;
                    StartCoroutine(GetArtistSummary("https://en.wikipedia.org/api/rest_v1/page/summary/" + artist.key));
                    string key = artist.key.Replace("_", "-").ToLower();
                    StartCoroutine(GetArtistImage("http://www.wikiart.org/en/" + key +"?json=2"));
                    StartCoroutine(GetRequest("https://www.wikiart.org/en/App/Painting/PaintingsByArtist?artistUrl=" + key + "&json=2"));
                    break;
            }
        }
    }

    IEnumerator GetArtistSummary(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + (webRequest.downloadHandler.text));
                    var artistIn = JsonUtility.FromJson<WikiArtArtist>(webRequest.downloadHandler.text);
                    infoText.text = artistIn.extract;
                    artist.extract = artistIn.extract;
                    break;
            }
        }
    }

    IEnumerator GetArtistImage(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    artist = JsonUtility.FromJson<WikiArtArtist>(webRequest.downloadHandler.text);
                    GetImage(artist.image, imageArtist);
                    break;
            }
        }
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    var data = JsonUtility.FromJson<RootObject>("{\"paintings\":" + webRequest.downloadHandler.text+ "}");
    
                    GetImage(data.paintings[0].image, image);
                    GetImage(data.paintings[1].image, image2);
                    GetImage(data.paintings[2].image, image3);
                    break;
            }
        }
    }

    public void GetImage(string url, Image image)
    {
        StartCoroutine(setImage(url, image)); //balanced parens CAS
    }

    IEnumerator setImage(string url, Image image) {
        WWW www = new WWW(url);
        yield return www;

        image.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
    }

    /*
    private void FindNearest()
    {
        StartCoroutine(GetAllArtists("http://www.wikiart.org/en/App/Artist/AlphabetJson?v=new&inPublicDomain={true/false}"));
    }*/ 

    /*
    IEnumerator GetAllArtists(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    search=FindArtist(JsonUtility.FromJson<Artists>("{\"artists\":" + webRequest.downloadHandler.text + "}").artists, search);
                    Debug.Log(search);
                    StartCoroutine(GetArtistInfo("https://api.wikimedia.org/core/v1/wikipedia/en/search/page?q=" + search +"&limit=1"));
                    break;
            }
        }
    }
    */

    /*
    private string FindArtist(WikiArtArtist[] artists, string search)
    {
        int min = Int32.MaxValue;
        string minArtist = "";
        foreach (WikiArtArtist artist in artists)
        {
            int distance = LevenshteinDistance(artist.artistName, search);
            if (distance < min)
            {
                min = distance;
                minArtist = artist.artistName;
            }
        }
        Debug.Log(minArtist);
        return minArtist;
    }

    private int LevenshteinDistance(string s, string t)
    {
        int n = s.Length;
        int m = t.Length;
        int[,] d = new int[n + 1, m + 1];

        if (n == 0)
        {
            return m;
        }

        if (m == 0)
        {
            return n;
        }

        // Step 2
        for (int i = 0; i <= n; d[i, 0] = i++)
        {
        }

        for (int j = 0; j <= m; d[0, j] = j++)
        {
        }
        for (int i = 1; i <= n; i++)
        {
            //Step 4
            for (int j = 1; j <= m; j++)
            {
            // Step 5
            int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

            // Step 6
            d[i, j] = Math.Min(
                Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                d[i - 1, j - 1] + cost);
            }
        }
        return d[n, m];
    }
    */
}