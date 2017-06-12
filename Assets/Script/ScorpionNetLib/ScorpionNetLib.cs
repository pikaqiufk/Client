
using System;
using System.Threading;
using UnityEngine;
using System.Collections;
using ServiceBase;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BehaviourMachine;
using EventSystem;
using ProtoBuf;

namespace ScorpionNetLib
{

    public enum EventType
    {
        ConnectionEstablished,
        ConnectionLost,
        MessageRecieved,
        ReplyRecieved,
        PublishMessageRecieved,
        Timeout,
        MessageSent,
        SendMessageFailed,
        Sync,
        DoSomething,
    }

    public class EventBase
    {
        public EventType Type { get; protected set; }
    }
    public class MessageWaiter : IEnumerator
    {
        private OutMessage mMessage;

        public MessageWaiter(OutMessage msg)
        {
            mMessage = msg;
        }

        public object Current
        {
            get { return mMessage; }
        }

        public bool MoveNext()
        {
            return mMessage.State == MessageState.Sent;
        }

        public void Reset()
        {
        }
    }

    public interface IAgentBase
    {
        void UnRegisterMessage(uint p);
        void RegisterMessage(uint p, OutMessage outMessage);
        void SendMessage(ServiceDesc protobufMessage);
        Coroutine StartCoroutineCall(IEnumerator enumerator);
        void AddPublishDataFunc(ServiceType serviceType, Func<uint, byte[], object> func);
        void AddPublishMessageFunc(ServiceType serviceType, Action<PublishMessageRecievedEvent> action);
    }

    public class ConnectionEstablishedEvent : EventBase
    {
        public ConnectionEstablishedEvent(string target, long targetId)
        {
            Type = EventType.ConnectionEstablished;
            Target = target;
            TargetId = targetId;
        }

        public string Target { get; private set; }

        public long TargetId { get; private set; }
    }

    public class ConnectionLostEvent : EventBase
    {
        public ConnectionLostEvent(string target, long targetId)
        {
            Type = EventType.ConnectionLost;
            Target = target;
            TargetId = targetId;
        }

        public string Target { get; private set; }

        public long TargetId { get; private set; }
    }

    public class MessageSentEvent : EventBase
    {
        public MessageSentEvent(object sender, ServiceDesc msg)
        {
            Type = EventType.MessageSent;
            Sender = sender;
            Message = msg;
        }

        public object Sender { get; private set; }
        public ServiceDesc Message { get; private set; }
        public object Data { get; private set; }
    }

    public class PublishMessageRecievedEvent : EventBase
    {
        public PublishMessageRecievedEvent(object sender, ServiceDesc msg, object data)
        {
            Type = EventType.PublishMessageRecieved;
            Sender = sender;
            Message = msg;
            Data = data;
        }

        public object Sender { get; private set; }
        public ServiceDesc Message { get; private set; }
        public object Data { get; private set; }
    }

    public class MessageRecievedEvent : EventBase
    {
        public MessageRecievedEvent(object sender, ServiceDesc msg, object data)
        {
            Type = EventType.MessageRecieved;
            Sender = sender;
            Message = msg;
            Data = data;
        }

        public object Sender { get; private set; }
        public ServiceDesc Message { get; private set; }
        public object Data { get; private set; }
    }

    public class DoSomethingEvent : EventBase
    {
        public DoSomethingEvent(Action act)
        {
            Type = EventType.DoSomething;
            Act = act;
        }

        public Action Act { get; private set; }
    }

    public class SendMessageFailedEvent : EventBase
    {
        public SendMessageFailedEvent(object sender, ServiceDesc msg)
        {
            Type = EventType.SendMessageFailed;
            Sender = sender;
            Message = msg;
        }

        public object Sender { get; private set; }
        public ServiceDesc Message { get; private set; }
    }


    public class TimeoutEvent : EventBase
    {
        public TimeoutEvent(object sender)
        {
            Type = EventType.Timeout;
            Sender = sender;
        }

        public object Sender { get; private set; }
    }

