using LitJson;
using System;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Svae unity Object : MonoBehaviour to Json
/// Plz re-write class to inherit from this class
/// </summary>
public abstract class JSONSE
{
    protected string obj_str;                 // In-game object data
    protected string savePath;                // Save path where json would be save/load

    public abstract void SetSaveURL(string _url);        // Pre-set the dir to save the files
    public abstract void SaveFile(LightCar _car);          // Override method using:SaveObjectToJson()
    public abstract void LoadFile();          // Override method using:LoadObjectFronJson()


    protected void ObjectToJson(object _carData)
    {
        if (_carData == null || _carData.IsUnityNull()) 
        {
            Debug.LogError("Empty object transfer");
            return;
        };

        obj_str = JsonMapper.ToJson(_carData);
        Debug.Log("Json transformed successfully");
    }

    protected void ObjectToJson(GameObject _car)
    {
        if (_car == null || _car.IsUnityNull())
        {
            Debug.LogError("Empty object transfer");
            return;
        };

        obj_str = JsonMapper.ToJson(_car);
        Debug.Log("Json transformed successfully");
    }

    protected void SaveObjectToJson(string _url)
    {
        if(_url == null || _url.Length == 0)
        {
            throw new System.Exception("Wrong save dir");
        }

        if(obj_str == null || obj_str.Length == 0)
        {
            Debug.LogWarning("Json hasn't load before saving");
        }

        // Name file by time
        string time = DateTime.Now.DayOfYear.ToString();

        string dataPath = _url+time+"_car.json";
        StreamWriter sw = new StreamWriter(dataPath);
        sw.Write(obj_str);
        sw.Close();
    }

    protected VehicleController LoadObjectFronJson(string _url)
    {
        if (_url == null || _url.Length == 0)
        {
            throw new System.Exception("Wrong save dir");
        }

        string dataPath = _url;
        if (!File.Exists(dataPath))
        {
            throw new System.Exception("File path contains nothing");
        }

        StreamReader sr = new StreamReader(dataPath);
        obj_str = sr.ReadToEnd();
        sr.Close();

        return JsonMapper.ToObject<VehicleController>(obj_str);
    }
}
