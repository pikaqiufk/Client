
using System;
using System.IO;
using System.Net.Sockets;

namespace ScorpionNetLib
{
    class MessageHandler
    {
        public bool HandleMessage(byte[] buffer, DataHoldingUserToken receiveSendToken, ref Int32 remainingBytesToProcess)
        {
            bool incomingTcpMessageIsReady = false;

            //Create the array where we'll store the complete message, 
            //if it has not been created on a previous receive op.
            if (receiveSendToken.receivedMessageBytesDoneCount == 0)
            {
                receiveSendToken.dataReceived.SetLength(receiveSendToken.lengthOfCurrentIncomingMessage);
                receiveSendToken.dataReceived.Seek(0, SeekOrigin.Begin);
            }
            // Remember there is a receiveSendToken.receivedPrefixBytesDoneCount
            // variable, which allowed us to handle the prefix even when it
            // requires multiple receive ops. In the same way, we have a 
            // receiveSendToken.receivedMessageBytesDoneCount variable, which
            // helps us handle message data, whether it requires one receive
            // operation or many.
            if (remainingBytesToProcess >= receiveSendToken.lengthOfCurrentIncomingMessage - receiveSendToken.receivedMessageBytesDoneCount)
            {
                // If we are inside this if-statement, then we got 
                // the end of the message. In other words,
                // the total number of bytes we received for this message matched the 
                // message length value that we got from the prefix.

                // Write/append the bytes received to the byte array in the 
                // DataHolder object that we are using to store our data.
                receiveSendToken.dataReceived.Write(buffer, receiveSendToken.receiveMessageOffset,
                    receiveSendToken.lengthOfCurrentIncomingMessage - receiveSendToken.receivedMessageBytesDoneCount);

                incomingTcpMessageIsReady = true;

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

                remainingBytesToProcess = 0;
            }

            return incomingTcpMessageIsReady;
        }
    }
}
