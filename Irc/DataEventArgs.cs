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
    /// A generic data class for events raised when a reply is received from an 
    /// Internet Relay Chat (IRC) server that only returns a single argument.
    /// </summary>
    /// <seealso cref="Irc.Numerics.RPL_ENDOFNAMES"/>
    /// <seealso cref="Irc.Numerics.RPL_YOURCOOKIE"/>
    /// <seealso cref="Irc.Numerics.RPL_YOURID"/>
    public class DataEventArgs : IrcEventArgs
    {
        /// <summary>
        /// Gets or sets the data sent by the server.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.DataEventArgs"/> class.
        /// </summary>
        public DataEventArgs() { }
    }
}
