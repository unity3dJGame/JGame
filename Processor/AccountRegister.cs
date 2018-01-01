using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using JGame.StreamObject;
using JGame;
using JGame.Network;
using JGame.Data;
using JGame.Logic;
using JGame.Processor;
using JGame.Log;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AccountRegister :  MonoBehaviour
{
	void Start()
	{
		JLogicHelper.registerProcessor (JPacketType.npt_accountRegister_req, new JProcessorAccountRegisterReq (), false);
		JProcessorAccountRegisterRet processor = new JProcessorAccountRegisterRet ();
		processor.registerRetGot += ShowRegisterRet;
		JLogicHelper.registerProcessor (JPacketType.npt_accountRegister_ret,  processor,false);
		JLogicHelper.registerProcessor (JPacketType.pt_createRole_req, new JProcessorCreateRole (), false);
	}

	// Use this for sign in
	public void ShowRegisterRet (JObjAccountRegisterRet.AccountRegisterResultType type) {
		JLog.Info ("Register result:" + type.ToString (), JLogCategory.Common);
		/*switch (type) {
		case JObjAccountRegisterRet.AccountRegisterResultType.failed:
			UnityEditor.EditorUtility.DisplayDialog("注册", "注册失败！", "确认");
			break;
		case JObjAccountRegisterRet.AccountRegisterResultType.accountNotAllowed:
			UnityEditor.EditorUtility.DisplayDialog("注册", "账号不合法！", "确认");
			break;
		case JObjAccountRegisterRet.AccountRegisterResultType.accountRepeated:
			UnityEditor.EditorUtility.DisplayDialog("注册", "账号已被占用！", "确认");
			break;
		case JObjAccountRegisterRet.AccountRegisterResultType.codeNotAllowed:
			UnityEditor.EditorUtility.DisplayDialog("注册", "密码不合法！", "确认");
			break;
		case JObjAccountRegisterRet.AccountRegisterResultType.codeIsTooSimple:
			UnityEditor.EditorUtility.DisplayDialog("注册", "密码设置太过简单！", "确认");
			break;	
		case JObjAccountRegisterRet.AccountRegisterResultType.emailIsRegistered:
			UnityEditor.EditorUtility.DisplayDialog("注册", "邮箱已被占用", "确认");
			break;
		case JObjAccountRegisterRet.AccountRegisterResultType.successed:
			UnityEditor.EditorUtility.DisplayDialog("注册", "注册成功！", "确认");
			break;
		}*/
	}
}