    public class SyncEvent : EventBase
    {
        public SyncEvent(object sender, SyncData data, ServiceType type)
        {
            Type = EventType.Sync;
            Sender = sender;
            SyncData = data;
            ServiceType = type;
        }

        public SyncData SyncData { get; private set; }

        public object Sender { get; private set; }

        public ServiceType ServiceType { get; private set; }
    }

    public enum MessageState
    {
        Sent,
        Reply,
        Timeout,
        Error,
    }
    public abstract class OutMessage
    {
        public ServiceDesc mMessage;
        protected IAgentBase mAgent;
        private int mState;
        private float mTimeout;
        private Coroutine mCoroutine;
        public float mStartTime;

        protected static MemoryStream sMemoryStream = new MemoryStream(4096);

        public float Timeout
        {
            get { return mTimeout; }
        }

        public float StartTime
        {
            get { return mStartTime; }
        }

        public MessageState State // added for convenience
        {
            get { return (MessageState)mState; }
            set { Interlocked.Exchange(ref mState, (int)value); }
        }

        public int ErrorCode { get; set; }

        public OutMessage(IAgentBase agent, ServiceType serviceType, uint funcId)
        {
            mAgent = agent;
            mMessage = new ServiceDesc();
            mMessage.ServiceType = (int)serviceType;
            mMessage.FuncId = funcId;
            mMessage.Type = (int)MessageType.CS;
            mMessage.PacketId = GetUniquePacketId();
        }

        protected abstract byte[] Serialize(MemoryStream s);

        public abstract void SetResponse(uint error, byte[] data);
        public abstract bool HasReturnValue { get; }
        private static int sPacketId = 0;

        public static uint GetUniquePacketId()
        {
            return (uint)Interlocked.Add(ref sPacketId, 1);
        }

        public static float Delay = 0;
        public static int Count = 0;

        public void SendAsync()
        {
            mAgent.RegisterMessage(mMessage.PacketId, this);

            sMemoryStream.SetLength(0);
            sMemoryStream.Seek(0, SeekOrigin.Begin);
            mMessage.Data = Serialize(sMemoryStream);
            mAgent.SendMessage(mMessage);
            State = MessageState.Sent;
        }

        public object SendAndWaitUntilDone(TimeSpan timeout = default(TimeSpan))
        {
            if (HasReturnValue)
            {
                mAgent.RegisterMessage(mMessage.PacketId, this);
                Count++;
                mStartTime = Time.fixedTime;
            }

            sMemoryStream.SetLength(0);
            sMemoryStream.Seek(0, SeekOrigin.Begin);
            mMessage.Data = Serialize(sMemoryStream);

            mAgent.SendMessage(mMessage);
            State = MessageState.Sent;

            if (HasReturnValue)
            {
                mTimeout = (float)(Time.fixedTime + (timeout.TotalSeconds == 0.0
                    ? 10.0f
                    : timeout.TotalSeconds));

                return mAgent.StartCoroutineCall(new MessageWaiter(this));
            }

            return null;
        }

    }

    /// <summary>
    /// Implementation of the Disruptor pattern
    /// </summary>
    /// <typeparam name="T">the type of item to be stored</typeparam>
    public class RingBuffer<T>
    {
        private readonly T[] _entries;
        private readonly int _modMask;
        private Volatile.PaddedLong _consumerCursor = new Volatile.PaddedLong();
        private Volatile.PaddedLong _producerCursor = new Volatile.PaddedLong();

        /// <summary>
        /// Creates a new RingBuffer with the given capacity
        /// </summary>
        /// <param name="capacity">The capacity of the buffer</param>
        /// <remarks>Only a single thread may attempt to consume at any one time</remarks>
        public RingBuffer(int capacity)
        {
            capacity = NextPowerOfTwo(capacity);
            _modMask = capacity - 1;
            _entries = new T[capacity];
        }

        /// <summary>
        /// The maximum number of items that can be stored
        /// </summary>
        public int Capacity
        {
            get { return _entries.Length; }
        }

