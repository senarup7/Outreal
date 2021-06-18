using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererImageExample : MonoBehaviour
{
    public Renderer sphere, cube;
    public string URL_1, URL_2;


    private void Start()
    {
        
    }



    public void SetRendererImage()
    {
        DownloadHandler.get()
            .load(URL_1)
            .into(cube)
            .setCached(false)
            .start();

        DownloadHandler.get()
            .load(URL_2)
            .into(sphere)
            .setCached(false)
            .start();
    }
}
