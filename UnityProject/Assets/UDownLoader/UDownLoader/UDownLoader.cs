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
        public Action<string, long, long> OnDownLoadProgress;

        public Action<string, string> OnDownLoadCompleted;

        public Action<string, Exception> OnDownLoadFailed;

        FileStream fileStream = null;

        AsyncTask asyncTask = null;


        /// <summary>
        /// 
        /// </summary>
        public UDownLoader()
        {
            asyncTask = new AsyncTask();
        }

        /// <summary>
        /// DownLoadFileAsync
        /// </summary>
        /// <param name="url"></param>
        /// <param name="savepath"></param>
        public void DownLoadFileAsync(string url, string savepath)
        {
            DownLoadAsync(url, false, savepath);
        }

        /// <summary>
        /// DownLoadStringAsync
        /// </summary>
        /// <param name="url"></param>
        public void DownLoadStringAsync(string url)
        {
            DownLoadAsync(url, true);
        }

        /// <summary>
        /// DownLoad Async
        /// </summary>
        /// <param name="url">url to download</param>
        /// <param name="url">filepath to save</param>
        /// /// <param name="url">is Text or not</param>
        private void DownLoadAsync(string url, bool isText = false,string savepath="")
        {
            Debug.Log("DownLoadAsync url=" + url + " isText=" + isText);

            asyncTask.url = url;
            asyncTask.isText = isText;
            asyncTask.asyncTaskState = AsyncTaskState.Prepare;

            //createfile
            if (!isText)
            {
                fileStream = new FileStream(savepath, FileMode.Append);
            }


            try
            {
                HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
                if (!isText)
                {
                    httpWebRequest.AddRange((int)fileStream.Length);
                }
                    

                asyncTask.httpWebRequest = httpWebRequest;

                httpWebRequest.BeginGetResponse(new AsyncCallback(ResponseCallback), asyncTask);
            }
            catch (WebException exp)
            {
                Debug.LogError("DownLoadAsync exception="+exp);

                asyncTask.asyncTaskState = AsyncTaskState.Failed;
                asyncTask.exception = exp;

            }
            catch (Exception exp)
            {
                Debug.LogError("DownLoadAsync exception=" + exp);

                asyncTask.asyncTaskState = AsyncTaskState.Failed;
                asyncTask.exception = exp;
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
                asyncTask.totalBytes = httpWebResponse.ContentLength;

                responseStream.BeginRead(asyncTask.bufferRead, 0, AsyncTask.bufferSize, new AsyncCallback(ReadCallBack), asyncTask);
            }
            catch (WebException exp)
            {
                httpWebResponse = exp.Response as HttpWebResponse;
                switch (httpWebResponse.StatusCode)
                {
                    case HttpStatusCode.RequestedRangeNotSatisfiable:
                        {
                            asyncTask.asyncTaskState = AsyncTaskState.Complete;
 
                            return;
                        }
                        break;
                }
                Debug.LogError("ResponseCallback exception=" + exp.ToString());

                asyncTask.asyncTaskState = AsyncTaskState.Failed;

                asyncTask.exception = exp;
            }
            catch (Exception exp)
            {
                Debug.LogError(exp.ToString());

                asyncTask.asyncTaskState = AsyncTaskState.Failed;

                asyncTask.exception = exp;
            }
        }

        private void ReadCallBack(IAsyncResult result)
        {
            //Debug.Log("ReadCallBack");
            AsyncTask asyncTask = result.AsyncState as AsyncTask;

            Stream responseStream = asyncTask.responseStream;

            try
            {
                int read = responseStream.EndRead(result);

                //Debug.Log("read size = " + read);

                if (read > 0)
                {
                    asyncTask.receivedBytes += read;


                    if (asyncTask.isText)
                    {
                        asyncTask.requestData.Append(System.Text.Encoding.Default.GetString(asyncTask.bufferRead));
                    }
                    else
                    {
                        //write file
                        fileStream.Write(asyncTask.bufferRead,0,read);
                        fileStream.Flush();
                    }

                    asyncTask.bufferRead = new byte[AsyncTask.bufferSize];
                    responseStream.BeginRead(asyncTask.bufferRead, 0, AsyncTask.bufferSize, new AsyncCallback(ReadCallBack), asyncTask);

                }
                else
                {
                    asyncTask.asyncTaskState = AsyncTaskState.Complete;
                }
            }
            catch(WebException exp)
            {
                Debug.LogError(exp.ToString());

                asyncTask.asyncTaskState = AsyncTaskState.Failed;
                asyncTask.exception = exp;
            }
            catch(Exception exp)
            {
                Debug.LogError(exp.ToString());

                asyncTask.asyncTaskState = AsyncTaskState.Failed;
                asyncTask.exception = exp;
            }
        }

        public void Release()
        {
            if (asyncTask != null)
            {
                asyncTask.Release();
            }

            if (fileStream != null)
            {
                fileStream.Flush();
                fileStream.Close();
                fileStream = null;
            }
        }


        public void Update()
        {
            if (asyncTask != null)
            {
                switch(asyncTask.asyncTaskState)
                {
                    case AsyncTaskState.DownLoading:
                        {
                            if (asyncTask.totalBytes > 0)
                            {
                                OnDownLoadProgress(asyncTask.url, asyncTask.receivedBytes, asyncTask.totalBytes);

                            }
                        }
                        break;
                    case AsyncTaskState.Complete:
                        {
                            OnDownLoadCompleted(asyncTask.url, asyncTask.requestData.ToString());

                            asyncTask.asyncTaskState = AsyncTaskState.None;

                            Release();
                        }
                        break;
                    case AsyncTaskState.Failed:
                        {
                            OnDownLoadFailed(asyncTask.url, asyncTask.exception);

                            asyncTask.asyncTaskState = AsyncTaskState.None;

                            Release();
                        }
                        break;
                }
            }
        }
    }

}
