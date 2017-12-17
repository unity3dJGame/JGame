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

public class SignInNet : MonoBehaviour {
	public Text _user_account;		//用户账号
	public Text _user_code;			//用户密码
	public Canvas SignInPage;
	public Canvas RegisterPage;

	private static string _server_ip = "127.0.0.1";
	private static int	  _server_port = 9796;	
	private static Socket _client_socket = null;
	private static bool   _connected = false;

	// Use this for initialization
	void Start () {


	}
	void Update(){
		JLogic.Logic ();
	}

	//登录检查
	public void CheckToSignIn ()
	{
		JObj_SignIn obj = new JObj_SignIn();
		obj._strAccount = _user_account.text;
		obj._strCode = _user_code.text;
	
		JLocalDataHelper.addData (JPacketType.npt_signin_req, obj);
	}

	//注册
	public void ToRegister()
	{
		//JObj_SignRet obj = new JObj_SignRet();
		//obj.Result = true;
		//JLocalDataHelper.addData (JPacketType.npt_signin_ret, obj);
		SignInPage.gameObject.SetActive(false);
		RegisterPage.gameObject.SetActive (true);
	}

	public void ToSignIn ()
	{
		RegisterPage.gameObject.SetActive (false);
		SignInPage.gameObject.SetActive(true);
	}

	public bool SendToServer(JObj_SignIn obj)
	{
		if (!_connected)
			return false;

		try
		{

			JOutputStream jstream = new JOutputStream();
			JBinaryReaderWriter.Write(ref jstream,  obj);
			_client_socket.Send(jstream.ToArray());
		}
		catch (Exception e) {
			Debug.Log ("发送数据失败");
			Debug.LogError (e.Message);
			return false;
		}

		int nReceivedCount = 0;
		byte[] buffer = new byte[JTcpDefines.max_buffer_size];
		do {
			nReceivedCount = _client_socket.Receive (buffer, JTcpDefines.max_buffer_size, SocketFlags.None);
		} while(nReceivedCount == 0);

		//...received packet
		JInputStream inputStream = new JInputStream(buffer);
		IStreamObj receivedObj = JBinaryReaderWriter.Read<IStreamObj>(inputStream);
		ushort utype = receivedObj.Type();

		return true;
	}
}
