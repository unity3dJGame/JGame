using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using JGame;
using JGame.Logic;

public class ServerManager : MonoBehaviour
{
	public Text ServerIP;
	public Text ServerPort;

	public static bool ServerActive = false;
	public static Thread ServerLogic = null;

	public void StartServer ()
	{
		if (null == ServerIP) {
			Debug.Log ("ServerIP is null");
			return;
		}
		if (null == ServerPort) {
			Debug.Log ("ServerPort is null");
			return;
		}

		JLog.Info ("Server IP:" + ServerIP.text + "   ServerPort:" + ServerPort.text);
		JGameManager.SingleInstance.initialize (true, ServerIP.text, int.Parse (ServerPort.text));
		ServerActive = true;
		JLog.Info ("initialize finished");

		try
		{
			ServerLogic = new Thread (Logic);
			ServerLogic.Start ();
			JLog.Info("ServerManager.StartServer logic thread start success.");
		}
		catch (Exception e) {
			JLog.Error ("ServerManager.StartServer error message:"+e.Message);
		}

	}

		
	void Start()
	{
	}

	private void Logic()
	{
		while (true) {
			if (ServerActive == false)
				break;
			Thread.Sleep (10);
			JLogic.Logic ();
		}
		JLog.Info("ServerManager.StartServer logic thread end.");
	}


	public static void ShutDown()
	{
		if (null != ServerLogic) {

			try
			{
				ServerLogic.Abort ();
				JLog.Info("ServerManager.StartServer logic thread abort.");
			}
			catch (Exception e) {
				JLog.Error ("ServerManager.StartServer error message:"+e.Message);
			}
		}
	}
}

