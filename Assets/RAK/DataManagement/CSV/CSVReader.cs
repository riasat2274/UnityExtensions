using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace RAK
{
    public class CSVReader : MonoBehaviour
    {

        public string path;
        public char commaCharacter = ',';
        void Start()
        {
            foreach (var csv in ReadFromResource(path, commaCharacter))
            {
                string s = "";
                foreach (string item in csv.fields)
                {
                    s += "#" + item + " ";
                }
                Debug.Log(s);
            }
        }


        public static CSVRow[] ReadFromResource(string path, char commaCharacter)
        {
            TextAsset csvAsset = Resources.Load<TextAsset>(path);
            return ReadCSVAsset(csvAsset, commaCharacter);
        }
        public static CSVRow[] ReadCSVAsset(TextAsset csvAsset, char commaCharacter, bool keepHeader =false)
        {
            string[] rows = csvAsset.text.Split(new char[] { '\n' });
            CSVRow[] csvRows = new CSVRow[rows.Length -  (keepHeader?0:1)];

            for (int i = 0; i < csvRows.Length; i++)
            {
                csvRows[i] = new CSVRow(rows[i + (keepHeader ? 0 : 1)].Split(new char[] { ',' }), commaCharacter);
            }

            return csvRows;
        }





        public static void Parse_toClassWithBasicConstructor<T>( List<T> targetList, TextAsset csvAsset) where T : new()
        {
            CSVRow[] rows = CSVReader.ReadCSVAsset(csvAsset, '|', true);
            CSVRow header = rows[0];
            targetList.Clear();
            Type type = typeof(T);

            for (int i = 1; i < rows.Length; i++)
            {
                T nt = new T();
                targetList.Add(nt);
            }
            for (int f = 0; f < header.fields.Length; f++)
            {
                string fieldName = header.fields[f];

                FieldInfo finfo = type.GetField(fieldName);
                if (finfo != null)
                {
                    for (int r = 1; r < rows.Length; r++)
                    {
                        Type t = finfo.FieldType;
                        if (t == typeof(int))
                        {
                            int intVal = 0;
                            int.TryParse(rows[r].fields[f], out intVal);
                            finfo.SetValue(targetList[r - 1], intVal);
                        }
                        else if (t == typeof(float) || t == typeof(double))
                        {
                            float val = 0;
                            float.TryParse(rows[r].fields[f], out val);
                            finfo.SetValue(targetList[r - 1], val);
                        }
                        else if (t.IsEnum)
                        {
                            finfo.SetValue(targetList[r - 1], Enum.Parse(t, rows[r].fields[f]));
                        }
                        else if (t == typeof(string))
                        {

                            finfo.SetValue(targetList[r - 1], rows[r].fields[f]);
                        }
                        else
                        {
                            throw new Exception($"unsupported format in parser! Type=>{finfo.FieldType}");
                        }
                    }
                }
            }
        }


        public static Dictionary<string, CSVRow> ReadFromResource(string path, int keyIndex, char commaCharacter)
        {
            TextAsset csvAsset = Resources.Load<TextAsset>(path);
            return ReadCSVAsset(csvAsset, keyIndex, commaCharacter);
        }
        public static Dictionary<string, CSVRow> ReadCSVAsset(TextAsset csvAsset, int keyIndex, char commaCharacter)
        {
            string[] rows = csvAsset.text.Split(new char[] { '\n' });

            Dictionary<string, CSVRow> dic = new Dictionary<string, CSVRow>(rows.Length - 1); //first row ignored

            for (int i = 0; i < rows.Length - 1; i++)
            {
                CSVRow csvRow = new CSVRow(rows[i + 1].Split(new char[] { ',' }), commaCharacter);

                dic.Add(csvRow.fields[keyIndex], csvRow);

            }

            return dic;
        }


    }
    public class CSVRow
    {
        public string[] fields;
        public CSVRow(string[] fields, char commaSubstitute = ',')
        {
            if (commaSubstitute != ',')
            {
                this.fields = new string[fields.Length];
                for (int i = 0; i < fields.Length; i++)
                {
                    this.fields[i] = fields[i].Replace(commaSubstitute, ',');
                }
            }
            else
            {
                this.fields = fields;
            }
        }
    }

}