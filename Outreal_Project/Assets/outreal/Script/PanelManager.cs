using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{


    [SerializeField]
    GameObject MainPanel;
    [SerializeField]
    GameObject BackPanel;
    [SerializeField]
    GameObject RendereShowPanel;
    [SerializeField]
    GameObject ImageDownloaderPanel;
    [SerializeField]
    GameObject RendererDownloaderPanel;
    // Start is called before the first frame update



    private void Start()
    {
        Main();
    }
    void SetPanelOff()
    {
        BackPanel.SetActive(false);
        MainPanel.SetActive(false);
        ImageDownloaderPanel.SetActive(false);
        RendererDownloaderPanel.SetActive(false);

    }
    void SetPanelOn(GameObject tPanel,bool val)
    {
        tPanel.SetActive(val);

    }

    void OnPanelActive(GameObject tPanel)
    {

        SetPanelOff();
        SetPanelOn(tPanel, true);

    }
    public void ImageDownload()
    {
        OnPanelActive(ImageDownloaderPanel);
        SetPanelOn(BackPanel, true);
        SetPanelOn(RendereShowPanel, false);
    }
    public void RendererDownload()
    {
        OnPanelActive(RendererDownloaderPanel);
        SetPanelOn(BackPanel,true);
        SetPanelOn(RendereShowPanel, true);
    }
    public void Main()
    {

        SetPanelOff();
        SetPanelOn(MainPanel, true);
        SetPanelOn(RendereShowPanel, false);
    }
}
