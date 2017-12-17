using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace JGame.Processor
{
	using JGame.StreamObject;
	using JGame;
	using JGame.Network;
	using JGame.Data;
	using JGame.Logic;
	using JGame.Processor;
	using JGame.Log;
	public class JProcesserSignInSet :  IProcessor
	{
		public void run(IDataSet dataSet)
		{
			IStreamObj obj = dataSet.getData (JObjectType.sign_in);
			JObj_SignIn signInObj = obj as JObj_SignIn;
			if (signInObj == null)
				return;
			SendToServer (signInObj);
		}

		#region 私有方法
		protected bool SendToServer(JObj_SignIn obj)
		{
			try {
				JNetworkDataOperator.SendData(JPacketType.npt_signin_req, obj);
				return true;
			} catch (Exception e) {
				JLog.Debug ("发送数据失败");
				JLog.Error (e.Message);
				return false;
			}
		}
		#endregion
	}

	public class JProcesserSignInGet: IProcessor
	{
		public delegate void ToSignIn();
		public ToSignIn toSignIn;

		public void run(IDataSet dataSet)
		{
			IStreamObj obj = dataSet.getData (JObjectType.sign_in_ret);
			if (null == obj || null == (obj as JObj_SignRet))
				JLog.Error ("JProcesserSignInGet : obj is empty!");

			if ((obj as JObj_SignRet).Result == false)
				JLog.Info ("Received JObj_SignRet but account and code is not registed!");
			//todo:...remind to regist
			else
				if (null != toSignIn)
					toSignIn ();
		}
	}
}