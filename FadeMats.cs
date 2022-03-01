using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayDev
{
	public class FadeMats : MonoBehaviour {
		public Material[] mats = null;
		public bool[] enableShowFlags = null;
		public bool[] showFlags = null;
		private float[] opaques = null;
		private float[] origAlphas = null;
		private float[] origMetallics = null;

		public float this[int index]
		{
			get
			{
				return opaques[index];
			}
			set
			{
				SetOpaque(index,value);
			}
		}

		// Use this for initialization
		void Start () {
			enableShowFlags = new bool[mats.Length];
			showFlags = new bool[mats.Length];
			opaques = new float[mats.Length];
			origAlphas = new float[mats.Length];
			origMetallics = new float[mats.Length];
			for(int i=0;i<mats.Length;++i)
			{
				enableShowFlags[i] = true;
				showFlags[i] = true;
				opaques[i] = 1;
				origAlphas[i] = mats[i].GetColor("_Color").a;
				origMetallics[i] = mats[i].GetFloat("_Metallic");
			}
		}
		
		// Update is called once per frame
		void Update () {
			float lerpV = 0;
			for(int i=0;i<mats.Length;++i)
			{
				if(enableShowFlags[i])
				{
					lerpV = Mathf.Lerp(opaques[i],showFlags[i]? 1:0, 0.2f);
					SetOpaque(i,lerpV);
				}
			}
		}

		public void SetOpaque(int i,float opaque)
		{
			opaques[i] = opaque;
			Color color = mats[i].GetColor("_Color");
			if(Mathf.Abs(color.a/origAlphas[i] -opaque) < 0.002f)
			{
				return;
			}
			opaque = Mathf.Clamp01(opaque);

			mats[i].SetFloat("_Metallic",opaque*origMetallics[i]);
			color.a = origAlphas[i]*opaque;
			mats[i].SetColor("_Color",color);
		}

		public int GetMatIndex(Material mat)
		{
			for(int i=0;i<mats.Length;++i)
			{
				if(mat == mats[i])
				{
					return i;
				}
			}
			return -1;
		}
	}
}