using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JGame.StreamObject
{
	
	/// <summary>
	/// 登录信息object
	/// </summary>
	public class JObj_SignIn : IStreamObj
	{
		public string _strAccount = "";
		public string _strCode = "";

		public ushort Type ()
		{
			return (ushort)JObjectType.sign_in;
		}
		public void Read (ref JInputStream stream)
		{
			_strAccount = JBinaryReaderWriter.Read<string> (stream);
			_strCode = JBinaryReaderWriter.Read<string> (stream);
		}
		public void Write (ref JOutputStream stream)
		{ 
			if (null == stream)
				stream = new JOutputStream ();
			JBinaryReaderWriter.Write (ref stream, _strAccount);
			JBinaryReaderWriter.Write (ref stream, _strCode);
			stream.Flush ();
		}
	}

	public class JObjRoleInfo : IStreamObj
	{
		public string roleName;
		public int roleType;
		public int roleLevel;
		public double x;
		public double y;
		public double z;
		public double rotatex;
		public double rotatey;
		public double rotatez;

		public ushort Type ()
		{
			return (ushort)JObjectType.role_info;
		}

		public void Read (ref JInputStream stream)
		{
			roleName = JBinaryReaderWriter.Read<string> (stream);
			roleType = JBinaryReaderWriter.Read<int> (stream);
			roleLevel = JBinaryReaderWriter.Read<int> (stream);
			x = JBinaryReaderWriter.Read<double> (stream);
			y = JBinaryReaderWriter.Read<double> (stream);
			z = JBinaryReaderWriter.Read<double> (stream);
			rotatex = JBinaryReaderWriter.Read<double> (stream);
			rotatey = JBinaryReaderWriter.Read<double> (stream);
			rotatez = JBinaryReaderWriter.Read<double> (stream);
		}

		public void Write (ref JOutputStream stream)
		{ 
			if (null == stream)
				stream = new JOutputStream ();
			JBinaryReaderWriter.Write (ref stream, roleName);
			JBinaryReaderWriter.Write (ref stream, roleType);
			JBinaryReaderWriter.Write (ref stream, roleLevel);
			JBinaryReaderWriter.Write (ref stream, x);
			JBinaryReaderWriter.Write (ref stream, y);
			JBinaryReaderWriter.Write (ref stream, z);
			JBinaryReaderWriter.Write (ref stream, rotatex);
			JBinaryReaderWriter.Write (ref stream, rotatey);
			JBinaryReaderWriter.Write (ref stream, rotatez);
			stream.Flush ();
		}
	}

	public class JObj_SignRet: IStreamObj
	{
		public bool Result = false;
		public List<JObjRoleInfo> RolesInfo;

		public ushort Type ()
		{
			return (ushort)JObjectType.sign_in_ret;
		}
		public void Read (ref JInputStream stream)
		{
			RolesInfo = new List<JObjRoleInfo>();
			Result = JBinaryReaderWriter.Read<bool> (stream);
			int count = JBinaryReaderWriter.Read<int> (stream);
			for (int i = 0; i < count; i++) {
				RolesInfo.Add(JBinaryReaderWriter.Read<JObjRoleInfo> (stream));
			}
		}
		public void Write (ref JOutputStream stream)
		{ 
			if (null == stream)
				stream = new JOutputStream ();
			JBinaryReaderWriter.Write (ref stream, Result);
			if (null == RolesInfo) {
				JBinaryReaderWriter.Write (ref stream, (int)0);
			} else {
				JBinaryReaderWriter.Write (ref stream, RolesInfo.Count);
				foreach (JObjRoleInfo roleInfo in RolesInfo) {
					JBinaryReaderWriter.Write (ref stream, roleInfo);
				}
			}
			stream.Flush ();
		}
	}
}
