using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace JGame
{
	using JGame.Data;
	using JGame.StreamObject;
	namespace Logic
	{
		internal static class JLogicRegisteredProcessors
		{
			public static JLogicProcessors processors = new JLogicProcessors(); 
		}
		internal static class JLogicUserData
		{
			public static Dictionary<IPEndPoint, UserData> Data = new Dictionary<IPEndPoint, UserData>();
			private static IPEndPoint _localEndPoint = new IPEndPoint (IPAddress.Parse ("127.0.0.1"), 9796);

			public static void setLocalData(IStreamObj obj)
			{
				if (null == obj)
					return;
				setData (_localEndPoint, obj);
			}

			public static void setData(IPEndPoint endPoint, IStreamObj obj)
			{
				if (null == obj || null == endPoint)
					return;
				if (!Data.ContainsKey (endPoint)) {
					UserData data =  new UserData ();
					data.EndPoint = endPoint;
					Data.Add (endPoint,  data);
				}

				Data [endPoint].setData (obj);
			}

			public static UserData getLocalData()
			{
				return getData (_localEndPoint);
			}

			public static UserData getData (IPEndPoint endPoint)
			{
				if (!Data.ContainsKey (endPoint)) {
					return null;
				}
				return Data[endPoint];
			}

			//public static UserData Data = new UserData();
		}
	}

}

