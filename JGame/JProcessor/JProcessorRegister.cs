using System;

namespace JGame
{
	using Logic;
	using JGame.Data;

	namespace Processor
	{
		public static class JProcessorRegister
		{
			public static void RegisterServerProcessor()
			{
				JLogicHelper.registerProcessor (JPacketType.npt_signin_req, new JProcesserSignInServer (), true);
				JLogicHelper.registerProcessor (JPacketType.npt_accountRegister_req, new JProcessorAccountRegisterServer (), true);
			}

			public static void RegisterClientProcessor()
			{
				//JLogicHelper.registerProcessor (JPacketType.npt_signin_req, new JProcesserSignInSet (), false);
				//JLogicHelper.registerProcessor (JPacketType.npt_signin_ret, new JProcesserSignInGet (), false);
			}
		}
	}
}

