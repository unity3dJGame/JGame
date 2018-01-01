using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SelectRole : MonoBehaviour {
	public GameObject TargetObject;
	public float MoveSpeed = 1.0f;
	public List<GameObject> CanMoveRoles;

	private struct RoleInfo
	{
		public Vector3 originalPos;
		public Quaternion originalRotation;
		public Vector3 targetPos;
	}
	private Dictionary<GameObject, RoleInfo> MovingRoleInfo = new Dictionary<GameObject, RoleInfo>();

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () 
	{

		bool bIsMoving = false;
		foreach (var obj in MovingRoleInfo) {
			if (obj.Key.transform.position != obj.Value.targetPos && obj.Key.transform.position != obj.Value.originalPos) {
				bIsMoving = true;
				break;
			}
		}

		if (!bIsMoving && Input.GetMouseButtonDown (0))
		{    //首先判断是否点击了鼠标左键
			GameObject selectedObj = null;
			Ray ray=Camera.main.ScreenPointToRay (Input.mousePosition);    //定义一条射线，这条射线从摄像机屏幕射向鼠标所在位置
			RaycastHit hit;    //声明一个碰撞的点(暂且理解为碰撞的交点)
			if (Physics.Raycast (ray, out hit)) {    //如果真的发生了碰撞，ray这条射线在hit点与别的物体碰撞了
				if (CanMoveRoles.Contains (hit.collider.gameObject)) {    //如果碰撞的点所在的物体的名字是“StartButton”(collider就是检测碰撞所需的碰撞器)
					selectedObj = hit.collider.gameObject;
				}
			}
				
			if (selectedObj != null) {
				foreach (var role in MovingRoleInfo)
				{
					if (role.Key != selectedObj && role.Value.targetPos == role.Key.gameObject.transform.position) {
						AnimationUtil.LookAt (role.Key, role.Value.originalPos);
						AnimationUtil.SwitchAnimation (role.Key, "walk", MoveSpeed);
						MoveUtil.MoveToPos (
							role.Key,
							role.Value.originalPos, MoveSpeed,
							MoveUtil.MoveType.UniformStraight);
					}
				}

				if ( !MovingRoleInfo.ContainsKey (selectedObj) || selectedObj.transform.position == MovingRoleInfo [selectedObj].originalPos) {
					RoleInfo info;
					info.originalPos = selectedObj.transform.position;
					info.originalRotation = selectedObj.transform.rotation;

					AnimationUtil.LookAt (selectedObj, TargetObject);
					AnimationUtil.SwitchAnimation (selectedObj, "walk", MoveSpeed);
					MoveUtil.MoveToObj (
						selectedObj,
						TargetObject, MoveSpeed,
						MoveUtil.MoveType.UniformStraight);
					info.targetPos = TargetObject.transform.position;
					if (MovingRoleInfo.ContainsKey (selectedObj)) {	
						MovingRoleInfo [selectedObj] = info;
					}
					else
						MovingRoleInfo.Add (selectedObj, info);
					
					CreatePlayerUtil.Role = selectedObj;
					foreach (var ro in CreatePlayerUtil.RoleModels)
					{
						if (ro.Value == selectedObj) 
						{
							CreatePlayerUtil.type = ro.Key;
							break;
						}
					}
				}
			}
		}

		foreach (var obj in MovingRoleInfo) 
		{
			if (obj.Key.transform.position == obj.Value.targetPos) {
				AnimationUtil.SwitchAnimation (obj.Key, "idle", -1.0f);
				AnimationUtil.LookAt (obj.Key, Camera.main.gameObject);
			} else if (obj.Key.transform.position == obj.Value.originalPos) {
				AnimationUtil.SwitchAnimation (obj.Key, "idle", -1.0f);
				obj.Key.transform.rotation = obj.Value.originalRotation;
			}
		}
	}
		
}