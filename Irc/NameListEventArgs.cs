/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Irc
{
    /// <summary>
    /// Provides data for an event raised after a response to the NAMES command is received.
    /// </summary>
    /// <seealso cref="Irc.Commands.NAMES"/>
    /// <seealso cref="Irc.Numerics.RPL_NAMEREPLY"/>
    public sealed class NameListEventArgs : IrcEventArgs
    {
        /// <summary>
        /// Gets the name of the channel for which the reponse was received.
        /// </summary>
        public IrcChannelName ChannelName { get; private set; }

        /// <summary>
        /// Gets a list of users on the channel.
        /// </summary>
        public ReadOnlyCollection<NameListItem> Users { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating if the chanenl is public.
        /// </summary>
        public bool IsPublicChannel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the channel is secret.
        /// </summary>
        public bool IsSecretChannel { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.NameListEventArgs"/> class.
        /// </summary>
        /// <param name="channelName">The name of the channel the user list belongs to.</param>
        /// <param name="nameList">A list of <see cref="Irc.NameListItem"/>s.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName is null.</exception>
        public NameListEventArgs(IrcChannelName channelName, IList<NameListItem> nameList)
        {
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }

            ChannelName = channelName;
            Users = new ReadOnlyCollection<NameListItem>(nameList);
        }
    }
}
