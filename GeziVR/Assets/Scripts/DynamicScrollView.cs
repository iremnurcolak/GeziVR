using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Extensions;
using Firebase.Database;

public class DynamicScrollView : MonoBehaviour
{
    public GameObject scrollBar;
    float scrollPos = 0f;
    float[] pos;

    [SerializeField] private Transform scrollViewContent;
    [SerializeField] private GameObject prefabPieceName;

    private string piece;
    public PlayerScriptable playerScriptable;

    private List<string> pieceList = new List<string>();

    private GameObject canvas;
    private GameObject canvas2;
    private GameObject piece3d;
    private string prefabName;
    private int position;

    void Start()
    {
        canvas = GameObject.Find("Canvas");
        canvas2 = GameObject.Find("Canvas2");
        canvas2.transform.GetChild(0).gameObject.SetActive(false);

        FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(playerScriptable.token).Child("gallery").GetValueAsync().ContinueWithOnMainThread(t => {
            if (t.IsFaulted)
            {
                Debug.Log("Error");
            }
            else if (t.IsCompleted)
            {
                DataSnapshot snapshot = t.Result;
                Debug.Log(snapshot.GetRawJsonValue());
                foreach (var child in snapshot.Children)
                {
                    piece = child.Key;
                    pieceList.Add(piece);
                    GameObject pieceName = Instantiate(prefabPieceName, scrollViewContent);
                    pieceName.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = piece;
                    pieceName.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = child.Child("piecePricePurchase").Value.ToString();
                }
            }
        });
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
           canvas.transform.GetChild(0).transform.GetChild(0).GetComponent<ScrollRect>().enabled = true;
           canvas.transform.GetChild(0).transform.GetChild(1).GetComponent<Button>().interactable = true;
           canvas2.transform.GetChild(0).gameObject.SetActive(false);
           Destroy(piece3d);
           
        }
        if(canvas2.transform.GetChild(0).gameObject.activeSelf == false)
        {
            
            pos = new float[pieceList.Count];
            float distance = 1f / (pos.Length - 1f);
            for (int i = 0; i < pos.Length; i++)
            {
                pos[i] = distance * i;
            }
            if (Input.GetMouseButton(0))
            {
                scrollPos = scrollBar.GetComponent<Scrollbar>().value;
                
            }
            else
            {
                for (int i = 0; i < pos.Length; i++)
                {
                    if (scrollPos < pos[i] + (distance / 2) && scrollPos > pos[i] - (distance / 2))
                    {
                        scrollBar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollBar.GetComponent<Scrollbar>().value, pos[i], 0.1f);
                    }
                }
            }

            for (int i = 0; i < pos.Length; i++)
            {
                if (scrollPos < pos[i] + (distance / 2) && scrollPos > pos[i] - (distance / 2))
                {
                    transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(1f, 1f), 0.1f);
                    position = i;
                    for (int j = 0; j < pos.Length; j++)
                    {
                        if (j != i)
                        {
                            transform.GetChild(j).localScale = Vector2.Lerp(transform.GetChild(j).localScale, new Vector2(0.8f, 0.8f), 0.1f);
                        }
                    }
                }
            }
        }
    }

    public void OpenPanel()
    {
        canvas.transform.GetChild(0).transform.GetChild(0).GetComponent<ScrollRect>().enabled = false;
        canvas.transform.GetChild(0).transform.GetChild(1).GetComponent<Button>().interactable = false;
        canvas2.transform.GetChild(0).gameObject.SetActive(true);
        prefabName = pieceList[position];
        piece3d  = GameObject.Instantiate(Resources.Load(prefabName)) as GameObject;
        piece3d.layer = LayerMask.NameToLayer("UI");
        piece3d.transform.position = new Vector3(-4, -1, 7);
        piece3d.AddComponent<RotationController>();
    }

}
