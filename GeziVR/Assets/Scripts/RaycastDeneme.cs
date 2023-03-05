using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RaycastDeneme : MonoBehaviour
{
    public Camera cam;
    public PlayerScriptable playerScriptable;
    private bool isPanelOpen = false;
    public GameObject panel;
    public GameObject popup;
    private DataSnapshot snapshot;
    private string id;
    private int piecePricePurchase;

    public TMPro.TextMeshProUGUI pieceName;
    public TMPro.TextMeshProUGUI pieceDescription;
    public TMPro.TextMeshProUGUI piecePrice;
    public TMPro.TextMeshProUGUI pieceOwner;
    public TMPro.TextMeshProUGUI message;

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
                Debug.Log(id);
                
                if(distance <= 40)
                {
                    Debug.Log(distance);
                    FirebaseDatabase.DefaultInstance.RootReference.Child("pieces").Child(id).GetValueAsync().ContinueWithOnMainThread(t => {
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
            }
        }
    }

    void OpenPanel()    
    {
        Debug.Log("Panel is opening");
        isPanelOpen = true;
        panel.GetComponent<Canvas>().enabled = true; 
        Cursor.lockState = CursorLockMode.None;
        
        //bu cursor şeyleri vr'da deneme yaparken olmamali
        Cursor.visible = true; 

        Debug.Log("Panel is opened");    
        FirebaseDatabase.DefaultInstance.RootReference.Child("pieces").Child(id).GetValueAsync().ContinueWithOnMainThread(t => {
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

    public void ClosePanel()
    {
        //panel = cam.transform.GetChild(0).gameObject; 
        isPanelOpen = false;
        panel.GetComponent<Canvas>().enabled = false;
        Debug.Log("Panel is closed");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 
        pieceName.text = "";
        pieceDescription.text = "";
        piecePrice.text = "";
        pieceOwner.text = "";
    }

    public void BuyPiece()
    {
        FirebaseDatabase.DefaultInstance.RootReference.Child("pieces").Child(id).GetValueAsync().ContinueWithOnMainThread(t => {

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
                        bool canBuy = CheckBalance(Convert.ToInt32(snapshot.Child("price").Value.ToString()));
                        popup.GetComponent<Canvas>().enabled = true;
                        panel.GetComponent<CanvasGroup>().interactable = false;
                        if(canBuy)
                        {
                            piecePricePurchase = Convert.ToInt32(snapshot.Child("price").Value.ToString());
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

    bool CheckBalance(int price)
    {
        Debug.Log("Checking balance...");
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
        FirebaseDatabase.DefaultInstance.RootReference.Child("pieces").Child(id).Child("owner").SetValueAsync(playerScriptable.token); 
        string json = "{\"piecePricePurchase\":" + piecePricePurchase + "}";
        FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(playerScriptable.token).Child("gallery").Child(id).SetRawJsonValueAsync(json);
        FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(playerScriptable.token).Child("balance").SetValueAsync(playerScriptable.balance - int.Parse(piecePrice.text));
        pieceOwner.text = playerScriptable.name;
        playerScriptable.balance -= int.Parse(piecePrice.text);
        PlayerPrefs.SetInt("balance", playerScriptable.balance);
        panel.transform.GetChild(0).transform.GetChild(5).GetComponent<Button>().interactable = false;
        ClosePopup();
    }

    public void ClosePopup()
    {
        popup.GetComponent<Canvas>().enabled = false;
        panel.GetComponent<CanvasGroup>().interactable = true;
    }   

}
