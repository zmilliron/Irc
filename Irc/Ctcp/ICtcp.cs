/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/
using System;

namespace Irc.Ctcp
{
    /// <summary>
    /// Defines methods for implementing the Client-To-Client Protocol (CTCP).
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Client-To-Client Protocol is a communication protocol used within the standard
    /// IRC protocol to query another client for information about itself or its user, as well as 
    /// a way for two clients to communicate directly with one another independent of an IRC server.
    /// </para>
    /// <para>
    /// Communication is initiated by a client by using a standard PRIVMSG command and bookending the outgoing
    /// message with the <see cref="CtcpCommands.CtcpDelimeter"/> character.  Replies to a CTCP query are sent 
    /// via the NOTICE command and similarly bookended with the delimeter.
    /// </para>
    ///</remarks>
    public interface ICtcp
    {
        /// <summary>
        /// Occurs when a CTCP message is received from a server.
        /// </summary>
        event EventHandler<CtcpEventArgs> CtcpReceived;

        /// <summary>
        /// Sends a message to the specified target to be interpreted as an action or emote.
        /// </summary>
        /// <param name="targetName">The recipient of the message.</param>
        /// <param name="message">A message to send.</param>
        void CtcpAction(IrcNameBase targetName, string message);

        /// <summary>
        /// Requests information about a remote client.
        /// </summary>
        /// <param name="targetName">The name of the target to send the request to.</param>
        void CtcpClientInfo(IrcNameBase targetName);

        /// <summary>
        /// Sends supported CTCP commands to a remote user.
        /// </summary>
        /// <param name="nickname">The nickname of the user to reply to.</param>
        /// <param name="message">A message providing supported CTCP commands.</param>
        void CtcpClientInfoReply(IrcNickname nickname, string message);

        /// <summary>
        /// Sends a reply that an unknown command was received.
        /// </summary>
        /// <param name="targetName">The name of the target to send the reply to.</param>
        /// <param name="failedCommand">The attempted command that failed to process.</param>
        /// <param name="errorMessage">The error message explaining the reason for the failure.</param>
        void CtcpErrMsg(IrcNameBase targetName, string failedCommand, string errorMessage);

        /// <summary>
        /// Pings the specified user.
        /// </summary>
        /// <param name="targetName">The name of the target to send the request to.</param>
        void CtcpPing(IrcNameBase targetName);
        
        /// <summary>
        /// Sends a ping respose to the specified user.
        /// </summary>
        /// <param name="nickname">The nickname of the user to reply to.</param>
        /// <param name="receivedTimestamp">The timestamp sent by the remote user.</param>
        void CtcpPingReply(IrcNickname nickname, string receivedTimestamp);

        /// <summary>
        /// Requests information about where to obtain a copy of the client software from the specified user.
        /// </summary>
        /// <param name="targetName">The name of the target to send the request to.</param>
        void CtcpSource(IrcNameBase targetName);

        /// <summary>
        /// Sends information about where to obtain a copy of the client software to the specified user.
        /// </summary>
        /// <param name="nickname">The nickname of the user to reply to.</param>
        /// <param name="message">A message about where to obtain the client software.</param>
        void CtcpSourceReply(IrcNickname nickname, string message);

        /// <summary>
        /// Requests the local time from the specified user.
        /// </summary>
        /// <param name="targetName">The name of the target to send the request to.</param>
        void CtcpTime(IrcNameBase targetName);

        /// <summary>
        /// Sends the local time to the specified user.
        /// </summary>
        /// <param name="nickname">The nickname of the user to send the time to.</param>
        void CtcpTimeReply(IrcNickname nickname);

        /// <summary>
        /// Requests the version and type of the client software.
        /// </summary>
        /// <param name="targetName">The name of the target to send the request to.</param>
        void CtcpVersion(IrcNameBase targetName);

        /// <summary>
        /// Sends the version and type of the client software to the specified user.
        /// </summary>
        /// <param name="nickname">The nickname of the user to send the request to.</param>
        /// <param name="message">The message containg the client version information.</param>
        void CtcpVersionReply(IrcNickname nickname, string message);
    }
}
