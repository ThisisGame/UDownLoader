using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using ThisisGame;

public class test : MonoBehaviour
{
    [SerializeField]
    Button downLoadFileAsyncBtn;
    [SerializeField]
    Button downLoadStringAsyncBtn;
    [SerializeField]
    Text downLoadStringAsyncText;

    [SerializeField]
    Slider progressSlider;
    [SerializeField]
    Text progressText;

    UDownLoader downLoader = null;
    

    // Use this for initialization
    void Start ()
    {
        downLoadFileAsyncBtn.onClick.AddListener(delegate () 
        {
            downLoader.DownLoadFileAsync("http://www.thisisgame.com.cn/uuu.zip", "./uuu.zip");

            downLoadStringAsyncBtn.interactable = false;
        });

        downLoadStringAsyncBtn.onClick.AddListener(delegate () 
        {
            downLoader.DownLoadStringAsync("http://www.thisisgame.com.cn");

            downLoadFileAsyncBtn.interactable = false;
        });



        downLoader = new UDownLoader();
        downLoader.OnDownLoadProgress = (url,receivedBytes,totalBytes) => 
        {
            float progress = (float)receivedBytes / totalBytes;
            //Debug.Log("OnDownLoadProgress "+ progress);
            progressSlider.value = progress;
        };

        downLoader.OnDownLoadFailed = (url,exception) => 
        {
            Debug.LogError("Request " + url + " Failed,Exception=" + exception.ToString());
        };

        downLoader.OnDownLoadCompleted = (url,result) => 
        {
            Debug.Log("Request " + url + " Completed,result=" + result);

            progressSlider.value = 1;

            downLoadStringAsyncText.text = result;
        };
        
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
