﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace JGame
{
	using JGame.StreamObject;
	using JGame.Data;
	namespace Network
	{
		using JGame.Network.Setting;

		public class JNetworkDataOperator
		{
			private static object _sendLocker = new object();
			private static Semaphore _sendSemaphore = new Semaphore(0,10000);
			private static object _receiveLocker = new object ();
			private static Semaphore _receivedSemaphore = new Semaphore (0, 10000);
			IPEndPoint a;
			internal static void SendData(byte [] data, IPEndPoint endPoint)
			{
				JNetworkData newData = new JNetworkData();
				newData.Data = data;
				newData.RemoteEndPoint = endPoint;//new  IPEndPoint(IPAddress.Parse(JNetworkServerInfo.ServerIP), JNetworkServerInfo.ServerPort);
				lock (_sendLocker) {
					JNetworkInteractiveData.SendData.Data.Enqueue (newData);
					_sendSemaphore.Release ();
				}
				JLog.Debug("JNetworkDataOperator.SendData[byte [] data] : send one data.", JGame.Log.JLogCategory.Network);
			}
			internal static void SendDataToServer(byte [] data)
			{
				SendData (data, JClientSocket.socket.RemoteEndPoint as IPEndPoint);
			}

			public static void SendData(
				JPacketType packetType,
				List<IStreamObj> objects,
				IPEndPoint endPoint)
			{
				JOutputStream jstream = new JOutputStream ();
				jstream.Writer.Write ((ushort)packetType);
				foreach(var obj in objects)
					JBinaryReaderWriter.Write (ref jstream, obj);

				SendData (jstream.ToArray (), endPoint);
			}
			public static void SendDataToServer(
				JPacketType packetType,
				List<IStreamObj> objects)
			{
				JOutputStream jstream = new JOutputStream ();
				jstream.Writer.Write ((ushort)packetType);
				foreach(var obj in objects)
					JBinaryReaderWriter.Write (ref jstream, obj);

				SendData (jstream.ToArray (), JClientSocket.socket.RemoteEndPoint as IPEndPoint);
			}
			public static void SendData(
				JPacketType packetType,  
				IStreamObj streamObject, 
				IPEndPoint endPoint)
			{
				JOutputStream jstream = new JOutputStream ();
				jstream.Writer.Write ((ushort)packetType);
				JBinaryReaderWriter.Write (ref jstream, streamObject);

				SendData (jstream.ToArray (), endPoint);
			}
			public static void SendDataToServer(
				JPacketType packetType,  
				IStreamObj streamObject)
			{
				JOutputStream jstream = new JOutputStream ();
				jstream.Writer.Write ((ushort)packetType);
				JBinaryReaderWriter.Write (ref jstream, streamObject);

				SendData (jstream.ToArray (), JClientSocket.socket.RemoteEndPoint as IPEndPoint);
			}

			/// <summary>
			/// Takes the send data.
			/// </summary>
			/// <returns>The send data. wait failed will return null</returns>
			/// <param name="millisecondsTimeout">Milliseconds timeout. <0 wait always</param>
			public static List<JNetworkData> TakeSendData(int millisecondsTimeout)
			{
				if (millisecondsTimeout < 0) {
					if (!_sendSemaphore.WaitOne ())
						return null;
				}
				else {
					if (!_sendSemaphore.WaitOne (millisecondsTimeout))
						return null;
				}
				_sendSemaphore = new Semaphore(0,10000);

				List<JNetworkData> listData = new List<JNetworkData> ();

				try{
					lock (_sendLocker) {
						while (JNetworkInteractiveData.SendData.Data.Count > 0)
							listData.Add (JNetworkInteractiveData.SendData.Data.Dequeue ());
					}						

				}
				catch (Exception e) {
					JLog.Error ("TakeSendData:" + e.Message, JGame.Log.JLogCategory.Network);
				}

				JLog.Debug("JNetworkDataOperator.TakeSendData : take all send data count:"
					+ listData.Count.ToString(), JGame.Log.JLogCategory.Network);
				return listData;
			}
				
			public static void ReceiveData(int len, byte[] data, EndPoint remoteEndPoint)
			{
				IPEndPoint endPoint = remoteEndPoint as IPEndPoint;
				if (null == endPoint)
					return;

				ReceiveData (len, data, endPoint);
			}
			public static void ReceiveData(int len, byte[] data, IPEndPoint remoteEndPoint)
			{
				JNetworkData networkData = new JNetworkData();
				networkData.Len = len;
				networkData.Data = data;
				networkData.RemoteEndPoint = remoteEndPoint;

				try
				{
					JLog.Debug("ReceiveData JNetworkInteractiveData.ReceivedData.Data.count : "+JNetworkInteractiveData.ReceivedData.Data.Count.ToString(), JGame.Log.JLogCategory.Network);
					lock (_receiveLocker)
					{
						JNetworkInteractiveData.ReceivedData.Data.Enqueue(networkData);
						_receivedSemaphore.Release();
					}
					JLog.Debug("JNetworkDataOperator.ReceiveData : added one data to received data queue", JGame.Log.JLogCategory.Network);
				}
				catch (Exception e) {
					JLog.Error ("ReceiveData:" + e.Message, JGame.Log.JLogCategory.Network);
				}
			}
			public static List<JNetworkData> TakeReceivedData()
			{
				List<JNetworkData> listData = new List<JNetworkData> ();

				if (!_receivedSemaphore.WaitOne (1)) {
					return listData;
				}
				_receivedSemaphore = new Semaphore(0,10000);

				try{
					lock (_receiveLocker) {
						JLog.Debug("JNetworkDataOperator.TakeReceivedData : take all received data count:"
							+ JNetworkInteractiveData.ReceivedData.Data.Count.ToString(),
							JGame.Log.JLogCategory.Network);	
						while (JNetworkInteractiveData.ReceivedData.Data.Count > 0)
							listData.Add (JNetworkInteractiveData.ReceivedData.Data.Dequeue ());
					}
				}
				catch (Exception e) {
					JLog.Error ("TakeSendData:" + e.Message, JGame.Log.JLogCategory.Network);
				}

				return listData;	
			}
		}//class JNetworkDataOperator

		public static class JNetworkHelper
		{
			public static JPacketType GetNetworkPacketType(JNetworkData data)
			{
				if (null == data)
					return JPacketType.npt_unknown;

				try
				{
					JInputStream inputStream = new JInputStream (data.Data);
					return (JPacketType)inputStream.Reader.ReadInt16 ();		
				}
				catch (Exception e) {
					JLog.Error ("JNetworkHelper.JPacketType error message :" + e.Message);
				}

				return JPacketType.npt_unknown;
			}

			public static bool IsValidPacketType(JPacketType type)
			{
				if (type > JPacketType.npt_min && type < JPacketType.npt_max) {
					return true;
				} else {
					return false;
				}
			}

			public static bool IsSameEndpoint(IPEndPoint endPoint1, IPEndPoint endPoint2)
			{
				if (null == endPoint1 || null == endPoint2)
					return false;

				if (endPoint1.Address.ToString () == endPoint2.Address.ToString () &&
					endPoint1.Port == endPoint2.Port) {
					return true;
				}

				return false;
			}
		}//class JNetworkHelper
	}//namespace Network
}//namespace JGame

