using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using Firebase.Database;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class RaycastGeziVR : MonoBehaviour
{
    public Camera cam;
    public PlayerScriptable playerScriptable;
    private bool isPanelOpen = false;
    public GameObject panel;
    public GameObject popup;
    private DataSnapshot snapshot;
    private string id;
    private float piecePricePurchase;
    private string tag = "";

    public TMPro.TextMeshProUGUI pieceName;
    public TMPro.TextMeshProUGUI pieceDescription;
    public TMPro.TextMeshProUGUI piecePrice;
    public TMPro.TextMeshProUGUI pieceOwner;
    public TMPro.TextMeshProUGUI message;

    [SerializeField] private GameObject loadingPurchase;

    void Start()
    {
        cam =  GameObject.Find("CameraHolder").transform.GetChild(0).GetComponent<Camera>();
        //panel = cam.transform.GetChild(0).gameObject; 
        panel.GetComponent<Canvas>().enabled = false;
        popup.GetComponent<Canvas>().enabled = false;

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isPanelOpen)
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
                
                id = hit.transform.name;
                GameObject go = GameObject.Find(id) as GameObject;
                tag = go.tag;
                if(distance <= 40)
                {
                    if(tag == "Skeleton")
                    {
                        FirebaseDatabase.DefaultInstance.RootReference.Child("pieces").Child("skeletons").Child(id).GetValueAsync().ContinueWithOnMainThread(t => {
                            if (t.IsFaulted)
                            {
                                Debug.Log("Error");
                            }
                            else if (t.IsCompleted)
                            {
                                snapshot = t.Result;
                                if (snapshot.Exists)
                                {
                                    OpenPanel();
                                    if(snapshot.Child("owner").Value.ToString() != "")
                                    {
                                        
                                        panel.transform.GetChild(0).transform.GetChild(5).GetComponent<Button>().interactable = false;
                                    }
                                    else
                                    {
                                        panel.transform.GetChild(0).transform.GetChild(5).GetComponent<Button>().interactable = true;
                                    }
                                    
                                }
                                else
                                {
                                    Debug.Log("Piece does not exist");
                                }
                            }
                        });
                    }
                    else if(tag == "Dino")
                    {
                        FirebaseDatabase.DefaultInstance.RootReference.Child("pieces").Child("dinosaurs").Child(id).GetValueAsync().ContinueWithOnMainThread(t => {
                            if (t.IsFaulted)
                            {
                                Debug.Log("Error");
                            }
                            else if (t.IsCompleted)
                            {
                                snapshot = t.Result;
                                if (snapshot.Exists)
                                {
                                    OpenPanel();
                                    if(snapshot.Child("owner").Value.ToString() != "")
                                    {
                                        
                                        panel.transform.GetChild(0).transform.GetChild(5).GetComponent<Button>().interactable = false;
                                    }
                                    else
                                    {
                                        panel.transform.GetChild(0).transform.GetChild(5).GetComponent<Button>().interactable = true;
                                    }
                                    
                                }
                                else
                                {
                                    Debug.Log("Piece does not exist");
                                }
                            }
                        }); 
                    }

                    else if(tag == "DynamicMuseum")
                    {
                        SceneManager.LoadScene("Wikipedia");
                    }
                }
            }
        }
    }

    void OpenPanel()    
    {
        isPanelOpen = true;
        panel.GetComponent<Canvas>().enabled = true; 
        Cursor.lockState = CursorLockMode.None;
        
        //bu cursor şeyleri vr'da deneme yaparken olmamali
        Cursor.visible = true; 

        if(tag == "Skeleton")
        {
            FirebaseDatabase.DefaultInstance.RootReference.Child("pieces").Child("skeletons").Child(id).GetValueAsync().ContinueWithOnMainThread(t => {
                if (t.IsFaulted)
                {
                    Debug.Log("Error");
                }
                else if (t.IsCompleted)
                {
                    snapshot = t.Result;
                    if (snapshot.Exists)
                    {
                        pieceName.text = snapshot.Child("name").Value.ToString();
                        pieceDescription.text = snapshot.Child("description").Value.ToString();
                        piecePrice.text = snapshot.Child("price").Value.ToString();

                        FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(snapshot.Child("owner").Value.ToString()).GetValueAsync().ContinueWithOnMainThread(t => {
                            if (t.IsFaulted)
                            {
                                Debug.Log("Error");
                            }
                            else if (t.IsCompleted)
                            {
                                snapshot = t.Result;
                                if (snapshot.Exists)
                                {
                                    pieceOwner.text = snapshot.Child("name").Value.ToString();
                                }
                                else
                                {
                                    Debug.Log("Piece does not exist");
                                }
                            }
                        });

                    }
                    else
                    {
                        Debug.Log("Piece does not exist");
                    }
                }
            });
        }
        else if(tag == "Dino")
        {
            FirebaseDatabase.DefaultInstance.RootReference.Child("pieces").Child("dinosaurs").Child(id).GetValueAsync().ContinueWithOnMainThread(t => {
                if (t.IsFaulted)
                {
                    Debug.Log("Error");
                }
                else if (t.IsCompleted)
                {
                    snapshot = t.Result;
                    if (snapshot.Exists)
                    {
                        pieceName.text = snapshot.Child("name").Value.ToString();
                        pieceDescription.text = snapshot.Child("description").Value.ToString();
                        piecePrice.text = snapshot.Child("price").Value.ToString();

                        FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(snapshot.Child("owner").Value.ToString()).GetValueAsync().ContinueWithOnMainThread(t => {
                            if (t.IsFaulted)
                            {
                                Debug.Log("Error");
                            }
                            else if (t.IsCompleted)
                            {
                                snapshot = t.Result;
                                if (snapshot.Exists)
                                {
                                    pieceOwner.text = snapshot.Child("name").Value.ToString();
                                }
                                else
                                {
                                    Debug.Log("Piece does not exist");
                                }
                            }
                        });

                    }
                    else
                    {
                        Debug.Log("Piece does not exist");
                    }
                }
            });
        }
           
    }

    public void ClosePanel()
    {
        //panel = cam.transform.GetChild(0).gameObject; 
        isPanelOpen = false;
        panel.GetComponent<Canvas>().enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 
        pieceName.text = "";
        pieceDescription.text = "";
        piecePrice.text = "";
        pieceOwner.text = "";
    }

    public void BuyPiece()
    {
        if(tag == "Skeleton")
        {
            FirebaseDatabase.DefaultInstance.RootReference.Child("pieces").Child("skeletons").Child(id).GetValueAsync().ContinueWithOnMainThread(t => {

            if (t.IsFaulted)
            {
                Debug.Log("Error");
            }
            else if (t.IsCompleted)
            {
                snapshot = t.Result;
                if (snapshot.Exists)
                {
                    if(snapshot.Child("owner").Value.ToString() == "")
                    {
                        bool canBuy = CheckBalance(float.Parse(snapshot.Child("price").Value.ToString()));
                        popup.GetComponent<Canvas>().enabled = true;
                        panel.GetComponent<CanvasGroup>().interactable = false;
                        if(canBuy)
                        {
                            piecePricePurchase = float.Parse(snapshot.Child("price").Value.ToString());
                            message.text = "Your balance is " + playerScriptable.balance + ". Do you want to buy this piece for " + snapshot.Child("price").Value.ToString() + "?";
                            popup.transform.GetChild(0).transform.GetChild(0).GetComponent<Button>().gameObject.SetActive(true);
                            popup.transform.GetChild(0).transform.GetChild(1).GetComponent<Button>().gameObject.SetActive(true);
                            popup.transform.GetChild(0).transform.GetChild(3).GetComponent<Button>().gameObject.SetActive(false);
                        }
                        else
                        {
                            message.text = "Your balance is " + playerScriptable.balance + ". You don't have enough money to buy this piece for " + snapshot.Child("price").Value.ToString() + ".";
                            popup.transform.GetChild(0).transform.GetChild(0).GetComponent<Button>().gameObject.SetActive(false);
                            popup.transform.GetChild(0).transform.GetChild(1).GetComponent<Button>().gameObject.SetActive(false);
                            popup.transform.GetChild(0).transform.GetChild(3).GetComponent<Button>().gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        Debug.Log("Piece is owned");
                    }
                }
                else
                {
                    Debug.Log("Piece does not exist");
                }
            }
        });
        }
        else if(tag == "Dino")
        {
            FirebaseDatabase.DefaultInstance.RootReference.Child("pieces").Child("dinosaurs").Child(id).GetValueAsync().ContinueWithOnMainThread(t => {

            if (t.IsFaulted)
            {
                Debug.Log("Error");
            }
            else if (t.IsCompleted)
            {
                snapshot = t.Result;
                if (snapshot.Exists)
                {
                    if(snapshot.Child("owner").Value.ToString() == "")
                    {
                        bool canBuy = CheckBalance(float.Parse(snapshot.Child("price").Value.ToString()));
                        popup.GetComponent<Canvas>().enabled = true;
                        panel.GetComponent<CanvasGroup>().interactable = false;
                        if(canBuy)
                        {
                            piecePricePurchase = float.Parse(snapshot.Child("price").Value.ToString());
                            message.text = "Your balance is " + playerScriptable.balance + ". Do you want to buy this piece for " + snapshot.Child("price").Value.ToString() + "?";
                            popup.transform.GetChild(0).transform.GetChild(0).GetComponent<Button>().gameObject.SetActive(true);
                            popup.transform.GetChild(0).transform.GetChild(1).GetComponent<Button>().gameObject.SetActive(true);
                            popup.transform.GetChild(0).transform.GetChild(3).GetComponent<Button>().gameObject.SetActive(false);
                        }
                        else
                        {
                            message.text = "Your balance is " + playerScriptable.balance + ". You don't have enough money to buy this piece for " + snapshot.Child("price").Value.ToString() + ".";
                            popup.transform.GetChild(0).transform.GetChild(0).GetComponent<Button>().gameObject.SetActive(false);
                            popup.transform.GetChild(0).transform.GetChild(1).GetComponent<Button>().gameObject.SetActive(false);
                            popup.transform.GetChild(0).transform.GetChild(3).GetComponent<Button>().gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        Debug.Log("Piece is owned");
                    }
                }
                else
                {
                    Debug.Log("Piece does not exist");
                }
            }
        });
        }
        
    
    }

    bool CheckBalance(float price)
    {
        Debug.Log("Checking balance");
        Debug.Log(playerScriptable.balance >= price);
        StartCoroutine(GetBalance("https://gezivr-web3.onrender.com/getBalance/" + playerScriptable.accountAddress));
        if(playerScriptable.balance >= price)
        {
            return true;
            
        }
        else
        {
            return false;
        }
    }

    public void ApprovePurchase()
    {
        string p = piecePricePurchase.ToString().Replace(",", ".");
        loadingPurchase.SetActive(true);
        message.text = "";
        popup.transform.GetChild(0).transform.GetChild(0).GetComponent<Button>().gameObject.SetActive(false);
        popup.transform.GetChild(0).transform.GetChild(1).GetComponent<Button>().gameObject.SetActive(false);

    
        StartCoroutine(StartTransaction("https://gezivr-web3.onrender.com/sendTransaction", playerScriptable.token,p));
        
        
    }

    

    IEnumerator StartTransaction(string url, string token, string amount)
    {
        JsonTransaction jsonData = new JsonTransaction(token, amount);
        var jsonDataToSend = JsonConvert.SerializeObject(jsonData);
        var data = System.Text.Encoding.UTF8.GetBytes(jsonDataToSend);
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            byte [] bodyRaw = Encoding.UTF8.GetBytes(jsonDataToSend);
            www.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
            yield return www.SendWebRequest();

            loadingPurchase.SetActive(false);
            Debug.Log(www.downloadHandler.text);
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                StartCoroutine(AutoClosePopup("Failed"));
            }
            else
            {
                Debug.Log("Transaction successful");
                if(tag == "Skeleton")
                {
                    FirebaseDatabase.DefaultInstance.RootReference.Child("pieces").Child("skeletons").Child(id).Child("owner").SetValueAsync(playerScriptable.token);
                }
                else if(tag == "Dino")
                {
                    FirebaseDatabase.DefaultInstance.RootReference.Child("pieces").Child("dinosaurs").Child(id).Child("owner").SetValueAsync(playerScriptable.token);
                }
                string json = "{\"piecePricePurchase\":" + piecePricePurchase.ToString().Replace(",", ".") + "}";
                FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(playerScriptable.token).Child("gallery").Child(id).SetRawJsonValueAsync(json);
                //FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(playerScriptable.token).Child("balance").SetValueAsync(playerScriptable.balance - int.Parse(piecePrice.text));
                pieceOwner.text = playerScriptable.name;
                //playerScriptable.balance -= float.Parse(piecePrice.text);
                //PlayerPrefs.SetFloat("balance", playerScriptable.balance);
                panel.transform.GetChild(0).transform.GetChild(5).GetComponent<Button>().interactable = false;
                StartCoroutine(AutoClosePopup("Success"));
            }
        }
        

        StartCoroutine(GetBalance("https://gezivr-web3.onrender.com/getBalance/" + playerScriptable.accountAddress));

        
    }

    IEnumerator GetBalance(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        playerScriptable.balance = float.Parse(www.text.Replace(".", ","));
        PlayerPrefs.SetFloat("balance", playerScriptable.balance);
    }

    IEnumerator AutoClosePopup(string status)
    {
        message.text = "Purchase " + status ;
        yield return new WaitForSeconds(2);
        popup.GetComponent<Canvas>().enabled = false;
        popup.transform.GetChild(0).transform.GetChild(0).GetComponent<Button>().gameObject.SetActive(true);
        popup.transform.GetChild(0).transform.GetChild(1).GetComponent<Button>().gameObject.SetActive(true);
        panel.GetComponent<CanvasGroup>().interactable = true;
    }   

    public void ClosePopup()
    {

        popup.GetComponent<Canvas>().enabled = false;
        panel.GetComponent<CanvasGroup>().interactable = true;
    }  
}

public class JsonTransaction
{
    public string userId;
    public string amount;

    public JsonTransaction(string userId, string amount)
    {
        this.userId = userId;
        this.amount = amount;
    }
}