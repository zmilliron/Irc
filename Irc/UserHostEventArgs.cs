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
    /// Provides data for an event raised after a reply to the USERHOST command is received 
    /// from a server.
    /// </summary>
    /// <seealso cref="Irc.Commands.USERHOST"/>
    /// <seealso cref="Irc.Numerics.RPL_USERHOST"/>
    public class UserHostEventArgs : IrcEventArgs
    {
        /// <summary>
        /// Gets a list of users.
        /// </summary>
        public Dictionary<string, string> Users { get; private set;}

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.UserHostEventArgs"/> class.
        /// </summary>
        public UserHostEventArgs() 
        {
            Users = new Dictionary<string, string>();
        }
    }
}
