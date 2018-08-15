using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonGroup : MonoBehaviour {

	public class ResponseUnit
	{
		private ButtonGroup btnGroup;
		public Button btn;
		public ResponseUnit(ButtonGroup btnGroup,Button btn)
		{
			this.btnGroup = btnGroup;
			this.btn = btn;
		}
		public void OnClickBtn()
		{
			btnGroup.OnAnyone(btn);
		}
	}

	public Transform[] msgReceivers = null;
	private Button[] btns = null;
	private ResponseUnit[] responses = null;

	// Use this for initialization
	void Start () {
		btns = transform.GetComponentsInChildren<Button>();
		responses = new ResponseUnit[btns.Length];
		for(int i=0;i<responses.Length;++i)
		{
			responses[i] = new ResponseUnit(this,btns[i]);
			btns[i].onClick.AddListener(responses[i].OnClickBtn);
		}
	}

	public void OnAnyone(Button btn)
	{
		for(int i=0;i<msgReceivers.Length;++i)
		{
			msgReceivers[i].SendMessage("OnGroupButton",btn,SendMessageOptions.DontRequireReceiver);
		}
	}
}
