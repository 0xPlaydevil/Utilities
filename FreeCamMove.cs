// 移动分为基于地形和不基于地形两种，旋转分为绕中心旋转和自旋转
// 基于地形的移动可以同时接受鼠标拖动和触发移动。非基于地形的移动以射线打到的物体为准拖动
// 移动中碰到面不能卡住不动，而是沿面滑动。
// 兼容多种输入方式包括鼠标键盘等
// 最好有限制范围的功能

using UnityEngine;
using System.Collections;
using Spoon;

public class FreeCamMove : MonoBehaviour
{
	[Tooltip(" 可以留空 ")]public Texture2D cursorHand;
	public Vector2 hotspot = Vector2.zero;

	public GameObject centerObj = null;	// 绕物体旋转模式。如果有值，将不能自由移动摄像机
	[Space]
	public float m_HRotateSpeed = 10;	// 旋转的速度 水平
	public float m_VRotateSpeed = 10;	// 旋转的速度 竖直
	public float m_minHeight =1;
	[Tooltip(" 影响水平移动的速度,最佳值为0-1000")]
	public float m_moveFactor = 100;
	// 脚本所用组件
	private Transform m_camTrans;
	private Camera m_thisCam;
	private GameObject m_gyroscope;		// 与摄像机位置一致，但欧拉角只有y与摄像机一致，xz平面保持水平
	private Transform m_gyroTrans;
	// 移动参数
	private float m_moveSpeed;
	private float m_horizSpeed;
	// 打高射线部件
	private const float m_hypHeight = 50000;
	private RaycastHit m_hypsometerHit;	// 保存测高信息
	[Tooltip(" Raycast位掩码，决定可以作高程测量的Layer，非常重要！ ")]
	public int m_heightBaseLayer =1;		// 位掩码，用于Physics.Raycast()
		// 测高仪，从高空往下打，测最上面一个碰撞体的高度(经过layerMask过滤的，防止一些 非高程基体 干扰)
	private Ray Hypsometer
	{
		get
		{
			Vector3 origin = m_camTrans.position;
			origin.y = m_hypHeight;
			return new Ray(origin,Vector3.down);
		}
	}
	// 打中心射线部件
	private RaycastHit m_camZHit;			// 延摄像机视轴打射线的信息
	[Tooltip(" Raycast位掩码，决定可以作旋转中心的Layer，非常重要！ ")]
	public int m_centerBaseLayer =1;			// 位掩码，限制打到的中心点
	// 旋转参数
	private float m_camZDist;	// 相机距视点中心距离
	private Vector3 m_rotateCenter;		// 旋转围绕的中心
	private bool m_isFocusing;		// 正在绕中心点旋转
	private bool m_focusCenter = true;			// 是否绕中心旋转
	// 输入轴
	private Vector3 m_gyroMove3D;		// 以陀螺坐标系移动
	private float m_localMoveZ;			// 沿摄像机z轴的运动
	private float m_localRotateX;
	private float m_localRotateY;
	// MouseDrag参数
	private Vector3 m_prePos;
	private Vector3 m_curMousePos;
	private float m_planeHeight;
	private bool m_isDragging;

	void Start()
	{
		m_gyroscope = new GameObject("gyroscope");
		m_gyroTrans = m_gyroscope.transform;
		m_camTrans = transform;
		m_thisCam = m_camTrans.GetComponent<Camera>();
		// 如果本物体上没有Camera，在子对象中找MainCamera; 如果也没有，找Depth最大的。
		if(!m_thisCam)
		{
			var cams= m_camTrans.GetComponentsInChildren<Camera>();
			float maxDepth=float.MinValue;
			int top=0;
			int i;
			for(i=0;i<cams.Length;++i)
			{
				if(cams[i].CompareTag("MainCamera"))
				{
					m_thisCam=cams[i];
					break;
				}
				if(cams[i].depth> maxDepth)
				{
					top=i;
					maxDepth=cams[i].depth;
				}
			}
			m_thisCam=cams[top];
			print("Camera of FreeCamMove: "+m_thisCam.transform.GetFullPath("/"));
		}
		Debug.Assert(m_thisCam, "GameObject层级中未找到Camera", this);
		// if(!m_thisCam)
		// {
		// 	Debug.LogError(" 此脚本需挂在摄像机上！ ");		// 这里应该改进
		// }
	}