        public T this[long index]
        {
            get { unchecked { return _entries[index & _modMask]; } }
            set { unchecked { _entries[index & _modMask] = value; } }
        }

        /// <summary>
        /// Removes an item from the buffer.
        /// </summary>
        /// <returns>The next available item</returns>
        public T Dequeue()
        {
            var next = _consumerCursor.ReadFullFence() + 1;
            while (_producerCursor.ReadFullFence() < next)
            {
                Thread.SpinWait(1);
            }
            var result = this[next];
            _consumerCursor.WriteFullFence(next);
            return result;
        }

        /// <summary>
        /// Attempts to remove an items from the queue
        /// </summary>
        /// <param name="obj">the items</param>
        /// <returns>True if successful</returns>
        public bool TryDequeue(out T obj)
        {
            var next = _consumerCursor.ReadFullFence() + 1;

            if (_producerCursor.ReadFullFence() < next)
            {
                obj = default(T);
                return false;
            }
            obj = Dequeue();
            return true;
        }

        /// <summary>
        /// Add an item to the buffer
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item)
        {
            var next = _producerCursor.ReadFullFence() + 1;

            long wrapPoint = next - _entries.Length;
            long min = _consumerCursor.ReadFullFence();

            while (wrapPoint > min)
            {
                min = _consumerCursor.ReadFullFence();
                Thread.SpinWait(1);
            }

            this[next] = item;
            _producerCursor.WriteUnfenced(next);
        }

        /// <summary>
        /// The number of items in the buffer
        /// </summary>
        /// <remarks>for indicative purposes only, may contain stale data</remarks>
        public int Count { get { return (int)(_producerCursor.ReadFullFence() - _consumerCursor.ReadFullFence()); } }

