using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using UnityEngine.UI;
using UnityEngine.Networking;

public class JSON_Data : MonoBehaviour
{


    string json;

    public List<Root> _RootData { get; set; }



    public GameObject cell;
    public GameObject content;

    int DataCount;
    Root response;
    // Use this for initialization
    void Awake()
    {
 
        
     //   StartCoroutine(GetRequest("https://picsum.photos/v2/list"));
    }

    public static List<Root> Map(string json)
    {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Root>>(json);
    }
    IEnumerator GetRequest(string uri)
    {
       
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                json = webRequest.downloadHandler.text;
                Debug.Log(pages[page] + ":\nReceived: " + json);
                
              //  _RootData = JsonConvert.DeserializeObject<List<Root>>(json);


            }
        }

       
    }
  
   

   
}


[System.Serializable]
public class Root
{
    public string id { get; set; }
    public string author { get; set; }
    public int width { get; set; }
    public int height { get; set; }
    public string url { get; set; }
    public string download_url { get; set; }
}

