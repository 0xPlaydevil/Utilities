using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

namespace Spoon
{
	public class DisplayLog : Singleton<DisplayLog> {
		public int maxLogLength = 8000;		// 在hololens下建议不超过10K，30K以上时高频Log会明显卡顿（近乎空场景下测试）
		[HideInInspector]public int keptLogLength = 800;
		[SerializeField]private Text debug;

		private Transform holder = null;
		private Transform origParent = null;
		private Vector3 origLocalPos;
		private Quaternion origLocalRot;
		private readonly StringBuilder bgString = new StringBuilder();
		private bool bgStrChanged = false;
		private int fixedCount = 0;

		// Use this for initialization
		void Awake () {
			base.Awake();
		    Application.logMessageReceived += HandleLog;
		}

		void Start()
		{
			Initialize();
			Debug.Log("Debug:");
		}

		void FixedUpdate()
		{
			fixedCount++;
			if(bgStrChanged && fixedCount%3 == 0)
			{
				if(bgString.Length>maxLogLength)
				{
					bgString.Remove(0,bgString.Length-keptLogLength);
				}
				debug.text = bgString.ToString();
				bgStrChanged = false;
			}
		}

		void HandleLog (string LogString, string stackTrace, LogType logType) {
			string formatStr = "<color=red>{0}</color>{1}";
			string trace = "";
			switch(logType)
			{
				case LogType.Log:
					formatStr = "{0}";
					break;
				case LogType.Warning:
					formatStr = "<color=yellow>{0}</color>{1}";
					break;
				case LogType.Exception:
					trace = "\nstackTrace:" + stackTrace;
					break;
			}
			string msg = string.Format(formatStr,LogString,trace);
			Append(msg);
		}

		public void Append(string text)
		{
			bgString.AppendLine(text);
			bgStrChanged = true;
		}

		void Initialize()
		{
			holder = debug.GetComponentInParent<Canvas>().transform;
			origParent = holder.parent;
			origLocalPos = holder.localPosition;
			origLocalRot = holder.localRotation;
		}

		public void Detach()
		{
			if(holder != null)
			{
				Vector3 curPos = holder.position;
				holder.SetParent(null);
				holder.position = curPos;
			}
		}

		public void Attach()
		{
			if(holder != null && holder.parent !=origParent)
			{
				holder.SetParent(origParent);
				holder.localPosition = origLocalPos;
				holder.localRotation = origLocalRot;
			}
		}
	}
}