        private static int NextPowerOfTwo(int x)
        {
            var result = 2;
            while (result < x)
            {
                result <<= 1;
            }
            return result;
        }


    }
    public static class Volatile
    {
        private const int CacheLineSize = 64;

        [StructLayout(LayoutKind.Explicit, Size = CacheLineSize * 2)]
        public struct PaddedLong
        {
            [FieldOffset(CacheLineSize)]
            private long _value;

            /// <summary>
            /// Create a new <see cref="PaddedLong"/> with the given initial value.
            /// </summary>
            /// <param name="value">Initial value</param>
            public PaddedLong(long value)
            {
                _value = value;
            }

            /// <summary>
            /// Read the value without applying any fence
            /// </summary>
            /// <returns>The current value</returns>
            public long ReadUnfenced()
            {
                return _value;
            }

            /// <summary>
            /// Read the value applying acquire fence semantic
            /// </summary>
            /// <returns>The current value</returns>
            public long ReadAcquireFence()
            {
                var value = _value;
                Thread.MemoryBarrier();
                return value;
            }

            /// <summary>
            /// Read the value applying full fence semantic
            /// </summary>
            /// <returns>The current value</returns>
            public long ReadFullFence()
            {
                Thread.MemoryBarrier();
                return _value;
            }

            /// <summary>
            /// Read the value applying a compiler only fence, no CPU fence is applied
            /// </summary>
            /// <returns>The current value</returns>
            [MethodImpl(MethodImplOptions.NoOptimization)]
            public long ReadCompilerOnlyFence()
            {
                return _value;
            }

            /// <summary>
            /// Write the value applying release fence semantic
            /// </summary>
            /// <param name="newValue">The new value</param>
            public void WriteReleaseFence(long newValue)
            {
                Thread.MemoryBarrier();
                _value = newValue;
            }

            /// <summary>
            /// Write the value applying full fence semantic
            /// </summary>
            /// <param name="newValue">The new value</param>
            public void WriteFullFence(long newValue)
            {
                Thread.MemoryBarrier();
                _value = newValue;
            }

            /// <summary>
            /// Write the value applying a compiler fence only, no CPU fence is applied
            /// </summary>
            /// <param name="newValue">The new value</param>
            [MethodImpl(MethodImplOptions.NoOptimization)]
            public void WriteCompilerOnlyFence(long newValue)
            {
                _value = newValue;
            }

            /// <summary>
            /// Write without applying any fence
            /// </summary>
            /// <param name="newValue">The new value</param>
            public void WriteUnfenced(long newValue)
            {
                _value = newValue;
            }

            /// <summary>
            /// Atomically set the value to the given updated value if the current value equals the comparand
            /// </summary>
            /// <param name="newValue">The new value</param>
            /// <param name="comparand">The comparand (expected value)</param>
            /// <returns></returns>
            public bool AtomicCompareExchange(long newValue, long comparand)
            {
                return Interlocked.CompareExchange(ref _value, newValue, comparand) == comparand;
            }

            /// <summary>
            /// Atomically set the value to the given updated value
            /// </summary>
            /// <param name="newValue">The new value</param>
            /// <returns>The original value</returns>
            public long AtomicExchange(long newValue)
            {
                return Interlocked.Exchange(ref _value, newValue);
            }

            /// <summary>
            /// Atomically add the given value to the current value and return the sum
            /// </summary>
            /// <param name="delta">The value to be added</param>
            /// <returns>The sum of the current value and the given value</returns>
            public long AtomicAddAndGet(long delta)
            {
                return Interlocked.Add(ref _value, delta);
            }

            /// <summary>
            /// Atomically increment the current value and return the new value
            /// </summary>
            /// <returns>The incremented value.</returns>
            public long AtomicIncrementAndGet()
            {
                return Interlocked.Increment(ref _value);
            }

            /// <summary>
            /// Atomically increment the current value and return the new value
            /// </summary>
            /// <returns>The decremented value.</returns>
            public long AtomicDecrementAndGet()
            {
                return Interlocked.Decrement(ref _value);
            }

            /// <summary>
            /// Returns the string representation of the current value.
            /// </summary>
            /// <returns>the string representation of the current value.</returns>
            public override string ToString()
            {
                var value = ReadFullFence();
                return value.ToString();
            }
        }
    }


    public abstract class ClientAgentBase : MonoBehaviour, IAgentBase
    {
        protected SocketClient mClient;
        private DateTime mConnectTimeout;
        public string ServerAddress = "127.0.0.1:18001";
        private Queue<EventBase> mWaitingMessages = new Queue<EventBase>(1024);
        private Queue<EventBase> mPublishMessagesForNextFrame = new Queue<EventBase>(1024);
        private Dictionary<uint, OutMessage> mDispatcher = new Dictionary<uint, OutMessage>();
        private bool mConnectedNotified = false;
        private DataSyncCenter mSyncCenter = new DataSyncCenter();
        
        private Queue<EventBase> mEventQueue = new Queue<EventBase>();
        public Queue<EventBase> EventQueue
        {
            get { return mEventQueue; }
        }

        public static int WaitingCoroutine = 0;

        //public bool ReconnectSuccess { get; set; }

        public DataSyncCenter SyncCenter { get { return mSyncCenter; } }

        public abstract bool OnMessageTimeout(OutMessage message);
        
        public Coroutine StartAndWaitConnect(TimeSpan span)
        {
            var splittedAddress = ServerAddress.Trim().Split(':');
            var ip = splittedAddress[0].Trim();
            var port = Convert.ToInt32(splittedAddress[1].Trim());
            try
            {
                SocketClientSettings settings = new SocketClientSettings(new IPEndPoint(Dns.GetHostAddresses(ip)[0], port));
                mClient = new SocketClient(settings);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.ToString());
                return null;
            }

            mClient.MessageReceived += MessageReceived;
            mClient.OnDisconnected += OnDisconnected;
            mClient.OnException += ex =>
            {
                Logger.Error(ex.ToString());
            };

            mConnectedNotified = false;
            mClient.OnConnected += () =>
            {
                mConnectedNotified = true;
                Logger.Debug("Connected.....");
            };

            try
            {
                mConnectTimeout = DateTime.Now + span;
                mClient.StartConnect();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }

            var co = this.StartCoroutine(Wait());
            return co;

        }

