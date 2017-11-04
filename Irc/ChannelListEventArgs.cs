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
    /// Provides data for an event raised after a channel on an Internet 
    /// Relay Chat(IRC) network's global channel list is received.
    /// </summary>
    /// <seealso cref="Irc.Numerics.RPL_LIST"/>
    public class ChannelListEventArgs : IrcEventArgs
    {
        /// <summary>
        /// Gets the name of a channel.
        /// </summary>
        public IrcChannelName ChannelName { get; private set; }

        /// <summary>
        /// Gets or sets the number of users currently in the channel.
        /// </summary>
        public int NumberOfUsers { get; set; }

        /// <summary>
        /// Gets or sets the channel topic.
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.ChannelCreatorEventArgs"/> class.
        /// </summary>
        /// <param name="channelName">The name of a channel in an IRC network's global channel list.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName is null.</exception>
        public ChannelListEventArgs(IrcChannelName channelName)
        {
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }

            ChannelName = channelName;
        }
    }
}
