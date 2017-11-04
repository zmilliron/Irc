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
    /// Represents errors that occur when communicating through the Client-to-Client Protocol.
    /// </summary>
    public class CtcpException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.Ctcp.CtcpException"/> class.
        /// </summary>
        public CtcpException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.Ctcp.CtcpException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CtcpException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.Ctcp.CtcpException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or null if none is specified.</param>
        public CtcpException(string message, Exception innerException) : base(message, innerException) { }
    }
}
