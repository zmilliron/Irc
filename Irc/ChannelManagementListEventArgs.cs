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
    /// Provides data for an event raised after a change to a channel's
    /// invitation, ban, or ban exception list is received from an IRC server.
    /// </summary>
    /// <seealso cref="Irc.Numerics.RPL_BANLIST"/>
    /// <seealso cref="Irc.Numerics.RPL_INVITELIST"/>
    /// <seealso cref="Irc.Numerics.RPL_INVITELIST"/>
    /// <seealso cref="Irc.Numerics.RPL_EXCEPTLIST"/>
    public class ChannelManagementListEventArgs : IrcEventArgs
    {
        /// <summary>
        /// Gets or sets the name of a channel the list applies to.
        /// </summary>
        public IrcChannelName ChannelName { get; private set; }

        /// <summary>
        /// Gets or sets an IP mask in the list.
        /// </summary>
        public string Mask { get; private set; }

        /// <summary>
        /// Gets or sets the user that added the list item.
        /// </summary>
        public IrcNickname SetBy { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.ChannelManagementListEventArgs"/> class.
        /// </summary>
        /// <param name="channelName">The name of the channel that is the source of the event.</param>
        /// <param name="mask">A nickname or mask that was added or removed.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName is null, or mask is null or empty.</exception>
        public ChannelManagementListEventArgs(IrcChannelName channelName, string mask)
        {
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }
            if (string.IsNullOrEmpty(mask)) { throw new ArgumentNullException(nameof(mask)); }

            ChannelName = channelName;
            Mask = mask;

        }
    }
}
