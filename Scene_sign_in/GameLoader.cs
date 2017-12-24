using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JGame;
using System.Net;

public class GameLoader : MonoBehaviour {
	public bool IsServer = false;
	public string ServerDomainName = "justonebit.vicp.io";
	public int ServerPort = 22486;
	//public string ServerIP = "192.168.1.101";
	//public int ServerPort = 9796;

	// Use this for initialization
	void Start () {
		
		InitializeManagers ();


	}
	
	// Update is called once per frame
	/*void Update () {
		
	}*/

	public void InitializeManagers()
	{
		IPAddress address = null;
		try
		{
			IPHostEntry hostEntry = Dns.GetHostEntry (ServerDomainName);
			address = hostEntry.AddressList [0];
		}
		catch (Exception e) {
			JLog.Error (e.Message, JGame.Log.JLogCategory.Network);
			return;
		}
		if (null == address)
			return;

		JGame.JGameManager.SingleInstance.initialize (IsServer, address.ToString(), ServerPort);
	}
}
