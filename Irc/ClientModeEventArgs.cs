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

namespace Irc
{
    /// <summary>
    /// Provides data for an event raised after a client mode change is received from a server.
    /// </summary>
    /// <seealso cref="Irc.Commands.MODE"/>
    /// <seealso cref="Irc.ClientModes"/>
    public class ClientModeEventArgs : UserEventArgs
    {
        /// <summary>
        /// Gets or sets a string containing modes that were set.
        /// </summary>
        public ClientModeString ModeString { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.ClientModeEventArgs"/> class.
        /// </summary>
        /// <param name="modeChanger">The nickname of the user changing the mode.</param>
        /// <param name="modeString">The mode string.</param>
        public ClientModeEventArgs(IrcNickname modeChanger, ClientModeString modeString)
            : base(modeChanger)
        {
            ModeString = modeString;
        }
    }
}
