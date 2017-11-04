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
    /// Provides data for a an event raised when a private message has been
    /// received from an IRC server.
    /// </summary>
    /// <seealso cref="Irc.Commands.PRIVMSG"/>
    public class PrivateMessageEventArgs : UserEventArgs
    {
        /// <summary>
        /// Gets the target recipient of the message.  This is either a channel
        /// the message was sent to, or the local client.
        /// </summary>
        public IrcNameBase MessageTarget { get; private set; }

        /// <summary>
        /// Gets or sets the message sent.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the message was sent by a network service.
        /// </summary>
        public bool IsNetworkService { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the message was sent by the client user.
        /// </summary>
        public bool IsLocalEcho { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.PrivateMessageEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The nickname of the user sending the message.</param>
        /// <param name="target">The user or channel that the message is being sent to.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if sender or target are null.</exception>
        public PrivateMessageEventArgs(IrcNickname sender, IrcNameBase target)
            : base(sender)
        {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }

            MessageTarget = target;
            TimeStamp = DateTime.Now;
        }
    }
}
