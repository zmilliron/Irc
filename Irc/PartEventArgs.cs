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
    /// Provides data for an event raised when a user has left a channel.
    /// </summary>
    /// <see cref="Irc.Commands.PART"/>
    public class PartEventArgs : UserEventArgs
    {
        /// <summary>
        /// Gets the name of the channel a user is leaving.
        /// </summary>
        public IrcChannelName ChannelName { get; private set; }

        /// <summary>
        /// Gets or sets a parting message sent from the user.
        /// </summary>
        public string FarewellMessage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.PartEventArgs"/> class.
        /// </summary>
        /// <param name="channelParted">The name of the channel that a user has left.</param>
        /// <param name="partingUser">The nickname of a user that has left the channel.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if channelParted or partingUser are null.</exception>
        public PartEventArgs(IrcChannelName channelParted, IrcNickname partingUser)
            : base(partingUser)
        {
            if (channelParted == null) { throw new ArgumentNullException(nameof(channelParted)); }
            if (partingUser == null) { throw new ArgumentNullException(nameof(partingUser)); }

            ChannelName = channelParted;
        }
    }
}
