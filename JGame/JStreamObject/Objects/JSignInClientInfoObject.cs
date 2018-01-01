using System;
using System.Collections.Generic;
using System.Net;

namespace JGame.StreamObject
{
	public struct ConnectedClientInfo
	{
		public string Account;
		//qt
	}
	
	public class JSignInClientInfoObject : IStreamObj
	{
		public ConnectedClientInfo Info;

		public JSignInClientInfoObject ()
		{
		}

		public ushort Type ()
		{
			return (ushort)JObjectType.sign_in_info;
		}
		public void Read (ref JInputStream stream)
		{
			Info.Account = JBinaryReaderWriter.Read<string> (stream);
		}
		public void Write (ref JOutputStream stream)
		{ 
			if (null == stream)
				stream = new JOutputStream ();
			JBinaryReaderWriter.Write(ref stream, Type());
			JBinaryReaderWriter.Write (ref stream, Info.Account);
			
			stream.Flush ();
		}
	}
}

