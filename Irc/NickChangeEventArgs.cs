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
    /// Provides data for an event raised when a user's nickname has been changed.
    /// </summary>
    /// <seealso cref="Irc.Commands.NICK"/>
    public class NickChangeEventArgs : UserEventArgs
    {
        /// <summary>
        /// Gets the new nickname of a user.
        /// </summary>
        public IrcNickname NewNickname { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.NickChangeEventArgs"/> class.
        /// </summary>
        /// <param name="oldNickname">The user's nickname before the change.</param>
        /// <param name="newNickname">The user's new nickname after the change.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if oldNickname or newNickname are null.</exception>
        public NickChangeEventArgs(IrcNickname oldNickname, IrcNickname newNickname)
            : base(oldNickname)
        {
            if (newNickname == null) { throw new ArgumentNullException(nameof(newNickname)); }

            NewNickname = newNickname;
        }
    }
}
