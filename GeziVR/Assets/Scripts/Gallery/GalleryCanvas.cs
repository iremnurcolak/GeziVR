using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalleryCanvas : MonoBehaviour
{
    private GameObject canvas2;
    private GameObject piece;

    // Start is called before the first frame update
    void Start()
    {
        canvas2 = GameObject.Find("Canvas2");
        canvas2.transform.GetChild(0).gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
           canvas2.transform.GetChild(0).gameObject.SetActive(false);
           Destroy(piece);
        }
        if (Input.GetMouseButtonDown(0))
        {
            OpenPanel();
        }
    }
    public void OpenPanel()
    {
        Debug.Log("OpenPanel");
        canvas2.transform.GetChild(0).gameObject.SetActive(true);
  
        Instantiate(Resources.Load("Horse Skull"), canvas2.transform.GetChild(0).transform.GetChild(0).transform);
        piece = GameObject.Find("Horse Skull(Clone)");
        piece.transform.position = new Vector3(0, 0, 0);
        piece.transform.localScale = new Vector3(1, 1, 1);
        piece.transform.rotation = Quaternion.Euler(0, 0, 0);
        piece.layer = LayerMask.NameToLayer("UI");
        piece.AddComponent<RotationController>();

    }
}
