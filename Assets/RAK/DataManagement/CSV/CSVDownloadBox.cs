using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
[CreateAssetMenu(fileName = "CSVBox", menuName ="CSV DownLoad Box", order =1)]
public class CSVDownloadBox : ScriptableObject
{

    public string baseDirectory;
    public List<CSVDownloadData> dataList = new List<CSVDownloadData>();
    //private const string head = "https://docs.google.com/spreadsheets/d/";
    //private const string tail = "export?format=csv&id=";



#if UNITY_EDITOR

    //public void DownloadAll()
    //{
    //    for (int i = 0; i < dataList.Count; i++)
    //    {
    //        Download(i);
    //    }
    //}

    public void Download(int i, System.Action<TextAsset> onComplete)
    {
        CSVDownloadData data =  dataList[i];
        string url = string.Format("https://docs.google.com/spreadsheets/d/{0}/export?format=csv&id={0}&gid={1}", data.googleID, data.sheetID);
        string save_path = string.Format( "Assets/{0}/{1}.csv",baseDirectory,data.title);

        RAK.CSVDownloader.DownloadCSV(url,save_path, onComplete);
    }
#endif
}

