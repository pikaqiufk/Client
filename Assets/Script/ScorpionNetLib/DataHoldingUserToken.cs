
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ServiceBase;

namespace ScorpionNetLib
{

    class DataHoldingUserToken
    {
        internal enum State
        {
            Prefix,
            Content
        }

        static long mainSessionId;

        internal Int32 socketHandleNumber;

        internal readonly Int32 bufferOffsetReceive;
        internal readonly Int32 bufferOffsetSend;  

        internal Int32 lengthOfCurrentIncomingMessage;
        internal bool currentMessageCompressed;

        internal State state = State.Prefix;

        //receiveMessageOffset is used to mark the byte position where the message
        //begins in the receive buffer. This value can sometimes be out of
        //bounds for the data stream just received. But, if it is out of bounds, the 
        //code will not access it.
        internal Int32 receiveMessageOffset;
        internal Int32 receivedMessageBytesDoneCount;
        //Remember, if a socket uses a byte array for its buffer, that byte array is
        //unmanaged in .NET and can cause memory fragmentation. So, first write to the
        //buffer block used by the SAEA object. Then, you can copy that data to another
        //byte array, if you need to keep it or work on it, and want to be able to put
        //the SAEA object back in the pool quickly, or continue with the data 
        //transmission quickly.
        internal MemoryStream dataReceived;

        //do not alloc memory each time a message recieved.
        //always use this buffer.
        private Byte[] prefixBuffer = new byte[4];

        internal Int32 sendBytesRemainingCount;
        internal Byte[] dataToSend;
        internal Int32 bytesSentAlreadyCount;
        
        public DataHoldingUserToken(Int32 rOffset, Int32 sOffset)
        {
            bufferOffsetReceive = rOffset;
            bufferOffsetSend = sOffset;
            receiveMessageOffset = rOffset;
            state = State.Prefix;
            dataReceived = new MemoryStream();
            lengthOfCurrentIncomingMessage = 6;
        }

        //Used to create sessionId variable in DataHoldingUserToken.
        //Called in ProcessAccept().
        internal ulong CreateClientId()
        {
            return (ulong)Interlocked.Increment(ref mainSessionId);
        }

        public void Reset()
        {
            receivedMessageBytesDoneCount = 0;
            state = State.Prefix;
            dataReceived.SetLength(0);
            dataReceived.Seek(0, SeekOrigin.Begin);
            lengthOfCurrentIncomingMessage = 6;
            bytesSentAlreadyCount = 0;
            sendBytesRemainingCount = 0;
        }
    }
}
