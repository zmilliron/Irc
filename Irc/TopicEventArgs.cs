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
    /// Provides data for an event raised when a channel topic is received.
    /// </summary>
    /// <seealso cref="Irc.Numerics.RPL_TOPIC"/>
    /// <seealso cref="Irc.Numerics.RPL_NOTOPIC"/>
    public class TopicEventArgs : IrcEventArgs
    {
        /// <summary>
        /// Gets the name of the channel for which the topic is set.
        /// </summary>
        public IrcChannelName ChannelName { get; private set; }

        /// <summary>
        /// Gets or sets a channel topic.
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.TopicEventArgs"/> class.
        /// </summary>
        /// <param name="channelName">The name of the channel for which the topic is set.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if channelname is null.</exception>
        public TopicEventArgs(IrcChannelName channelName)
        {
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }

            ChannelName = channelName;
        }
    }
}
