using BestHTTP;
using NBCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GoogleSheetService : Singleton<GoogleSheetService>
{
    public class SheetData
    {
        public string range;
        public string majorDimension;
        public IList<string[]> values;
    }

    public async Task<IList<string[]>> GetGoogleSheetValue(string path)
    {
        TaskCompletionSource<IList<string[]>> processTask = new TaskCompletionSource<IList<string[]>>();
        try
        {
            //string fullURL = "https://sheets.googleapis.com/v4/spreadsheets/1JFsubiK8YczIp6nBiIoNb7tXoNZmOzS8Db32h2zyyJ8/values/Stage_Config!A2:M100?key=AIzaSyArJaIzgcoaVJBAMhRPq3vqGJ5tjShICX0";
            Debug.Log($"Post Async Request : {path} == {DateTime.Now.ToString()}");

            var request = new HTTPRequest(new Uri(path), HTTPMethods.Get, (req, res) =>
            {
                Debug.Log($"Get Async Response : {res.DataAsText} == {DateTime.Now.ToString()}");
                //Sheet data
                var sheetData = JsonConvert.DeserializeObject<SheetData>(res.DataAsText);
                Debug.LogWarning("====" + sheetData.values.Count);

                if (processTask?.Task.IsCompleted == false)
                {
                    processTask.SetResult(sheetData?.values);
                    processTask = null;
                }
            });

            Debug.Log("request :" + path);
            request.Send();
            return await processTask.Task;
        }
        catch (Exception ex)
        {
            Debug.LogError($"PostAsync Error: {ex.Message} === {ex.StackTrace}");
            return null;
        }
    }
}
