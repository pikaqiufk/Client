
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Threading;
using ProtoBuf;
using ServiceBase;
using LZ4s;
using UnityEngine;

namespace ScorpionNetLib
{

    public sealed class SocketClient
    {

        PrefixHandler prefixHandler;
        MessageHandler messageHandler;
        DataHoldingUserToken token;

        private Socket socket;

        public Action<ServiceDesc> MessageReceived;
        public Action OnConnected;
        public Action OnDisconnected;
        public Action<Exception> OnException;

        private bool closing = false;
        private bool connected = false;
        private TimeSpan latency;

        private MemoryStream mMemoryStream = new MemoryStream(4096);
        private byte[] mEncodeBuffer = new byte[1024 * 64];
        private byte[] mDecodeBuffer = new byte[1024 * 64];
        private Queue<ServiceDesc> mMessageQueue = new Queue<ServiceDesc>();
        private SocketClientSettings socketClientSettings;

        private byte[] sendPrefixBuffer = new byte[4];
        private byte[] buffer;

        private float PingTime = 10;

        public bool Connected
        {
            get { return connected; }
            set { connected = value; }
        }

        public Socket Socket
        {
            get { return socket; }
        }

        public TimeSpan Latency
        {
            get { return latency; }
        }

        public void Stop()
        {
            closing = true;
            connected = false;
            CloseSocket();
        }

        public void SendMessage(ServiceDesc message)
        {
            // When we want to close all these, just stop to do anything more.
            if (closing)
            {
                return;
            }

            mMessageQueue.Enqueue(message);
        }

        public SocketClient(SocketClientSettings theSocketClientSettings)
        {
            prefixHandler = new PrefixHandler();
            messageHandler = new MessageHandler();
            socketClientSettings = theSocketClientSettings;
            token = new DataHoldingUserToken(0, 0);
        }

        private void Init()
        {
            connected = true;
        }

        public void StartConnect()
        {
            try
            {
                if (socket == null)
                {
                    socket = new Socket(socketClientSettings.ServerEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 3000);
                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 3000);
                    buffer = new byte[socket.ReceiveBufferSize];
                }
                else
                {
                    try
                    {
                        socket.Close();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString());
                    }
                }

