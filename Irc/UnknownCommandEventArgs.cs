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
    /// Provides data for an event raised after an unknown command is received from a server.
    /// </summary>
    /// <see cref="Irc.Commands"/>
    public class UnhandledCommandEventArgs : IrcEventArgs
    {
        /// <summary>
        /// Gets or sets an unknown command.
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets a message associated with the command.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.UnhandledCommandEventArgs"/> class.
        /// </summary>
        public UnhandledCommandEventArgs() { }
    }
}
