using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class Config : MonoBehaviour {
	public string jsonPath = null;
	private string jsonStr = null;
	private JSONObject jsonRoot = null;
	public bool Initialized
	{
		get
		{return jsonStr != null;}
	}
	private static Config _instance = null;
	public static Config instance
	{
		get
		{
			if(_instance==null)
			{
				_instance = FindObjectOfType<Config>();
				if(_instance!=null)
				{
					_instance.Init();
				}
			}
			return _instance;
		}
	}

	public string ReadJsonFile(string path)
	{
		if(File.Exists(path))
		{
			return File.ReadAllText(path);
		}
		else
		{
			return null;
		}
	}

	public JSONObject GetJsonObj(string uri)
	{
		string[] fields = uri.Split("/".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
		JSONObject jo = jsonRoot;
		foreach(string field in fields)
		{
			jo = jo.GetField(field);
			if(jo == null)
			{
				return null;
			}
		}
		return jo;
	}

	public string GetString(string uri, string fallback)
	{
		JSONObject jObj=GetJsonObj(uri);
		if(jObj!=null && jObj.IsString)
		{
			return jObj.str;
		}
		return fallback;
	}

	public float GetFloat(string uri, float fallback)
	{
		JSONObject jObj=GetJsonObj(uri);
		if(jObj!=null && jObj.IsNumber)
		{
			return jObj.f;
		}
		return fallback;
	}
	public int GetInt(string uri, int fallback)
	{
		JSONObject jObj=GetJsonObj(uri);
		if(jObj!=null && jObj.IsNumber)
		{
			return (int)jObj.i;
		}
		return fallback;
	}
	public bool GetBool(string uri, bool fallback)
	{
		JSONObject jObj=GetJsonObj(uri);
		if(jObj!=null && jObj.IsBool)
		{
			return jObj.b;
		}
		return fallback;
	}
	
	void Init()
	{
        LoadConfig();
    }

    public bool LoadConfig()
    {
        jsonStr = ReadJsonFile(jsonPath);
        if (jsonStr == null) return false;
        try
        {
            jsonRoot = new JSONObject(jsonStr);
            return true;
        }
        catch
        {
            jsonStr = null;
            return false;
        }
    }

    void OnEnable()
	{
		if(_instance==null)
		{
			_instance = this;
			if(!Initialized)
			{
				Init();
			}
		}
		else if(_instance!=this)
		{
			Debug.LogWarning("Try to initialize another instance for singleton class Config!");
		}
	}

	void OnDestroy()
	{
		if(_instance==this)
		{
			_instance = null;
		}
	}
}
