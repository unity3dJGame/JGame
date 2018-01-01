using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using JGame.StreamObject;
using JGame;
using JGame.Network;
using JGame.Data;
using JGame.Log;
using JGame.Logic;
using JGame.Processor;

public class CreateRole : MonoBehaviour
{
	public Text RoleName;
	// Use this for initialization
	void Start ()
	{
		JLogicHelper.registerProcessor (JPacketType.pt_createRole_req,  new JProcessorCreateRole(), false);
		JProcessorCreateRoleRet processorCreateRoleRet = new JProcessorCreateRoleRet ();
		processorCreateRoleRet.createRole += AddRole;
		JLogicHelper.registerProcessor (JPacketType.pt_createRole_ret,  processorCreateRoleRet, false);
	}
	public void AddRole(string roleName, int roleType)
	{
		if(CreatePlayerUtil.RoleModels.ContainsKey( roleType))
		{
			GameObject obj =  CreatePlayerUtil.RoleModels [roleType];
			if (null == obj)
				return;

			GameObject textObj = new GameObject ();
			textObj.transform.parent = obj.transform;
			textObj.transform.position = new Vector3 (obj.transform.position.x-0.3f, obj.transform.position.y + 1.9f, obj.transform.position.z);

			TextMesh textMesh = textObj.AddComponent<TextMesh> ();
			textMesh.color = new Color (0, 255, 0); 
			textMesh.characterSize = 0.1f; 
			textMesh.text = "lv.1"+  " " + roleName;
		}
	}
	public void CreateRoleButtonClicked()
	{
		JCreateRoleReqObject obj = new JCreateRoleReqObject ();
		//ToDo:检测是否合法
		obj.RoleName = RoleName.text;
		obj.RoleType = CreatePlayerUtil.type;
		try {
			JNetworkDataOperator.SendDataToServer(JPacketType.pt_createRole_req, obj);
			return;
		} catch (Exception e) {
			Debug.LogError ("发送数据失败");
			JLog.Error (e.Message);
			return;
		}
	}

	// Update is called once per frame
	void Update ()
	{
	
	}
}

