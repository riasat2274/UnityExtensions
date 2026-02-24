using UnityEngine;
using System;
using System.Collections.Generic;

namespace RAK
{
    public static class HardDataCleaner
    {
        public static void Clean()
        {
            HardData<int>.ResetAllHardData();
            HardData<float>.ResetAllHardData();
            HardData<double>.ResetAllHardData();
            HardData<string>.ResetAllHardData();
            HardData<bool>.ResetAllHardData();
        }
    }

    //public class HardDataList<T>
    //{
    //    public int N { get { return countHD.value; } }

    //    HardData<int> countHD;
    //    List<HardData<T>> datas;
    //    string key;
    //    HardDataList(string key)
    //    {
    //        this.key = key;
    //        countHD = new HardData<int>(key,0);
    //        datas = new List<HardData<T>>();

    //    }

    //    public void Add(T initialValue)
    //    {
    //        HardData<T> hd = new HardData<T>($"{key}_{N}", initialValue);
    //        datas.Add(hd);
    //    }

    //    public HardData<T> this[int i]
    //    {
    //        get { return datas[i]; }
    //        set { datas[i] = value; }
    //    }
    //}
    public class HardStringDictionary<T>
    {
        //public int N { get { return countHD.value; } }

        HardData<string> keysHD;
        string mainKey;
        List<string> klist = new List<string>();
        Dictionary<string, HardData<T>> datas = new Dictionary<string, HardData<T>>();
        T defaultV;
        public HardStringDictionary(string mainKey, T defaultValue)
        {
            defaultV = defaultValue;
            this.mainKey = mainKey;
            keysHD = new HardData<string>(mainKey, "");
            string[] karr = keysHD.value.Split(',');
            foreach (var k in karr)
            {
                if (string.IsNullOrEmpty(k)) continue;

                HardData<T> hd = new HardData<T>($"{mainKey}_{k}", defaultValue);
                //$"{mainKey}--- {k}: {hd.value}".Debug(Color.cyan);
                datas.Add(k, hd);
                klist.Add(k);
            }
        }
        void Add(string key, T initialValue)
        {
            if (string.IsNullOrEmpty(key))
            {
                {
                    Debug.LogError($"key is null orempty");
                    return;
                }
            }
            if (klist.Contains(key))
            {
                Debug.LogError($"key \"{key}\" alreay exists");
                return;
            }


            HardData<T> hd = new HardData<T>($"{mainKey}_{key}", initialValue);
            datas.Add(key, hd);
            klist.Add(key);
            if (string.IsNullOrEmpty(keysHD.value)) keysHD.value = key;
            else keysHD.value = $"{keysHD.value},{key}";

        }

        public T this[string key]
        {
            get {
                if (!klist.Contains(key))
                {
                    Add(key, defaultV);
                }
                return datas[key].value;
            }
            set
            {
                if (!klist.Contains(key))
                {
                    Add(key, defaultV);
                }
                datas[key].value = value;
            }
        }
    }
    public class HardData<T>
    {

        public static implicit operator T (HardData<T> hd) => hd.value;

        string savedKey;
        T localValue;
        SettingTypes type;

        public static List<string> keyList = new List<string>();
        public static void ResetAllHardData()
        {
            foreach (string key in keyList)
            {
                PlayerPrefs.HasKey(key);
                PlayerPrefs.DeleteKey(key);
            }
        }

        public HardData(string Key, T initValue)
        {
            #region defineType
            if (typeof(T) == typeof(bool))
            {
                type = SettingTypes._bool;
            }
            else if (typeof(T) == typeof(int))
            {
                type = SettingTypes._int;
            }
            else if (typeof(T) == typeof(float))
            {
                type = SettingTypes._float;
            }
            else if (typeof(T) == typeof(double))
            {
                type = SettingTypes._double;
            }
            else if (typeof(T) == typeof(long))
            {
                type = SettingTypes._long;
            }
            else if (typeof(T) == typeof(string)) 
            {
                type = SettingTypes._string;
            }
            else if ( typeof ( T ) == typeof ( DateTime ) )
            {
                type = SettingTypes._dateTime;
            }
            else 
            {
                type = SettingTypes._UNDEFINEDTYPE;
                Debug.LogError ("Undefined setting type!!!");
            }
            #endregion
            savedKey = Key;
            if (keyList.Contains(savedKey))
                Debug.LogWarningFormat("Duplicate keys: {0}!",savedKey);
            keyList.Add(savedKey);
            localValue = initValue;
            loadFromPref ();
            saveToPref();
        }

