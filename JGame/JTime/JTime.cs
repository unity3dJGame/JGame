using System;
using System.Timers;

namespace JGame
{
	using JGame.Log;

	namespace Time
	{
		public static class JTime
		{
			public static ulong CurrentTime {
				get;
				internal set;
			}

			private static Timer _timer = null;

			public static void Start()
			{
				//second time
				if (null == _timer) {
					_timer = new Timer (1000);
					_timer.Elapsed += (object sender, ElapsedEventArgs e) => {CurrentTime ++;};
					_timer.Enabled = true;
					_timer.Start ();
					JLog.Debug ("JTime start");
				} else {
					_timer.Start ();
					JLog.Debug ("JTime start");
				}
			}
				
		}
	}
}

