using System;
using System.Threading;

namespace JGame.Logic
{
	public class JLogicThread
	{
		private static Thread _thread;
		private static bool _shutdown = false;

		public static void Start()
		{
			if (null == _thread) {
				_thread = new Thread (Logic);
			}
			_thread.Start ();
		}

		public static void ShutDown()
		{
			_shutdown = true;
			Thread.Sleep (100);
			_thread.Abort ();
		}

	    private static void Logic()
		{
			while (true) {
				if (_shutdown)
					break;
				JLogic.Logic ();
				Thread.Sleep (10);
			}
		}
	}
}

