
using System;
using System.Net.Sockets;

namespace ScorpionNetLib
{
    class PrefixHandler
    {
        /// <summary>
        /// Writes a int value to a byte array from a starting index.
        /// </summary>
        /// <param name="buffer">Byte array to write int value</param>
        /// <param name="startIndex">Start index of byte array to write</param>
        /// <param name="number">An integer value to write</param>
        public void WriteInt32(byte[] buffer, int startIndex, int number)
        {
            buffer[startIndex] = (byte)((number >> 24) & 0xFF);
            buffer[startIndex + 1] = (byte)((number >> 16) & 0xFF);
            buffer[startIndex + 2] = (byte)((number >> 8) & 0xFF);
            buffer[startIndex + 3] = (byte)((number) & 0xFF);
        }

        /// <summary>
        /// Deserializes and returns a serialized integer.
        /// </summary>
        /// <returns>Deserialized integer</returns>
        public int ReadInt32(byte[] buffer, int startIndex)
        {
            return ((buffer[startIndex] << 24) |
                    (buffer[startIndex + 1] << 16) |
                    (buffer[startIndex + 2] << 8) |
                    (buffer[startIndex + 3])
                   );
        }

        public Int32 HandlePrefix(byte[] buffer, DataHoldingUserToken receiveSendToken,
            Int32 remainingBytesToProcess)
        {
            bool incomingTcpMessageIsReady = false;

            // Remember there is a receiveSendToken.receivedPrefixBytesDoneCount
            // variable, which allowed us to handle the prefix even when it
            // requires multiple receive ops. In the same way, we have a 
            // receiveSendToken.receivedMessageBytesDoneCount variable, which
            // helps us handle message data, whether it requires one receive
            // operation or many.
            if (remainingBytesToProcess >=
                receiveSendToken.lengthOfCurrentIncomingMessage - receiveSendToken.receivedMessageBytesDoneCount)
            {
                // If we are inside this if-statement, then we got 
                // the end of the message. In other words,
                // the total number of bytes we received for this message matched the 
                // message length value that we got from the prefix.

                // Write/append the bytes received to the byte array in the 
                // DataHolder object that we are using to store our data.
                receiveSendToken.dataReceived.Write(buffer, receiveSendToken.receiveMessageOffset,
                    receiveSendToken.lengthOfCurrentIncomingMessage - receiveSendToken.receivedMessageBytesDoneCount);

                remainingBytesToProcess -=
                    receiveSendToken.lengthOfCurrentIncomingMessage -
                    receiveSendToken.receivedMessageBytesDoneCount;

                if (remainingBytesToProcess > 0)
                {
                    receiveSendToken.receiveMessageOffset +=
                       receiveSendToken.lengthOfCurrentIncomingMessage -
                       receiveSendToken.receivedMessageBytesDoneCount;
                }
                else
                {
                    receiveSendToken.receiveMessageOffset = receiveSendToken.bufferOffsetReceive;
                }

                receiveSendToken.lengthOfCurrentIncomingMessage = ReadInt32(receiveSendToken.dataReceived.GetBuffer(), 2);
                receiveSendToken.currentMessageCompressed = (receiveSendToken.lengthOfCurrentIncomingMessage & 1 << 30) != 0;
                if (receiveSendToken.currentMessageCompressed)
                {
                    receiveSendToken.lengthOfCurrentIncomingMessage ^= 1 << 30;
                }
                receiveSendToken.state = DataHoldingUserToken.State.Content;
                receiveSendToken.receivedMessageBytesDoneCount = 0;

                return remainingBytesToProcess;
            }
            else
            {
                // If we are inside this else-statement, then that means that we
                // need another receive op. We still haven't got the whole message,
                // even though we have examined all the data that was received.
                // Not a problem. In SocketListener.ProcessReceive we will just call
                // StartReceive to do another receive op to receive more data.
                receiveSendToken.dataReceived.Write(buffer, receiveSendToken.receiveMessageOffset, remainingBytesToProcess);

                receiveSendToken.receiveMessageOffset = receiveSendToken.bufferOffsetReceive;

                receiveSendToken.receivedMessageBytesDoneCount += remainingBytesToProcess;

                return 0;
            }
        }
    }
}