	void LateUpdate()
	{
		if(!centerObj)
		{
			ApplyMouseDrag();
			ApplyMove();
		}
		ApplyRotate();
		ApplyStretch();
	}

	void TestPointCalc()
	{
		if(Input.GetMouseButtonDown(0)
			&& Physics.Raycast(m_thisCam.ScreenPointToRay(Input.mousePosition),out m_camZHit))
		{
			Ray ray = m_thisCam.ScreenPointToRay(Input.mousePosition);
			float angle = Vector3.Angle(ray.direction,m_camTrans.forward);

			print("hit point:" + m_camZHit.point);
			Vector3 mousePos = Input.mousePosition;
			mousePos.z = m_camZHit.distance*Mathf.Cos(angle*Mathf.Deg2Rad);
			print("calc point:" + m_thisCam.ScreenToWorldPoint(mousePos));
		}
	}

	void ApplyMouseDrag()
	{
		if(Input.GetMouseButtonDown(0)
			&& Physics.Raycast(m_thisCam.ScreenPointToRay(Input.mousePosition),out m_camZHit,Mathf.Infinity,m_centerBaseLayer))
		{
			m_prePos = m_camZHit.point;
			m_planeHeight = m_camZHit.point.y;
			m_isDragging = true;
			if(cursorHand)
			{
				Cursor.SetCursor(cursorHand,hotspot,CursorMode.Auto);
			}
		}
		if(m_isDragging)
		{
			// 计算拖动
			Ray ray = m_thisCam.ScreenPointToRay(Input.mousePosition);
			float plumbAngle = Vector3.Angle(ray.direction,Vector3.down);
			float cosAngle = Mathf.Cos(plumbAngle*Mathf.Deg2Rad);
			if(cosAngle !=0)
			{
				float dist = (m_camTrans.position.y-m_planeHeight)/cosAngle;
				float offsetAngle = Vector3.Angle(ray.direction,m_camTrans.forward);
				float zValue = dist*Mathf.Cos(offsetAngle*Mathf.Deg2Rad);
				m_curMousePos = Input.mousePosition;
				m_curMousePos.z = zValue;
				if(Mathf.Abs(cosAngle)<0.1)
				{
					// print(cosAngle);
					// print("zValue:" + zValue);
					// Vector3 delta = m_thisCam.ScreenToWorldPoint(m_curMousePos)-m_prePos;
					// if(delta.z>0)
					// {
					// 	delta.z=0;
					// }
					// m_camTrans.position -= delta;
				}
				else
				{
					m_camTrans.position -= (m_thisCam.ScreenToWorldPoint(m_curMousePos)-m_prePos);
				}
				m_prePos = m_thisCam.ScreenToWorldPoint(m_curMousePos);
				GroundedAdjust();
			}
		}
		if(Input.GetMouseButtonUp(0))
		{
			m_isDragging = false;
			Cursor.SetCursor(null,Vector2.zero,CursorMode.Auto);
			SyncGyro();		// 拖动过程中，其它行为都不响应，无需同步Gyro,拖动结束同步一次即可
		}
	}

	void ApplyMove()
	{
		if(Input.GetMouseButtonDown(0))
		{
			MovePrepare();
			m_horizSpeed = m_moveSpeed*m_moveFactor/Screen.width;
			if(cursorHand)
			{
				Cursor.SetCursor(cursorHand,hotspot,CursorMode.Auto);
			}
		}
		if(Input.GetMouseButton(0))
		{
			m_gyroMove3D.x = -Input.GetAxis("Mouse X")*m_horizSpeed;
			m_gyroMove3D.z = -Input.GetAxis("Mouse Y")*m_horizSpeed;
		}
		else
		{
			m_gyroMove3D /= 1.05f;
		}
		if(Input.GetMouseButtonUp(0))
		{
			Cursor.SetCursor(null,hotspot,CursorMode.Auto);
		}

		// 应用移动
		if(!m_isDragging)
		{
			m_camTrans.Translate(m_gyroTrans.TransformDirection(m_gyroMove3D),Space.World);
			GroundedAdjust();
			SyncGyro();
		}
	}

	void ApplyStretch()
	{
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if(axis != 0)
		{
			MovePrepare();
			m_localMoveZ = axis*m_moveSpeed;
			m_camTrans.Translate(0,0,m_localMoveZ);
			GroundedAdjust();
			SyncGyro();
		}
	}

