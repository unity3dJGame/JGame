using System;
using System.Timers;

namespace JGame
{
	using JGame.Log;

	namespace Time
	{
		public static class JTime
		{
			private static Timer _timer = null;

			public static ulong CurrentTime {
				get;
				internal set;
			}

			public static void Start()
			{
				//second time
				if (null == _timer) {
					_timer = new Timer (1);
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

