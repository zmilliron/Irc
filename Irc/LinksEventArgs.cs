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
    /// Provides data for an event raised when a response is received to a LINKS command.
    /// </summary>
    /// <seealso cref="Irc.Numerics.RPL_LINKS"/>
    public class LinksEventArgs : IrcEventArgs
    {
        /// <summary>
        /// The mask used to return information about the server.
        /// </summary>
        public string Mask { get; set; }
        
        /// <summary>
        /// The server name.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// The hop count between the client and the server.
        /// </summary>
        public int HopCount { get; set; }

        /// <summary>
        /// A description of the server.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.LinksEventArgs"/> class.
        /// </summary>
        public LinksEventArgs() : base() { }
    }
}
