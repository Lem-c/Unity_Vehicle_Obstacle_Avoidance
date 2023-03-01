using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonAssist : JSONSE
{
    /*Templet path: "Assets/Build/PlayerData" */

    public JsonAssist(string _url)
    {
        SetSaveURL(_url);
    }

    public override void LoadFile()
    {
        LoadObjectFronJson(savePath);
    }

    public override void SaveFile(LightCar _car)
    {
        ObjectToJson(_car.dashboard);
        SaveObjectToJson(savePath);
    }

    public void SaveFile(GameObject _car)
    {
        ObjectToJson(_car);
        SaveObjectToJson(savePath);
    }

    public override void SetSaveURL(string _url)
    {
        if(_url.Length==0 || !_url.Contains("/"))
        {
            throw new System.ArgumentException("Wrong url");
        }

        savePath = _url;
        // Debug.LogWarning("Warning:Save path changed");
    }
}
