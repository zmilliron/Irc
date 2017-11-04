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
    /// Provides data for an event raised after the creator of a channel is received.
    /// </summary>
    /// <seealso cref="Irc.Numerics.RPL_UNIQOPIS"/>
    public class ChannelCreatorEventArgs : IrcEventArgs
    {
        /// <summary>
        /// Gets the name of a channel that was queried.
        /// </summary>
        public IrcChannelName ChannelName { get; private set; }

        /// <summary>
        /// Gets the nickname of the channel creator.
        /// </summary>
        public IrcNickname Nickname { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.ChannelCreatorEventArgs"/> class.
        /// </summary>
        /// <param name="channelName">The name of a channel that was queried.</param>
        /// <param name="creator">The nickname of the user that created the channel.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName or creator are null.</exception>
        public ChannelCreatorEventArgs(IrcChannelName channelName, IrcNickname creator)
        {
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }
            if (creator == null) { throw new ArgumentNullException(nameof(creator)); }

            Nickname = creator;
            ChannelName = channelName;
        }
    }
}