	// 非法状态检测并调整，返回值表示是否非法
	bool GroundedAdjust()
	{
		// 摄像机触地调整
		// 简单上移至就近合法位置
		if(Physics.Raycast(Hypsometer,out m_hypsometerHit,Mathf.Infinity,m_heightBaseLayer))
		{
			if(m_camTrans.position.y-m_hypsometerHit.point.y<m_minHeight)
			{
				m_camTrans.position = m_hypsometerHit.point + new Vector3(0,m_minHeight+0.1f,0);
				return true;
			}
		}
		return false;
	}

	void ApplyRotate()
	{
		m_focusCenter = centerObj || !Input.GetKey(KeyCode.LeftAlt);
		if(!m_focusCenter)	m_isFocusing = false;
		// 应用旋转
		if(Input.GetMouseButton(1))
		{
			m_localRotateX = -Input.GetAxis("Mouse Y")*m_HRotateSpeed;
			m_localRotateY = Input.GetAxis("Mouse X")*m_VRotateSpeed;
			m_camTrans.Rotate(Vector3.right,m_localRotateX);
			m_camTrans.Rotate(Vector3.up,m_localRotateY,Space.World);
		}

		if(m_focusCenter)
		{
			if(centerObj)
			{
				m_rotateCenter = centerObj.transform.position;
				m_camZDist = Vector3.Distance(centerObj.transform.position,m_camTrans.position);
				m_isFocusing = true;
			}
			else if(Input.GetMouseButtonDown(1)
				&& Physics.Raycast(m_camTrans.position,m_camTrans.forward,out m_camZHit,Mathf.Infinity,m_centerBaseLayer))
			{
				m_rotateCenter = m_camZHit.point;
				m_camZDist = m_camZHit.distance;
				m_isFocusing = true;
			}
			if(m_isFocusing)
			{
				// 绕点旋转
				Vector3 differV = new Vector3(0,0,-1)*m_camZDist;
				differV = m_camTrans.rotation * differV;
				m_camTrans.position = m_rotateCenter + differV;
				// 非法状态调整
				int count =0;
				while(GroundedAdjust())		// 这里，用while代替if，以防调整后的位置非法。但通常只发生一次，多次的情况极少
				{
					m_camTrans.LookAt(m_rotateCenter);		// 这里有问题，当摄像机“头”朝地时，此句会把头调为朝上，发生一次突变。考虑到情况不多，暂不修改
					differV = m_camTrans.position - m_rotateCenter;
					differV = differV.normalized*m_camZDist;
					m_camTrans.position = m_rotateCenter + differV;
					++count;
					if(count>10)
					{
						Debug.LogWarning(" GroundedAdjust is too much! ");
						m_isFocusing = false;		// 防止死循环卡死
						break;
					}
				}
			}
			if(!centerObj && Input.GetMouseButtonUp(1))
			{
				m_isFocusing = false;
			}
		}
	}

	void GetInput()
	{
		// 取得输入
		if(Input.GetMouseButton(1))
		{
			m_localRotateX = -Input.GetAxis("Mouse Y")*m_HRotateSpeed;
			m_localRotateY = Input.GetAxis("Mouse X")*m_VRotateSpeed;
		}
		else
		{
			m_localRotateX = 0;
			m_localRotateY = 0;
		}
		if(Input.GetMouseButton(0))
		{
			m_gyroMove3D.x = -Input.GetAxis("Mouse X")*100;
			m_gyroMove3D.z = -Input.GetAxis("Mouse Y")*100;
		}
		else
		{
			m_gyroMove3D /= 1.05f;
		}
		m_localMoveZ = Input.GetAxis("Mouse ScrollWheel")*100;
	}

	void SyncGyro()
	{
		m_gyroTrans.position = m_camTrans.position;
		m_gyroTrans.rotation = Quaternion.Euler(0,m_camTrans.eulerAngles.y,0);
	}

	void MovePrepare()
	{
		if(Physics.Raycast(m_camTrans.position,m_camTrans.forward,out m_camZHit,Mathf.Infinity,m_centerBaseLayer))
		{
			m_moveSpeed = Mathf.Abs(m_camTrans.position.y - m_camZHit.point.y);
		}
		else if(Physics.Raycast(Hypsometer,out m_hypsometerHit,Mathf.Infinity,m_heightBaseLayer))
		{
			m_moveSpeed = Mathf.Abs(m_camTrans.position.y - m_hypsometerHit.point.y);
		}
		else
		{
			// 保持先前的合法速度
		}
	}

}