//         public void SendReconnectMessageToGate()
//         {
//             if (ClientId == 0 || Key == 0)
//             {
//                 ReconnectSuccess = false;
//                 mConnectedNotified = true;
//             }
//             else
//             {
//                 ServiceDesc desc = new ServiceDesc();
//                 desc.Type = 100;//(int)MessageType.Authorize
//                 desc.ClientId = ClientId;
//                 desc.CharacterId = Key;
//                 mClient.SendMessage(desc);
//             }
//         }

        private IEnumerator Wait()
        {
            while (!mConnectedNotified && mConnectTimeout > DateTime.Now)
            {
                yield return null;
            }
        }
        void OnApplicationQuit()
        {
            Stop();
        }

        /// <summary>
        /// This method is a SYNC method, when connection is down, 1 second is used to query the connection status.
        /// So, do not call it frequently.
        /// </summary>
        public bool Connected
        {
            get
            {
                try
                {
                    if (mClient == null || mClient.Socket == null)
                        return false;

                    var s = mClient.Socket;

                    mClient.Connected =
                        (s.Connected && (!(s.Poll(1000, SelectMode.SelectRead) && (s.Available == 0))));

                    return mClient.Connected;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public int Latency
        {
            get
            {
                if (mClient != null)
                {
                    return (int) (mClient.Latency.TotalMilliseconds/2);
                }

                return 0;
            }
        }

        public void RegisterMessage(uint id, OutMessage msg)
        {
            mDispatcher.Add(id, msg);

            //Logger.Warn("wait: " + msg.mMessage.FuncId);
        }

        public void UnRegisterMessage(uint id)
        {
            mDispatcher.Remove(id);
        }

        public int WaitingMessageCount;
        public float AverageDelay;
        public int MessagePerSecond;

        private static int c = 0;
        private static float t = 0;
        public bool IsReconnecting {get;set;}
        public bool NeedReconnet {get;set;}
        List<OutMessage> removal = new List<OutMessage>(100);

        //private ulong ClientId;
        //private ulong Key;

        public void Update()
        {
#if !UNITY_EDITOR
            try
            {
#endif
            if (Time.fixedTime - t > 1)
            {
                t = Time.fixedTime;

                WaitingMessageCount = mDispatcher.Count;
                AverageDelay = Math.Max(0, Math.Min(OutMessage.Delay/OutMessage.Count, 10000));
                if (float.IsNaN(AverageDelay))
                {
                    AverageDelay = 10000;
                }
                MessagePerSecond = c;

                c = 0;
                if (OutMessage.Count > 10)
                {
                    OutMessage.Count = 0;
                    OutMessage.Delay = 0;
                }
            }

            if(mClient != null)
                mClient.StartReceive();

            //Profiler.BeginSample("t1");

            removal.Clear();
            bool isRemove = false;
                // foreach(var message in mDispatcher)
            var __enumerator1 = (mDispatcher).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var message = __enumerator1.Current;
                {
                    if (message.Value.Timeout < Time.fixedTime)
                    {
                        //Profiler.BeginSample(message.Value.GetType().ToString());
                        message.Value.State = MessageState.Timeout;
                        //Logger.Warn("timeout: " + message.Value.mMessage.FuncId);
                        removal.Add(message.Value);
                        isRemove = true;
                        c++;
                        //Profiler.EndSample();

                        OutMessage.Delay += Time.fixedTime - message.Value.mStartTime;
                    }
                }
            }
            if (isRemove)
            {
                var __listCount2 = removal.Count;
                for (int __i2 = 0; __i2 < __listCount2; ++__i2)
                {
                    var id = removal[__i2];
                    {
                        OnMessageTimeout(id);
                        UnRegisterMessage(id.mMessage.PacketId);
                    }
                }
            }
            //Profiler.EndSample();

            lock (mEventQueue)
            {
                while(mEventQueue.Count > 0)
                    mWaitingMessages.Enqueue(mEventQueue.Dequeue());
            }

            int count = 10; // process 10 packages per frame.
            while (mPublishMessagesForNextFrame.Count > 0 && count >= 0)
            {
                var e = mPublishMessagesForNextFrame.Dequeue();
                var evt = e as PublishMessageRecievedEvent; 
                DispatchPublishMessage(evt);
                count--;
            }

            //Profiler.BeginSample("t2");
            //lock (mWaitingMessages)
            {
                while (mWaitingMessages.Count > 0)
                {
                    var e = mWaitingMessages.Dequeue();
                    switch (e.Type)
                    {
                        case EventType.ConnectionLost:
                        {
                            this.StartCoroutine(OnServerLost());
                        }
                            break;
                        case EventType.MessageRecieved:
                        {
                            //Profiler.BeginSample("t2-1");
                            var evt = e as MessageRecievedEvent;
                            //Logger.Warn("received: " + evt.Message.FuncId);
                            ProcessReplyMessage(evt.Message);
                            //Profiler.EndSample();
                        }
                            break;
                        case EventType.ReplyRecieved:
                            break;
                        case EventType.PublishMessageRecieved:
                        {
                            //Profiler.BeginSample("t2-2");
                            // do it next frame
                            //var evt = e as PublishMessageRecievedEvent;
                            mPublishMessagesForNextFrame.Enqueue(e);
                            //Profiler.BeginSample("t2-2-" + evt.Message.FuncId);
                            //DispatchPublishMessage(evt);
                            c++;
                            //Profiler.EndSample();
                            //Profiler.EndSample();
                        }
                            break;
                        case EventType.Timeout:
                            break;
                        case EventType.MessageSent:
                            break;
                        case EventType.SendMessageFailed:
                            break;
                        case EventType.Sync:
                        {

                            //Profiler.BeginSample("t2-3");
                            var evt = e as SyncEvent;
                            StartCoroutine(ProcessSync(evt));
                            //Profiler.EndSample();
                        }
                            break;
                        case EventType.DoSomething:
                        {
                            var evt = e as DoSomethingEvent;
                            try
                            {
                                evt.Act();
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex.ToString());
                            }
                        }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            //Profiler.EndSample();

            if (mClient != null)
                mClient.StartSend();

#if !UNITY_EDITOR
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
#endif
        }

        void LateUpdate()
        {
#if !UNITY_EDITOR
try
{
#endif

            WaitingCoroutine = 0;
        
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

        private void ProcessReplyMessage(ServiceDesc desc)
        {
            OutMessage outMessage;
            if (!mDispatcher.TryGetValue(desc.PacketId, out outMessage))
            {
                return;
            }

            //Logger.Warn("continue: " + desc.FuncId);

            mDispatcher.Remove(desc.PacketId);

            try
            {
                outMessage.SetResponse(desc.Error, desc.Data);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());

                if (outMessage.ErrorCode == 0)
                {
                    // 如果服务器没有报错，修改错误码，因为网络包无法解析
                    outMessage.ErrorCode = 1004;
                }
            }

            OutMessage.Delay += Time.fixedTime - outMessage.mStartTime;
            c++;
            return;
        }

        private IEnumerator ProcessSync(SyncEvent evt)
        {
            yield return new WaitForEndOfFrame();
            SyncCenter.ApplySync(evt.ServiceType, evt.SyncData);
        }

        private void OnDisconnected()
        {
            //lock (mWaitingMessages)
            {
                mWaitingMessages.Enqueue(new ConnectionLostEvent(ServerAddress, 0));
            }
        }

        private void OnMessageSend(ServiceDesc desc)
        {
            //lock (mWaitingMessages)
            {
                mWaitingMessages.Enqueue(new MessageSentEvent(this, desc));
            }
        }

        public void Stop()
        {
            if (mClient != null)
            {
                try
                {
                    //Key = 0;
                    //ClientId = 0;
                    NeedReconnet = false;
                    mConnectedNotified = false;
                    mClient.Stop();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
                }
            }
        }

        public void PrepareForReconnect()
        {
            mClient.Stop();
            mConnectedNotified = false;
        }

        public abstract IEnumerator OnServerLost();

        public Coroutine StartCoroutineCall(IEnumerator enumerator)
        {
            if (null != enumerator)
            {
                return this.StartCoroutine(enumerator);
            }
            return null;
        }

        public void AddPublishDataFunc(ServiceType serviceType, Func<uint, byte[], object> func)
        {
            mMessageSerializer.Add(serviceType, func);
        }

        private Dictionary<ServiceType, Func<uint, byte[], object>> mMessageSerializer = new Dictionary<ServiceType, Func<uint, byte[], object>>();
        protected object GetPublishData(ServiceType serviceType, uint p, byte[] list)
        {
            Func<uint, byte[], object> action;
            if (mMessageSerializer.TryGetValue(serviceType, out action))
            {
                return action(p, list);
            }

            return null;
        }

        public void AddPublishMessageFunc(ServiceType serviceType, Action<PublishMessageRecievedEvent> action)
        {
            mServiceDispatcher.Add((int)serviceType, action);
        }

        private Dictionary<int, Action<PublishMessageRecievedEvent>> mServiceDispatcher = new Dictionary<int, Action<PublishMessageRecievedEvent>>();
        protected void DispatchPublishMessage(PublishMessageRecievedEvent evt)
        {
            Action<PublishMessageRecievedEvent> action;
            if (mServiceDispatcher.TryGetValue(evt.Message.ServiceType, out action))
            {
                action(evt);
            }
            //StartCoroutine(DispatchPublishMessageCoroutine(evt));
        }

        /// <summary>
        /// 因为所有CS的包的返回值是在Coroutine中处理的，所以如果直接处理SC的包，会导致包的顺序错乱
        /// 把SC的包延迟到下一帧，也在Coroutine中处理，可以在一定程度上避免SC的包比CS的返回值处理的早的问题
        /// 实际上如果出现 CSC 的情况，处理的顺序会是 CCS, 所以编码的时候，不能太依赖不同类型包的顺序
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        private IEnumerator DispatchPublishMessageCoroutine(PublishMessageRecievedEvent evt)
        {
            yield return new WaitForEndOfFrame();
        }

        private void MessageReceived(ServiceDesc desc)
        {
            try
            {
                if (desc.Type == (int)MessageType.SC)
                {
                    //lock (mWaitingMessages)
                    {
                        mWaitingMessages.Enqueue(new PublishMessageRecievedEvent(null, desc,
                            GetPublishData((ServiceType)desc.ServiceType, desc.FuncId, desc.Data)));
                    }
                    return;
                }
                else if (desc.Type == (int)MessageType.Sync)
                {
                    //lock (mWaitingMessages)
                    {
                        using (var ms = new MemoryStream(desc.Data, false))
                        {
                            mWaitingMessages.Enqueue(new SyncEvent(null, Serializer.Deserialize<SyncData>(ms), (ServiceType) desc.ServiceType));
                        }
                    }
                    return;
                }
                else if (desc.Type == (int)MessageType.CS)
                {
                    //lock (mWaitingMessages)
                    {
                        mWaitingMessages.Enqueue(new MessageRecievedEvent(null, desc, null));
                    }
                    return;
                }
//                 // 服务器发给客户端重连用的key
//                 else if (desc.Type == 100)//(int)MessageType.Authorize
//                 {
//                     if (desc.ServiceType == 0)
//                     {
//                         ClientId = desc.ClientId;
//                         Key = desc.CharacterId;
//                     }
//                     else
//                     {
//                         if (desc.Error == 0)
//                         {
//                             ReconnectSuccess = true;
//                             mConnectedNotified = true;
//                         }
//                         else
//                         {
//                             ReconnectSuccess = false;
//                             mConnectedNotified = true;
//                         }
//                     }
//                 }
                else if (desc.Type == (int)MessageType.Ping)
                {
                    return;
                }

                else
                {
                    Logger.Error("Unknow message type.");
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        public void SendMessage(ServiceDesc protobufMessage)
        {
            if (mClient==null) return;
            mClient.SendMessage(protobufMessage);
        }

    }

}
