using System.Collections;
using JGame.Data;
using JGame.Logic;
using JGame.StreamObject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitialRole : MonoBehaviour
{
	
	// Use this for initialization
	void Start ()
	{
		CreatePlayerUtil.RoleModels.Add(1,  GameObject.Find ("gs"));
		CreatePlayerUtil.RoleModels.Add(2,  GameObject.Find ("ms"));
		CreatePlayerUtil.RoleModels.Add(3,  GameObject.Find ("fs"));
		CreatePlayerUtil.RoleModels.Add(4,  GameObject.Find ("zs1"));
		CreatePlayerUtil.RoleModels.Add(5,  GameObject.Find ("zs2"));
		CreatePlayerUtil.RoleModels.Add(6,  GameObject.Find ("zs3"));

		IStreamObj obj = JLogicUserData.getLocalData ().getData (JObjectType.sign_in_ret);
		if (null == obj)
			return;
		JObj_SignRet retObj = obj as JObj_SignRet;
		if (null == retObj)
			return;
		foreach (var role in retObj.RolesInfo)
		{
			initial (role);
		}
	}

	private void initial(JObjRoleInfo role)
	{
		if(CreatePlayerUtil.RoleModels.ContainsKey( role.roleType))
		{
			GameObject obj =  CreatePlayerUtil.RoleModels [role.roleType];
			if (null == obj)
				return;
			/*Vector2 nameSize = GUI.skin.label.CalcSize(new GUIContent(heroInfoText.text));  
			Vector3 worldPos = obj.transform.position;
			worldPos.z = worldPos.z + 1.0f;
			Vector2 pos = Camera.main.WorldToScreenPoint(worldPos);
			GUI.Label(new Rect(pos.x - nameSize.x / 2, pos.y, nameSize.x, nameSize.y), heroInfoText.text);  */
			GameObject textObj = new GameObject ();
			textObj.transform.parent = obj.transform;
			textObj.transform.position = new Vector3 (obj.transform.position.x-0.3f, obj.transform.position.y + 1.9f, obj.transform.position.z);

			TextMesh textMesh = textObj.AddComponent<TextMesh> ();
			textMesh.color = new Color (0, 255, 0); 
			textMesh.characterSize = 0.1f; 
			textMesh.text = "lv."+ role.roleLevel.ToString() +  " " + role.roleName;
		}
	}

	// Update is called once per frame
	void Update ()
	{
	
	}
}

