using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace JGame
{

	using Data;
	using StreamObject;
	using Network;
	using DB;
	namespace Processor
	{
		public class JProcessorAccountRegisterServer : IProcessor
		{
			public void run(IDataSet dataSet)
			{
				IStreamObj obj = dataSet.getData (JObjectType.account_register);
				JObjAccountRegisterReq accountRegisterObj = obj as JObjAccountRegisterReq;
				if (accountRegisterObj == null)
					return;
				
				JMySqlAccess sqlite = new JMySqlAccess ("mysql", "127.0.0.1", "root", "684268");
				if (!sqlite.Open())
					return;
				if (!sqlite.Connected)
						return;

				DataSet data = sqlite.Select (string.Format(
					@"Select count(1) from user_info t where t.user_account = '{0}' or t.user_email = '{1}'" ,
					accountRegisterObj._strAccount,
					accountRegisterObj._strEmailAddress));

				if (null == data || null == data.Tables)
					return;
				if (data.Tables.Count <= 0 || data.Tables [0].Rows.Count <= 0)
					return;
				int reusltCount = (int)data.Tables [0].Rows [0] [0];
				JObjAccountRegisterRet retObj = new JObjAccountRegisterRet();
				try
				{
					if (reusltCount == 0)
					{
						string resultMssage = "";
						bool inserResult = sqlite.DoSql(
							string.Format("INSERT into  {0}  ( user_account, user_code, user_email)  VALUES( ' {1}' , '{2}' , '{3}' );",
								"user_info", 
								accountRegisterObj._strAccount, 
								accountRegisterObj._strCode, 
								accountRegisterObj._strEmailAddress),  ref resultMssage);
						JLog.Info ("resultMssage");
						if (inserResult)
							retObj.Result = JObjAccountRegisterRet.AccountRegisterResultType.successed;
						else
							retObj.Result = JObjAccountRegisterRet.AccountRegisterResultType.failed;
					}
					else
					{
						retObj.Result = JObjAccountRegisterRet.AccountRegisterResultType.accountRepeated;
					}
				}
				catch(Exception e) {
					JLog.Error (e.Message);
				}
				finally {
					sqlite.Close ();
				}

				try {
					JNetworkDataOperator.SendData (JPacketType.npt_accountRegister_ret, retObj);
					JLog.Info("send npt_accountRegister_ret packet to client", JGame.Log.JLogCategory.Network);
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

