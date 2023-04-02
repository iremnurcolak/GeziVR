using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System;
using Newtonsoft.Json;
using System.Threading;
public class WikipediaAPI : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Image imageArtist;
    [SerializeField] private PlayerScriptable playerScriptable;
    [SerializeField] private TextMeshProUGUI artTitle;
    [SerializeField] private TextMeshProUGUI artYear;
    [SerializeField] private TextMeshProUGUI artSize;
    public static bool isExitedPlane2 = false;
    public static bool isStatusChanged = false;

    private WikiArtArtist artist;
    private string search;

    private WikiArtPainting [] allPaintings; 
    private WikiArtPainting [] window = new WikiArtPainting[8];
    private int index = 0;

    private bool isArtistImageSet = false;
    private bool isArtistInfoSet = false;
    private bool isAllArtistsSet = false;
    
    private WikiArtArtist[] allArtists;
    private List<WikiArtArtist> recommendedArtists = new List<WikiArtArtist>();

    private void Start()
    {
        GameObject.Find("Player").GetComponent<PlayerMovement2>().enabled = false;
        GameObject.Find("PlayerCam").GetComponent<PlayerCamera2>().enabled = false;
        //bu cursor şeyleri vr'da deneme yaparken olmamali
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true; 
        GameObject.Find("CanvasLoading").transform.GetChild(0).gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.GetChild(0).gameObject.SetActive(false);
        GameObject.Find("CanvasDescription").transform.GetChild(0).gameObject.SetActive(false);
        StartCoroutine(GetAllArtists("http://www.wikiart.org/en/App/Artist/AlphabetJson?v=new&inPublicDomain={true/false}"));
        
    } 
    private void Update()
    {
        if(isStatusChanged)
        {
            CheckPlanes();
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                var hitPoint = hit.point;
                hitPoint.y = 0;
                var playerPosition = transform.position;
                playerPosition.y = 0;
                var distance = Vector3.Distance(hitPoint, playerPosition);
                if(hit.transform.tag == "ArtInfo")
                {
                    GameObject parent  = hit.transform.parent.gameObject;
                    int index = parent.name.Substring(parent.name.Length - 1)[0] - '1';
                    artTitle.text = window[index].title;
                    artYear.text = window[index].yearAsString;
                    artSize.text = window[index].width + "x" + window[index].height;    
                    GameObject.Find("CanvasDescription").transform.GetChild(0).gameObject.SetActive(true);
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true; 
                }
            }
        }

        if(isArtistImageSet && isArtistInfoSet)
        {
            GameObject.Find("CanvasLoading").transform.GetChild(0).gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.GetChild(0).gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
            isArtistImageSet = false;
            isArtistInfoSet = false;
        }

    }
    public void Search()
    {
        GameObject.Find("Canvas").transform.GetChild(0).gameObject.SetActive(false);
        GameObject.Find("CanvasLoading").transform.GetChild(0).gameObject.SetActive(true);
        artist = new WikiArtArtist();
        search = inputField.text;
        foreach (WikiArtArtist artist1 in allArtists)
        {
            if(artist1.url == search)
            {
                artist = artist1;
                if(artist1.wikipediaUrl != "")
                {
                    StartCoroutine(GetArtistSummary("https://en.wikipedia.org/api/rest_v1/page/summary/" + artist1.wikipediaUrl.Substring(artist1.wikipediaUrl.LastIndexOf('/') + 1)));
                }
                else
                {
                    infoText.text = "No Wikipedia page found";
                }
            
                GetImage(artist1.image, imageArtist);
                StartCoroutine(PutVisitedMuseum("https://gezivr.onrender.com/addVisitedMuseum/" + playerScriptable.token + "/" + artist1.contentId));
                break;
            }
        }
    }
    public void DisableCanvas()
    {
        GameObject.Find("Canvas").transform.GetChild(0).gameObject.SetActive(false);
        //bu cursor şeyleri vr'da deneme yaparken olmamali
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 
        GameObject.Find("Player").GetComponent<PlayerMovement2>().enabled = true;
        GameObject.Find("PlayerCam").GetComponent<PlayerCamera2>().enabled = true;
    }
    public void CloseDescription()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 
        GameObject.Find("CanvasDescription").transform.GetChild(0).gameObject.SetActive(false);
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

    IEnumerator GetArtistSummary(string uri)
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
                    var artistIn = JsonUtility.FromJson<WikiArtArtist>(webRequest.downloadHandler.text);
                    infoText.text = artistIn.extract;
                    artist.extract = artistIn.extract;
                    isArtistInfoSet = true;
                    StartCoroutine(GetPaintings("https://www.wikiart.org/en/App/Painting/PaintingsByArtist?artistUrl=" + artist.url + "&json=2"));
                    break;
            }
        }
    }  

    IEnumerator GetPaintings(string uri)
    {
     
        GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);

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
                    allPaintings = JsonUtility.FromJson<RootObject>("{\"paintings\":" + webRequest.downloadHandler.text+ "}").paintings;

                    for (int i = 0; i < 8; i++)
                    {
                        try
                        {
                            allPaintings[i].imageBytes = new System.Net.WebClient().DownloadData(allPaintings[i].image);
                        }
                        catch (Exception e)
                        {
                            continue;
                        }
                    }
                    
                    for (int i = 0; i < 8; i++)
                    {
                        if(i >= allPaintings.Length)
                        {
                            break;
                        }

                        window[i] = allPaintings[i];
                        
                        Texture2D tex = new Texture2D(2, 2);
                        tex.LoadImage(allPaintings[i].imageBytes);
                        GameObject go = GameObject.Find("Frame" + (i + 1));
                        float imageRatioX = float.Parse(allPaintings[i].width)/( float.Parse(allPaintings[i].width) + float.Parse(allPaintings[i].height));
                        float imageRatioY = float.Parse(allPaintings[i].height) / (float.Parse(allPaintings[i].width) + float.Parse(allPaintings[i].height));
                        if(imageRatioY > 0.4)
                        {
                            imageRatioY = 0.4f;
                            imageRatioX = imageRatioX*0.4f/imageRatioY;
                        }
                        go.transform.GetChild(3).transform.localScale = new Vector3( imageRatioX, go.transform.GetChild(3).transform.localScale.y, imageRatioY);
                        go.transform.GetChild(3).GetComponent<Renderer>().material.mainTexture = tex;
                    }
                    index = 8;
                    GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
                    GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);  
                    break;
            }
        }
    }

    private void CheckPlanes()
    {
        isStatusChanged = false;
        if(isExitedPlane2)
        {
            isExitedPlane2 = false;
            SetNewWindow();
        }
    }

    private void SetNewWindow()
    {
        for (int i = index; i < index+8; i++)
        {
            if(i >= allPaintings.Length)
            {
                break;
            }
       
            try
            {
                allPaintings[i].imageBytes = new System.Net.WebClient().DownloadData(allPaintings[i].image);
            }
            catch (Exception e)
            {
                continue;
            }
            window[i%8] = allPaintings[i];
            
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(allPaintings[i].imageBytes);
            GameObject go = GameObject.Find("Frame" + (i%8 + 1));
            float imageRatioX = float.Parse(allPaintings[i].width)/( float.Parse(allPaintings[i].width) + float.Parse(allPaintings[i].height));
            float imageRatioY = float.Parse(allPaintings[i].height) / (float.Parse(allPaintings[i].width) + float.Parse(allPaintings[i].height));
            if(imageRatioY > 0.4)
            {
                imageRatioY = 0.4f;
                imageRatioX = imageRatioX*0.4f/imageRatioY;
            }
            go.transform.GetChild(3).transform.localScale = new Vector3( imageRatioX, go.transform.GetChild(3).transform.localScale.y, imageRatioY);
            go.transform.GetChild(3).GetComponent<Renderer>().material.mainTexture = tex;
        }
        index += 8;
    }

    IEnumerator setImage(string url, Image image) {
        WWW www = new WWW(url);
        yield return www;

        image.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
        isArtistImageSet = true;
    }
    private void GetImage(string url, Image image)
    {
        StartCoroutine(setImage(url, image));
    }
    IEnumerator GetAllArtists(string uri)
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
                    allArtists = JsonUtility.FromJson<Artists>("{\"artists\":" + webRequest.downloadHandler.text + "}").artists;
                    GameObject.Find("CanvasLoading").transform.GetChild(0).gameObject.SetActive(false);
                    GameObject.Find("Canvas").transform.GetChild(0).gameObject.SetActive(true);
                    GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
                    GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
                    break;
            }
        }
    }
}