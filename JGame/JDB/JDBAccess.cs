using System;
using System.Data;

namespace JGame.DB
{
	public enum JDBType
	{
		Unknown = 0,
		MySql = 1,
	}
	
	public interface JDBAccess
	{
		string DatabaseName {
			get;
		}

		string Datasource {
			get;
		}

		string Port {
			get;
		}

		string UserID {
			get;
		}

		string UserPassword {
			get;
		}



		bool Connected
		{
			get;
		}

		IDbConnection DBConnection
		{
			get;
		}

		JDBType DBType();
			
		bool Open();

		bool Close();

		bool DoSql(string sql, ref string msg);

		DataSet Select(string sql);
	}
}

