using System;

namespace JGame.StreamObject
{
	public class JCreateRoleReqObject : IStreamObj
	{
		public string RoleName;
		public string RoleType;
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
			RoleType = JBinaryReaderWriter.Read<string> (stream);
		}

		/// <summary>
		/// Write the specified stream.
		/// </summary>
		/// <param name="stream">out put stream</param>
		public void Write (ref JOutputStream stream)
		{
			if (null == stream)
				stream = new JOutputStream ();
			JBinaryReaderWriter.Write (ref stream, Type());
			JBinaryReaderWriter.Write (ref stream, RoleName);
			JBinaryReaderWriter.Write (ref stream, RoleType);
		}
	}

	public class JCreateRoleRetObject : IStreamObj
	{
		public CreateRoleResultType Result;
		public string UserId;

		public enum CreateRoleResultType
		{
			successed = 0,
			RoleNameRepeated,
			RoleNameNotAllowed,
			RoleTypeError,
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
			JBinaryReaderWriter.Write (ref stream, Type());
			JBinaryReaderWriter.Write (ref stream, (ushort)Result);
			JBinaryReaderWriter.Write (ref stream, UserId);
		}
	}
}

