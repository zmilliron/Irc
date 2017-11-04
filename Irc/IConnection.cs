/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/
using System;

namespace Irc
{
    /// <summary>
    /// Defines members for implementing a socket connection for an Internet Relay Chat (IRC) client.
    /// </summary>
    public interface IConnection : IDisposable
    {
        /// <summary>
        /// Gets a value indicating if the <see cref="Irc.IConnection"/> is currently connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Gets the remote port.
        /// </summary>
        int Port { get; }

        /// <summary>
        /// Gets the remote addresss.
        /// </summary>
        string RemoteAddress { get; }

        /// <summary>
        /// Gets a value indicating if the <see cref="Irc.IConnection"/> supports a 
        /// Secure Sockets Layer (SSL) connection.
        /// </summary>
        bool SupportsSsl { get; }

        /// <summary>
        /// Gets the URI of the remote connection.
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// Occurs when the value of <see cref="Irc.IConnection.IsConnected"/> changes.
        /// </summary>
        event EventHandler IsConnectedChanged;

        /// <summary>
        /// Occurs when a message is received from an IRC server.
        /// </summary>
        event EventHandler<IrcMessageEventArgs> MessageReceived;

        /// <summary>
        /// Occurs when an error occurs on the underlying socket.
        /// </summary>
        event EventHandler<SocketErrorEventArgs> SocketConnectionError;

        /// <summary>
        /// Connects to the remote address and port in the specified URI.
        /// </summary>
        /// <param name="address">A URI specifying the connection details.</param>
        void Connect(Uri address);

        /// <summary>
        /// Closes the connection and disposes of the socket.
        /// </summary>
        void Close();

        /// <summary>
        /// Writes a line of text to the server.
        /// </summary>
        /// <param name="line">A line of text to send.</param>
        void WriteLine(string line);
    }
}