        public T value
        {
            set
            {
                localValue = value;
                saveToPref ();
                onValueUpdated?.Invoke(value);
            }
            get
            {
                return localValue;
            }
        }
        public event Action<T> onValueUpdated;
        public string GetKey()
        {
            return savedKey;
        }

        void saveToPref()
        {
            switch (type) {
                default:
                    Debug.LogError ("Pref saving not defined for this type");
                    break;
                case SettingTypes._bool:
                    {
                        bool locBoolValue = (bool)Convert.ChangeType (localValue, typeof(bool));
                        int prefValue = (locBoolValue ? 1 : 0);
                        PlayerPrefs.SetInt (savedKey, prefValue);
                    }
                    break;
                case SettingTypes._int:
                    {
                        int locValue = (int)Convert.ChangeType (localValue, typeof(int));
                        PlayerPrefs.SetInt (savedKey, locValue);
                    }
                    break;      
                case SettingTypes._float:
                    {
                        float locValue = (float)Convert.ChangeType (localValue, typeof(float));
                        PlayerPrefs.SetFloat (savedKey, locValue);
                    }
                    break;
                case SettingTypes._double:
                    {
                        double locValue = (double)Convert.ChangeType(localValue, typeof(double));
                        PlayerPrefs.SetString(savedKey, locValue.ToString());
                    }
                    break;
                case SettingTypes._long:
                    {
                        long longValue = (long)Convert.ChangeType(localValue, typeof(long));
                        string longString = longValue.ToString();
                        PlayerPrefs.SetString(savedKey, longString);
                    }
                    break;
                case SettingTypes._string:
                    {
                        string locValue = (string)Convert.ChangeType (localValue, typeof(string));
                        PlayerPrefs.SetString (savedKey, locValue);
                    }
                    break;
                case SettingTypes._dateTime:
                    {
                        DateTime dateValue = (DateTime) Convert.ChangeType ( localValue, typeof ( DateTime ) );
                        string dateValueStr = dateValue.Ticks.ToString();
                        PlayerPrefs.SetString ( savedKey, dateValueStr );
                    }
                    break;
            }
        }
        void loadFromPref()
        {
            if (PlayerPrefs.HasKey(savedKey))
            {
                switch (type)
                {
                    default:
                        Debug.LogError("Pref loading not defined for this type");
                        break;
                    case SettingTypes._bool:
                        {
                            int prefValue = PlayerPrefs.GetInt(savedKey);
                            bool prefBool = (prefValue != 0);
                            localValue = (T)Convert.ChangeType(prefBool, typeof(T));
                        }
                        break;
                    case SettingTypes._int:
                        {
                            int prefValue = PlayerPrefs.GetInt(savedKey);
                            localValue = (T)Convert.ChangeType(prefValue, typeof(T));
                        }
                        break;
                    case SettingTypes._float:
                        {
                            float prefValue = PlayerPrefs.GetFloat(savedKey);
                            localValue = (T)Convert.ChangeType(prefValue, typeof(T));
                        }
                        break;
                    case SettingTypes._double:
                        {
                            string prefValue = PlayerPrefs.GetString(savedKey);
                            double doblValue = double.Parse(prefValue);
                            localValue = (T)Convert.ChangeType(doblValue, typeof(T));
                        }
                        break;
                    case SettingTypes._long:
                        {
                            string prefValue = PlayerPrefs.GetString(savedKey);
                            long longValue = long.Parse(prefValue);
                            localValue = (T)Convert.ChangeType(longValue, typeof(T));
                        }
                        break;
                    case SettingTypes._string:
                        {
                            string prefValue = PlayerPrefs.GetString(savedKey);
                            localValue = (T)Convert.ChangeType(prefValue, typeof(T));
                        }
                        break;
                    case SettingTypes._dateTime:
                        {
                            string prefValue = PlayerPrefs.GetString(savedKey);
                            long ticks = long.Parse(prefValue);
                            DateTime dateValue = new DateTime(ticks);
                            localValue = (T)Convert.ChangeType(dateValue, typeof(T));
                        }
                        break;
                }
            }
        }

        public enum SettingTypes
        {
            _bool,
            _int,
            _float,
            _double,
            _long,
            _string,
            _dateTime,
            _UNDEFINEDTYPE
        }
    }

}