
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text;
using UnityEngine.Events;
using System.IO;
using System;

public class DownloadHandler : MonoBehaviour
{

    public bool enableLog=true;
    private bool cached = true;
    public bool isDataLoaded;

    public Slider progressSlider;
    private enum RendererType
    {
        none,
        uiImage,
        renderer
    }

    private RendererType rendererType = RendererType.none;
    private GameObject targetObj;
    private string url = null;

    [SerializeField]
    Texture2D PlaceholderImage;

    internal object loadingProgress(float loadingPercent)
    {
        throw new NotImplementedException();
    }

    Texture2D errorImage;

    private UnityAction onStartAction,
        onDownloadedAction,
        OnLoadedAction,
        onEndAction;

    private UnityAction<int> onDownloadProgressChange;

    private UnityAction<string> onErrorAction;

    private static Dictionary<string, DownloadHandler> underProcessDownload
        = new Dictionary<string, DownloadHandler>();

    private string uniqueHash;
    private int progress;

    public bool success = false;

    static string filePath = Application.persistentDataPath + "/" +
             "outreal" + "/";
    public void start()
    {
        if (url == null)
        {
            LogError("URL Not Set");
            return;
        }

        try
        {
            Uri uri = new Uri(url);
            this.url = uri.AbsoluteUri;

            
        }
        catch (Exception ex)
        {
            LogError("Incorrect URL");
            return;
        }

        if (rendererType == RendererType.none || targetObj == null)
        {
            LogError("Set the target");
            return;
        }

        if (enableLog)
            Debug.Log("Loadinfg Started");

        if (PlaceholderImage != null)
            SetLoadingImage();

        if (onStartAction != null)
            onStartAction.Invoke();

        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }

        uniqueHash = CreateMD5(url);

