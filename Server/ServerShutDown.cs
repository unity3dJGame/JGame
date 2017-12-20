using System;
using UnityEngine;
using JGame;

public class ServerShutDown : MonoBehaviour
{
	public void ShutDownServer()
	{
		if (null != ServerManager.ServerLogic && ServerManager.ServerLogic.IsAlive) {
			try
			{
				ServerManager.ServerLogic.Abort ();
				JLog.Info("Server logic aborted");
			}
			catch (Exception e) {
				JLog.Error (e.Message);
			}
		}
		
		if (JGameManager.SingleInstance.ShutDown ()) {
			ServerManager.ServerActive = false;
			Debug.Log ("Server shut down : finished");
		} else {
			Debug.Log ("Server shut down : falied");
		}
	}
}


