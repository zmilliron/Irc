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
    /// Provides data for an event raised when a reply to the USERS command is received.
    /// </summary>
    /// <seealso cref="Irc.Commands.USERS"/>
    /// <seealso cref="Irc.Numerics.RPL_USERS"/>
    public class UsersCommandEventArgs : IrcEventArgs
    {
        /// <summary>
        /// The ID of a user logged into the server machine.
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Terminal { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.UsersCommandEventArgs"/>Irc.UsersCommandEventArgs class.
        /// </summary>
        public UsersCommandEventArgs() { }
    }
}
