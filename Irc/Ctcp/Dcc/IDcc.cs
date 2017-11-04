/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/
using System;

namespace Irc.Ctcp.Dcc
{
    /// <summary>
    /// Provides methods and events for implementing the Direct Client-to-Client (DCC) protocol.
    /// </summary>
    public interface IDcc
    {
        /// <summary>
        /// Occurs when a request is received to resume a previous file transfer.
        /// </summary>
        event EventHandler<DccResumeEventArgs> DccResumeRequest;

        /// <summary>
        /// Occurs when a request is received to transfer a file to the local client.
        /// </summary>
        event EventHandler<DccSendEventArgs> DccSendRequest;

        /// <summary>
        /// Occurs when a request is received to open a private chat with the local client.
        /// </summary>
        event EventHandler<DccChatEventArgs> DccChatRequest;

        /// <summary>
        /// Accepts a request to resume a file transfer using the DCC protocol.
        /// </summary>
        /// <param name="nickname">The nickname of the recipient.</param>
        /// <param name="fileName">The name of the file to accept.</param>
        /// <param name="port">The port number on which to conduct the transfer, in network byte order.  
        /// If zero is specified, this will initiate the reverse DCC protocol for clients that implement it.</param>
        /// <param name="startPosition">The position in the file, in bytes, from which to start the transfer.</param>
        /// <param name="token">The unique token identifying the file transfer sent by the initiator of the file transfer.</param>
        void DccAccept(IrcNickname nickname, string fileName, ushort port, long startPosition, string token);

        /// <summary>
        /// Sends a request to resume a file transfer using the DCC protocol.
        /// </summary>
        /// <param name="nickname">The nickname of the recipient.</param>
        /// <param name="fileName">The name of the file to resume.</param>
        /// <param name="port">The port number on which to conduct the transfer, in network byte order.  
        /// If zero is specified, this will initiate the reverse DCC protocol for clients that implement it.</param>
        /// <param name="startPosition">The position in the file, in bytes, from which to start the transfer.</param>
        /// <param name="token">The unique token identifying the file transfer sent by the initiator of the file transfer.</param>
        void DccResume(IrcNickname nickname, string fileName, ushort port, long startPosition, string token);

        /// <summary>
        /// Sends a request to a user to transer a file using the DCC protocol.
        /// </summary>
        /// <param name="nickname">The nickname of the recipient.</param>
        /// <param name="fileName">The name of the file to send.</param>
        /// <param name="address">The local address for the remote client to connect to, in network byte order.</param>
        /// <param name="port">The port number on which to conduct the transfer, in network byte order.  
        /// If zero is specified, this will initiate the reverse DCC protocol for clients that implement it.</param>
        /// <param name="token">A unique token identifying the file transfer.</param>
        /// <param name="fileSize">The size of the file to send, in bytes.</param>
        void DccSend(IrcNickname nickname, string fileName, long address, ushort port, long fileSize, string token);

        /// <summary>
        /// Sends a request to a user to open a private chat session using the DCC protocol.
        /// </summary>
        /// <param name="nickname">The nickname of the recipient.</param>
        /// <param name="address">The local address for the remote client to connect to, in network byte order.</param>
        /// <param name="port">The local port number for the remote client to connect on, in network byte order.</param>
        void DccChat(IrcNickname nickname, long address, ushort port);
    }
}
