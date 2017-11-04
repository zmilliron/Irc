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
    /// Provides data for an event raised after receiving a list of supported options
    /// from a server.
    /// </summary>
    /// <see cref="Irc.Numerics.RPL_ISUPPORT"/>
    /// <see cref="Irc.SupportedOptions"/>
    public class SupportedOptionsEventArgs : IrcEventArgs
    {
        /// <summary>
        /// Gets a list of supported options and their parameters.
        /// </summary>
        public Dictionary<string, string> Options { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.SupportedOptionsEventArgs"/> class.
        /// </summary>
        public SupportedOptionsEventArgs()
        {
            Options = new Dictionary<string, string>();
        }
    }
}
