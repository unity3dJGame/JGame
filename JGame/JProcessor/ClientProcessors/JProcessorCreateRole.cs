using System;

namespace JGame.Processor
{
	using JGame.Data;
	using JGame.StreamObject;
	using JGame.Network;
	using JGame.Logic;

	public class JProcessorCreateRole : IProcessor
	{
		public void run(IDataSet dataSet)
		{
			IStreamObj roleObj = dataSet.getData (JObjectType.create_role);
			if (null == roleObj) {
				JLog.Error ("JProcessorCreateRole.run roleObj is null.");
				return;
			}
			JCreateRoleReqObject createRoleReqObj = roleObj as JCreateRoleReqObject;
			if (null == createRoleReqObj) {
				JLog.Error ("JProcessorCreateRole.run createRoleReqObj is null.");
				return;
			}

			try {
				JNetworkDataOperator.SendDataToServer (JPacketType.pt_createRole_req, createRoleReqObj);
				JLogicUserData.setLocalData(createRoleReqObj);
			} catch (Exception e) {
				JLog.Debug ("JProcessorCreateRole 发送数据失败");
				JLog.Error ("JProcessorCreateRole 发送数据失败 "+e.Message);
				return;
			}
		}
	}
}