       if (underProcessDownload.ContainsKey(uniqueHash))
        {
            DownloadHandler sameProcess = underProcessDownload[uniqueHash];
            sameProcess.onDownloadedAction += () =>
            {
                if (onDownloadedAction != null)
                    onDownloadedAction.Invoke();

                loadSpriteToImage();
            };
        }
        else
        {
            if (File.Exists(filePath + uniqueHash))
            {
                if (onDownloadedAction != null)
                    onDownloadedAction.Invoke();

                loadSpriteToImage();
            }
            else
            {
                underProcessDownload.Add(uniqueHash, this);
                StopAllCoroutines();
                StartCoroutine("Downloader");
            }
        }
    }



    /// <summary>
    /// Set the sprite of image when davinci is downloading and loading image
    /// </summary>
    /// <param name="loadingPlaceholder">loading texture</param>
    /// <returns></returns>
    public DownloadHandler setLoadingPlaceholder(Texture2D loadingPlaceholder)
    {

        this.PlaceholderImage = loadingPlaceholder;

        if (enableLog)
            Debug.Log("Loading placeholder has been set.");

        return this;
    }
    /// <summary>
    /// Set the sprite of image when davinci is downloading and loading image
    /// </summary>
    /// <param name="loadingPlaceholder">loading texture</param>
    /// <returns></returns>
    public DownloadHandler loadingProgress(Text progress)
    {
        Debug.Log("<color=red> Progress has been set.</color>"+ this.progress.ToString());
        progress.text = this.progress.ToString();

        if (enableLog)
            Debug.Log("Progress has been set.");

        return this;
    }


    private IEnumerator Downloader()
    {

        if (enableLog) 
            Debug.Log("Download started.");

        var www = new WWW(url);

        while (!www.isDone )
        {
           
            if (www.error != null)
            {
                LogError("Error while downloading the image : " + www.error);
                yield break;
            }

            progress = Mathf.FloorToInt(www.bytesDownloaded * 100);

            Debug.Log("<color=green>Downloading progress :</color> " + progress + "%");
            if (onDownloadProgressChange != null)
            {
                
                onDownloadProgressChange.Invoke(progress);
            }
               

            if (enableLog)

                Debug.Log("<color=green>Downloading progress :</color> " + progress + "%");

            yield return null;
        }

        if (www.error == null)
            File.WriteAllBytes(filePath + uniqueHash, www.bytes);

        www.Dispose();
        www = null;

        if (onDownloadedAction != null)
            onDownloadedAction.Invoke();

        loadSpriteToImage();

        underProcessDownload.Remove(uniqueHash);

        
    }


    public DownloadHandler withDownloadProgressChangedAction(UnityAction<int> action)
    {
        this.onDownloadProgressChange = action;

        if (enableLog)
            Debug.Log(" download progress changed action set : " + action);

        return this;
    }

    public static string CreateMD5(string input)
    {
        // Use input string to calculate MD5 hash
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
    private void SetLoadingImage()
    {
        switch (rendererType)
        {
            case RendererType.renderer:
                Renderer renderer = targetObj.GetComponent<Renderer>();
                renderer.material.mainTexture = PlaceholderImage;
                break;

            case RendererType.uiImage:
                Image image = targetObj.GetComponent<Image>();
                Sprite sprite = Sprite.Create(PlaceholderImage,
                     new Rect(0, 0, PlaceholderImage.width, PlaceholderImage.height),
                     new Vector2(0.5f, 0.5f));
                image.sprite = sprite;

                break;
        }

    }
    private void LogError(string message)
    {
        success = false;

        if (enableLog)
            Debug.LogError("Error : " + message);

        if (onErrorAction != null)
            onErrorAction.Invoke(message);

        if (errorImage != null)
            StartCoroutine(LoadImage(errorImage));
        else clearChache();
    }
    /// <summary>
    /// Get instance of downloadHandler class
    /// </summary>
    public static DownloadHandler get()
    {
        return new GameObject("Handler").AddComponent<DownloadHandler>();
    }
    /// <summary>
    /// Set image url for download.
    /// </summary>
    /// <param name="url">Image Url</param>
    /// <returns></returns>
    public DownloadHandler load(string url)
    {
        if (enableLog)
            Debug.Log(">>>>>>>Download Handler>>>>>>> Url set : " + url);

        this.url = url;
        return this;
    }

    /// <summary>
    /// Set target Image component.
    /// </summary>
    /// <param name="image">target Unity UI image component</param>
    /// <returns></returns>
    public DownloadHandler into(Image image)
    {
        if (enableLog)
            Debug.Log(">>>>>>> DownloadHandler >>>>>>>>>> Target as UIImage set : " + image);

        rendererType = RendererType.uiImage;
        this.targetObj = image.gameObject;
        return this;
    }

    /// <summary>
    /// Set target Renderer component.
    /// </summary>
    /// <param name="renderer">target renderer component</param>
    /// <returns></returns>
    public DownloadHandler into(Renderer renderer)
    {
        if (enableLog)
            Debug.Log("[DownloadHandler] Target as Renderer set : " + renderer);

        rendererType = RendererType.renderer;
        this.targetObj = renderer.gameObject;
        return this;
    }


    #region Actions
    public DownloadHandler withStartAction(UnityAction action)
    {
        this.onStartAction = action;

        if (enableLog)
            Debug.Log("[DownloadHandler] On start action set : " + action);

        return this;
    }
    #endregion
    private void loadSpriteToImage()
    {
        progress = 100;
        if (onDownloadProgressChange != null)
            onDownloadProgressChange.Invoke(progress);

        if (enableLog)
            Debug.Log(" Downloading progress : " + progress + "%");

        if (!File.Exists(filePath + uniqueHash))
        {
            Debug.LogError("Loading image file has been failed.");
            return;
        }

        StopAllCoroutines();
        StartCoroutine(LoadImage());
    }

    ///
    private IEnumerator LoadImage(Texture2D tex = null)
    {
       
        if (tex == null)
        {
            byte[] fileData;
            fileData = File.ReadAllBytes(filePath + uniqueHash);
            tex = new Texture2D(2, 2);
             tex = new Texture2D(Convert.ToInt32(500), Convert.ToInt32(300), TextureFormat.RGB24, false);

            tex.Compress(false);
            //ImageConversion.LoadImage(texture, fileData);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }


        if (targetObj != null)
            switch (rendererType)
            {
                case RendererType.renderer:
                    Renderer renderer = targetObj.GetComponent<Renderer>();

                    if (renderer == null || renderer.material == null)
                        break;

                    renderer.material.mainTexture = tex;

                    yield return null;

                    break;

                case RendererType.uiImage:
                    Image image = targetObj.GetComponent<Image>();

                    if (image == null)
                        break;

                    Sprite sprite = Sprite.Create(tex,
                         new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

                    image.sprite = sprite;

                    yield return null;
                    break;
            }

        if (OnLoadedAction != null)
            OnLoadedAction.Invoke();

        if (enableLog)
            Debug.Log("Image has been loaded.");

        success = true;

        if (OnLoadedAction != null)
            OnLoadedAction.Invoke();

        if (enableLog)
            Debug.Log("[DownoadHandler] Image has been loaded.");

        success = true;
   
        clearChache();
        
    }

    /// <summary>
    /// 
    /// </summary>
    private void clearChache()
    {
        if (enableLog)
            Debug.Log("<<<<<< Download Handler >>>>>>> Operation has been finished.");

        if (!cached)
        {
            try
            {
                File.Delete(filePath + uniqueHash);
            }
            catch (Exception ex)
            {
                if (enableLog)
                    Debug.LogError(">>>> Error while removing cached file: {ex.Message}");
            }
        }

        if (onEndAction != null)
            onEndAction.Invoke();

           Invoke("destroyGameObject", 0.5f);
    }
    /// <summary>
    /// Set image sprite when some error occurred during downloading or loading image
    /// </summary>
    /// <param name="errorPlaceholder">error texture</param>
    /// <returns></returns>
    public DownloadHandler setErrorPlaceholder(Texture2D errorPlaceholder)
    {
        this.errorImage = errorPlaceholder;

        if (enableLog)
            Debug.Log(" Error placeholder has been set.");

        return this;
    }
    /// <summary>
    /// Enable cache
    /// </summary>
    /// <returns></returns>
    public DownloadHandler setCached(bool cached)
    {
        this.cached = cached;

        if (enableLog)
            Debug.Log(" Cache enabled : " + cached);

        return this;
    }



    private void destroyGameObject()
    {
        Destroy(gameObject);
    }

    public DownloadHandler DataPullGet(bool isSuccess)
    {
        this.isDataLoaded = isSuccess;

        return this;
    }
    public DownloadHandler DataPullSet(bool isSuccess)
    {
        this.isDataLoaded = isSuccess;
        return this;
    }
}