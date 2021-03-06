﻿using System;
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

public class JProcesserSignIn :  MonoBehaviour
{
	public Slider _progress = null; //进度条
	public Text _progress_text = null;//进度条文字

	void Start()
	{
		JLogicHelper.registerProcessor (JPacketType.npt_signin_req, new JProcesserSignInSet (), false);
		JProcesserSignInGet processor = new JProcesserSignInGet ();
		processor.toSignIn += SignIn;
		JLogicHelper.registerProcessor (JPacketType.npt_signin_ret,  processor,false);
	}
		
	// Use this for sign in
	public void SignIn () {
		Debug.Log ("start game button is clicked.");
		StartCoroutine( SwitchScene ("select_player"));
		Debug.Log ("Welcome!");
	}

	public IEnumerator SwitchScene (string strSceneName)
	{
		AsyncOperation aop = SceneManager.LoadSceneAsync (strSceneName);
		//aop.allowSceneActivation = false;

		_progress.gameObject.SetActive (true);

		while (aop.progress < 1.0f) {
			_progress.value = aop.progress;
			_progress_text.text = (aop.progress*100).ToString()+"%";
			yield return new WaitForEndOfFrame ();
		}

		_progress.GetComponent<Slider> ().value = 1.0f;
		//aop.allowSceneActivation = true;

		//yield retun aop;
	}
}

