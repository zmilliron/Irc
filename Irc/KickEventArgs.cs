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
    /// Provides data for an event raised when a user is kicked from a channel.
    /// </summary>
    /// <see cref="Irc.Commands.KICK"/>
    public sealed class KickEventArgs : UserEventArgs
    {
        /// <summary>
        /// Gets or sets the reason a user was kicked from a channel.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets the name of the channel from which a user was kicked.
        /// </summary>
        public IrcChannelName ChannelName { get; private set; }

        /// <summary>
        /// Gets the nickname of the user that was kicked.
        /// </summary>
        public IrcNickname UserKicked { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.KickEventArgs"/> class.
        /// </summary>
        /// <param name="channelName">The name of the channel a user was kicked from.</param>
        /// <param name="kicker">The nickname of the user that performed the kick.</param>
        /// <param name="userKicked">The nickname of the user that was kicked.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName, kicker, or userKicked is null.</exception>
        public KickEventArgs(IrcChannelName channelName, IrcNickname kicker, IrcNickname userKicked) : base(kicker)
        {
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }
            if (userKicked == null) { throw new ArgumentNullException(nameof(userKicked)); }

            ChannelName = channelName;
            UserKicked = userKicked;
        }
    }
}
