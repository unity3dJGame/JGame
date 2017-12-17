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
	}

	// Use this for sign in
	public void ShowRegisterRet (JObjAccountRegisterRet.AccountRegisterResultType type) {
		Debug.Log ("Register result:" + type.ToString());
		JLog.Info ("Register result:" + type.ToString (), JLogCategory.Common);
		//StartCoroutine( SwitchScene ("select_player"));
		//ToDo:show register result on main menu
		switch (type) {
		case JObjAccountRegisterRet.AccountRegisterResultType.accountNotAllowed:
			break;
		case JObjAccountRegisterRet.AccountRegisterResultType.accountRepeated:
			break;
		case JObjAccountRegisterRet.AccountRegisterResultType.aodeNotAllowed:
			break;
		case JObjAccountRegisterRet.AccountRegisterResultType.codeIsTooSimple:
			break;	
		case JObjAccountRegisterRet.AccountRegisterResultType.emailIsRegistered:
			break;
		case JObjAccountRegisterRet.AccountRegisterResultType.successed:
			break;
		}
	}
}

