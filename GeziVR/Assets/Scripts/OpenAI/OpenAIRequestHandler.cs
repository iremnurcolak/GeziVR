using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Net;

namespace OpenAI_handler
{
    public class OpenAIRequestHandler : MonoBehaviour
    {
        public void generate()
        {
            StartCoroutine(generateImages("https://gezivr.onrender.com/generate_images"));
        }

        IEnumerator generateImages(string uri)
        {
            var webRequest = UnityWebRequest.Get(uri);
            webRequest.certificateHandler = null;

            yield return webRequest.SendWebRequest();
            MyDictionary dict = JsonUtility.FromJson<MyDictionary>(webRequest.downloadHandler.text);
            using (var client = new WebClient())
            {
                
                // kac tane kullanilacaksa eklenip silinebilir, api limiti 5 oldugu icin 5 olarak guncellendi
                client.DownloadFile(dict.img1, Application.dataPath + "/GeneratedImages/img1.png");
                client.DownloadFile(dict.img2, Application.dataPath + "/GeneratedImages/img2.png");
                client.DownloadFile(dict.img3, Application.dataPath + "/GeneratedImages/img3.png");
                client.DownloadFile(dict.img4, Application.dataPath + "/GeneratedImages/img4.png");
                client.DownloadFile(dict.img5, Application.dataPath + "/GeneratedImages/img5.png");
                print("generated");
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
    }
}