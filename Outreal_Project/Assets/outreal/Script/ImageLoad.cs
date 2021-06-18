using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ImageLoad : MonoBehaviour
{

    public int index;
    public string url;
    public Text progressPercent;
    // Start is called before the first frame update
    void Start()
    {
        
        SetImage(url, GetComponent<Image>(), progressPercent);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetImage(string url,Image image,Text loadingPercent)
    {
        DownloadHandler.get().load(url).into(image).start();
    }

}
