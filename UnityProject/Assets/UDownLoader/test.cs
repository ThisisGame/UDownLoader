using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using ThisisGame;

public class test : MonoBehaviour
{
    [SerializeField]
    Slider progressSlider;
    [SerializeField]
    Text progressText;

    UDownLoader downLoader = null;
    

    // Use this for initialization
    void Start ()
    {
        downLoader = new UDownLoader();
        downLoader.OnDownLoadProgress = (url,receivedBytes,totalBytes) => 
        {
            progressSlider.value = (float)receivedBytes / totalBytes;
        };

        downLoader.OnDownLoadFailed = (url,exception) => 
        {
            Debug.LogError("Request " + url + " Failed,Exception=" + exception.ToString());
        };

        downLoader.OnDownLoadCompleted = (url,result) => 
        {
            Debug.Log("Request " + url + " Completed,result=" + result);

            progressSlider.value = 1;
        };

        downLoader.DownLoadAsync("http://shcdn.igamesofficial.com/snk/AssetbundleUpdate/A20_zhubao/version.txt", true);
    }

    // Update is called once per frame
    void Update()
    {
        if (downLoader != null)
        {
            downLoader.Update();
        }
    }


    void OnDestroy()
    {
        if (downLoader != null)
        {
            downLoader.Release();
        }
    }
}
