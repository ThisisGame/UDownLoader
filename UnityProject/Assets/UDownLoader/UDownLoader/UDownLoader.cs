using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.IO;
using System.Threading;
using System.Text;
using System.Collections.Generic;

namespace ThisisGame
{
    public class UDownLoader
    {
        /// <summary>
        /// 
        /// </summary>
        public UDownLoader()
        {
        }

        /// <summary>
        /// DownLoad Async
        /// </summary>
        /// <param name="url">url to download</param>
        /// /// <param name="url">is Text or not</param>
        public void DownLoadAsync(string url, bool isText = false)
        {
            Debug.Log("DownLoadAsync url=" + url + " isText=" + isText);

            try
            {

                AsyncTask asyncTask = new AsyncTask();
                asyncTask.url = url;
                asyncTask.isText = isText;
                asyncTask.asyncTaskState = AsyncTaskState.Prepare;

                HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;

                asyncTask.httpWebRequest = httpWebRequest;

                httpWebRequest.BeginGetResponse(new AsyncCallback(ResponseCallback), asyncTask);
            }
            catch (WebException ex)
            {
                Debug.LogException(ex);

            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }


        private void ResponseCallback(IAsyncResult result)
        {
            Debug.Log("ResponseCallback");

            AsyncTask asyncTask = result.AsyncState as AsyncTask;
            HttpWebRequest httpWebRequest = asyncTask.httpWebRequest;
            HttpWebResponse httpWebResponse = null;
            Stream responseStream = null;
            try
            {
                httpWebResponse = httpWebRequest.EndGetResponse(result) as HttpWebResponse;
                responseStream = httpWebResponse.GetResponseStream();

                asyncTask.asyncTaskState = AsyncTaskState.DownLoading;
                asyncTask.httpWebResponse = httpWebResponse;
                asyncTask.responseStream = responseStream;

                responseStream.BeginRead(asyncTask.bufferRead, 0, AsyncTask.bufferSize, new AsyncCallback(ReadCallBack), asyncTask);
            }
            catch (WebException exp)
            {
                Debug.LogError(exp.ToString());
            }
            catch (Exception exp)
            {
                Debug.LogError(exp.ToString());
            }
        }

        private void ReadCallBack(IAsyncResult result)
        {
            Debug.Log("ReadCallBack");
            AsyncTask asyncTask = result.AsyncState as AsyncTask;
        }

        public void Release()
        {

        }


        public void Update()
        {

        }


    }

}
