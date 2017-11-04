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
    /// Provides data for an event raised by a request to send a file through the DCC protocol.
    /// </summary>
    public class DccSendEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether the request should be accepted.
        /// </summary>
        public bool Accept { get; set; }

        /// <summary>
        /// Gets the name of the file to transfer.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets or sets the network name the request originated from.
        /// </summary>
        public string NetworkName { get; set; }

        /// <summary>
        /// Gets or sets the location to save the file on the local machine.
        /// </summary>
        public string SaveFileLocation { get; set; }

        /// <summary>
        /// Gets the nickname of the user that sent the request.
        /// </summary>
        public IrcNickname Sender { get; private set; }

        /// <summary>
        /// Gets the IP of the remote host to send the file to.
        /// </summary>
        public byte[] RemoteAddress { get; private set; }

        /// <summary>
        /// Gets the port on the remote host on which to transfer the file.
        /// </summary>
        public int RemotePort { get; private set; }

        /// <summary>
        /// Gets the size of the file, in bytes.
        /// </summary>
        public long FileSize { get; private set; }

        /// <summary>
        /// Gets or sets a token identifying the send request.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.Ctcp.Dcc.DccSendEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The nickname of the user sending a file transfer request.</param>
        /// <param name="fileName">The name of the file to send.</param>
        /// <param name="fileSize">The size of the file.</param>
        /// <param name="address">The address of the remote host to send the file to, each octet represented by a byte.</param>
        /// <param name="port">The port on the remote host to connect to.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if fileName is null or empty, or address is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if fileSize is less than 1 or port is not between 0 and 65535.</exception>
        public DccSendEventArgs(IrcNickname sender, string fileName, byte[] address, int port, long fileSize)
        {
            if (sender == null) { throw new ArgumentNullException(nameof(sender)); }
            if (string.IsNullOrEmpty(fileName)) { throw new ArgumentNullException(fileName); }
            if (fileSize < 1) { throw new ArgumentOutOfRangeException(nameof(fileSize)); }
            if (port < 0 || port > 65535) { throw new ArgumentOutOfRangeException(nameof(port)); }
            
            Sender = sender;
            FileName = fileName;
            FileSize = fileSize;
            RemoteAddress = address;
            RemotePort = port;
            NetworkName = "Unknown Network";
        }
    }
}
