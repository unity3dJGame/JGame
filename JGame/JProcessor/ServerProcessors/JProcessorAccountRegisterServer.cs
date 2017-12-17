using System;

namespace JGame
{

	using Data;
	using StreamObject;
	using Network;
	namespace Processor
	{
		public class JProcessorAccountRegisterServer : IProcessor
		{
			public void run(IDataSet dataSet)
			{
				IStreamObj obj = dataSet.getData (JObjectType.account_register);
				JObjAccountRegisterReq signInObj = obj as JObj_SignIn;
				if (signInObj == null)
					return;
			}
		}
	}

}

