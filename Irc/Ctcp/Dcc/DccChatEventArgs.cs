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
    /// Provides data for an event raised by a DCC chat request.
    /// </summary>
    public class DccChatEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether the request should be accepted.
        /// </summary>
        public bool Accept { get; set; }

        /// <summary>
        /// Gets or sets the network name the request originated from.
        /// </summary>
        public string NetworkName { get; set; }

        /// <summary>
        /// Gets the IP of the remote host to send the file to.
        /// </summary>
        public byte[] RemoteAddress { get; private set; }

        /// <summary>
        /// Gets the port on the remote host on which to transfer the file.
        /// </summary>
        public int RemotePort { get; private set; }

        /// <summary>
        /// Gets the nickname of the user that sent the request.
        /// </summary>
        public IrcNickname Sender { get; private set; }

        /// <summary>
        /// Gets or sets the chat protocol to use.
        /// </summary>
        public string Protocol { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.Ctcp.Dcc.DccChatEventArgs"/> class.
        /// </summary>
        /// <param name="nickname">The nickname of the user sending the chat request.</param>
        /// <param name="address">The address of the remote host to connect to.</param>
        /// <param name="port">The port on the remote host to connect to.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname or address is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if port is not between 0 and 65535.</exception>
        public DccChatEventArgs(IrcNickname nickname, byte[] address, int port)
        {
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); }
            if (address == null) { throw new ArgumentNullException(nameof(address)); }
            if (port < 0 || port > 65535) { throw new ArgumentOutOfRangeException(nameof(port)); }

            Sender = nickname;
            RemoteAddress = address;
            RemotePort = port;
        }
    }
}
