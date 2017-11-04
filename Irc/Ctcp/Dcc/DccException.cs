/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irc.Ctcp.Dcc
{
    /// <summary>
    /// Represents errors that occur when communcating through a Direct Client Connection.
    /// </summary>
    public class DccException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.Ctcp.Dcc.DccException"/> class.
        /// </summary>
        public DccException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.Ctcp.Dcc.DccException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DccException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.Ctcp.Dcc.DccException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or null if none is specified.</param>
        public DccException(string message, Exception innerException) : base(message, innerException) { }
    }
}
