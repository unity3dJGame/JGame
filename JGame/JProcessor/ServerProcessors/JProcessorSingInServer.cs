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
					if (data.Tables.Count <= 0 )
						break;
					if (null == data.Tables[0].Rows || data.Tables[0].Rows.Count <= 0)
						break;
					if (data.Tables[0].Rows.Count > 0)
						bSuccess = true;
				}
				while(false);
				resultObj.Result = bSuccess;

				string[] items 		= 	new string[] { "role_name", "role_type", "role_level", "x", "y", "z", "x_rotation", "y_rotation", "z_rotation", "user_account"};
				string[] where_cols =	new string[] {"user_account"};
				string[] operation 	= 	new string[] {"="};
				string[] values 	= 	new string[] {signInObj._strAccount};

				string strAccount = null;
				DataSet roleInfo = sqlite.Select ("role_info", items, where_cols, operation, values);
				do
				{
					if (null == roleInfo || null == roleInfo.Tables)
						break;
					if (roleInfo.Tables.Count <= 0)
						break;
					if (null == roleInfo.Tables[0].Rows || roleInfo.Tables[0].Rows.Count <= 0)
						break;
					if (roleInfo.Tables [0].Rows.Count > 0) {
						resultObj.RolesInfo = new System.Collections.Generic.List<JObjRoleInfo> ();
						foreach (DataRow dataRow in roleInfo.Tables[0].Rows) {
							JObjRoleInfo role = new JObjRoleInfo ();
							role.roleName = dataRow [0].ToString ();
							role.roleType = int.Parse (dataRow [1].ToString ());
							role.roleLevel = int.Parse (dataRow [2].ToString ());
							role.x = double.Parse (dataRow [3].ToString ());
							role.y = double.Parse (dataRow [4].ToString ());
							role.z = double.Parse (dataRow [5].ToString ());
							role.rotatex = double.Parse (dataRow [6].ToString ());
							role.rotatey = double.Parse (dataRow [7].ToString ());
							role.rotatez = double.Parse (dataRow [8].ToString ());
							resultObj.RolesInfo.Add (role);
							strAccount = dataRow[9].ToString();
						}
					}
				}
				while(false);

				if (null != strAccount) {
					//记录当前新增的登录信息到对应的dataSet
					IStreamObj clientobj = dataSet.getData(JObjectType.sign_in_info);
					if (null != clientobj) {
						JSignInClientInfoObject clientInfo = clientobj as JSignInClientInfoObject;
						clientInfo.Info.Account = strAccount;
					} else {
						JSignInClientInfoObject clientInfo= new JSignInClientInfoObject();
						clientInfo.Info.Account = strAccount;
						clientobj = clientInfo;
					}
					JLog.Info("JProcesserSignInServer.run add connected client info dataset, account:"+strAccount, JGame.Log.JLogCategory.Network);
					dataSet.setData (clientobj);
				}

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