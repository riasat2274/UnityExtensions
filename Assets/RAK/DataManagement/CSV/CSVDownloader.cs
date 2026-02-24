using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using UnityEditor;

namespace RAK
{
    public static class CSVDownloader
    {
#if UNITY_EDITOR

        private const string head = "https://docs.google.com/spreadsheets/d/";
        private const string tail = "/export?format=csv";


        [MenuItem("Download/Piercing Games/Sequence Data")]
        public static void DownloadCSV_Sequence()
        {

            string url = head + "1fq56sGsNlo8BRiVd5XI7TIZ7jx2DZW-ytpDXHDUxlV8" + tail;
            string save_path = "Assets/Resources/CSV/CSV_Sequence.csv";

            DownloadCSV(url, save_path);
        }


        [MenuItem("Download/Piercing Games/Level Detail Data")]
        public static void DownloadCSV_Details()
        {

            string url = head + "1nfIl-tBv6kdVSC6l4Ojt2lkbbGXgrwB3LhrTY77zkas" + tail;
            string save_path = "Assets/Resources/CSV/CSV_LevelDetail.csv";

            DownloadCSV(url, save_path);
        }



        public static void DownloadCSV(string url, string save_path, System.Action<TextAsset> onComplete = null)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(url);

            webRequest.SendWebRequest().completed += (AsyncOperation asop) =>
            {
                if (webRequest.isNetworkError)
                {
                    Debug.Log("Download Error: " + webRequest.error);
                }
                else
                {
                    Debug.LogFormat("Download success: {0}", save_path);
                    //Debug.Log("Data: " + webRequest.downloadHandler.text);

                    File.WriteAllText(save_path, webRequest.downloadHandler.text);
                    AssetDatabase.Refresh();
                    TextAsset ta= AssetDatabase.LoadAssetAtPath<TextAsset>(save_path);
                    Debug.Log(ta.text);
                    onComplete?.Invoke(ta);
                }
            };
        }
#endif

    }
}