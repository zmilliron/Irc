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
    /// Provides data for an event raised after channel mode information
    /// has been received.
    /// </summary>
    /// <seealso cref="Irc.ChannelModes"/>
    /// <seealso cref="Irc.Commands.MODE"/>
    public class ChannelModeEventArgs : UserEventArgs
    {
        /// <summary>
        /// Gets the channel name.
        /// </summary>
        public IrcChannelName ChannelName { get; private set; }

        /// <summary>
        /// Gets the channel mode string.
        /// </summary>
        public ChannelModeString ModeString { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.ChannelModeEventArgs"/> class.
        /// </summary>
        /// <param name="channelName">The channel that is the source of the mode change.</param>
        /// <param name="modeChanger">The nickname of the user that changed the channel modes, or null if none exists.</param>
        /// <param name="modeString">The mode string containing a list of added and removed channel modes.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName is null.</exception>
        public ChannelModeEventArgs(IrcChannelName channelName, IrcNickname modeChanger, ChannelModeString modeString)
            : base(modeChanger)
        {
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }

            ChannelName = channelName;

            ModeString = modeString;
        }
    }
}
