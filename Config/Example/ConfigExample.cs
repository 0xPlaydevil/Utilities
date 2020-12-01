using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigExample : MonoBehaviour {
	// Use this for initialization
	void Awake () {
		// 配置视频地址
		JSONObject videoAddr = Config.instance.GetJsonObj("VideoAddr");
		if(videoAddr!= null)
		{
			string text = videoAddr.str;
			print(text);
		}
	}

	void Start()
	{
		JSONObject robotShow = Config.instance.GetJsonObj("RobotShow");
		if(robotShow!=null && robotShow.b)
		{
			print("RobotShow exist.");
		}
	}

}
