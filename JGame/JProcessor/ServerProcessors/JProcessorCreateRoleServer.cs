using System;
using System.Data;

namespace JGame.Processor
{
	using JGame.Data;
	using JGame.StreamObject;
	using JGame.DB;
	using JGame.Network;

	public class JProcessorCreateRoleServer : IProcessor
	{
		public void run (IDataSet dataSet)
		{
			JCreateRoleRetObject createRoleRetObj = new JCreateRoleRetObject ();
			createRoleRetObj.Result = JCreateRoleRetObject.CreateRoleResultType.failed;
			do 
			{
				//check role info existed
				IStreamObj signinObj = dataSet.getData(JObjectType.sign_in_info);
				if (null == signinObj)
				{
					createRoleRetObj.Result = JCreateRoleRetObject.CreateRoleResultType.failed;
					JLog.Error("JProcessorCreateRole.run can not find sign in account info");
					break;
				}
				JSignInClientInfoObject clientInfo = signinObj as JSignInClientInfoObject;
				string account = clientInfo.Info.Account;

				//process create role request
				IStreamObj obj = dataSet.getData (JObjectType.create_role);
				JCreateRoleReqObject createRoleReqObj = obj as JCreateRoleReqObject;
				if (createRoleReqObj == null)
				{
					createRoleRetObj.Result = JCreateRoleRetObject.CreateRoleResultType.failed;
					break;
				}

				string roleName = createRoleReqObj.RoleName;
				int roleType = createRoleReqObj.RoleType;

				//open data base
				JMySqlAccess mysql = new JMySqlAccess (JDBUtil.ServerDatabaseName, JDBUtil.ServerDatasource, JDBUtil.ServerUser, JDBUtil.ServerUserCode);
				try
				{
					if (!mysql.Open())
						break;
					if (!mysql.Connected)
						break;
				}
				catch(Exception e)
				{
					JLog.Error ("CheckRoleNameIsValud."+e.Message);
				}
				finally
				{
					mysql.Close();
				}

				if (!mysql.Connected)
				{
					JLog.Error("JProcessorCreateRole.run open database fialied");
					break;
				}

				//check name is valid or not
				int nErrorType = 0;
				if (!CheckRoleNameIsValud ( mysql, account, roleName, ref nErrorType)) {
					createRoleRetObj.Result = (JCreateRoleRetObject.CreateRoleResultType)nErrorType;
					JLog.Info ("JProcessorCreateRole.run Create role request not valid, error type:" + createRoleRetObj.Result.GetDescription ());
					mysql.Close();
					break;
				}
				if (!CheckRoleTypeIsValud(roleType, ref nErrorType))
				{
					createRoleRetObj.Result = (JCreateRoleRetObject.CreateRoleResultType)nErrorType;
					JLog.Info ("Create role request not valid, error type:" + createRoleRetObj.Result.GetDescription ());
					mysql.Close();
					break;
				}

				//insert new role record to role_info table
				string roleID = "";
				if (!CreateRole(mysql, account, roleName, roleType, ref roleID, ref nErrorType))
				{
					createRoleRetObj.Result = (JCreateRoleRetObject.CreateRoleResultType)nErrorType;
					JLog.Info ("JProcessorCreateRole.run CreateRole falied, error type:" + createRoleRetObj.Result.GetDescription ());
				}
				mysql.Close ();

			} while (false);

			try {
				JNetworkDataOperator.SendData (JPacketType.pt_createRole_ret, createRoleRetObj, dataSet.EndPoint);
			} catch (Exception e) {
				JLog.Debug ("JProcessorCreateRole 发送数据失败");
				JLog.Error ("JProcessorCreateRole 发送数据失败 "+e.Message);
			}
		}

		private bool CheckRoleNameIsValud( JMySqlAccess mysql, string account, string roleName, ref int ErrorType)
		{
			string strSql = 
				string.Format ("select * from {0} where user_account = '{1}' and role_name = '{2}'", 
					JDBUtil.TableName_Server_RoleInfo, 
					account,
					roleName);

			try
			{
				DataSet dataset = mysql.Select(strSql);
				if (null == dataset || null == dataset.Tables)
				{
					ErrorType = (int)JCreateRoleRetObject.CreateRoleResultType.failed;
					return false;
				}
				if (dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
				{
					ErrorType = (int)JCreateRoleRetObject.CreateRoleResultType.RoleNameRepeated;
					return false;
				}
			}
			catch (Exception e) {
				JLog.Error ("CheckRoleNameIsValud."+e.Message);
				return false;
			}

			return true;
		}

		private bool CheckRoleTypeIsValud(int roleType, ref int ErrorType)
		{
			return true;
		}

		private bool CreateRole ( JMySqlAccess mysql, string account, string roleName, int roleType, ref string roleId, ref int ErrorType)
		{
			try
			{
				string resultMssage = "";
				bool inserResult = mysql.DoSql(
					string.Format("INSERT into  {0}  ( role_name, user_account, role_type, scene_id)  VALUES( '{1}' , '{2}' , {3} ,  {4} );",
						JDBUtil.TableName_Server_RoleInfo, 
						roleName, 
						account, 
						roleType,
						JDBUtil.TableValue_SenceID_Default), ref resultMssage);
				JLog.Info ("resultMssage");
				if (inserResult)
				{
					ErrorType = (int) JCreateRoleRetObject.CreateRoleResultType.successed;
					JLog.Debug("Create role success, account: "+account+" roleName:"+roleName+" roleType:"+roleType);
					return false;
				}
				else
				{
					ErrorType = (int) JCreateRoleRetObject.CreateRoleResultType.failed;
					JLog.Debug("Create role falied, account: "+account+" roleName:"+roleName+" roleType:"+roleType);
					return true;
				}
			}
			catch(Exception e) {
				JLog.Error (e.Message);
				ErrorType = (int) JCreateRoleRetObject.CreateRoleResultType.failed;
				return false;
			}
		}
	}
}

