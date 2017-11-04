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
    /// Provides data for an event raised by a request to resume a DCC file transfer.
    /// </summary>
    public class DccResumeEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the name of the file to transfer.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the port on the remote host on which to transfer the file.
        /// </summary>
        public int RemotePort { get; private set; }

        /// <summary>
        /// Gets the position from the start of the file to resume from, in bytes.
        /// </summary>
        public long StartPosition { get; private set; }

        /// <summary>
        /// Gets or sets a token identifying the send request.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.Ctcp.Dcc.DccResumeEventArgs"/> class.
        /// </summary>
        /// <param name="fileName">The name of the file to send.</param>
        /// <param name="port">The remote port to connect to.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if fileName is null or empty.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if position is less than 0 or port is not between 0 and 65535.</exception>
        public DccResumeEventArgs(string fileName, int port)
        {
            if (string.IsNullOrEmpty(fileName)) { throw new ArgumentNullException(fileName); }
            if (port < 0 || port > 65535) { throw new ArgumentOutOfRangeException(nameof(port)); }

            FileName = fileName;
            RemotePort = port;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.Ctcp.Dcc.DccResumeEventArgs"/> class.
        /// </summary>
        /// <param name="fileName">The name of the file to send.</param>
        /// <param name="port">The remote port to connect to.</param>
        /// <param name="position">The position from the start of the file to resume from, in bytes.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if fileName is null or empty.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if position is less than 0 or port is not between 0 and 65535.</exception>
        public DccResumeEventArgs(string fileName, int port, long position) : this(fileName, port)
        {
            if (position < 0) { throw new ArgumentOutOfRangeException(nameof(position)); }

            StartPosition = position;
        }
    }
}
