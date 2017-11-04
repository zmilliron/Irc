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
    /// Provides data for an event raised when an channel invite is
    /// received from an IRC server.
    /// </summary>
    /// <seealso cref="Irc.Commands.INVITE"/>
    public class InviteEventArgs : UserEventArgs
    {
        /// <summary>
        /// Gets the name of the channel the invitation was sent from.
        /// </summary>
        public IrcChannelName ChannelName { get; private set; }

        /// <summary>
        /// Gets the nickname of the user being invited to a channel.
        /// </summary>
        public IrcNickname RecipientName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.InviteEventArgs"/> class.
        /// </summary>
        /// <param name="channelName">The channel the user is being invited to.</param>
        /// <param name="sender">The nickname of the user sending the invitation.</param>
        /// <param name="recipient">The nickname of the recipient of the invitation.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName, sender, or recipient are null.</exception>
        public InviteEventArgs(IrcChannelName channelName, IrcNickname sender, IrcNickname recipient) : base(sender)
        {
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }
            if (sender == null) { throw new ArgumentNullException(nameof(sender)); }
            if (recipient == null) { throw new ArgumentNullException(nameof(recipient)); }

            ChannelName = channelName;
            RecipientName = recipient;
        }
    }
}
