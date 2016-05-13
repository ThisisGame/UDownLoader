using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;

namespace ThisisGame
{
    class AsyncTask
    {
        public string url;
        public bool isText;
        public long receivedBytes;
        public long totalBytes;

        public const int bufferSize = 1024;
        public StringBuilder requestData;
        public byte[] bufferRead;
        public HttpWebRequest httpWebRequest;
        public HttpWebResponse httpWebResponse;
        public Stream responseStream;

        public AsyncTaskState asyncTaskState;

        public Exception exception;

        public float idleTime;
        public int retryTime;

        public AsyncTask()
        {
            url = string.Empty;
            isText = false; 
            receivedBytes = 0;
            totalBytes = 0;

            bufferRead = new byte[bufferSize];
            requestData = new StringBuilder();
            

            asyncTaskState = AsyncTaskState.None;

            exception = null;

            idleTime = 0f;
            retryTime = 0;
        }


        public void Release()
        {
            if(httpWebRequest!=null)
            {
                httpWebRequest.Abort();
                httpWebRequest = null;
            }

            if(httpWebResponse!=null)
            {
                httpWebResponse.Close();
                httpWebResponse = null;
            }

        }

    }
}
