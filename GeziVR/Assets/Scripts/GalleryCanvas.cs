using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalleryCanvas : MonoBehaviour
{
    private GameObject panel;

    // Start is called before the first frame update
    void Start()
    {
        panel = GameObject.Find("Canvas2");
        panel.transform.GetChild(0).gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
           panel.transform.GetChild(0).gameObject.SetActive(false);
        }
        if (Input.GetMouseButtonDown(0))
        {
            OpenPanel();
        }
    }
    public void OpenPanel()
    {
        Debug.Log("OpenPanel");
       panel.transform.GetChild(0).gameObject.SetActive(true);
    }
}
