/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irc
{
    /// <summary>
    /// Provides data for an event raised after a URL for a channel homepage 
    /// has been received.
    /// </summary>
    /// <seealso cref="Irc.Numerics.RPL_CHANNEL_URL"/>
    public class ChannelUrlEventArgs : IrcEventArgs
    {
        /// <summary>
        /// Gets the channel name.
        /// </summary>
        public IrcChannelName ChannelName { get; private set; }

        /// <summary>
        /// Gets the URL of the channel homepage.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.ChannelUrlEventArgs"/> class.
        /// </summary>
        /// <param name="channelName">The name of the channel.</param>
        /// <param name="url">The URL of the channel homepage.</param>
        public ChannelUrlEventArgs(IrcChannelName channelName, string url)
        {
            ChannelName = channelName;
            Url = url;
        }
    }
}
