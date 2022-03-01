// 该脚本挂在一个物体上，会记录这个物体所有子孙物体在Start()执行时的local坐标信息。
// RestoreTransform(Transform trans)用来恢复参数所指定的单个子孙对象的local坐标值。
// GetOrigTransInfo(Transform trans)返回参数所指定的物体的初始local坐标信息。

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PlayDev
{
	public class Original : MonoBehaviour {

		public class TransInfo
		{
			public Vector3 position;
			public Quaternion rotation;
			public Transform parentTrans;

			public TransInfo(Transform trans)
			{
				position = trans.localPosition;
				rotation = trans.localRotation;
				parentTrans = trans.parent;
			}

			public TransInfo()
			{
				position = Vector3.zero;
				rotation = Quaternion.identity;
				parentTrans = null;
			}

			public void ToTransform(Transform trans, bool restoreTreeRelation = true)
			{
				if(trans.parent != parentTrans && restoreTreeRelation)
				{
					trans.parent = parentTrans;
				}
				if(parentTrans == null)
				{
					trans.position = position;
					trans.rotation = rotation;
				}
				else
				{
					trans.localPosition = position;
					trans.localRotation = rotation;
				}
			}
		}

		private Dictionary<int,TransInfo> m_childInfos;

		// Use this for initialization
		void Start () {
			InitInfos();
		}
		
		private void InitInfos()
		{
			m_childInfos = new Dictionary<int,TransInfo>();
			RecordTree(transform);
		}

		private void RecordTree(Transform parent)
		{
			for(int i=0;i<parent.childCount;++i)
			{
				Transform trans = parent.GetChild(i);
				m_childInfos.Add(trans.GetInstanceID(),new TransInfo(trans));
				if(trans.childCount>0)
				{
					RecordTree(trans);
				}
			}
		}
		// 重设当前树的子节点信息，如果在Record后树结构发生过变化，无法恢复原树结构
		private void ResetTree(Transform nodeTrans)
		{
			for(int i=0;i<nodeTrans.childCount;++i)
			{
				Transform trans = nodeTrans.GetChild(i);
				m_childInfos[trans.GetInstanceID()].ToTransform(trans,false);
				if(trans.childCount>0)
				{
					ResetTree(trans);
				}
			}
		}

		public void ResetBody()
		{
			ResetTree(transform);
		}

		// return null if record NOT found
		public TransInfo GetOrigTransInfo(Transform trans)
		{
			try
			{
				return m_childInfos[trans.GetInstanceID()];
			}
			catch
			{
				return null;
			}
		}

		// Throw KeyNotFoundException if record NOT found
		public void RestoreTransform(Transform trans)
		{
			m_childInfos[trans.GetInstanceID()].ToTransform(trans);
		}

	}
}