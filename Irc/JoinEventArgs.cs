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
    /// Provides data for an event raised when the client has joined a channel or a user
    /// has joined a channel the client is connected to.
    /// </summary>
    /// <see cref="Irc.Commands.JOIN"/>
    public class JoinEventArgs : UserEventArgs
    {
        /// <summary>
        /// Gets the name of the channel a user is joining.
        /// </summary>
        public IrcChannelName ChannelName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the Irc.JoinEventArgs class.
        /// </summary>
        /// <param name="channelName">The name of the channel that has been joined.</param>
        /// <param name="newUserName">The nickname of the new user that has joined.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName or nickname are null.</exception>
        public JoinEventArgs(IrcChannelName channelName, IrcNickname newUserName)
            : base(newUserName)
        {

            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }

            ChannelName = channelName;
        }
    }
}
