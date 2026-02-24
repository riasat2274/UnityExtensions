using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System;

public class GXMLSerializer {
	
	public static string pDataFileExt = ".xml";


    public static void InitializeRootForThreading()
    {
        subRoot = "";
#if UNITY_EDITOR
        subRoot = Path.Combine(Application.dataPath, "SerializedStorage");
#else
        subRoot = Path.Combine(Application.persistentDataPath,"SerializedStorage");
#endif
    }
    public static string subRoot = "";
   public static string root
	{
		get
        {
            if (string.IsNullOrEmpty(subRoot))
            {
#if UNITY_EDITOR
                subRoot = Path.Combine(Application.dataPath, "SerializedStorage");
#else
                subRoot = Path.Combine(Application.persistentDataPath,"SerializedStorage");
#endif
            }


            if (!Directory.Exists (subRoot)) {
				Directory.CreateDirectory (subRoot);
			}

			return subRoot;
		}
	}

	public static void Save<T>(T dataObject, string filename){

		if(dataObject.GetType().IsSerializable){
			string path = System.IO.Path.Combine (root, filename);
			var serializer = new XmlSerializer (dataObject.GetType());

            using (var stream = new StreamWriter(path,false,System.Text.Encoding.ASCII)){
				serializer.Serialize (stream, dataObject);
				stream.Flush ();
				stream.Close ();
			}//using
		} else {
			Debug.LogError ("Data Object passed on GXMLSerializer is not serializable");
		}

	}//save

    public static void Save_FullPath<T>(T dataObject, string fullFilePath, bool overwrite = true){


        if(dataObject.GetType().IsSerializable){
            if (File.Exists(fullFilePath))
            {
                if (overwrite) File.Delete(fullFilePath);
                else
                {
                    Debug.LogError("File already exists");
                    return;
                }
            }

            var serializer = new XmlSerializer (dataObject.GetType());

            using (var stream = new StreamWriter(fullFilePath,false,System.Text.Encoding.ASCII)){
                serializer.Serialize (stream, dataObject);
                stream.Flush ();
                stream.Close ();
            }//using
        } else {
            Debug.LogError ("Data Object passed on GXMLSerializer is not serializable");
        }

    }//save
    
	public static string GetXMLString<T>(T dataObject){

		if(dataObject.GetType().IsSerializable){
			XmlSerializer serializer = new XmlSerializer (dataObject.GetType());

			using (StringWriter sWriter = new StringWriter()){
				serializer.Serialize (sWriter, dataObject);
				return sWriter.ToString ();
			}//using
		} else {
			Debug.LogError ("Data Object passed on GXMLSerializer is not serializable");
			return null;
		}

	}//GetXMLString
    
	public static T GetObjectFromXML<T>(string xmlString){

		var serializer = new XmlSerializer (typeof(T));
		StringReader reader = new StringReader (xmlString);
        try {

            return (T)serializer.Deserialize(reader);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
            return default(T);
        }
	}//GetXMLString

    public static bool FileExistsInGXMLRoot(string filename)
    {
        string path = System.IO.Path.Combine(root, filename);
        return File.Exists(path);
    }
    public static bool DeleteFileInGXMLRoot(string filename)
    {
        string path = System.IO.Path.Combine(root, filename);
        if (File.Exists(path))
        {
            File.Delete(path);
            return true;
        }else
            return false;
    }

    public static T Load<T>(string filename)
    {
        string path = System.IO.Path.Combine(root, filename);

        // Check if the file exists before attempting to open it
        if (!File.Exists(path))
        {
            Debug.LogError($"File not found: {path}");
            return default(T);
        }

        // Proceed with file deserialization if the file exists
        try
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var stream = new FileStream(path, FileMode.Open))
            {
                if (typeof(T).IsSerializable)
                {
                    return (T)serializer.Deserialize(stream);
                }
                else
                {
                    Debug.LogError("Loading failed, because the type is not serializable.");
                    return default(T);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred while loading the file: {ex.Message}");
            return default(T);
        }
    }


}//GXMLSerializer
