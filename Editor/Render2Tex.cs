using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;


public class Render2Tex {

	[MenuItem("Tools/RenderAgain")]
	public static void RenderAgain () {
		PicFileParam pic = RenderDialog.lastPic;
		pic.SetFileName();
		Render2File(pic);
	}

	[MenuItem("Tools/Render...")]
	public static void Render()
	{
		RenderDialog.Init();
	}

	public static void Render2File(PicFileParam pic)
	{
		RenderTexture tex = new RenderTexture(pic.width,pic.height,16);
		pic.camera.targetTexture = tex;
		pic.camera.Render();
		pic.camera.targetTexture = null;
		RenderTexture.active = tex;
		Texture2D tex2D = new Texture2D(pic.width,pic.height);
		tex2D.ReadPixels(new Rect(0,0,pic.width,pic.height),0,0);
		byte[] data = pic.isPng? tex2D.EncodeToPNG(): tex2D.EncodeToJPG();
		File.WriteAllBytes(pic.filePath,data);
	}
}

public class RenderDialog: EditorWindow
{
	public static PicFileParam lastPic;
	PicFileParam m_pic;
	string m_fileDefaultName = "";
	string[] toggleNames = new string[]{"png","jpg"};
	int curIdx = 0;

	public static RenderDialog Init()
	{
		RenderDialog dialog = EditorWindow.GetWindow<RenderDialog>();
		if(!lastPic.initialized)
		{
			lastPic.Init();
		}
		dialog.m_pic = lastPic;
		dialog.m_fileDefaultName = DateTime.Now.ToString("yyMMddHHmmss_fff");
		dialog.curIdx = lastPic.isPng? 0: 1;
		dialog.m_pic.filePath = Path.Combine(dialog.m_pic.directory,dialog.m_fileDefaultName)+(dialog.m_pic.isPng? ".png": ".jpg");
		dialog.Show();
		return dialog;
	}

	void OnGUI()
	{
		m_pic.width = EditorGUILayout.IntField("width",m_pic.width);
		m_pic.height = EditorGUILayout.IntField("height",m_pic.height);
		m_pic.camera = EditorGUILayout.ObjectField("Camera:",m_pic.camera,typeof(Camera),true) as Camera;

		// This is an implement of single selected toggle group, by Rick Liu
		for(int i=0;i<toggleNames.Length;++i)
		{
			if(EditorGUILayout.ToggleLeft(toggleNames[i],i==curIdx? true: false) && i!= curIdx)
			{
				curIdx = i;
			}
		}
		m_pic.isPng = curIdx==0;

		EditorGUILayout.BeginHorizontal();
		m_pic.filePath = EditorGUILayout.TextField("FilePath:",m_pic.filePath);
		if(GUILayout.Button("Browser",GUILayout.ExpandWidth(false)))
		{
			string path = EditorUtility.SaveFilePanel("save file",m_pic.directory,m_fileDefaultName,m_pic.isPng? "png": "jpg");
			if(path != "")
			{
				m_pic.filePath = path;
			}
		}
		EditorGUILayout.EndHorizontal();

		Rect rect = this.position;
		rect.y = rect.height-30;
		rect.x = 0;
		rect.height = 30;
		GUILayout.BeginArea(rect);
		if(GUILayout.Button("Render"))
		{
			if(!m_pic.camera)
			{
				m_pic.camera = Camera.main;
				if(m_pic.camera)
				{
					Debug.LogWarning("No camera specified, use Camera.main as a default.");
				}
				else
				{
					Debug.LogError("No camera to use, set one before render");
					return;
				}
			}
			Render2Tex.Render2File(m_pic);
			m_pic.SetFileName();
			lastPic = m_pic;
			Debug.Log("Render Over");
		}
		GUILayout.EndArea();
	}

}

public struct PicFileParam
{
	public bool initialized;
	public int width;
	public int height;
	private bool _isPng;
	public bool isPng
	{
		get
		{
			return _isPng;
		}
		set
		{
			if(_isPng != value)
			{
				_isPng = value;
				ResetExt();
			}
		}
	}
	public string directory
	{
		get
		{
			return Path.GetDirectoryName(filePath);
		}
	}
	public string filePath;
	// string defaultName;
	public Camera camera;

	public PicFileParam(int w, int h, bool isPng, string filePath, Camera cam)
	{
		width = w;
		height = h;
		this._isPng = isPng;
		this.filePath = filePath;
		camera = cam;
		initialized = true;
	}

	public void Init()
	{
		width = 1920;
		height = 1080;
		_isPng = true;
		filePath = Path.Combine(Application.dataPath,"template.png");
		camera = Camera.main;
		initialized = true;
	}

	public void SetFileName(string name ="")
	{
		if(name =="")
		{
			name = DateTime.Now.ToString("yyMMddHHmmss_fff");
		}
		filePath = Path.Combine(Path.GetDirectoryName(filePath),name) + (_isPng? ".png": ".jpg");
	}

	public void ResetExt()
	{
		string ext = Path.GetExtension(filePath);
		if(ext!="")
		{
			filePath = filePath.Replace(ext,_isPng? ".png": ".jpg");
		}
		else
		{
			filePath += _isPng? ".png": ".jpg";
		}
	}
}
