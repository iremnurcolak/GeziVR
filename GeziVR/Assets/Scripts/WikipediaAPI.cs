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
    [SerializeField] private Image imageArtist;
    [SerializeField] private PlayerScriptable playerScriptable;
    
    private WikiArtArtist artist;
    private string search;

    private WikiArtArtist[] allArtists;
    private List<WikiArtArtist> recommendedArtists = new List<WikiArtArtist>();

    private void Start()
    {
        //bu cursor şeyleri vr'da deneme yaparken olmamali
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true; 
        StartCoroutine(GetAllArtists("http://www.wikiart.org/en/App/Artist/AlphabetJson?v=new&inPublicDomain={true/false}"));
    }

    public void GetImage(string url, Image image)
    {
        StartCoroutine(setImage(url, image));
    }

    public void DisableCanvas()
    {
        GameObject.Find("Canvas").SetActive(false);
        //bu cursor şeyleri vr'da deneme yaparken olmamali
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 
    }

    public void Search()
    {
        artist = new WikiArtArtist();
        search = inputField.text;
        foreach (WikiArtArtist artist1 in allArtists)
        {
            if(artist1.url == search)
            {
                Debug.Log(artist1.wikipediaUrl);
                if(artist1.wikipediaUrl != "")
                {
                    StartCoroutine(GetArtistSummary("https://en.wikipedia.org/api/rest_v1/page/summary/" + artist1.wikipediaUrl.Substring(artist1.wikipediaUrl.LastIndexOf('/') + 1)));
                }
                else
                {
                    infoText.text = "No Wikipedia page found";
                }
                StartCoroutine(PutVisitedMuseum("https://gezivr.onrender.com/addVisitedMuseum/" + playerScriptable.token + "/" + artist1.contentId));
                Debug.Log(artist1.url);
                StartCoroutine(GetArtistImage("http://www.wikiart.org/en/" + artist1.url + "?json=2"));
                StartCoroutine(GetRequest("https://www.wikiart.org/en/App/Painting/PaintingsByArtist?artistUrl=" + artist1.url + "&json=2"));
                break;
            }
        }
    }

    public void GetRecommendedMuseums()
    {
        StartCoroutine(GetRecommendedMuseums("https://gezivr.onrender.com/getRecommendedMuseums/" + playerScriptable.token));
    }

    IEnumerator GetRecommendedMuseums(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
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
                    Debug.Log("Success");
                    var data = webRequest.downloadHandler.text;
                    data = data.Substring(1, data.LastIndexOf(']') - 1);
                    string[] museums = data.Split(',');
                    foreach (string museum in museums)
                    {
                        foreach (WikiArtArtist artist1 in allArtists)
                        {
                            if (artist1.contentId == museum)
                            {
                                recommendedArtists.Add(artist1);
                                break;
                            }
                        }
                    }
                    break;
            }
        }
    }

    IEnumerator PutVisitedMuseum(string uri)
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
                    Debug.Log("Success");
                    break;
            }
        }
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

                    for (int i = 0; i < 8; i++)
                    {
                        byte[] imageBytes;
                        try
                        {
                            imageBytes = new System.Net.WebClient().DownloadData(data.paintings[i].image);
                        }
                        catch (Exception e)
                        {
                            continue;
                        }
                        
                        Texture2D tex = new Texture2D(2, 2);
                        tex.LoadImage(imageBytes);
                        GameObject go = GameObject.Find("Frame" + (i + 1));
                        go.transform.GetChild(3).GetComponent<Renderer>().material.mainTexture = tex;
                    }
                    break;
            }
        }
    }

    IEnumerator setImage(string url, Image image) {
        WWW www = new WWW(url);
        yield return www;

        image.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
    }

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
                    allArtists = JsonUtility.FromJson<Artists>("{\"artists\":" + webRequest.downloadHandler.text + "}").artists;
                    break;
            }
        }
    }

}