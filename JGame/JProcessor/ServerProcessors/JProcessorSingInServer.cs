using System;
using System.Data;

namespace JGame
{
	using Data;
	using StreamObject;
	using Network;
	using DB;
	namespace Processor
	{
		
		public class JProcesserSignInServer :  IProcessor
		{
			public void run(IDataSet dataSet)
			{
				IStreamObj obj = dataSet.getData (JObjectType.sign_in);
				JObj_SignIn signInObj = obj as JObj_SignIn;
				if (signInObj == null)
					return;

				string account = signInObj._strAccount;
				string code = signInObj._strCode;
				JLog.Info("receive npt_signin_req packet from client: account:"+account+"  code:"+code, JGame.Log.JLogCategory.Network);

				JObj_SignRet resultObj = new JObj_SignRet ();
				JMySqlAccess sqlite = new JMySqlAccess ("mysql", "127.0.0.1", "root", "684268");
				if (!sqlite.Open())
					return;
				if (!sqlite.Connected)
					return;

				DataSet data = sqlite.Select (string.Format(
					@"Select count(1) from user_info t where t.user_account = '{0}' and t.user_code = '{1}'" ,
					signInObj._strAccount,
					signInObj._strCode));

				if (null == data || null == data.Tables)
					return;
				if (data.Tables.Count <= 0 || data.Tables [0].Rows.Count <= 0)
					return;
				int reusltCount = (int)data.Tables [0].Rows [0] [0];

				if (reusltCount > 0) 
				{
					resultObj.Result = true;
				}
				else
				{
					resultObj.Result = false;
				}

				try {
					JNetworkDataOperator.SendData (JPacketType.npt_signin_ret, resultObj);
					JLog.Info("send npt_signin_ret packet to client", JGame.Log.JLogCategory.Network);
					return;
				} catch (Exception e) {
					JLog.Debug ("发送数据失败");
					JLog.Error (e.Message);
					return;
				}
			}
		}

	}
}