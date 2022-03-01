using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayDev
{
	public class VisionAttr : MonoBehaviour {
		private float treeOpacity = 1;
		MeshRenderer[] renderers;

		public float TreeOpacity
		{
			get{return treeOpacity;}
			set
			{
				if(value<0 || value>1)	Debug.LogWarning("treeOpacity should between 0 and 1!");
				value = Mathf.Clamp01(value);
				if(treeOpacity != value)
				{
					treeOpacity = value;
					for(int i = 0;i<renderers.Length;++i)
					{
						renderers[i].enabled = treeOpacity !=0;
						Material[] mats = renderers[i].materials;
						for(int j=0;j<mats.Length;++j)
						{
							// MatOperate.SetMaterialRenderingMode(mats[j],RenderingMode.Fade);
							Color c = mats[j].color;
							c.a = treeOpacity;
							mats[j].color = c;
						}
					}		
				}
			}
		}
		// Use this for initialization
		void Start () {
			renderers = transform.GetComponentsInChildren<MeshRenderer>();
			if(renderers.Length<1)
			{
				Debug.LogWarning("MeshRenderer NOT found in object tree!");
			}
		}
		
	}
}