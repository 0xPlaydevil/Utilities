using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;


public class Render2Tex : MonoBehaviour {
	static RenderTexture tex = null;
	static int width = 6000;
	static int height = 4000;

	[MenuItem("Tools/Render2Tex")]
	public static void RenderAndSave () {
		tex = new RenderTexture(width,height,16);
		Camera cam = Selection.activeTransform.GetComponent<Camera>();
		cam.targetTexture = tex;
		cam.Render();
		Texture2D tex2D = new Texture2D(width,height);
		RenderTexture.active = tex;
		tex2D.ReadPixels(new Rect(0,0,width,height),0,0);
		byte[] data = tex2D.EncodeToPNG();
		File.WriteAllBytes("D:/render.png",data);
	}
	
}
