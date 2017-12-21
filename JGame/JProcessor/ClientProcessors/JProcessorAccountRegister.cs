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
	public class JProcessorAccountRegisterReq :  IProcessor
	{
		public void run(IDataSet dataSet)
		{
			IStreamObj obj = dataSet.getData (JObjectType.account_register);
			JObjAccountRegisterReq accountRegisterObj = obj as JObjAccountRegisterReq;
			if (accountRegisterObj == null)
				return;
			SendToServer (accountRegisterObj);
		}

		#region 私有方法
		protected bool SendToServer(JObjAccountRegisterReq obj)
		{
			try {
				JNetworkDataOperator.SendDataToServer(JPacketType.npt_accountRegister_req, 	obj);
				return true;
			} catch (Exception e) {
				JLog.Debug ("发送数据失败");
				JLog.Error (e.Message);
				return false;
			}
		}
		#endregion
	}

	public class JProcessorAccountRegisterRet: IProcessor
	{
		public delegate void RegisterRetGot(JObjAccountRegisterRet.AccountRegisterResultType type);
		public RegisterRetGot registerRetGot;

		public void run(IDataSet dataSet)
		{
			IStreamObj obj = dataSet.getData (JObjectType.account_register_ret);
			if (null == obj || null == (obj as JObjAccountRegisterRet))
				JLog.Error ("JProcessorAccountRegisterRet : obj is empty!");
			else
				if (null != registerRetGot)
					registerRetGot ((obj as JObjAccountRegisterRet).Result);
		}
	}
}