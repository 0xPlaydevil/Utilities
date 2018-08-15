using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class Util {
	static int folderCnt = 0,metaCnt = 0;

	[MenuItem("Tools/CleanEmptyDirectories")]
	public static void CleanDir()
	{
		// Debug.Log(Application.dataPath);
		folderCnt = 0;metaCnt = 0;
		DelEmptyDirRecursively(new DirectoryInfo(Application.dataPath));
		AssetDatabase.Refresh();
		Debug.LogFormat("Delete {0} folders and {1} metas.",folderCnt,metaCnt);
	}

	private static void DelEmptyDirRecursively(DirectoryInfo dir)
	{
		DirectoryInfo[] subDirs = dir.GetDirectories();
		foreach(DirectoryInfo subDir in subDirs)
		{
			DelEmptyDirRecursively(subDir);
		}

		if(dir.GetFileSystemInfos().Length ==0)
		{
			string metaPath = dir.FullName + ".meta";
			if(File.Exists(metaPath))
			{
				File.Delete(metaPath);
				metaCnt++;
				Debug.Log("deleta meta file: " + metaPath);
			}
			dir.Delete();
			folderCnt++;
			Debug.Log("delete folder: " + dir.FullName);
		}
	}
}