                socket.BeginConnect(socketClientSettings.ServerEndPoint, ar =>
                {
                    Socket s = (Socket)ar.AsyncState;
                    try
                    {
                        s.EndConnect(ar);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.ToString());
                        ProcessConnectionError();
                        return;
                    }

                    if (OnConnected != null)
                    {
                        OnConnected();
                    }

                    Init();

                }, socket);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                ProcessConnectionError();
            }
        }

        //____________________________________________________________________________
        private void ProcessConnectionError()
        {
            if (socket != null)
            {
                socket.Close();
            }

            if (OnDisconnected != null && !closing)
            {
                OnDisconnected();
            }
        }

        static System.Random r = new System.Random();
        static byte[] head = new byte[1];
        void CopyDataToSendBuffer(IEnumerable<ServiceDesc> messages)
        {
            var ms = mMemoryStream;
            ms.Seek(0, SeekOrigin.Begin);
            ms.SetLength(0);
            {
                // foreach(var message in messages)
                var __enumerator1 = (messages).GetEnumerator();
                while (__enumerator1.MoveNext())
                {
                    var message = __enumerator1.Current;
                    {
                        var l = ms.Length;
                        ms.Seek(l + 6, SeekOrigin.Begin);
                        SerializerUtility.Serialize(ms, message);
                        var length = ms.Length - l - 6;
                        if (length > 128)
                        {
                            var b = LZ4Codec.Encode32(ms.GetBuffer(), (int) (l + 6), (int) length, mEncodeBuffer, 0,
                                mEncodeBuffer.Length);
                            ms.SetLength(l + 6);
                            ms.Seek(l + 6, SeekOrigin.Begin);
                            ms.Write(mEncodeBuffer, 0, b);
                            prefixHandler.WriteInt32(sendPrefixBuffer, 0, (int)b + (1 << 30));
                        }
                        else
                        {
                            prefixHandler.WriteInt32(sendPrefixBuffer, 0, (int)length);
                        }
                        ms.Seek(l, SeekOrigin.Begin);
                        r.NextBytes(head);
	                    head[0] = (byte) ((head[0] & 0xFE) | 0x08);
						ms.WriteByte(head[0]);
                        ms.WriteByte((byte)(0xFF ^ head[0]));
                        ms.Write(sendPrefixBuffer, 0, 4);
                        ms.Seek(ms.Length, SeekOrigin.Begin);
                    }
                }
            }
        }

        //____________________________________________________________________________
        //set the send buffer and post a send op
        public void StartSend()
        {
            // When we want to close all these, just stop to do anything more.
            if (closing)
            {
                return;
            }

            if(!connected)
                return;

            if (socket.Connected && CanSend())
            {
                PingTime -= Time.deltaTime;
                if (PingTime < 0)
                {
                    SendMessage(new ServiceDesc { Type = (int)MessageType.Ping, ClientId = (ulong)DateTime.Now.ToBinary() });
                    PingTime = 10.0f;
                }

                CopyDataToSendBuffer(mMessageQueue);
                mMessageQueue.Clear();
                try
                {
                    int length = (int)mMemoryStream.Length;
                    int offset = 0;
                    while (length > 0)
                    {
                        try
                        {
                            var send = socket.Send(mMemoryStream.GetBuffer(), offset,
                                Math.Min(socket.SendBufferSize, length), SocketFlags.None);
                            offset += send;
                            length -= send;
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.ToString());
                            StartDisconnect();
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
                    StartDisconnect();
                }
            }
        }

        private bool CanSend()
        {
            if (socket != null)
            {
                if (socket.Poll(0, SelectMode.SelectWrite))
                {
                    return true;
                }
            }
            return false;
        }

        private bool CanReceive()
        {
            if (socket != null)
            {
                if (socket.Poll(0, SelectMode.SelectRead))
                {
                    return true;
                }
            }
            return false;
        }

        //____________________________________________________________________________
        // Set the receive buffer and post a receive op.
        public void StartReceive()
        {
            // When we want to close all these, just stop to do anything more.
            if (closing)
            {
                return;
            }

            if (!connected)
                return;

            if (socket.Connected && CanReceive())
            {
                int remainingBytesToProcess = 0;
                try
                {
                    remainingBytesToProcess = socket.Receive(buffer);
                    if (remainingBytesToProcess == 0)
                    {
                        StartDisconnect();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
                    StartDisconnect();
                }

                while (ReceiveOneMessage(buffer, token, ref remainingBytesToProcess))
                {
                    //Console.WriteLine("another.");
                }
            }

        }

        /// <summary>
        /// If buffer still has data after call this function, it will return true.
        /// </summary>
        /// <param name="receiveSendEventArgs"></param>
        /// <param name="receiveSendToken"></param>
        /// <param name="remainingBytesToProcess"></param>
        /// <returns></returns>
        private bool ReceiveOneMessage(byte[] buffer, DataHoldingUserToken receiveSendToken,
            ref int remainingBytesToProcess)
        {
            // If we have not got all of the prefix then we need to work on it. 
            // receivedPrefixBytesDoneCount tells us how many prefix bytes were
            // processed during previous receive ops which contained data for 
            // this message. (In normal use, usually there will NOT have been any 
            // previous receive ops here. So receivedPrefixBytesDoneCount would be 0.)
            if (receiveSendToken.state == DataHoldingUserToken.State.Prefix)
            {
                remainingBytesToProcess = prefixHandler.HandlePrefix(buffer, receiveSendToken,
                    remainingBytesToProcess);

                if (remainingBytesToProcess == 0)
                {
                    //Jump out of the method, since there is no more data.
                    return false;
                }
            }

            // If we have processed the prefix, we can work on the message now.
            // We'll arrive here when we have received enough bytes to read
            // the first byte after the prefix.
            bool incomingTcpMessageIsReady = messageHandler.HandleMessage(buffer, receiveSendToken,
                ref remainingBytesToProcess);

            if (incomingTcpMessageIsReady)
            {
                // Process the data received, this function should use the data in dataReceived, and Deserialize the data
                // then the dataReceived will be reused  to receive other data.
                //OnMessageRecieved(receiveSendToken.SessionId, receiveSendToken.dataReceived);
                byte[] b;
                int length = 0;
                if (receiveSendToken.currentMessageCompressed)
                {
                    try
                    {
                        length = LZ4Codec.Decode32(receiveSendToken.dataReceived.GetBuffer(), 0, (int)receiveSendToken.dataReceived.Length, mDecodeBuffer, 0, mDecodeBuffer.Length, false);
                        b = mDecodeBuffer;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Message can not unwrap, this may caused by a hacker's attack." + ex.ToString());
                        b = null;
                    }
                }
                else
                {
                    b = receiveSendToken.dataReceived.GetBuffer();
                    length = (int) receiveSendToken.dataReceived.Length;
                }

                if (b != null)
                {
                    using (var ms = new MemoryStream(b, 0, length))
                    {
                        var message = SerializerUtility.Deserialize(ms);
                        receiveSendToken.Reset();

                        if (message.Type == (int)MessageType.Ping)
                        {
                            latency = DateTime.Now - DateTime.FromBinary((long) message.ClientId);
                        }
                        else
                        {
                            if (MessageReceived != null)
                            {
                                MessageReceived(message);
                            }
                        }
                    }
                }

                // continue to process another message which is not completed.
                if (remainingBytesToProcess > 0)
                {
                    return true;
                }

                return false;
            }
            else
            {
                // Since we have NOT gotten enough bytes for the whole message,
                // we need to do another receive op. Reset some variables first.

                // All of the data that we receive in the next receive op will be
                // message. None of it will be prefix. So, we need to move the 
                // receiveSendToken.receiveMessageOffset to the beginning of the 
                // buffer space for this SAEA.
                receiveSendToken.receiveMessageOffset = receiveSendToken.bufferOffsetReceive;
                return false;
            }
        }

        //____________________________________________________________________________
        // Disconnect from the host.        
        private void StartDisconnect()
        {
            try
            {
                if (OnDisconnected != null && !closing)
                {
                    OnDisconnected();
                }

                CloseSocket();
                ProcessDisconnectAndCloseSocket();
                connected = false;
            }
            catch (Exception ex)
            {
                // If server close the socket, exception may catch here.
            }
        }


        //____________________________________________________________________________
        private void ProcessDisconnectAndCloseSocket()
        {
            token.Reset();
        }


        //____________________________________________________________________________
        private void CloseSocket()
        {
            if (socket == null)
            {                
			    return;
            }

            try
            {

                socket.Close();
                socket = null;
            }
            catch (Exception ex)
            {
                if (OnException != null)
                {
                    OnException(ex);
                }
            }
        }
    }
}
