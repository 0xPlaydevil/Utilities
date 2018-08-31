using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// 遇到的第一个问题：删除后会影响遍历
// 第二个问题：延迟删除会影响判定条件
public class EditorTools {
	// static Queue<GameObject> queue = new Queue<GameObject>();

	[MenuItem("Tools/TrimEmpty")]
	public static void TrimTree()
	{
		TrimEmptyRecurse(Selection.activeTransform);
		// while(queue.Count>0)
		// {
		// 	GameObject.DestroyImmediate(queue.Dequeue());
		// }
	}

	public static bool TrimEmptyRecurse(Transform trans)
	{
		// for(int i=0;i<trans.childCount;++i)
		// {
		// 	TrimEmptyRecurse(trans.GetChild(i));
		// }
		int i = 0;
		while(i<trans.childCount)	// i加count减，超过则终止
		{
			if(!TrimEmptyRecurse(trans.GetChild(i)))
			{
				++i;
			}
		}
		if(IsGameObjEmpty(trans) && trans.childCount<2)
		{
			if(trans.childCount>0)
			{
				trans.GetChild(0).parent = trans.parent;
			}
			// queue.Enqueue(trans.gameObject);
			GameObject.DestroyImmediate(trans.gameObject);
			return true;
		}
		return false;
	}

	public static bool IsGameObjEmpty(Transform trans)
	{
		return trans.GetComponents<Component>().Length ==1;
	}
}

