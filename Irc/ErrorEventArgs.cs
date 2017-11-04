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
    /// Provides data for an event raised after an error code is received from
    /// an IRC server.
    /// </summary>
    public class ErrorEventArgs : IrcEventArgs
    {
        /// <summary>
        /// Gets or sets an error code;
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets a default message associated with an error.
        /// </summary>
        public string DefaultMessage { get; set; }

        /// <summary>
        /// The target of the error message.
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.ErrorEventArgs"/> class.
        /// </summary>
        public ErrorEventArgs()
        {
        }
    }
}
