using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayDev
{
	public class VisionCtrl : MonoBehaviour {
		public Transform[] transList;

		private VisionAttr[] visionAttrs;

		// Use this for initialization
		void Start () {
			visionAttrs = new VisionAttr[transList.Length];
			for(int i=0;i<transList.Length;++i)
			{
				visionAttrs[i] = transList[i].GetComponent<VisionAttr>();
				if(visionAttrs[i] == null)	Debug.LogError(string.Format("VisionAttr NOT found in {0}",transList[i].name));
			}
		}
		
		public void SetBodyVision(float percent)
		{
			int cnt = transList.Length;
			int sum = (99+1)*cnt-1;
			int index;
			int value;
			index = (int)(percent*sum)/(99+1);
			value = (int)(percent*sum)%(99+1);
			for(int i=0;i<cnt;++i)
			{
				int curValue=0;
				if(i<index) curValue = 99;
				if(i==index) curValue = value;
				if(i>index) curValue = 0;
				// SetAlpha(transList[i],(float)curValue/99);
				visionAttrs[i].TreeOpacity = (float)curValue/99;
			}
		}

		static public void SetAlpha(Transform objTrans,float destAlpha,bool containChildren=true)
		{
			MeshRenderer[] renderers = null;
			if(containChildren)
			{
				renderers = objTrans.GetComponentsInChildren<MeshRenderer>();
			}
			else
			{
				MeshRenderer temp = objTrans.GetComponent<MeshRenderer>();
				if(temp)
				{
					renderers = new MeshRenderer[1]{temp};
				}
			}

			if(renderers!=null && renderers.Length>0)
			{
				for(int i = 0;i<renderers.Length;++i)
				{
					Material[] mats = renderers[i].materials;
					for(int j=0;j<mats.Length;++j)
					{
						// MatOperate.SetMaterialRenderingMode(mats[j],RenderingMode.Fade);
						Color c = mats[j].color;
						c.a = destAlpha;
						mats[j].color = c;
					}
					renderers[i].enabled = destAlpha >0.01f;		
				}
			}
		}

	}
}