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

	public class JProcessorCreateRoleRet: IProcessor
	{
		public delegate void CreateRole(string roleName, int roleType);
		public CreateRole createRole;

		public void run(IDataSet dataSet)
		{
			IStreamObj obj = dataSet.getData (JObjectType.create_role_ret);
			if (null == obj || null == (obj as JCreateRoleRetObject))
				JLog.Error ("JProcesserCreateRoleRet : obj is empty!");

			if ((obj as JCreateRoleRetObject).Result  != JCreateRoleRetObject.CreateRoleResultType.successed)
				JLog.Info ("Received JCreateRoleRetObject create failed:!"+(obj as JCreateRoleRetObject).Result .GetDescription());
			//todo:...remind to regist
			else if (null != createRole) {
				IStreamObj createRoleObjTemp = JLogicUserData.getLocalData ().getData (JObjectType.create_role);
				if (null == createRoleObjTemp)
					return;
				JCreateRoleReqObject createRoleObj = createRoleObjTemp as JCreateRoleReqObject;
				if (null == createRoleObj)
					return;
				createRole (createRoleObj.RoleName, createRoleObj.RoleType);
			}
		}
	}
}