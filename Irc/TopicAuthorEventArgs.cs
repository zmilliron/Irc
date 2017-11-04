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
    /// Provides data for an event raised when the author of a channel topic is received.
    /// </summary>
    /// <seealso cref="Irc.Numerics.RPL_TOPICWHOTIME"/>
    public class TopicAuthorEventArgs : IrcEventArgs
    {
        /// <summary>
        /// Gets the name of the channel on which the topic was set.
        /// </summary>
        public IrcChannelName ChannelName { get; set; }

        /// <summary>
        /// Gets the nickname of the user that last set the topic.
        /// </summary>
        public IrcNickname Author { get; private set; }

        /// <summary>
        /// Gets or sets the time the topic was set.
        /// </summary>
        public DateTime TimeSet { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.TopicAuthorEventArgs"/> class.
        /// </summary>
        /// <param name="channelName">The channel on which the topic was set.</param>
        /// <param name="author">The nickname of the user that set the topic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName or author are null.</exception>
        public TopicAuthorEventArgs(IrcChannelName channelName, IrcNickname author)
        {
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }
            if (author == null) { throw new ArgumentNullException(nameof(author)); }

            ChannelName = channelName;
            Author = author;
        }
    }
}
