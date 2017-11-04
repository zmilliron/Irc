/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/
using System;
using System.Collections.Generic;

namespace Irc
{
    /// <summary>
    /// Provides data for an event raised after entries in a client's
    /// ignore list is received.
    /// </summary>
    /// <see cref="Irc.Commands.SILENCE"/>
    public class SilenceEventArgs : IrcEventArgs
    {
        /// <summary>
        /// Gets a list of silenced hostmaks.
        /// </summary>
        public Dictionary<string, bool> Masks { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.SilenceEventArgs"/> class.
        /// </summary>
        public SilenceEventArgs()
        {
            Masks = new Dictionary<string, bool>();
        }
    }
}
