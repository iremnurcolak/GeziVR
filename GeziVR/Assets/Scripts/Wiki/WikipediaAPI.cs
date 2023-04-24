using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class WikipediaAPI : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI artistNameText;
    [SerializeField] private Image imageArtist;
    [SerializeField] private PlayerScriptable playerScriptable;
    [SerializeField] private TextMeshProUGUI artTitle;
    [SerializeField] private TextMeshProUGUI artYear;
    [SerializeField] private TextMeshProUGUI artSize;

    [SerializeField] private GameObject scrollViewRecommendation;
    [SerializeField] private GameObject buttonRecommend;
    [SerializeField] private GameObject panelRecommendation;

    [SerializeField] private GameObject scrollViewSearch;
    [SerializeField] private GameObject buttonTemplate;
    [SerializeField] private GameObject searchPlane;
    [SerializeField] private GameObject textRecommending;

    
    public static bool isStatusChanged = false;

    private WikiArtArtist artist;
    private string search;

    private WikiArtPainting [] allPaintings; 
    private WikiArtPainting [] window = new WikiArtPainting[8];

    private bool areAllPaintingsShown = false;
    private int index = 0;

    private bool isArtistImageSet = false;
    private bool isArtistInfoSet = false;
    private float timeEnter = 0f;
    
    private bool isCanvasActive = false;
    private WikiArtArtist[] allArtists;
    private List<WikiArtArtist> recommendedArtists = new List<WikiArtArtist>();

    private void Start()
    {
        GameObject.Find("Player").GetComponent<PlayerMovement2>().enabled = false;
        GameObject.Find("PlayerCam").GetComponent<PlayerCamera2>().enabled = false;
        //bu cursor şeyleri vr'da deneme yaparken olmamali
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true; 
         isCanvasActive = true;
        GameObject.Find("CanvasLoading").transform.GetChild(0).gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.GetChild(0).gameObject.SetActive(false);
        GameObject.Find("CanvasDescription").transform.GetChild(0).gameObject.SetActive(false);
        StartCoroutine(GetAllArtists("http://www.wikiart.org/en/App/Artist/AlphabetJson?v=new&inPublicDomain={true/false}"));
        searchPlane.transform.GetChild(1).gameObject.SetActive(false);
    } 
    
    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && isCanvasActive == false)
            {
                var hitPoint = hit.point;
                hitPoint.y = 0;
                var playerPosition = GameObject.Find("Player").gameObject.GetComponent<Rigidbody>().position;
                playerPosition.y = 0;
                
                var distance = Vector3.Distance(hitPoint, playerPosition);
                if(distance < 20)
                {
                    Debug.Log("Distance: " + distance);
                    if(hit.transform.tag == "ArtInfo")
                    {
                        Debug.Log("ArtInfo");
                        GameObject parent  = hit.transform.parent.gameObject;
                        int index = parent.name.Substring(parent.name.Length - 1)[0] - '1';
                        if(window[index] != null)
                        {
                            artTitle.text = window[index].title;
                            artYear.text = window[index].yearAsString;
                            artSize.text = window[index].width + "x" + window[index].height;    
                            GameObject.Find("CanvasDescription").transform.GetChild(0).gameObject.SetActive(true);
                            Cursor.lockState = CursorLockMode.None;
                            Cursor.visible = true; 
                        }
                        
                    }
                    else if(hit.transform.tag == "ExitDoor")
                    {
                        SceneManager.LoadScene("MainArea");
                    }
                }
            }
        }

        if(isArtistImageSet && isArtistInfoSet)
        {
            
            GameObject.Find("CanvasLoading").transform.GetChild(0).gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.GetChild(0).gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(3).gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(4).gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(10).gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
            StartCoroutine(GetPaintings("https://www.wikiart.org/en/App/Painting/PaintingsByArtist?artistUrl=" + artist.url + "&json=2"));
            isArtistImageSet = false;
            isArtistInfoSet = false;
        }
        

    }

    public void SearchByUrl(string search)
    {
        GameObject.Find("Canvas").transform.GetChild(0).gameObject.SetActive(false);
        GameObject.Find("CanvasLoading").transform.GetChild(0).gameObject.SetActive(true);
        artist = new WikiArtArtist();
        foreach (WikiArtArtist artist1 in allArtists)
        {
            if(artist1.url == search)
            {
                artist = artist1;
                artistNameText.text = artist1.artistName;
                
                if(artist1.wikipediaUrl != ""  && artist1.wikipediaUrl != null)
                {
                    StartCoroutine(GetArtistSummary("https://en.wikipedia.org/api/rest_v1/page/summary/" + artist1.wikipediaUrl.Substring(artist1.wikipediaUrl.LastIndexOf('/') + 1)));
                }
                else
                {
                    isArtistInfoSet = true;
                    infoText.text = "No Wikipedia page found";
                }
                GetImage(artist1.image, imageArtist);
                break;
            }
        }
    }

    public void DisableCanvas()
    {
        inputField.text = "";
        infoText.text = "";
        artistNameText.text = "";
        imageArtist.sprite = null;
        if(textRecommending.activeSelf)
        {
            textRecommending.gameObject.GetComponent<TMP_Text>().text = "Recommending museums for you...";
           textRecommending.gameObject.SetActive(false);
        }
        
        GameObject.Find("Canvas").transform.GetChild(0).gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.GetChild(1).gameObject.SetActive(true);
        GameObject.Find("Player").GetComponent<PlayerMovement2>().enabled = true;
        GameObject.Find("PlayerCam").GetComponent<PlayerCamera2>().enabled = true;
        timeEnter = Time.time;
        isCanvasActive = false;
    }

    public void ButtonExit()
    {
        isCanvasActive = true;
        float duration = Time.time - timeEnter;
        GameObject.Find("Player").GetComponent<PlayerMovement2>().enabled = false;
        GameObject.Find("PlayerCam").GetComponent<PlayerCamera2>().enabled = false;
        GameObject.Find("Canvas").transform.GetChild(1).gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.GetChild(0).gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(3).gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(10).gameObject.SetActive(false);
        //GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(4).gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(5).gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Button>().interactable = true;
        GameObject.Find("Canvas").transform.GetChild(1).transform.GetChild(1).gameObject.GetComponent<Button>().interactable = true;
        StartCoroutine(DeleteFromSuggestions("https://gezivr.onrender.com/deleteFromSuggestedMuseums/" + playerScriptable.token + "/" + artist.contentId));
        StartCoroutine(PutVisitedMuseum("https://gezivr.onrender.com/addVisitedMuseum/" + playerScriptable.token + "/" + artist.contentId + "/" + duration.ToString().Replace(',', '.')));
        index = 0;
    }

    public void CloseDescription()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false; 
        GameObject.Find("CanvasDescription").transform.GetChild(0).gameObject.SetActive(false);
        isCanvasActive = false;
    }

    public void GetRecommendedMuseums()
    {
        StartCoroutine(GetRecommendedMuseums("https://gezivr.onrender.com/getRecommendedMuseums/" + playerScriptable.token));
        

    }

    IEnumerator DeleteFromSuggestions(string uri)
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

    IEnumerator GetRecommendedMuseums(string uri)
    {
        recommendedArtists = new List<WikiArtArtist>();
        buttonRecommend.SetActive(false);
        //GameObject.Find("textRecommending").gameObject.SetActive(true);
                    
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
                    Debug.Log("No recommended museums found");
                    textRecommending.gameObject.GetComponent<TMP_Text>().text = "No recommended museums found";
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
                    //scrollViewRecommendation.gameObject.SetActive(true);
                    GameObject.Find("textRecommending").gameObject.SetActive(false);
                    panelRecommendation.gameObject.SetActive(true);
                    //StartCoroutine(GetRecommendedMuseums("https://gezivr.onrender.com/getRecommendedMuseums/" + playerScriptable.token));
                    int children = scrollViewRecommendation.transform.GetChild(0).transform.GetChild(0).transform.childCount;
                    GameObject content = scrollViewRecommendation.transform.GetChild(0).transform.GetChild(0).gameObject;

                    int i;
                    for (i = 0; i < recommendedArtists.Count; i++)
                    {
                        GameObject child = content.transform.GetChild(i).gameObject;
                        
                        child.transform.GetChild(0).GetComponent<TMP_Text>().text = recommendedArtists[i].artistName;
                    }
                    if (i < children)
                    {
                        for (int j = i ; j < children; j++)
                        {
                            content.transform.GetChild(j).gameObject.SetActive(false);
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
                    break;
            }
        }
    }  

    IEnumerator GetPaintings(string uri)
    {
        GameObject.Find("Canvas").transform.GetChild(1).transform.GetChild(2).gameObject.GetComponent<Button>().interactable = false;
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
                            GameObject go = GameObject.Find("Frame" + (i%8 + 1));
                            areAllPaintingsShown = true;
                            go.transform.GetChild(3).GetComponent<Renderer>().material.mainTexture = null;
                            window[i] = null;
                        }

                        else{
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
                    }
                    index = 8;
                    GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
                    GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);  
                    GameObject.Find("Player").gameObject.GetComponent<Rigidbody>().position = new Vector3(138.02f, -25.79f, -159.6f);
                    break;
            }
        }
    }

    public void SetNewWindow()
    {
        GameObject.Find("Canvas").transform.GetChild(1).transform.GetChild(2).gameObject.GetComponent<Button>().interactable = true;
        for (int i = index; i < index+8; i++)
        {
            if(areAllPaintingsShown)
            {
                GameObject.Find("Canvas").transform.GetChild(1).transform.GetChild(1).gameObject.GetComponent<Button>().interactable = false;
            }
            if(i >= allPaintings.Length)
            {
                GameObject go = GameObject.Find("Frame" + (i%8 + 1));
                areAllPaintingsShown = true;
                go.transform.GetChild(3).GetComponent<Renderer>().material.mainTexture = null;
                window[i%8] = null;
            }
            else
            {
                try
                {
                    allPaintings[i].imageBytes = new System.Net.WebClient().DownloadData(allPaintings[i].image);
                }
                catch (Exception e)
                {
                    window[i%8] = null;
                    GameObject.Find("Frame" + (i%8 + 1)).transform.GetChild(3).GetComponent<Renderer>().material.mainTexture = null;
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
        }
        index += 8;
    }

    public void SetPrevWindow()
    {
        areAllPaintingsShown = false;
        GameObject.Find("Canvas").transform.GetChild(1).transform.GetChild(1).gameObject.GetComponent<Button>().interactable = true;
        if(index == 16)
        {
            GameObject.Find("Canvas").transform.GetChild(1).transform.GetChild(2).gameObject.GetComponent<Button>().interactable = false;
        }
        for (int i = index-16; i < index-8; i++)
        {
            try
            {
                allPaintings[i].imageBytes = new System.Net.WebClient().DownloadData(allPaintings[i].image);
            }
            catch (Exception e)
            {
                window[i%8] = null;
           
                GameObject.Find("Frame" + (i%8 + 1)).transform.GetChild(3).GetComponent<Renderer>().material.mainTexture = null;
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
        index -= 8;
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
                    GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(3).gameObject.SetActive(false);
                    GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(10).gameObject.SetActive(false);
                    GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(4).gameObject.SetActive(false);
                    GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(5).gameObject.SetActive(true);
                    break;
            }
        }
    }

    public void GoBackMenu()
    {
        SceneManager.LoadScene("MenuScreen");
    }

    public void LoadRecommendedMuseum()
    {
        int index = int.Parse(EventSystem.current.currentSelectedGameObject.name);
        panelRecommendation.SetActive(false);
        
        SearchByUrl(recommendedArtists[index].url);
    }

    public void OnSearchValueChanged()
    {
        if(inputField.text != ""){

            searchPlane.transform.GetChild(1).gameObject.SetActive(true);
            if(scrollViewSearch.transform.childCount > 0)
            {
                for (int j = 0; j < scrollViewSearch.transform.childCount; j++)
                {
                    Destroy(scrollViewSearch.transform.GetChild(j).gameObject);
                }
            }

            for (int i = 0; i < allArtists.Length; i++)
        {
            if(allArtists[i].artistName.ToLower().StartsWith(inputField.text))
            {
                GameObject btn = (GameObject)Instantiate(buttonTemplate);
                btn.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = allArtists[i].artistName;
                btn.gameObject.GetComponent<Button>().onClick.AddListener(() => { Search2(btn.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text); });
                btn.transform.SetParent(scrollViewSearch.transform);
            }
        }
        }
        else
        {
            searchPlane.transform.GetChild(1).gameObject.SetActive(false);
            if(scrollViewSearch.transform.childCount > 0)
            {
                for (int j = 0; j < scrollViewSearch.transform.childCount; j++)
                {
                    Destroy(scrollViewSearch.transform.GetChild(j).gameObject);
                }
            }
        }
    }

    private void Search2(string artistName)
    {
        searchPlane.transform.GetChild(1).gameObject.SetActive(false);
        
        GameObject.Find("Canvas").transform.GetChild(0).gameObject.SetActive(false);
        GameObject.Find("CanvasLoading").transform.GetChild(0).gameObject.SetActive(true);
        artist = new WikiArtArtist();
        foreach (WikiArtArtist artist1 in allArtists)
        {
            if(artist1.artistName == artistName)
            {
                artist = artist1;
                artistNameText.text = artist1.artistName;
                if(artist1.wikipediaUrl != ""  && artist1.wikipediaUrl != null)
                {
                    StartCoroutine(GetArtistSummary("https://en.wikipedia.org/api/rest_v1/page/summary/" + artist1.wikipediaUrl.Substring(artist1.wikipediaUrl.LastIndexOf('/') + 1)));
                }
                else
                {
                    isArtistInfoSet = true;
                    infoText.text = "No Wikipedia page found";
                }
                GetImage(artist1.image, imageArtist);
                break;
            }
        }
    }

    public void BackToArea()
    {
        SceneManager.LoadScene("MainArea");
    }
}