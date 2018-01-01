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
				
				JMySqlAccess mysql = new JMySqlAccess (JDBUtil.ServerDatabaseName, JDBUtil.ServerDatasource, JDBUtil.ServerUser, JDBUtil.ServerUserCode);
				if (!mysql.Open())
					return;
				if (!mysql.Connected)
						return;

				DataSet data = mysql.Select (string.Format(
					@"Select count(1) from {0} t where t.user_account = '{1}' or t.user_email = '{2}'" ,
					JDBUtil.TableName_Server_UserInfo,
					accountRegisterObj._strAccount,
					accountRegisterObj._strEmailAddress));

				bool bAreadyExisted = false;
				do
				{
					if (null == data || null == data.Tables)
						break;
					if (data.Tables.Count <= 0 )
						break;
					if (null == data.Tables[0].Rows || data.Tables[0].Rows.Count <= 0)
						break;
					if (int.Parse(data.Tables[0].Rows[0][0].ToString()) == 1)
						bAreadyExisted = true;
				}
				while(false);

				JObjAccountRegisterRet retObj = new JObjAccountRegisterRet();
				try
				{
					if (!bAreadyExisted)
					{
						string resultMssage = "";
						bool inserResult = mysql.DoSql(
							string.Format("INSERT into  {0}  ( user_account, user_code, user_email)  VALUES( '{1}' , '{2}' , '{3}' );",
								JDBUtil.TableName_Server_UserInfo, 
								accountRegisterObj._strAccount, 
								accountRegisterObj._strCode, 
								accountRegisterObj._strEmailAddress),  ref resultMssage);
						JLog.Info ("resultMssage");
						if (inserResult)
						{
							retObj.Result = JObjAccountRegisterRet.AccountRegisterResultType.successed;
							JLog.Debug("Create account success, account: "+accountRegisterObj._strAccount);
						}
						else
						{
							retObj.Result = JObjAccountRegisterRet.AccountRegisterResultType.failed;
							JLog.Debug("Create account falied, account: "+accountRegisterObj._strAccount);
						}
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
					mysql.Close ();
				}

				try {
					JNetworkDataOperator.SendData (JPacketType.npt_accountRegister_ret, retObj, dataSet.EndPoint);
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

