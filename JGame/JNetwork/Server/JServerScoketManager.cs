using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Timers;

namespace JGame.Network
{
	using JGame.StreamObject;
	using JGame.Network.Setting;
	using JGame.Time;

	public class JServerSocketManager
	{
		private static JServerSocketManager _manager = null;
		private static Thread				_serverReceiveThread = null;
		private static Thread				_serverAcceptThread = null;
		private static Thread 				_serverSendThread = null;
		private static bool					_initialized = false;
		private static Semaphore			_semaphore = null;
		private static object				_socketLocker = null;
		private static bool					_forceEnd = false; 
		private static System.Timers.Timer  _headbeatTimer = null;
		private static object 				_heartbeatLocker = null;
		private static Dictionary<Socket, ulong> _socketHeartDelayTime;

		private JServerSocketManager ()
		{
			JConnectedClientSocket.sockets = new List<Socket> ();
		}

		public static JServerSocketManager SingleInstance
		{
			get {
				if (null == _manager) {
					JServerSocketManager manager = new JServerSocketManager ();
					System.Threading.Interlocked.CompareExchange<JServerSocketManager> (ref _manager, manager, null);
				}
				return _manager;
			}
		}

		public void ShutDown()
		{
			_forceEnd = true;
			try
			{
				JServerSocket.socket.Close ();
			}
			catch (Exception e) {
				JLog.Error (e.Message, JGame.Log.JLogCategory.Network);
			}

			_headbeatTimer.Stop();
			_headbeatTimer.Close();
		}

		public bool Initialized
		{
			get { return _initialized; }
		}
			
		/*private byte[] KeepAliveTime
		{
			get
			{
				uint dummy = 0;
				byte[] inOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
				BitConverter.GetBytes((uint)1).CopyTo(inOptionValues, 0);
				BitConverter.GetBytes((uint)5000).CopyTo(inOptionValues, Marshal.SizeOf(dummy));
				BitConverter.GetBytes((uint)5000).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);
				return inOptionValues;
			}
		}
		private const int keepAlive = -1744830460;
		private byte[] inValue = new byte[] { 1, 0, 0, 0, 0x20, 0x4e, 0, 0, 0xd0, 0x07, 0, 0 }; 
		byte[] inValue = new byte[] { 1, 0, 0, 0, 0x10, 0x27, 0, 0, 0xe8, 0x03, 0, 0 }; // True, 10秒, 1 秒
		JServerSocket.socket.IOControl(keepAlive, inValue, null);
		JServerSocket.socket.IOControl(IOControlCode.KeepAliveValues, inValue, null);*/

		public void Initialize(string serverIP, int serverPort)
		{
			if (_initialized) {
				JLog.Error ("JServerSocketManager initialized aready !", JGame.Log.JLogCategory.Network);
				return;
			}

			JLog.Info ("JServerSocketManager begin to initialize :", JGame.Log.JLogCategory.Network);

			_socketHeartDelayTime = new Dictionary<Socket, ulong> ();
			_heartbeatLocker = new object ();
			JNetworkServerInfo.ServerIP = serverIP;
			JNetworkServerInfo.ServerPort = serverPort;
			_socketLocker = new object ();
			_semaphore = new Semaphore (0, 10000);
			JNetworkInteractiveData.ReceivedData = new JNetworkDataQueue ();
			JNetworkInteractiveData.SendData = new JNetworkDataQueue ();
			IPAddress ip_server = IPAddress.Parse (serverIP); 
			IPEndPoint server_edp = new IPEndPoint (ip_server, serverPort);

			JServerSocket.socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			try
			{
				JServerSocket.socket.Bind(server_edp);
				JLog.Info("JServerSocketManager server socket bind to server endpoint finished.\nIP:"+serverIP+"\nPort:"+serverPort.ToString(),
					JGame.Log.JLogCategory.Network);

				JServerSocket.socket.Listen(JTcpDefines.max_tcp_connect);
				JLog.Info("JServerSocketManager server socket begin listen", JGame.Log.JLogCategory.Network);

				_serverSendThread = new Thread(SendLoop) { IsBackground = true };
				_serverSendThread.Start();

				_serverReceiveThread = new Thread(ReceiveLoop) { IsBackground = true };
				_serverReceiveThread.Start();

				_serverAcceptThread = new Thread(AcceptLoop) { IsBackground = true };
				_serverAcceptThread.Start();
			}
			catch (Exception e) {
				JLog.Error ("JServerSocketManager initialize faield  error message: " + e.Message, JGame.Log.JLogCategory.Network);
				return;
			}

			//start heart beat check
			_headbeatTimer = new System.Timers.Timer(1000);
			_headbeatTimer.Elapsed +=  heatbeatTest;
			_headbeatTimer.Start ();
		}

