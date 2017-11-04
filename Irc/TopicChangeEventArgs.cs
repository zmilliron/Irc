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
    /// Provides data for an event raised when a channel topic has been changed.
    /// </summary>
    /// <seealso cref="Irc.Numerics.RPL_TOPIC"/>
    public class TopicChangeEventArgs : TopicEventArgs
    {
        /// <summary>
        /// Gets the nickname of the user that changed the channel topic.
        /// </summary>
        public IrcNickname Nickname { get; private set; }

        /// <summary>
        /// Gets or sets the username of the user that changed the channel topic.
        /// </summary>
        public IrcUsername UserName { get; set; }

        /// <summary>
        /// Gets or sets the hostname of the user that changed the channel topic.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.TopicEventArgs"/> class.
        /// </summary>
        /// <param name="channelName">The name of the channel on which the topic is set.</param>
        /// <param name="author">The nickname of the user that set the topic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName or author are null.</exception>
        public TopicChangeEventArgs(IrcChannelName channelName, IrcNickname author) : base(channelName)
        {
            if (author == null) { throw new ArgumentNullException(nameof(author)); }

            Nickname = author;
        }
    }
}
