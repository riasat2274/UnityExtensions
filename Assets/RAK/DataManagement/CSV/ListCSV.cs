using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RAK
{
    [System.Serializable]
    public class ListCSV<T> where T : new()
    {
        public TextAsset csvFile;

        public List<T> data = new List<T>();
        public ListCSV() 
        {
            data = new List<T>();
            CSVReader.Parse_toClassWithBasicConstructor(data, csvFile);
        }

    }
}
