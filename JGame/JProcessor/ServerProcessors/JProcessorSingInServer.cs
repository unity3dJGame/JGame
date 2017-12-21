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
				JLog.Info("JProcesserSignInServer.run receive npt_signin_req packet from client: account:"+account+"  code:"+code, JGame.Log.JLogCategory.Network);

				JObj_SignRet resultObj = new JObj_SignRet ();
				JMySqlAccess sqlite = new JMySqlAccess ("mysql", "127.0.0.1", "root", "684268");
				if (!sqlite.Open())
					return;
				if (!sqlite.Connected)
					return;

				DataSet data = sqlite.Select (string.Format(
					@"Select * from user_info t where t.user_account = '{0}' and t.user_code = '{1}'" ,
					signInObj._strAccount,
					signInObj._strCode));

				bool bSuccess = false;
				do
				{
					if (null == data || null == data.Tables)
						break;
					if (data.Tables.Count <= 0 || data.Tables [0].Rows.Count <= 0)
						break;
					if (null == data.Tables[0].Rows || data.Tables[0].Rows.Count <= 0)
						break;
					if (data.Tables[0].Rows.Count > 0)
						bSuccess = true;
				}
				while(false);

				resultObj.Result = bSuccess;

				try {
					JNetworkDataOperator.SendData (JPacketType.npt_signin_ret, resultObj, dataSet.EndPoint);
					JLog.Info("JProcesserSignInServer.run send npt_signin_ret packet to client", JGame.Log.JLogCategory.Network);
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