// version: unity3d 5.2.3

using UnityEngine;
using System.Collections;

public class ColorCreater : MonoBehaviour {
	[Range(0,1)]public float intensity;
	[Range(0,1)]public float saturate;
	[Range(0,360)]public int angle;
	public Material mat;
	// public int step =5;
	// int flag = 1;
	// int index =0;
	// int[] color = new int[3]{255,0,0};
	// Color32 color32 = new Color32(255,0,0,255);

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// if(Input.GetButton("Jump") && index<3)
		// {
		// 	if(flag ==1 && (color[(index+1)%3] +=step)>255)
		// 	{
		// 		flag = -1;
		// 	}
		// 	if(flag ==-1 && (color[index%3] -=step) <0)
		// 	{
		// 		flag = 1;
		// 		++index;
		// 	}
		// 	index %=3;
		// }
		// color32.r = (byte)Mathf.Clamp(color[0],0,255);
		// color32.g = (byte)Mathf.Clamp(color[1],0,255);
		// color32.b = (byte)Mathf.Clamp(color[2],0,255);
		// mat.color = color32;

		mat.color = GetColor(angle,intensity,saturate);
	}

	Color32 GetColor(float angle,float intensity,float saturate)
	{
		angle = (angle%360+360)%360;	// 将参数限制到0~360
		intensity = Mathf.Clamp01(intensity);
		saturate = Mathf.Clamp01(saturate);
		int ceil = (int)(intensity*255);
		int floor = (int)((1-saturate)*ceil);
		int seq = (int)(angle/360*1536);
		int cycle = seq/256;
		int diffValue = seq%256;
		int[] clr = new int[3];
		int index = cycle/2;
		int flag = cycle%2;
		if(flag ==0)
		{
			clr[index%3] = ceil;
			clr[(index+1)%3] = (int)(diffValue/256.0f *(ceil-floor) + floor);
			clr[(index+2)%3] = floor;
		}
		if(flag ==1)
		{
			clr[(index+1)%3] = ceil;
			clr[index%3] = (int)((255-diffValue)/256.0f *(ceil-floor) + floor);
			clr[(index+2)%3] = floor;
		}
		return new Color32((byte)clr[0],(byte)clr[1],(byte)clr[2],255);
	}
}