		private void AcceptLoop()
		{
			JLog.Info("JServerSocketManager server accept loop started", JGame.Log.JLogCategory.Network);

			while (true)
			{
				//JLog.Debug("AcceptLoop loop one begin ...", JGame.Log.JLogCategory.Network);

 				if (_forceEnd)
					break;
				
				try
				{
					JLog.Debug("JServerSocketManager.AcceptLoop begin to accept", JGame.Log.JLogCategory.Network);
					Socket currentConnectedSocket = JServerSocket.socket.Accept();
					JLog.Debug("JServerSocketManager.AcceptLoop accept finished, to process accepted socket", JGame.Log.JLogCategory.Network);
					if (null != currentConnectedSocket && currentConnectedSocket.Connected)
					{
						lock (_socketLocker)
						{
							if (!JConnectedClientSocket.sockets.Contains(currentConnectedSocket))
							{
								JConnectedClientSocket.sockets.Add(currentConnectedSocket);

								JLog.Info("JServerSocketManager.AcceptLoop client connected :"+
									(currentConnectedSocket.RemoteEndPoint as IPEndPoint).Address.ToString(), JGame.Log.JLogCategory.Network);
								_semaphore.Release();
							}
							else {
								JLog.Info("JServerSocketManager.AcceptLoop connected client connected again"
									+(currentConnectedSocket.RemoteEndPoint as IPEndPoint).Address.ToString(), JGame.Log.JLogCategory.Network);
							}
						}

						lock (_heartbeatLocker)
						{
							if (!_socketHeartDelayTime.ContainsKey(currentConnectedSocket))
							{
								_socketHeartDelayTime.Add(currentConnectedSocket, 0);
							}
						}
					}
				}
				catch (Exception e)
				{
					JLog.Error ("JServerSocketManager accept loop error message:" + e.Message, JGame.Log.JLogCategory.Network);
					Thread.Sleep (200);
				}

				//JLog.Debug("AcceptLoop loop one end ...", JGame.Log.JLogCategory.Network);
			}

			JLog.Info("JServerSocketManager server accept loop end.", JGame.Log.JLogCategory.Network);
		}

