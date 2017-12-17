using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using JGame;
using JGame.StreamObject;
using JGame.Network;
using JGame.Data;
using JGame.Processor;
using JGame.LocalData;
using JGame.Logic;

public class AccountRegisterNet : MonoBehaviour {
	public Text UserAccount;		//用户账号
	public Text UserCode1;			//用户密码1
	public Text UserCode2;			//用户密码2
	public Text UserEmail;

	// Use this for initialization
	void Start () {
	}

	//登录检查
	public void ToSendRegisterData()
	{
		if (!UserCode1.text.Equals (UserCode2.text)) {
			JLog.Info ("两次密码输入不一致！");
			return;
		}

		//ToDo:验证用户输入的账号、密码、邮箱合法

		JObjAccountRegisterReq obj = new JObjAccountRegisterReq();
		obj._strAccount = UserAccount.text;
		obj._strCode = UserCode1.text;
		obj._strEmailAddress = UserEmail.text;
		JLocalDataHelper.addData (JPacketType.npt_accountRegister_req, obj);
	}
}
