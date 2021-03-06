﻿using System;
using System.Threading;

namespace JGame
{
	using JGame.Network;
	using JGame.Processor;
	using JGame.Data;

	namespace Logic
	{
		public static class JLogicHelper
		{
			public static bool IsServer = false;
			private static ReaderWriterLockSlim _readerWriteLocker = new ReaderWriterLockSlim();

			public static bool registerProcessor(JPacketType packetType, IProcessor processor, bool isServerProcessor = false)
			{
				if (IsServer && !isServerProcessor || !IsServer && isServerProcessor)
					return true;
				
				bool bSuccess = false;
				try
				{
					_readerWriteLocker.EnterWriteLock ();

					if (!JLogicRegisteredProcessors.processors.PacketTypeToProcessor.ContainsKey(packetType))
					{
						JLogicRegisteredProcessors.processors.PacketTypeToProcessor.Add(packetType, processor);
						bSuccess = true;
					}

						
				}
				catch (Exception e) {
					JLog.Error ("JLogicHelper.RegisterProcessor error message:" + e.Message);
				}
				finally {
					_readerWriteLocker.ExitWriteLock ();
				}

				return bSuccess;
			}

			public static IProcessor getProcessor(JPacketType packetType)
			{
				IProcessor processor = null;
				try
				{
					_readerWriteLocker.EnterReadLock ();

					if (JLogicRegisteredProcessors.processors.PacketTypeToProcessor.ContainsKey(packetType))
					{
						processor = (IProcessor)JLogicRegisteredProcessors.processors.PacketTypeToProcessor[packetType];
					}
				}
				catch (Exception e) {
					JLog.Error ("JLogicHelper.RegisterProcessor error message:" + e.Message);
				}
				finally {
					_readerWriteLocker.ExitReadLock ();
				}

				if (null == processor)
					JLog.Error("JLogicHelper.RegisterProcessor: Unregistered packet type:" + packetType.GetDescription());

				return processor;
			}
		}
	}
}

