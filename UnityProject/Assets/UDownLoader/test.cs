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
