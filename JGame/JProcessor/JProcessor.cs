using System;
using System.Net;
using System.Net.Sockets;

namespace JGame
{
	using StreamObject;
	using Network;
	using Data;

	namespace Processor
	{

		public interface IProcessor 
		{
			void run (IDataSet dataSet);
		}
	}
}

