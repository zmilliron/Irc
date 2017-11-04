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
    /// Provides data for an event raised after an away notice is received.
    /// </summary>
    /// <seealso cref="Irc.Numerics.RPL_AWAY"/>
    public class AwayEventArgs : IrcEventArgs
    {
        /// <summary>
        /// Gets the nickname of the user that is the source of the event.
        /// </summary>
        public IrcNickname Nickname { get; private set; }

        /// <summary>
        /// Gets or sets the away message set by the user, or null if there is no message.
        /// </summary>
        public string AwayMessage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.AwayEventArgs"/> class.
        /// </summary>
        /// <param name="nickname">The nickname of the user that 
        /// is the source of the event.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        public AwayEventArgs(IrcNickname nickname)
        {
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); }

            Nickname = nickname;
        }
    }
}
