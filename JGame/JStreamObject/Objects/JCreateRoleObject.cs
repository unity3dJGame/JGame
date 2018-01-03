using System;
using System.ComponentModel;

namespace JGame.StreamObject
{
	public class JCreateRoleReqObject : IStreamObj
	{
		public string RoleName;
		public int RoleType;
		public JCreateRoleReqObject ()
		{
		}


		/// <summary>
		/// Headers the stream object type.
		/// </summary>
		/// <returns>The type.</returns>
	    public ushort Type ()
		{
			return (ushort)JObjectType.create_role;
		}

		/// <summary>
		/// Read the specified data.
		/// </summary>
		/// <param name="data">input data</param>
		public void Read (ref JInputStream stream)
		{
			RoleName = JBinaryReaderWriter.Read<string> (stream);
			RoleType = JBinaryReaderWriter.Read<Int16> (stream);
		}

		/// <summary>
		/// Write the specified stream.
		/// </summary>
		/// <param name="stream">out put stream</param>
		public void Write (ref JOutputStream stream)
		{
			if (null == stream)
				stream = new JOutputStream ();
			JBinaryReaderWriter.Write (ref stream, RoleName);
			JBinaryReaderWriter.Write (ref stream, (Int16)RoleType);
		}
	}

	public class JCreateRoleRetObject : IStreamObj
	{
		public CreateRoleResultType Result;
		public string UserId = "";

		public enum CreateRoleResultType
		{
			[Description("成功")]
			successed = 0,
			[Description("角色名已存在")]
			RoleNameRepeated,
			[Description("角色名不合法")]
			RoleNameNotAllowed,
			[Description("角色类型错误")]
			RoleTypeError,
			[Description("其它错误")]
			failed
		}

		public JCreateRoleRetObject ()
		{
		}


		/// <summary>
		/// Headers the stream object type.
		/// </summary>
		/// <returns>The type.</returns>
		public ushort Type ()
		{
			return (ushort)JObjectType.create_role_ret;
		}

		/// <summary>
		/// Read the specified data.
		/// </summary>
		/// <param name="data">input data</param>
		public void Read (ref JInputStream stream)
		{
			Result = (CreateRoleResultType)JBinaryReaderWriter.Read<ushort> (stream);
			UserId = JBinaryReaderWriter.Read<string> (stream);
		}

		/// <summary>
		/// Write the specified stream.
		/// </summary>
		/// <param name="stream">out put stream</param>
		public void Write (ref JOutputStream stream)
		{
			if (null == stream)
				stream = new JOutputStream ();
			JBinaryReaderWriter.Write (ref stream, (ushort)Result);
			JBinaryReaderWriter.Write (ref stream, UserId);
		}
	}
}

