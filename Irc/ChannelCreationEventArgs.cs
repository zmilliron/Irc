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
    /// Provides data for an event raised after information about when a
    /// channel was created is received.
    /// </summary>
    /// <seealso cref="Irc.Numerics.RPL_CREATIONTIME"/>
    public class ChannelCreationEventArgs : IrcEventArgs
    {
        /// <summary>
        /// Gets the name of the channel that is the source of the event.
        /// </summary>
        public IrcChannelName ChannelName { get; private set; }

        /// <summary>
        /// Gets the date and time when the channel was created.
        /// </summary>
        public DateTime CreationTime { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.ChannelCreationEventArgs"/> class.
        /// </summary>
        /// <param name="channelName">The name of a channel that is the source of the event being raised.</param>
        /// <param name="creationTime">The date and time when the channel was created.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName is null.</exception>
        public ChannelCreationEventArgs(IrcChannelName channelName, DateTime creationTime)
        {
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); };

            ChannelName = channelName;
            CreationTime = creationTime;
        }
    }
}
