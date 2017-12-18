using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JGame.StreamObject
{

	/// <summary>
	/// 登录信息object
	/// </summary>
	public class JObjAccountRegisterReq : IStreamObj
	{
		public string _strAccount = "";
		public string _strCode = "";
		public string _strEmailAddress = "";

		public ushort Type ()
		{
			return (ushort)JObjectType.account_register;
		}
		public void Read (ref JInputStream stream)
		{
			_strAccount = JBinaryReaderWriter.Read<string> (stream);
			_strCode = JBinaryReaderWriter.Read<string> (stream);
			_strEmailAddress = JBinaryReaderWriter.Read<string> (stream);
		}
		public void Write (ref JOutputStream stream)
		{ 
			if (null == stream)
				stream = new JOutputStream ();
			JBinaryReaderWriter.Write(ref stream, Type());
			JBinaryReaderWriter.Write (ref stream, _strAccount);
			JBinaryReaderWriter.Write (ref stream, _strCode);
			JBinaryReaderWriter.Write (ref stream, _strEmailAddress);
			stream.Flush ();
		}
	}

	public class JObjAccountRegisterRet: IStreamObj
	{
		public AccountRegisterResultType Result;
		public enum AccountRegisterResultType
		{
			successed = 0,
			accountRepeated ,
			accountNotAllowed,
			codeIsTooSimple,
			codeNotAllowed,
			emailIsRegistered,
			failed
		}

		public ushort Type ()
		{
			return (ushort)JObjectType.account_register_ret;
		}
		public void Read (ref JInputStream stream)
		{
			Result = (AccountRegisterResultType)JBinaryReaderWriter.Read<ushort> (stream);
		}
		public void Write (ref JOutputStream stream)
		{ 
			if (null == stream)
				stream = new JOutputStream ();
			JBinaryReaderWriter.Write(ref stream, Type());
			JBinaryReaderWriter.Write (ref stream, (ushort) Result);
			stream.Flush ();
		}
	}
}

