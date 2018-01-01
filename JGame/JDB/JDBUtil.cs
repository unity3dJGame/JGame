using System;

namespace JGame.DB
{
	public static class JDBUtil
	{
		//db info
		public static string ServerDatabaseName = "mysql";
		public static string ServerDatasource = "127.0.0.1";
		public static string ServerPort = "3306";
		public static string ServerUser = "root";
		public static string ServerUserCode = "684268";

		//table info
		public static string TableName_Server_UserInfo = "user_info";
		public static string TableName_Server_RoleInfo = "role_info";
		public static string TableName_Server_Sence = "scene";
		public static string TableName_Server_SenceMonster = "scene_monster";

		//sence
		public static int TableValue_SenceID_Default = 0;
	}
}

