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
    /// Provides error data for an event raised after a 
    /// socket exception is thrown.
    /// </summary>
    public class SocketErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the error code returned from a socket exception.
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the exception that is the cause of the error, or null.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the error occurred during the connection process.
        /// </summary>
        public bool IsConnecting { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.SocketErrorEventArgs"/> class.
        /// </summary>
        public SocketErrorEventArgs() { }
    }
}
