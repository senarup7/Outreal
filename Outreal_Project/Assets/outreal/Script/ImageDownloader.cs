using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class ImageDownloader : MonoBehaviour
{

    public List<string> DownloadURL = new List<string>();

    public JSON_Data _JSON_Data;


    public Text MessageText;
  // Use for GetText
    //[SerializeField]
    //Image image;


 
    void Start()
    {
      

    }


    ///
    void ShowMessage(string message)
    {
        MessageText.text = message.ToString();
    }
    void ShowMessageEnabled(bool val)
    {
        MessageText.enabled=val;
    }
    /// <summary>
    /// Only Editor URL Images
    /// </summary>
    public void Editor_URL_Images()
    {
        ShowMessage("Downloading From Editor URL");
        foreach (Transform child in _JSON_Data.content.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        for (int n = 0; n < DownloadURL.Count; n++)
        {

            GameObject img = Instantiate(_JSON_Data.cell);
            img.transform.SetParent(_JSON_Data.content.transform, false);
            img.GetComponent<ImageLoad>().url = DownloadURL[n];

        }
        //ShowMessageEnabled(false);
    }

    /// <summary>
    /// 
    /// </summary>
    public void JSON_URL_Images()
    {
        ShowMessage("Downloading From JSON; It's take time, so please wait");
        foreach (Transform child in _JSON_Data.content.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        for (int n = 0; n < _JSON_Data._RootData.Count; n++)
        {

            GameObject img = Instantiate(_JSON_Data.cell);
            img.transform.SetParent(_JSON_Data.content.transform, false);
            img.GetComponent<ImageLoad>().url = _JSON_Data._RootData[n].download_url;

        }
        ShowMessageEnabled(false);
    }


    /// <summary>
    /// Testing 
    /// </summary>
    /// <returns></returns>
    IEnumerator GetText()
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture("https://dl.dropboxusercontent.com/s/5xg8hj6kov27vw0/tntbox.png"))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                Debug.Log("Downloading........");
                // Get downloaded Images
                var texture = DownloadHandlerTexture.GetContent(uwr);
                Sprite sprite = Sprite.Create(texture,
                         new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));


                //image.GetComponent<Image>().overrideSprite = sprite;
            }
        }
    }


}
