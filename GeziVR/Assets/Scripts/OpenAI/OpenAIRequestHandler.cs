using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Net;
using System.Text.Json;

namespace OpenAI_handler
{
    public class OpenAIRequestHandler : MonoBehaviour
    {
        public void generate()
        {
            StartCoroutine(generateImages("http://127.0.0.1:5000/generate_images"));
        }

        IEnumerator generateImages(string uri)
        {
            print("generate");
            var webRequest = UnityWebRequest.Get(uri);
            webRequest.certificateHandler = null;
            
            yield return webRequest.SendWebRequest();
            print(webRequest.downloadHandler.text);
            MyDictionary dict = JsonUtility.FromJson<MyDictionary>(webRequest.downloadHandler.text);
            using (var client = new WebClient())
            {
                for (int i = 1; i <= 1; i++)
                {
                    client.DownloadFile(dict.img1, Application.dataPath+string.Format("/GeneratedImages/img{0}.png", i));
                }
                
            }
        }
    }
    [System.Serializable]
    public class MyDictionary
    {
        public string img1;
        public string img2;
        public string img3;
        public string img4;
        public string img5;
        public string img6;
        public string img7;
        public string img8;

    }


}

