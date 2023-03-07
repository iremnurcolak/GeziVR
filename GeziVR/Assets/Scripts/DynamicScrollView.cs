using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Extensions;
using Firebase.Database;

public class DynamicScrollView : MonoBehaviour
{
    [SerializeField] private Transform scrollViewContent;
    [SerializeField] private GameObject prefabPieceName;

    private string piece;
    public PlayerScriptable playerScriptable;

    private List<string> pieceList = new List<string>();

    private void Start()
    {
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
                    Debug.Log(piece);
                    pieceList.Add(piece);
                    GameObject pieceName = Instantiate(prefabPieceName, scrollViewContent);
                    if(pieceName.TryGetComponent(out TextMeshProUGUI textMeshProUGUI))
                    {
                        textMeshProUGUI.text = piece;
                        Debug.Log("aa");
                    }
                }
            }
        });
    }
}