		private void ReceiveLoop()
		{
			JLog.Info ("JServerSocketManager server receive loop started", JGame.Log.JLogCategory.Network);
			List<Socket> clientScokets = new List<Socket> ();

			while (true)
			{
				if (_forceEnd)
					break;
				
				if (_semaphore.WaitOne (1)) {
					_semaphore = new Semaphore (0, 10000);
					JLog.Info ("JServerSocketManager ReceiveLoop _semaphore.WaitOne(1) success", JGame.Log.JLogCategory.Network);
					lock (_socketLocker) {
						foreach (Socket socket in JConnectedClientSocket.sockets) {
							clientScokets.Add (socket);
						}	
						JLog.Info ("JServerSocketManager ReceiveLoop add socket to clientsockets: count : " + clientScokets.Count.ToString (), JGame.Log.JLogCategory.Network);
					}

					JLog.Debug ("connected client sockets : " + clientScokets.Count.ToString (), JGame.Log.JLogCategory.Network);
				} 
				if (clientScokets.Count == 0 ) {
					_semaphore.WaitOne ();
					_semaphore.Release();

					JLog.Debug ("JServerSocketManager ReceiveLoop._semaphore.WaitOne success. " , JGame.Log.JLogCategory.Network);				

					continue;
				} 
					
				//JLog.Debug ("JServerSocketManager ReceiveLoop.Socket.Select begin Socket.Select clientsocket:count :" + clientScokets.Count.ToString (), JGame.Log.JLogCategory.Network);				
				Socket.Select (clientScokets, null, null, 10000);
				if (clientScokets.Count > 0)
					JLog.Debug ("JServerSocketManager ReceiveLoop.Socket.Select selected clientsockets: count : " + clientScokets.Count.ToString (), JGame.Log.JLogCategory.Network);
				
				List<Socket> disconnectedSockets = new List<Socket>();
				foreach (Socket socket in clientScokets) 
				{
					/*if (socket.Available <= 0)
						continue;*/

					//如果接收到了新的消息就重置包时间
					lock (_heartbeatLocker) 
					{
						if (_socketHeartDelayTime.ContainsKey (socket)) 
						{
							_socketHeartDelayTime [socket] = JTime.CurrentTime;
						}
						else 
						{
							_socketHeartDelayTime.Add (socket, JTime.CurrentTime);
						}
					}
					
					//receive form client socket
					bool bReceivedSuccess = false;
					if (socket.Connected) {
						try {
							byte[] recBuffer = new byte[JTcpDefines.max_buffer_size];
							JLog.Info ("try to receive from socket : " + (socket.RemoteEndPoint as IPEndPoint).Address.ToString (), JGame.Log.JLogCategory.Network);
							int recLen = socket.Receive (recBuffer);
							if (recLen > 0) {
								JLog.Info ("receive one packet from client : IP" + (socket.RemoteEndPoint as IPEndPoint).Address.ToString () + " len:" + recLen.ToString (), JGame.Log.JLogCategory.Network);
								//save the received data
								JNetworkDataOperator.ReceiveData (recLen, recBuffer, socket.RemoteEndPoint);

								//add the selected socket to select sockets list
								//clientScokets.Add(socket);

								bReceivedSuccess = true;
							}
						} catch (Exception e) {
							JLog.Error ("JServerSocketManager ReceiveLoop exception reveive error message:" + e.Message, JGame.Log.JLogCategory.Network);
						}
					}

					try
					{
						if (!bReceivedSuccess) 
						{
							//client disconnect
							if (socket.Connected)
								socket.Close();
							//record disconnected socket from list
							disconnectedSockets.Add(socket);

							JLog.Info("client socket disconnected : " + socket.RemoteEndPoint.ToString(), JGame.Log.JLogCategory.Network);
						}
					}
					catch (Exception e) {
						JLog.Error ("JServerSocketManager ReceiveLoop exception error message1:" + e.Message, JGame.Log.JLogCategory.Network);
					}
				}

				try
				{
					//remove disconnected socket form list
					if (disconnectedSockets.Count > 0) {
						lock (_socketLocker) {
							foreach ( Socket socket in disconnectedSockets)
							{
								JConnectedClientSocket.sockets.Remove (socket);
								clientScokets.Remove (socket);
							}
						}
					}


					//add old sockets to client sockets
					lock (_socketLocker) {
						foreach ( Socket socket in JConnectedClientSocket.sockets)
						{
							clientScokets.Add (socket);
						}
					}
				}
				catch (Exception e) {
					JLog.Error ("JServerSocketManager ReceiveLoop exception error message2:" + e.Message, JGame.Log.JLogCategory.Network);
				}


			}

			JLog.Info ("JServerSocketManager server receive loop end.", JGame.Log.JLogCategory.Network);
		}

		private void SendLoop()
		{
			JLog.Info("JServerSocketManager server send loop started", JGame.Log.JLogCategory.Network);

			while (true) {
				if (_forceEnd)
					break;
				List<JNetworkData> dataList = JNetworkDataOperator.TakeSendData (1000);
				if (null == dataList) {
					continue;
				}

				foreach (JNetworkData data in dataList) {
					lock (_socketLocker) {
						foreach ( Socket socket in JConnectedClientSocket.sockets)
						{
							if (null == socket || null == socket.RemoteEndPoint)
								continue;
							
							if (JNetworkHelper.IsSameEndpoint (data.RemoteEndPoint as IPEndPoint, socket.RemoteEndPoint as IPEndPoint)) {
								try
								{
									socket.Send (data.Data);
									JLog.Info("send one data to "+(socket.RemoteEndPoint as IPEndPoint).Address.ToString(), JGame.Log.JLogCategory.Network);
								}
								catch(Exception e) {
									JLog.Error ("SendLoop error message:" + e.Message, JGame.Log.JLogCategory.Network);
								}						
							}
						}
					}
				}
			}

			JLog.Info("JServerSocketManager server send loop end", JGame.Log.JLogCategory.Network);

		}

		private void heatbeatTest(object sender, ElapsedEventArgs e)
		{
			if (null == _socketHeartDelayTime || _socketHeartDelayTime.Count <= 0)
				return;

			foreach (var time in _socketHeartDelayTime) 
			{
				if (JTime.CurrentTime - time.Value > 10000) //10s 
				{
					lock (_heartbeatLocker) 
					{
						_socketHeartDelayTime.Remove (time.Key);
						JLog.Info (string.Format ("JServerSocektManager.Heartbeat Client dissconnected : {0}", 
							(time.Key.RemoteEndPoint as IPEndPoint).Address.ToString ()), JGame.Log.JLogCategory.Network);
					}
				}
			}
		}
	}
}

